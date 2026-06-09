using System;
using System.Runtime.InteropServices;

namespace FIR.MMKV {

    public class MMKV {
        private string mmapId;
        private string path;
        private IntPtr mmkvPtr;
        private static string rootDir;
        private static bool isInitialized = false;

        public MMKV(string mmapId, string path) {
            if (string.IsNullOrEmpty(mmapId)) {
                throw new Exception("mmapId cannot be null");
            }

            if (!isInitialized) {
                throw new Exception("mmkv not initialized");
            }

            if (!string.IsNullOrEmpty(path)) {
                this.path = rootDir + "/" + path;
            } else {
                this.path = rootDir;
            }

            this.mmapId = mmapId;
            mmkvPtr = Native_MMKVWithID(this.mmapId, this.path);
            if (mmkvPtr == IntPtr.Zero) {
                throw new Exception("mmkv MMKVWithID failed");
            }
        }

        public static void InitializeMMKV(string m_rootDir) {
            Native_InitializeMMKV(m_rootDir);
            rootDir = m_rootDir;
            isInitialized = true;
        }

        public bool IsNull() {
            return mmkvPtr == IntPtr.Zero;
        }

        public bool SetBool(string key, bool v) {
            return Native_SetBool(mmkvPtr, key, v);
        }

        public bool SetInt(string key, int v) {
            return Native_SetInt(mmkvPtr, key, v);
        }

        public bool SetUInt(string key, uint v) {
            return Native_SetUInt(mmkvPtr, key, v);
        }

        public bool SetLong(string key, long v) {
            return Native_SetLong(mmkvPtr, key, v);
        }

        public bool SetULong(string key, ulong v) {
            return Native_SetULong(mmkvPtr, key, v);
        }

        public bool SetFloat(string key, float v) {
            return Native_SetFloat(mmkvPtr, key, v);
        }

        public bool SetString(string key, string v) {
            return Native_SetString(mmkvPtr, key, v);
        }

        public bool GetBool(string key, bool defaultValue = false) {
            return Native_GetBool(mmkvPtr, key, defaultValue);
        }

        public int GetInt(string key, int defaultValue = 0) {
            return Native_GetInt(mmkvPtr, key, defaultValue);
        }

        public uint GetUInt(string key, uint defaultValue = 0) {
            return Native_GetUInt(mmkvPtr, key, defaultValue);
        }

        public long GetLong(string key, long defaultValue = 0) {
            return Native_GetLong(mmkvPtr, key, defaultValue);
        }

        public ulong GetULong(string key, ulong defaultValue = 0) {
            return Native_GetULong(mmkvPtr, key, defaultValue);
        }

        public float GetFloat(string key, float defaultValue = 0) {
            return Native_GetFloat(mmkvPtr, key, defaultValue);
        }

        public string GetString(string key, string defaultValue = "") {
            IntPtr resPtr = Native_GetString(mmkvPtr, key, defaultValue);
            return IntPtrToStringAnsi(resPtr);
        }

        public bool HasKey(string key) {
            return Native_HasKey(mmkvPtr, key);
        }

        public void DeleteKey(string key) {
            Native_DeleteKey(mmkvPtr, key);
        }

        public void DeleteAll(bool keepSpace = false) {
            Native_DeleteAll(mmkvPtr, keepSpace);
        }

        public void Close() {
            Native_MMKVClose(mmkvPtr);
        }

        public bool RemoveStorage() {
            return Native_RemoveStorage(mmapId, path);
        }

        #region libmmkv
#if UNITY_IOS && !UNITY_EDITOR
    private const string dll = "__Internal";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    private const string dll = "libmmkv.dylib";
#elif UNITY_ANDROID
        private const string dll = "mmkv";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    private const string dll = "mmkv";
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
    private const string dll = "libmmkv.so";
#else
    private const string dll = "mmkv";
#endif

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "InitializeMMKV")]
        private static extern void Native_InitializeMMKV(string rootDir);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "MMKVWithID")]
        private static extern IntPtr Native_MMKVWithID(string mmapId, string path);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetString")]
        private static extern bool Native_SetString(IntPtr mmkvPtr, string key, string v);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetString")]
        private static extern IntPtr Native_GetString(IntPtr mmkvPtr, string key, string defaultValue);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetBool")]
        private static extern bool Native_SetBool(IntPtr mmkvPtr, string key, bool v);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetBool")]
        private static extern bool Native_GetBool(IntPtr mmkvPtr, string key, bool defaultValue);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetInt")]
        private static extern bool Native_SetInt(IntPtr mmkvPtr, string key, int v);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetInt")]
        private static extern int Native_GetInt(IntPtr mmkvPtr, string key, int defaultValue);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetUInt")]
        private static extern bool Native_SetUInt(IntPtr mmkvPtr, string key, uint v);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetUInt")]
        private static extern uint Native_GetUInt(IntPtr mmkvPtr, string key, uint defaultValue);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetLong")]
        private static extern bool Native_SetLong(IntPtr mmkvPtr, string key, long v);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLong")]
        private static extern long Native_GetLong(IntPtr mmkvPtr, string key, long defaultValue);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetULong")]
        private static extern bool Native_SetULong(IntPtr mmkvPtr, string key, ulong v);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetULong")]
        private static extern ulong Native_GetULong(IntPtr mmkvPtr, string key, ulong defaultValue);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetFloat")]
        private static extern bool Native_SetFloat(IntPtr mmkvPtr, string key, float v);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetFloat")]
        private static extern float Native_GetFloat(IntPtr mmkvPtr, string key, float defaultValue);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "HasKey")]
        private static extern bool Native_HasKey(IntPtr mmkvPtr, string key);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "DeleteKey")]
        private static extern void Native_DeleteKey(IntPtr mmkvPtr, string key);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "DeleteAll")]
        private static extern void Native_DeleteAll(IntPtr mmkvPtr, bool keepSpace);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "MMKVClose")]
        private static extern void Native_MMKVClose(IntPtr mmkvPtr);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FreeString")]
        private static extern int Native_FreeString(IntPtr str);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "RemoveStorage")]
        private static extern bool Native_RemoveStorage(string mmapId, string path);

        private static string IntPtrToStringAnsi(IntPtr ptr) {
            string res = Marshal.PtrToStringAnsi(ptr);
            Native_FreeString(ptr);
            return res;
        }
        #endregion
    }

}