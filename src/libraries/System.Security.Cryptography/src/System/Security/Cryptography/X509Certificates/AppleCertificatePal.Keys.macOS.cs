// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Security.Cryptography.Apple;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates
{
    internal sealed partial class AppleCertificatePal : ICertificatePal
    {
        public DSA? GetDSAPrivateKey()
        {
            if (_identityHandle == null)
                return null;

            Debug.Assert(!_identityHandle.IsInvalid);
            SafeSecKeyRefHandle publicKey = Interop.AppleCrypto.X509GetPublicKey(_certHandle);
            SafeSecKeyRefHandle privateKey = Interop.AppleCrypto.X509GetPrivateKeyFromIdentity(_identityHandle);

            if (publicKey.IsInvalid)
            {
                // SecCertificateCopyKey returns null for DSA, so fall back to manually building it.
                publicKey = Interop.AppleCrypto.ImportEphemeralKey(_certData.SubjectPublicKeyInfo, false);
            }

            privateKey.SetParentHandle(_certHandle);
            return new DSAImplementation.DSASecurityTransforms(publicKey, privateKey);
        }

        public ICertificatePal CopyWithPrivateKey(DSA privateKey)
        {
            var typedKey = privateKey as DSAImplementation.DSASecurityTransforms;

            if (typedKey != null)
            {
                return CopyWithPrivateKey(typedKey.GetKeys().PrivateKey);
            }

            DSAParameters dsaParameters = privateKey.ExportParameters(true);

            using (PinAndClear.Track(dsaParameters.X!))
            using (typedKey = new DSAImplementation.DSASecurityTransforms())
            {
                typedKey.ImportParameters(dsaParameters);
                return CopyWithPrivateKey(typedKey.GetKeys().PrivateKey);
            }
        }

        public ICertificatePal CopyWithPrivateKey(ECDsa privateKey)
        {
            var typedKey = privateKey as ECDsaImplementation.ECDsaSecurityTransforms;
            byte[] ecPrivateKey;

            if (typedKey != null)
            {
                ECParameters ecParameters = default;

                if (typedKey.TryExportDataKeyParameters(includePrivateParameters: true, ref ecParameters))
                {
                    using (PinAndClear.Track(ecParameters.D!))
                    {
                        AsnWriter writer = EccKeyFormatHelper.WriteECPrivateKey(ecParameters);
                        ecPrivateKey = writer.Encode();
                        writer.Reset();
                    }
                }
                else
                {
                    return CopyWithPrivateKey(typedKey.GetKeys().PrivateKey);
                }
            }
            else
            {
                ecPrivateKey = privateKey.ExportECPrivateKey();
            }

            using (PinAndClear.Track(ecPrivateKey))
            using (SafeSecKeyRefHandle privateSecKey = Interop.AppleCrypto.ImportEphemeralKey(ecPrivateKey, true))
            {
                return CopyWithPrivateKey(privateSecKey);
            }
        }

        public ICertificatePal CopyWithPrivateKey(ECDiffieHellman privateKey)
        {
            var typedKey = privateKey as ECDiffieHellmanImplementation.ECDiffieHellmanSecurityTransforms;
            byte[] ecPrivateKey;

            if (typedKey != null)
            {
                ECParameters ecParameters = default;

                if (typedKey.TryExportDataKeyParameters(includePrivateParameters: true, ref ecParameters))
                {
                    using (PinAndClear.Track(ecParameters.D!))
                    {
                        AsnWriter writer = EccKeyFormatHelper.WriteECPrivateKey(ecParameters);
                        ecPrivateKey = writer.Encode();
                        writer.Reset();
                    }
                }
                else
                {
                    return CopyWithPrivateKey(typedKey.GetKeys().PrivateKey);
                }
            }
            else
            {
                ecPrivateKey = privateKey.ExportECPrivateKey();
            }

            using (PinAndClear.Track(ecPrivateKey))
            using (SafeSecKeyRefHandle privateSecKey = Interop.AppleCrypto.ImportEphemeralKey(ecPrivateKey, true))
            {
                return CopyWithPrivateKey(privateSecKey);
            }
        }

        public ICertificatePal CopyWithPrivateKey(MLDsa privateKey)
        {
            throw new PlatformNotSupportedException(
                SR.Format(SR.Cryptography_AlgorithmNotSupported, nameof(MLDsa)));
        }

        public ICertificatePal CopyWithPrivateKey(RSA privateKey)
        {
            var typedKey = privateKey as RSAImplementation.RSASecurityTransforms;

            if (typedKey != null)
            {
                return CopyWithPrivateKey(typedKey.GetKeys().PrivateKey);
            }

            byte[] rsaPrivateKey = privateKey.ExportRSAPrivateKey();

            using (PinAndClear.Track(rsaPrivateKey))
            using (SafeSecKeyRefHandle privateSecKey = Interop.AppleCrypto.ImportEphemeralKey(rsaPrivateKey, true))
            {
                return CopyWithPrivateKey(privateSecKey);
            }
        }

        private AppleCertificatePal CopyWithPrivateKey(SafeSecKeyRefHandle? privateKey)
        {
            if (privateKey == null)
            {
                // Both Windows and Linux/OpenSSL are unaware if they bound a public or private key.
                // Here, we do know.  So throw if we can't do what they asked.
                throw new CryptographicException(SR.Cryptography_CSP_NoPrivateKey);
            }

            SafeKeychainHandle keychain = Interop.AppleCrypto.SecKeychainItemCopyKeychain(privateKey);

            // If we're using a key already in a keychain don't add the certificate to that keychain here,
            // do it in the temporary add/remove in the shim.
            SafeKeychainHandle cloneKeychain = SafeTemporaryKeychainHandle.InvalidHandle;

            if (keychain.IsInvalid)
            {
                keychain = Interop.AppleCrypto.CreateTemporaryKeychain();
                cloneKeychain = keychain;
            }

            // Because SecIdentityRef only has private constructors we need to have the cert and the key
            // in the same keychain.  That almost certainly means we're going to need to add this cert to a
            // keychain, and when a cert that isn't part of a keychain gets added to a keychain then the
            // interior pointer of "what keychain did I come from?" used by SecKeychainItemCopyKeychain gets
            // set. That makes this function have side effects, which is not desired.
            //
            // It also makes reference tracking on temporary keychains broken, since the cert can
            // DangerousRelease a handle it didn't DangerousAddRef on.  And so CopyWithPrivateKey makes
            // a temporary keychain, then deletes it before anyone has a chance to (e.g.) export the
            // new identity as a PKCS#12 blob.
            //
            // Solution: Clone the cert, like we do in Windows.
            SafeSecCertificateHandle tempHandle;

            {
                byte[] export = RawData;
                const bool exportable = false;
                SafeSecIdentityHandle identityHandle;
                tempHandle = Interop.AppleCrypto.X509ImportCertificate(
                    export,
                    X509ContentType.Cert,
                    SafePasswordHandle.InvalidHandle,
                    cloneKeychain,
                    exportable,
                    out identityHandle);

                Debug.Assert(identityHandle.IsInvalid, "identityHandle should be IsInvalid");
                identityHandle.Dispose();

                Debug.Assert(!tempHandle.IsInvalid, "tempHandle should not be IsInvalid");
            }

            using (keychain)
            using (tempHandle)
            {
                SafeSecIdentityHandle identityHandle = Interop.AppleCrypto.X509CopyWithPrivateKey(
                    tempHandle,
                    privateKey,
                    keychain);

                AppleCertificatePal newPal = new AppleCertificatePal(identityHandle);
                return newPal;
            }
        }
    }
}
