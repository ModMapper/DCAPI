using DCAPI.REST;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DCAPI.Sessions {
    /// <summary>디시인사이드 앱에 사용되는 토큰을 관리하는 클래스입니다</summary>
    public class AppToken {
        private const string    signature   = "ReOo4u96nnv8Njd7707KpYiIVYQ3FlcKHDJE046Pg6s=";
        private const string    package     = "com.dcinside.app";
        private const long      vCode       = 30413; //30207;
        private const string    vName       = "4.2.6"; //"3.8.12";

        private static readonly SHA256 sha = SHA256.Create();

        /*
        private static readonly Lazy<AppToken> _Shared
            = new(() => new AppToken());

        /// <summary>토큰 미 지정시 공용으로 사용되는 앱 토큰입니다.</summary>
        public static AppToken Shared => _Shared.Value;
        */

        /// <summary>새 토큰을 발급합니다.</summary>
        public AppToken() : this(RESTClient.Shared) { }

        /// <summary>해당 클라이언트를 사용해 새 토큰을 발급합니다.</summary>
        /// <param name="rest">서버 연결시에 사용될 클라이언트 입니다.</param>
        public AppToken(RESTClient rest) {
            ClientToken = FCM.GetToken(rest).Result ?? FCM.GetTempToken();
            var json = DCID.AppKey(rest, GetValueToken(rest).Result,
                signature, package, vCode, vName, ClientToken).Result; //Fallback...
            DCException.CheckResult(json);
            AppId = json.GetString("app_id");
            Expires = DateTime.Now.AddHours(11);
        }

        /// <summary>ClientToken과 AppId를 사용해 새 토큰을 생성합니다..</summary>
        /// <param name="clienttoken">알림 수신에 사용되는 FCM의 Token값입니다.</param>
        /// <param name="appid">앱 인증에 사용되는 AppId값입니다.</param>
        public AppToken(string clienttoken, string appid)
            => (ClientToken, AppId, Expires) = (clienttoken, appid, DateTime.Now.AddHours(11));

        /// <summary>ClientToken과 AppId 그리고 만료일을 통해 새 토큰을 생성합니다.</summary>
        /// <param name="clienttoken">알림 수신에 사용되는 FCM의 Token값입니다.</param>
        /// <param name="appid">앱 인증에 사용되는 AppId값입니다.</param>
        /// <param name="expires">해당 토큰의 만료 기한입니다.</param>
        protected AppToken(string clienttoken, string appid, DateTime expires)
            => (ClientToken, AppId, Expires) = (clienttoken, appid, expires);

        /// <summary>스트림으로부터 직렬화된 토큰을 읽어들입니다.</summary>
        /// <param name="stream">토큰을 읽어들일 스트림입니다.</param>
        public AppToken(Stream stream) {
            using var doc = JsonDocument.Parse(stream);
            var element = doc.RootElement;
            ClientToken = element.GetProperty("clienttoken").GetString();
            AppId = element.GetProperty("appid").GetString();
            Expires = element.GetProperty("expires").GetDateTime();
        }

        /// <summary>앱 인증에 사용되는 AppId입니다.</summary>
        public string AppId { get; }

        /// <summary>알림 수신에 사용되는 FCM의 Token값입니다.</summary>
        public string ClientToken { get; }

        /// <summary>해당 토큰이 생성된 시간입니다.</summary>
        public DateTime Expires { get; }

        /// <summary>해당 토큰을 스트림에 직렬화하여 작성합니다.</summary>
        /// <param name="stream">토큰을 작성할 스트림입니다.</param>
        public void WriteTo(Stream stream) {
            using var writer = new Utf8JsonWriter(stream);
            writer.WriteStartObject();
            writer.WriteString("clienttoken", ClientToken);
            writer.WriteString("appid", AppId);
            writer.WriteString("expires", Expires);
            writer.WriteEndObject();
            writer.Flush();
        }

        /// <summary>새 토큰을 비동기적으로 발급합니다.</summary>
        /// <param name="rest">서버 연결시에 사용될 클라이언트 입니다.</param>
        /// <returns>발급된 새 토큰입니다.</returns>
        public static async Task<AppToken> GetAsync(RESTClient rest) {
            var client = await FCM.GetToken(rest) ?? FCM.GetTempToken();
            var json = await DCID.AppKey(rest, await GetValueToken(rest),
                signature, package, vCode, vName, client); //Fallback...
            DCException.CheckResult(json);
            return new AppToken(client, json.GetString("app_id"));
        }

        /// <summary>ClientToken을 사용하여 토큰을 발급받습니다.</summary>
        /// <param name="rest">서버 연결시에 사용될 클라이언트 입니다.</param>
        /// <param name="clienttoken">알림 수신에 사용되는 FCM의 Token값입니다.</param>
        /// <returns>발급된 새 토큰입니다.</returns>
        public static async Task<AppToken> FromClientToken(RESTClient rest, string clienttoken) {
            var json = await DCID.AppKey(rest, await GetValueToken(rest),
                signature, package, vCode, vName, clienttoken); //Fallback...
            DCException.CheckResult(json);
            return new AppToken(clienttoken, json.GetString("app_id"));
        }

        /// <summary>스트림으로부터 직렬화된 토큰을 읽어들입니다.</summary>
        /// <param name="stream">토큰을 읽어들일 스트림입니다.</param>
        /// <returns>스트림으로부터 읽어들인 앱 토큰입니다. 실패시 null을 반환합니다.</returns>
        public static async Task<AppToken> FromStream(Stream stream) {
            try {
                using var doc = await JsonDocument.ParseAsync(stream);
                var element = doc.RootElement;
                return new(
                        element.GetProperty("clienttoken").GetString(),
                        element.GetProperty("appid").GetString(),
                        element.GetProperty("expires").GetDateTime());
            } catch {
                return null;
            }
        }

        /// <summary>서버로부터 인증시 사용될 ValueToken을 생성합니다.</summary>
        /// <param name="rest">서버 연결시에 사용될 클라이언트 입니다.</param>
        /// <returns>생성된 ValueToken값 입니다.</returns>
        protected static async Task<string> GetValueToken(RESTClient rest) {
            const string prefix = "dcArdchk_";
            var time = (await GetServerTime(rest)) ?? GetClientTime();  //Fallback...?
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes($"{prefix}{time}"));
            return string.Join(null, System.Linq.Enumerable.Select(hash, (i) => i.ToString("x2")));
        }

        /// <summary>서버로부터 Date문자열을 받아옵니다.</summary>
        /// <param name="rest">서버 연결시에 사용될 클라이언트 입니다.</param>
        /// <returns>디시인사이드 서버의 Date 문자열입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static async Task<string> GetServerTime(RESTClient rest)
            => (await Json2.AppCheck(rest)).GetString("date");

        /// <summary>디시인사이드 규격의 Date문자열을 생성합니다.</summary>
        /// <returns>디시인사이드 규격의 Date 문자열입니다.</returns>
        protected static string GetClientTime() {
            //https://github.com/organization/KotlinInside/issues/3
            //KotlinInside의 dateToString
            //사실 사용되지는 않을 듯 하지만 혹시 싶어서 만들어 놓음
            var date = DateTime.UtcNow.AddHours(9); //UTC +09:00
            var week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            var name = DateTimeFormatInfo.InvariantInfo.GetAbbreviatedDayName(date.DayOfWeek);
            var day = (int)date.DayOfWeek;
            return $"{name}{date.DayOfYear - 1}{date.Day}{((day + 6) % 7) + 1}{day}{week - 1:d2}{date:MddMM}";
        }
    }
}
