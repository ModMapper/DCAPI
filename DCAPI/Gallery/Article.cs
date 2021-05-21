using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DCAPI.REST;
using DCAPI.Sessions;

namespace DCAPI.Gallery {
    /// <summary>디시인사이드 게시글입니다.</summary>
    public record Article (RESTClient REST, AppToken Token, string Id, long No) {

        /// <summary>URL로부터 갤러리 Id와 게시글 번호를 가져옵니다.</summary>
        /// <param name="url">가져올 게시글의 URL문자열입니다.</param>
        /// <returns>해당 게시글의 갤러리 Id와 번호입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (string id, long no) ParseURL(string url) {
            if(!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return default;
            return ParseURL(uri);
        }

        /// <summary>URL로부터 갤러리 Id와 게시글 번호를 가져옵니다.</summary>
        /// <param name="uri">가져올 게시글의 Uri 데이터입니다.</param>
        /// <returns>해당 게시글의 갤러리 Id와 번호입니다.</returns>
        public static (string id, long no) ParseURL(Uri uri) {
            switch(uri.Host) {
            case "gall.dcinside.com": {
                switch(uri.AbsolutePath) {
                case "/mgallery/board/view/":
                case "/board/view/":
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    if(!Int64.TryParse(query["no"], out long no)) return default;
                    return (query["id"], no);
                }
                return default;
            }
            case "m.dcinside.com": {
                var segment = uri.AbsolutePath.Split('/', 5);
                if(segment.Length < 4 || segment[1] != "board") return default;
                if(!Int64.TryParse(segment[3], out long no)) return default;
                return (uri.Segments[2], no);
            }
            default:
                return default;
            }
        }


        /// <summary>해당 게시글을 삭제합니다.</summary>
        /// <param name="user">해당 게시글을 삭제할 비밀번호입니다.</param>
        /// <returns>해당 게시글의 삭제 성공 여부입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<(bool result, string cause)> Delete([NotNull] string password)
            => DCException.GetResult(await App.GalleryDelete(
                REST, password, null, Token.ClientToken, Id, No, "board_del", Token.AppId));

        /// <summary>해당 게시글을 삭제합니다.</summary>
        /// <param name="user">해당 게시글을 삭제할 유저입니다.</param>
        /// <returns>해당 게시글의 삭제 성공 여부입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<(bool result, string cause)> Delete([NotNull] IUser user)
            => DCException.GetResult(await App.GalleryDelete(
                REST, user.Password, user.UserId, Token.ClientToken, Id, No, "board_del", Token.AppId));

        /// <summary>해당 게시글을 추천합니다.</summary>
        /// <returns>해당 게시글의 추천 성공성공 여부입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<(bool result, string cause)> Recommend()
            => DCException.GetResult(await App.Recommend(REST, Id, null, No, Token.AppId));

        /// <summary>해당 게시글을 비추천합니다.</summary>
        /// <returns>해당 게시글의 비추천 성공 여부입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<(bool result, string cause)> NonRecommend()
            => DCException.GetResult(await App.NonRecommend(REST, Id, null, No, Token.AppId));

        /// <summary>해당 게시글을 힛갤 추천합니다.</summary>
        /// <returns>해당 게시글의 힛갤 추천 성공 여부입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<(bool result, string cause)> HitRecommend()
            => DCException.GetResult(await App.HitRecommend(REST, Id, No, null, Token.AppId));

        /// <summary>해당 게시글을 추천합니다.</summary>
        /// <param name="user">해당 게시글을 추천할 유저입니다. null의 경우 유저를 사용하지 않습니다.</param>
        /// <returns>해당 게시글의 추천 여부입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<(bool result, string cause)> Recommend(IUser user)
            => DCException.GetResult(await App.Recommend(
                REST, Id, user?.UserId, No, Token.AppId));

        /// <summary>해당 게시글을 비추천합니다.</summary>
        /// <param name="user">해당 게시글을 추천할 유저입니다. null의 경우 유저를 사용하지 않습니다.</param>
        /// <returns>해당 게시글의 비추천 성공 여부입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<(bool result, string cause)> NonRecommend(IUser user)
            => DCException.GetResult(await App.NonRecommend(
                REST, Id, user?.UserId, No, Token.AppId));

        /// <summary>해당 게시글을 힛갤 추천합니다.</summary>
        /// <param name="user">해당 게시글을 추천할 유저입니다. null의 경우 유저를 사용하지 않습니다.</param>
        /// <returns>해당 게시글의 힛갤 추천 성공 여부입니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<(bool result, string cause)> HitRecommend(IUser user)
            => DCException.GetResult(await App.HitRecommend(
                REST, Id, No, user?.UserId, Token.AppId));

        /// <summary>해당 게시글에 댓글을 작성합니다.</summary>
        /// <param name="user">해당 게시글에 댓글을 작성할 유저입니다.</param>
        /// <param name="memo">작성할 댓글의 내용입니다.</param>
        /// <returns>댓글의 작성 성공 여부와 번호를 가져옵니다.</returns>
        public async Task<(bool result, string cause, Comment comment)> WriteComment([NotNull]IUser user, string memo) {
            var json = await App.CommentOK(REST, Id, No, null, null, null, "com_write",
                user.Name, user.Password, user.UserId, Token.ClientToken, memo, null, Token.AppId);
            var (result, cause) = DCException.GetResult(json);
            return result ?
                (true, cause, new Comment(REST, Token, Id, No, json.GetInt64("data"))) : (false, cause, null);
        }

        /// <summary>해당 게시글에 댓글에 보이스 리플을 작성합니다.</summary>
        /// <param name="user">해당 게시글에 댓글을 작성할 유저입니다.</param>
        /// <param name="memo">작성할 댓글의 내용입니다.</param>
        /// <param name="stream">댓글에 작성할 사운드 파일의 스트림입니다.</param>
        /// <param name="filename">사운드 파일의 파일명입니다.</param>
        /// <param name="mediatype">사운드 파일의 미디어 타입입니다</param>
        /// <param name="download">보이스 리플 다운로드 가능 여부입니다.</param>
        /// <returns>댓글의 작성 성공 여부와 번호를 가져옵니다.</returns>
        public async Task<(bool result, string cause, Comment comment)>WriteVoiceComment([NotNull]IUser user, string memo, Stream stream, string filename, string mediatype, bool download) {
            var userno = (user is Member mem) ? mem.UserNo : null;
            var json = await Upload.UploadFile(REST, null, Id, No, "com_write", filename, new(stream, filename, mediatype),
                userno, user.UserId, user.Name, user.Password, Token.ClientToken, memo, Token.AppId, download ? 1 : null);
            var (result, cause) = DCException.GetResult(json);
            return result ?
                (true, cause, new Comment(REST, Token, Id, No, json.GetInt64("data"))) : (false, cause, null);
        }

    }
}
