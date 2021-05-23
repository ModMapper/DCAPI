using DCAPI.REST;
using DCAPI.Sessions;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DCAPI.Gallery {
    /// <summary>디시인사이드의 갤러리입니다.</summary>
    public record Gallery(RESTClient REST, AppToken Token, string Id) {

        /// <summary>해당 갤러리에 게시글을 작성합니다.</summary>
        /// <param name="user">해당 게시글을 작성할 유저입니다.</param>
        /// <param name="subject">작성할 게시글의 제목입니다.</param>
        /// <param name="memo">작성할 게시글의 내용입니다.</param>
        /// <returns>게시글의 작성 성공 여부와 해당 게시글을 가져옵니다.</returns>
        public async Task<(bool result, string cause, Article article)> Write([NotNull] IUser user, string subject, string memo) {
            var json = await Upload.GalleryWrite(REST, Id, Token.AppId, "write", Token.ClientToken,
                subject, user.Name, user.Password, user.UserId, new string[] { memo }, null, null, "", 0);
            var (result, cause) = DCException.GetResult(json);
            if(result)
                return (true, null, new(REST, Token, Id, long.Parse(cause)));
            else
                return (false, cause, null);
        }
    }
}
