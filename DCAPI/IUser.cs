using DCAPI.REST;
using System.Text.Json;
using System.Threading.Tasks;

namespace DCAPI {
    /// <summary>디시인사이드 유동 및 로그인 멤버입니다.</summary>
    public interface IUser {
        /// <summary>해당 유저의 유동 닉네임입니다. 로그인시에는 null입니다.</summary>
        public abstract string Name { get; }

        /// <summary>해당 유저의 유동 비밀번호입니다. 로그인시에는 null입니다.</summary>
        public abstract string Password { get; }

        /// <summary>해당 유저의 로그인 UserId입니다. 비 로그인시에는 null입니다.</summary>
        public abstract string UserId { get; }
    }

    /// <summary>로그인이 되지 않은 유동 유저입니다.</summary>
    public record Guest(string Name, string Password) : IUser {
        string IUser.UserId => null;
    }

    /// <summary>로그인이 되어있는 멤버의 클래스입니다.</summary>
    public class Member : IUser {
        private Task<JsonElement> User;
        private readonly RESTClient rest;

        /// <summary>로그인되지 않은 비어있는 멤버를 생성합니다.</summary>
        public Member()
            : this(RESTClient.Shared) { }

        /// <summary>해당 아이디와 비밀번호를 사용해 로그인된 멤버를 생성합니다.</summary>
        /// <param name="id">로그인할 유저 아이디입니다.</param>
        /// <param name="pw">로그인할 유저 비밀번호입니다.</param>
        public Member(string id, string pw)
            : this(RESTClient.Shared, id, pw) { }

        /// <summary>로그인되지 않은 비어있는 멤버를 생성합니다.</summary>
        /// <param name="rest">서버 연결시에 사용될 클라이언트 입니다.</param>
        public Member(RESTClient rest)
            => this.rest = rest;

        /// <summary>해당 아이디와 비밀번호를 사용해 로그인된 멤버를 생성합니다.</summary>
        /// <param name="rest">서버 연결시에 사용될 클라이언트 입니다.</param>
        /// <param name="id">로그인할 유저 아이디입니다.</param>
        /// <param name="pw">로그인할 유저 비밀번호입니다.</param>
        public Member(RESTClient rest, string id, string pw) {
            this.rest = rest;
            DCException.CheckResult((User = DCID.Login(rest, id, pw, "login_normal", null)).Result);
        }

        string IUser.Name => null;

        string IUser.Password => null;

        /// <summary>해당 유저의 UserId입니다. 로그인 되지 않았을 시 null을 반환합니다.</summary>
        public string UserId => User?.Result.GetString("user_id");

        /// <summary>해당 유저의 닉네임입니다. 로그인 되지 않았을 시 null을 반환합니다.</summary>
        public string Name => User?.Result.GetString("username");

        /// <summary>해당 유저의 유저 넘버입니다. 로그인 되지 않았을 시 null을 반환합니다.</summary>
        public string UserNo => User?.Result.GetString("user_no");

        /// <summary>해당 아이디와 비밀번호를 사용해 로그인합니다.</summary>
        /// <param name="id">로그인할 유저 아이디입니다.</param>
        /// <param name="pw">로그인할 유저 비밀번호입니다.</param>
        /// <returns>해당 로그인의 결과입니다.</returns>
        public (bool result, string cause) Login(string id, string pw)
            => DCException.GetResult((User = DCID.Login(rest, id, pw, "login_normal", null)).Result);

        /// <summary>해당 아이디와 비밀번호를 사용해 로그인합니다.</summary>
        /// <param name="id">로그인할 유저 아이디입니다.</param>
        /// <param name="pw">로그인할 유저 비밀번호입니다.</param>
        /// <returns>해당 로그인의 결과입니다.</returns>
        public async Task<(bool result, string cause)> LoginAsync(string id, string pw) 
            => DCException.GetResult(await(User = DCID.Login(rest, id, pw, "login_normal", null)));

    }
}
