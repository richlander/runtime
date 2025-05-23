// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Advapi32
    {
        [LibraryImport(Libraries.Advapi32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool OpenThreadToken(
            nint ThreadHandle,
            int DesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool OpenAsSelf,
            out SafeTokenHandle TokenHandle);
    }
}
