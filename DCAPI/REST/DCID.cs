#nullable enable
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace DCAPI.REST {
    public static class DCID {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> AppKey(RESTClient rest, string value_token, string? signature, string? pkg, long? vCode, string? vName, string client_token)
            => rest.PostApp("https://dcid.dcinside.com/join/mobile_app_key_verification_3rd.php",
                new { value_token, signature, pkg, vCode, vName, client_token });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> Login(RESTClient rest, string user_id, string user_pw, string mode, string? client_token)
            => rest.PostApp("https://dcid.dcinside.com/join/mobile_app_login.php",
                new { user_id, user_pw, mode, client_token });
    }
}
