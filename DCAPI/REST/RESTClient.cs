using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DCAPI.REST {
    /// <summary>서버와 통신에 사용되는 REST클라이언트 입니다.</summary>
    public class RESTClient {
        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0";
        private const string AppUserAgent = "dcinside.app";
        private const string Referer = "http://www.dcinside.com";

        private readonly HttpClient http;

        private static readonly Lazy<HttpMessageHandler> handler
            = new(() => new SocketsHttpHandler() { UseCookies = false, AllowAutoRedirect = false });

        private static readonly Lazy<RESTClient> _Shared
            = new(() => new RESTClient());

        public static RESTClient Shared => _Shared.Value;

        /// <summary>새 REST 클라언트를 생성합니다.</summary>
        public RESTClient() 
            => http = new(handler.Value);

        /// <summary>해당 <see cref="SocketsHttpHandler"/>로부터 REST 클라이언트를 생성합니다.</summary>
        /// <param name="handler">서버와의 통신에 사용될 <see cref="SocketsHttpHandler"/>입니다.</param>
        public RESTClient(SocketsHttpHandler handler) {
            handler.UseCookies = false;
            handler.AllowAutoRedirect = false;
            http = new HttpClient(handler);
        }

        /// <summary>해당 <see cref="HttpClientHandler"/>로부터 REST 클라이언트를 생성합니다.</summary>
        /// <param name="handler">서버와의 통신에 사용될 <see cref="HttpClientHandler"/>입니다.</param>
        public RESTClient(HttpClientHandler handler) {
            handler.UseCookies = false;
            handler.AllowAutoRedirect = false;
            http = new HttpClient(handler);
        }

        /// <summary>해당 URL로부터 일반 형식의 GET을 요청합니다.</summary>
        /// <param name="url">요청할 URL주소입니다.</param>
        /// <returns>해당 요청 결과에 대한 <see cref="JsonElement"/>입니다.</returns>
        public async Task<JsonElement> Get(string url) {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("User-Agent",   UserAgent);
            req.Headers.Add("Referer",      Referer);
            return await Send(req);
        }

        /// <summary>해당 URL로부터 앱 형식의 GET을 요청합니다.</summary>
        /// <param name="url">요청할 URL주소입니다.</param>
        /// <returns>해당 요청 결과에 대한 <see cref="JsonElement"/>입니다.</returns>
        public async Task<JsonElement> GetApp(string url) {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("User-Agent",   AppUserAgent);
            req.Headers.Add("Referer",      Referer);
            return await Send(req);
        }

        /// <summary>해당 URL로부터 XMLHttpRequset형식의 GET을 요청합니다.</summary>
        /// <param name="url">요청할 URL주소입니다.</param>
        /// <returns>해당 요청 결과에 대한 <see cref="JsonElement"/>입니다.</returns>
        public async Task<JsonElement> GetXHR(string url) {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("User-Agent",       UserAgent);
            req.Headers.Add("Referer",          Referer);
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            return await Send(req);
        }

        /// <summary>해당 URL로부터 일반 형식의 POST를 전송합니다.</summary>
        /// <param name="url">요청할 URL주소입니다.</param>
        /// <param name="form">해당 요청에 대한 폼 데이터입니다.</param>
        /// <returns>해당 요청 결과에 대한 <see cref="JsonElement"/>입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<JsonElement> Post(string url, object form)
            => Post(url, GetQuery(form));

        /// <summary>해당 URL로부터 앱 형식의 POST를 전송합니다.</summary>
        /// <param name="url">요청할 URL주소입니다.</param>
        /// <param name="form">해당 요청에 대한 폼 데이터입니다.</param>
        /// <returns>해당 요청 결과에 대한 <see cref="JsonElement"/>입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<JsonElement> PostApp(string url, object form)
            => PostApp(url, GetQuery(form));

        /// <summary>해당 URL로부터 XMLHttpRequset형식의 POST를 전송합니다.</summary>
        /// <param name="url">요청할 URL주소입니다.</param>
        /// <param name="form">해당 요청에 대한 폼 데이터입니다.</param>
        /// <returns>해당 요청 결과에 대한 <see cref="JsonElement"/>입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<JsonElement> PostXHR(string url, object form)
            => PostXHR(url, GetQuery(form));

        /// <summary>해당 URL로부터 XMLHttpRequset형식의 POST를 전송합니다.</summary>
        /// <param name="url">요청할 URL주소입니다.</param>
        /// <param name="content">해당 요청에 대한 <see cref="HttpContent"/>데이터 입니다.</param>
        /// <returns>해당 전송 결과에 대한 <see cref="JsonElement"/>입니다.</returns>
        public async Task<JsonElement> Post(string url, HttpContent content) {
            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("User-Agent", UserAgent);
            req.Headers.Add("Referer", Referer);
            req.Content = content;
            return await Send(req);
        }

        /// <summary>해당 URL로부터 XMLHttpRequset형식의 POST를 전송합니다.</summary>
        /// <param name="url">요청할 URL주소입니다.</param>
        /// <param name="content">해당 요청에 대한 <see cref="HttpContent"/>데이터 입니다.</param>
        /// <returns>해당 전송 결과에 대한 <see cref="JsonElement"/>입니다.</returns>
        public async Task<JsonElement> PostApp(string url, HttpContent content) {
            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("User-Agent",   AppUserAgent);
            req.Headers.Add("Referer",      Referer);
            req.Content = content;
            return await Send(req);
        }

        /// <summary>해당 URL로부터 XMLHttpRequset형식의 POST를 전송합니다.</summary>
        /// <param name="url">요청할 URL주소입니다.</param>
        /// <param name="content">해당 요청에 대한 <see cref="HttpContent"/>데이터 입니다.</param>
        /// <returns>해당 전송 결과에 대한 <see cref="JsonElement"/>입니다.</returns>
        public async Task<JsonElement> PostXHR(string url, HttpContent content) {
            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("User-Agent",       UserAgent);
            req.Headers.Add("Referer",          Referer);
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            req.Content = content;
            return await Send(req);
        }

        /// <summary>해당 <see cref="HttpRequestMessage"/>를 전송하고 <see cref="HttpResponseMessage"/>를 받아옵니다.</summary>
        /// <param name="req">전송할 <see cref="HttpRequestMessage"/>입니다.</param>
        /// <returns>해당 전송 결과에 대한 <see cref="JsonElement"/>입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual async Task<JsonElement> Send(HttpRequestMessage req) {
            using var res = await http.SendAsync(req);
            using var stream = await res.Content.ReadAsStreamAsync();
            if(stream.Length == 0) return default;
            var json = JsonDocument.Parse(stream).RootElement;
            return json.ValueKind == JsonValueKind.Array &&
                json.GetArrayLength() == 1 ? json[0] : json;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static HttpContent GetQuery(object form) {
            var sb = new StringBuilder();
            foreach(var item in form.GetType().GetProperties()) {
                var value = item.GetValue(form, null);
                if(value is not null) sb.Append($"{item.Name}={value}&");
            }
            if(0 < sb.Length) sb.Length--;
            return new StringContent(sb.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
        }
    }
}
