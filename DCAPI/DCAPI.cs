using DCAPI.Gallery;
using DCAPI.REST;
using DCAPI.Sessions;
using System;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DCAPI {
    /// <summary>디시인사이드 API를 관리하는 클래스입니다.</summary>
    public class DCAPI {
        private Task<AppToken> token;

        /// <summary>새 디시인사이드 API를 생성합니다.</summary>
        public DCAPI() : this(RESTClient.Shared) { }

        /// <summary>해당 REST 클라이언트를 이용해 API를 생성합니다.</summary>
        /// <param name="rest">서버 연결시에 사용될 클라이언트 입니다.</param>
        public DCAPI(RESTClient rest) {
            REST = rest;
            token = AppToken.GetAsync(rest);
        }

        /// <summary>해당 REST 클라이언트와 토큰을 이용해 API를 생성합니다.</summary>
        /// <param name="rest">서버 연결시에 사용될 클라이언트 입니다.</param>
        /// <param name="token">서디시인사이드 앱을 인증하는 토큰입니다.</param>
        public DCAPI(RESTClient rest, AppToken token) {
            REST = rest;
            this.token = Task.FromResult(token);
        }

        /// <summary>서버 연결시에 사용될 클라이언트 입니다.</summary>
        public RESTClient REST { get; }

        /// <summary>디시인사이드 앱을 인증하는 토큰입니다.</summary>
        public AppToken Token {
            get => token.Result;
            set => token = Task.FromResult(value);
        }

        /// <summary>새로운 토큰으로 갱신합니다.</summary>
        public void Update() 
            => token = AppToken.GetAsync(REST);

        /// <summary>갤러리의 번호를 통해 게시글을 가져옵니다.</summary>
        /// <param name="id">갤러리 Id입니다.</param>
        /// <param name="no">게시글의 번호입니다.</param>
        /// <returns>해당 갤러리와 번호의 게시글입니다</returns>
        public Article GetArticle(string id, long no)
            => new (REST, Token, id, no);

    }
}
