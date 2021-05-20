using DCAPI.REST;
using DCAPI.Sessions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DCAPI.Gallery {
    /// <summary>디시인사이드 게시글의 댓글입니다.</summary>
    public record Comment (RESTClient REST, AppToken Token, string Id, long No, long CommentNo) {

        /// <summary>해당 댓글을 삭제합니다.</summary>
        /// <param name="user">해당 댓글을 삭제할 유저입니다.</param>
        /// <returns>해당 댓글의 삭제 성공 여부입니다.</returns>
        public async Task<(bool result, string cause)> DeleteComment([NotNull]IUser user)
            => DCException.GetResult(await App.CommentDelete(
                REST, user.Password, user.UserId, Token.ClientToken, Id, No, null, "comment_del", null, null, CommentNo, Token.AppId));
    }
}
