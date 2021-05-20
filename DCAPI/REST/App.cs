#nullable enable
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace DCAPI.REST {
    public static class App {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> TotalSearch(RESTClient rest, string keyword, string app_id, string confirm_id)
            => rest.PostApp("http://app.dcinside.com/api/_total_search.php",
                new { keyword, app_id, confirm_id});

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> MyGall(RESTClient rest, string user_id, string app_id)
            => rest.PostApp("http://app.dcinside.com/api/mygall.php",
                new { user_id, app_id });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> GalleryList(RESTClient rest, string id, long page, string app_id, string confirm_id)
            => rest.PostApp("http://app.dcinside.com/api/gall_list_new.php",
                new { id, page, app_id, confirm_id });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> GalleryView(RESTClient rest, string id, long no, string app_id, string confirm_id)
            => rest.PostApp("http://app.dcinside.com/api/gall_view_new.php",
                new { id, no, app_id, confirm_id });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> RelationList(RESTClient rest, string id, string app_id)
            => rest.PostApp("http://app.dcinside.com/api/relation_list.php",
                new { id, app_id });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> Recommend(RESTClient rest, string id, string confirm_id, long no, string app_id)
            => rest.PostApp("http://app.dcinside.com/api/_recommend_up.php",
                new { id, confirm_id, no, app_id });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> NonRecommend(RESTClient rest, string id, string confirm_id, long no, string app_id)
            => rest.PostApp("http://app.dcinside.com/api/_recommend_down.php",
                new { id, confirm_id, no, app_id });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> HitRecommend(RESTClient rest, string id, long no, string confirm_id, string app_id)
            => rest.PostApp("http://app.dcinside.com/api/hit_recommend",
                new { id, no, confirm_id, app_id });

        public static Task<JsonElement> GalleryModify(RESTClient rest, string? password, string? user_id, string id, long no, string app_id)
            => rest.PostApp("http://app.dcinside.com/api/gall_modify.php",
                new { password, user_id, id, no, app_id });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> GalleryDelete(RESTClient rest, string? write_pw, string? user_id, string client_token, string id, long no, string mode, string app_id)
            => rest.PostApp("http://app.dcinside.com/api/gall_delete.php",
                new { write_pw, user_id, client_token, id, no, mode, app_id });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> CommentOK(RESTClient rest, string id, long no, string? board_id, string? best_chk, int? best_comno, string mode,
                string? comment_nick, string? comment_pw, string? user_id, string client_token, string? comment_memo, long? detail_idx, string app_id)
            => rest.PostApp("http://app.dcinside.com/api/comment_ok.php",
                new { id, no, board_id, best_chk, best_comno, mode, comment_nick, comment_pw, user_id, client_token, comment_memo, detail_idx, app_id });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> CommentDelete(RESTClient rest, string? comment_pw, string? user_id, string client_token, string id, long no,
                string board_id, string mode, string? best_chk, int? best_comno, long comment_no, string app_id)
            => rest.PostApp("http://app.dcinside.com/api/comment_del.php",
                new { comment_pw, user_id, client_token, id, no, board_id, mode, best_chk, best_comno, comment_no, app_id });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> DCCon(RESTClient rest, string? user_id, long? package_idx, long? detail_idx, string type, string app_id)
            => rest.PostApp("https://app.dcinside.com/api/dccon.php",
                new { user_id, package_idx, detail_idx, type, app_id });

    }
}
