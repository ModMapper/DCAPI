using DCAPI.REST;
using DCAPI.Sessions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DCAPI.Gallery {
    /// <summary>디시인사이드의 갤러리입니다.</summary>
    public record Gallery(RESTClient REST, AppToken Token, string Id) {

        public async Task<(bool result, string cause, Article article)> Write([NotNull] IUser user, string subject, string memo) {
            var json = await Upload.GalleryWrite(REST, Id, Token.AppId, "write", Token.ClientToken,
                subject, user.Name, user.Password, user.UserId, new string[] { memo }, null, null, "", 0);
            var (result, cause) = DCException.GetResult(json);
            if(result)
                return (true, null, new(REST, Token, Id, json.GetInt64("cause")));
            else
                return (false, cause, null);
        }
    }
}
