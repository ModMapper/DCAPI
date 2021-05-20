#pragma warning disable IDE0049
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DCAPI {
    internal static class JsonExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this JsonElement element, string name)
            => element.TryGetProperty(name, out _);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(this JsonElement element, string name)
            => element.TryGetProperty(name, out var value) ? value.GetString() : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBoolean(this JsonElement element, string name)
            => element.TryGetProperty(name, out var value) 
                && (value.ValueKind == JsonValueKind.True);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 GetInt32(this JsonElement element, string name)
            => element.TryGetProperty(name, out var value) ?
                (value.TryGetInt32(out var ret) ? ret : default) : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 GetInt64(this JsonElement element, string name)
            => element.TryGetProperty(name, out var value) ?
                (value.TryGetInt64(out var ret) ? ret : default) : default;
    }
}
