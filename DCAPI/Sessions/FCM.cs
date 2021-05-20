using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DCAPI.Sessions {
    /// <summary>구글 Firebase의 디시인사이드 FCM토큰을 가져오는 클래스입니다.</summary>
    public static class FCM {
        private const string TokenValue =
            "APA91bFMI-0d1b0wJmlIWoDPVa_V5Nv0OWnAefN7fGLegy6D76TN_CRo5RSUO-6V7Wnq44t7Rzx0A4kICVZ7wX-hJd3mrczE5NnLud722k5c-XRjIxYGVM9yZBScqE3oh4xbJOe2AvDe";
        private static readonly Random rand = new();

        /// <summary>새 FCM Token을 생성합니다.</summary>
        /// <returns>알림 수신에 사용되는 FCM토큰입니다/returns>
        public static Task<string> GetToken()
            => GetToken(REST.RESTClient.Shared);

        /// <summary>새 FCM Token을 생성합니다.</summary>
        /// <param name="rest">서버 연결시에 사용될 클라이언트 입니다.</param>
        /// <returns>알림 수신에 사용되는 FCM토큰입니다/returns>
        public static async Task<string> GetToken(REST.RESTClient rest) {
            var fid = await GetFId(rest);
            return fid == null ? null : $"{fid}:{TokenValue}";
        }

        /// <summary>새 FCM Id를 발급받습니다.</summary>
        /// <param name="rest">서버 연결시에 사용될 클라이언트 입니다.</param>
        /// <param name="fid">이전에 사용되었던 FCM Id입니다. null일시 사용되지 않습니다.</param>
        /// <returns>발급받은 새 FCM Id입니다.</returns>
        public static async Task<string> GetFId(REST.RESTClient rest, string fid = null) {
            const string url = "https://firebaseinstallations.googleapis.com/v1/projects/dcinside-b3f40/installations";
            using var req = new HttpRequestMessage(HttpMethod.Post, url);

            req.Headers.Add("X-Android-Package", "com.dcinside.app");
            req.Headers.Add("X-Android-Cert", "E6DA04787492CDBD34C77F31B890A3FAA3682D44");
            req.Headers.Add("x-goog-api-key", "AIzaSyDcbVof_4Bi2GwJ1H8NjSwSTaMPPZeCE38");

            req.Content = JsonContent.Create(new {
                fid,
                appId = "1:477369754343:android:1f4e2da7c458e2a7",
                authVersion = "FIS_v2",
                sdkVersion = "a:16.3.1" });

            try {
                return (await rest.Send(req)).GetString("fid");
            } catch {
                return null;
            }
        }

        /// <summary>FCM 규격의 임시 토큰을 만듭니다. 알림 수신시에 사용할 수 없습니다.</summary>
        /// <returns>인증에서만 사용되는 FCM토큰입니다/returns>
        public static string GetTempToken() {
            //Random String
            Span<byte> buf = stackalloc byte[10];
            lock(rand) rand.NextBytes(buf);
            return $"{Convert.ToBase64String(buf)}:{TokenValue}";
        }
    }
}
