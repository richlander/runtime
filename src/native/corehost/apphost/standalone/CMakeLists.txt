# Licensed to the .NET Foundation under one or more agreements.
# The .NET Foundation licenses this file to you under the MIT license.

include_directories(..)

set(SOURCES
    ../bundle_marker.cpp
    ./hostfxr_resolver.cpp
    ../../corehost.cpp
)

set(HEADERS
    ../bundle_marker.h
    ../../hostfxr_resolver.h
)

if(CLR_CMAKE_TARGET_WIN32)
    add_compile_definitions(UNICODE)
    list(APPEND SOURCES
        ../apphost.windows.cpp)

    list(APPEND HEADERS
        ../apphost.windows.h)
endif()

if(CLR_CMAKE_TARGET_WIN32)
    list(APPEND SOURCES ${HEADERS})
endif()

add_compile_definitions(FEATURE_APPHOST)

add_executable(apphost ${SOURCES} ${RESOURCES})

target_link_libraries(apphost PRIVATE hostmisc fxr_resolver)

add_sanitizer_runtime_support(apphost)

if(NOT CLR_CMAKE_TARGET_WIN32)
    disable_pax_mprotect(apphost)
endif()

install_with_stripped_symbols(apphost TARGETS corehost)

if(CLR_CMAKE_TARGET_WIN32)
    # Disable manifest generation into the file .exe on Windows
    target_link_options(apphost PRIVATE "/MANIFEST:NO")

    # Enable CET-compatibility
    if (CLR_CMAKE_HOST_ARCH_AMD64)
        target_link_options(apphost PRIVATE "/CETCOMPAT")
    endif()
endif()

if (CLR_CMAKE_TARGET_WIN32)
    target_link_libraries(apphost PRIVATE shell32)
endif()

if (CLR_CMAKE_HOST_APPLE)
    adhoc_sign_with_entitlements(apphost "${CLR_ENG_NATIVE_DIR}/entitlements.plist")
endif()
