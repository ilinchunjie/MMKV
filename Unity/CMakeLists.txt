#
# MMKV Unity Plugin - CMake Build Configuration
#
# Supports building native libraries for Unity:
#   - iOS: Static library (libmmkv.a)
#   - Android: Shared library (libmmkv.so) via NDK r28+
#   - Linux: Shared library (libmmkv.so)
#   - macOS: Shared library (mmkv.dylib)
#   - Windows: Shared library (mmkv.dll)
#

cmake_minimum_required(VERSION 3.10.0)

project(mmkv)

# iOS builds as static library, all others as shared
option(MMKV_UNITY_IOS "Build as iOS static library" OFF)
option(MMKV_UNITY_STATIC_GNU_RUNTIME "Statically link libstdc++ and libgcc for Linux Unity builds" ON)

IF(APPLE)
    add_compile_definitions(FORCE_POSIX)
ENDIF()

# On Linux, force embedded zlib to avoid runtime dependency on libz.so.1
# (target environments like minimal Docker containers may not have it)
IF(UNIX AND NOT APPLE AND NOT ANDROID)
    set(MMKV_FORCE_EMBED_ZLIB ON CACHE BOOL "Force embedded zlib for Linux Unity" FORCE)
ENDIF()

add_subdirectory(../Core Core)

# Determine library type
IF(MMKV_UNITY_IOS)
    set(MMKV_LIB_TYPE STATIC)
ELSE()
    set(MMKV_LIB_TYPE SHARED)
ENDIF()

add_library(mmkv ${MMKV_LIB_TYPE}
        mmkv_unity_bridge.cpp
        )

target_include_directories(mmkv PUBLIC
        ${CMAKE_CURRENT_SOURCE_DIR}
        ${CMAKE_CURRENT_SOURCE_DIR}/../Core)

set_target_properties(mmkv PROPERTIES
        CXX_STANDARD 20
        CXX_EXTENSIONS OFF
        POSITION_INDEPENDENT_CODE ON
        )

# Platform-specific linking
IF(ANDROID)
    find_library(log-lib log)
    target_link_libraries(mmkv core ${log-lib})
ELSEIF(WIN32)
    target_link_libraries(mmkv core advapi32)
ELSEIF(APPLE)
    IF(MMKV_UNITY_IOS)
        # iOS static library: just link core
        target_link_libraries(mmkv core)
    ELSE()
        # macOS shared library
        target_link_libraries(mmkv core pthread)
    ENDIF()
ELSE()
    # Linux
    target_link_libraries(mmkv core pthread)
    IF(MMKV_UNITY_STATIC_GNU_RUNTIME AND CMAKE_CXX_COMPILER_ID MATCHES "GNU|Clang")
        target_link_options(mmkv PRIVATE -static-libstdc++ -static-libgcc)
    ENDIF()
ENDIF()

# Set default symbol visibility to hidden (exports via MMKV_EXPORT)
IF(NOT WIN32 AND NOT MMKV_UNITY_IOS)
    set_target_properties(mmkv PROPERTIES
            C_VISIBILITY_PRESET hidden
            CXX_VISIBILITY_PRESET hidden
            )
ENDIF()
