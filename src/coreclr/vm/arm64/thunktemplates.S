// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#include "unixasmmacros.inc"
#include "asmconstants.h"

#define DATA_SLOT(stub, field) . - (. - C_FUNC(stub##Code\STUB_PAGE_SIZE)) + \STUB_PAGE_SIZE + stub##Data__##field

    .irp STUB_PAGE_SIZE, 16384, 32768, 65536

    LEAF_ENTRY StubPrecodeCode\STUB_PAGE_SIZE
        ldr x10, DATA_SLOT(StubPrecode, Target)
        ldr x12, DATA_SLOT(StubPrecode, SecretParam)
        br x10
    LEAF_END_MARKED StubPrecodeCode\STUB_PAGE_SIZE

    LEAF_ENTRY FixupPrecodeCode\STUB_PAGE_SIZE
        ldr x11, DATA_SLOT(FixupPrecode, Target)
        br  x11
        ldr x12, DATA_SLOT(FixupPrecode, MethodDesc)
        ldr x11, DATA_SLOT(FixupPrecode, PrecodeFixupThunk)
        br  x11        
    LEAF_END_MARKED FixupPrecodeCode\STUB_PAGE_SIZE

    LEAF_ENTRY CallCountingStubCode\STUB_PAGE_SIZE
LOCAL_LABEL(StubStart\STUB_PAGE_SIZE):
        ldr  x9, DATA_SLOT(CallCountingStub, RemainingCallCountCell)
        ldrh w10, [x9]
        subs w10, w10, #1
        strh w10, [x9]
        beq LOCAL_LABEL(CountReachedZero\STUB_PAGE_SIZE)
        ldr  x9, DATA_SLOT(CallCountingStub, TargetForMethod)
        br   x9
LOCAL_LABEL(CountReachedZero\STUB_PAGE_SIZE):
        ldr  x10, DATA_SLOT(CallCountingStub, TargetForThresholdReached)
        br   x10
    LEAF_END_MARKED CallCountingStubCode\STUB_PAGE_SIZE

    .endr
