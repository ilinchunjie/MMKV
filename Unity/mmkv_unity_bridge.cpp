/*
 * Tencent is pleased to support the open source community by making
 * MMKV available.
 *
 * Copyright (C) 2025 THL A29 Limited, a Tencent company.
 * All rights reserved.
 *
 * Licensed under the BSD 3-Clause License (the "License"); you may not use
 * this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 *       https://opensource.org/licenses/BSD-3-Clause
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#include "MMKV.h"
#include <cstdlib>
#include <cstring>

#ifdef _WIN32
#  define MMKV_EXPORT extern "C" __declspec(dllexport)
#else
#  define MMKV_EXPORT extern "C" __attribute__((visibility("default")))
#endif

#ifdef MMKV_WIN32
static MMKVPath_t ToMMKVPath(const char *str) {
    if (!str) {
        return {};
    }
    int len = MultiByteToWideChar(CP_UTF8, 0, str, -1, nullptr, 0);
    if (len <= 0) {
        return {};
    }
    std::wstring wstr(len - 1, 0);
    MultiByteToWideChar(CP_UTF8, 0, str, -1, &wstr[0], len);
    return wstr;
}
#else
static MMKVPath_t ToMMKVPath(const char *str) {
    return str ? std::string(str) : std::string();
}
#endif

MMKV_EXPORT void InitializeMMKV(const char *rootDir) {
    if (rootDir) {
        MMKV::initializeMMKV(ToMMKVPath(rootDir));
    }
}

MMKV_EXPORT void *MMKVWithID(const char *mmapId, const char *path) {
    if (!mmapId) {
        return nullptr;
    }
    MMKVPath_t mmkvPath = ToMMKVPath(path);
    MMKVPath_t *pathPtr = (path && path[0]) ? &mmkvPath : nullptr;
    return static_cast<void *>(MMKV::mmkvWithID(std::string(mmapId), MMKV_SINGLE_PROCESS, nullptr, pathPtr));
}

MMKV_EXPORT bool SetBool(void *ptr, const char *key, bool v) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return false;
    }
    return kv->set(v, key);
}

MMKV_EXPORT bool GetBool(void *ptr, const char *key, bool defaultValue) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return defaultValue;
    }
    return kv->getBool(key, defaultValue);
}

MMKV_EXPORT bool SetInt(void *ptr, const char *key, int32_t v) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return false;
    }
    return kv->set(v, key);
}

MMKV_EXPORT int32_t GetInt(void *ptr, const char *key, int32_t defaultValue) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return defaultValue;
    }
    return kv->getInt32(key, defaultValue);
}

MMKV_EXPORT bool SetLong(void *ptr, const char *key, int64_t v) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return false;
    }
    return kv->set(v, key);
}

MMKV_EXPORT int64_t GetLong(void *ptr, const char *key, int64_t defaultValue) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return defaultValue;
    }
    return kv->getInt64(key, defaultValue);
}

MMKV_EXPORT bool SetULong(void *ptr, const char *key, uint64_t v) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return false;
    }
    return kv->set(v, key);
}

MMKV_EXPORT uint64_t GetULong(void *ptr, const char *key, uint64_t defaultValue) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return defaultValue;
    }
    return kv->getUInt64(key, defaultValue);
}

MMKV_EXPORT bool SetFloat(void *ptr, const char *key, float v) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return false;
    }
    return kv->set(v, key);
}

MMKV_EXPORT float GetFloat(void *ptr, const char *key, float defaultValue) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return defaultValue;
    }
    return kv->getFloat(key, defaultValue);
}

MMKV_EXPORT bool SetString(void *ptr, const char *key, const char *v) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return false;
    }
    return kv->set(v ? v : "", key);
}

MMKV_EXPORT const char *GetString(void *ptr, const char *key, const char *defaultValue) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return defaultValue ? strdup(defaultValue) : nullptr;
    }
    std::string result;
    if (kv->getString(key, result)) {
        return strdup(result.c_str());
    }
    return defaultValue ? strdup(defaultValue) : nullptr;
}

MMKV_EXPORT bool HasKey(void *ptr, const char *key) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return false;
    }
    return kv->containsKey(key);
}

MMKV_EXPORT void DeleteKey(void *ptr, const char *key) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv || !key) {
        return;
    }
    kv->removeValueForKey(key);
}

MMKV_EXPORT void DeleteAll(void *ptr, bool keepSpace) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv) {
        return;
    }
    kv->clearAll(keepSpace);
}

MMKV_EXPORT void MMKVClose(void *ptr) {
    auto kv = static_cast<MMKV *>(ptr);
    if (!kv) {
        return;
    }
    kv->close();
}

MMKV_EXPORT int FreeString(const char *str) {
    if (str) {
        free(const_cast<char *>(str));
    }
    return 0;
}

MMKV_EXPORT bool RemoveStorage(const char *mmapId, const char *path) {
    if (!mmapId) {
        return false;
    }
    MMKVPath_t mmkvPath = ToMMKVPath(path);
    MMKVPath_t *pathPtr = (path && path[0]) ? &mmkvPath : nullptr;
    return MMKV::removeStorage(std::string(mmapId), pathPtr);
}
