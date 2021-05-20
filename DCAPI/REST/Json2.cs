using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace DCAPI.REST {
    /// <summary>디시인사이드 갤러리 정보 API (http://json2.dcinside.com/)</summary>
    public static class Json2 {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> AppCheck(RESTClient rest)
            => rest.GetApp("https://json2.dcinside.com/json0/app_check_A_rina.php");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> AppCheckBeta(RESTClient rest)
            => rest.GetApp("https://json2.dcinside.com/json0/app_check_A_rina_beta.php");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> UpdateNotice(RESTClient rest)
            => rest.GetApp("http://json2.dcinside.com/json0/update_notice_A_rina.php");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> AppNotice(RESTClient rest)
            => rest.GetApp("http://json2.dcinside.com/json0/app_dc_notice.php");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> MainContent(RESTClient rest)
            => rest.GetApp("http://json2.dcinside.com/json3/main_content.php");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> GalleryRanking(RESTClient rest)
            => rest.GetApp("http://json2.dcinside.com/json1/ranking_gallery.php");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> MinorRanking(RESTClient rest)
            => rest.GetApp("http://json2.dcinside.com/json1/mgallmain/mgallery_ranking.php");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> CategoryNames(RESTClient rest)
            => rest.GetApp("http://json2.dcinside.com/json3/category_name.php");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> GalleryNames(RESTClient rest)
            => rest.GetApp("http://json2.dcinside.com/json3/gall_name.php");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonElement> AdCharge(RESTClient rest)
            => rest.GetApp("http://json2.dcinside.com/json1/app_ad_charge.php");
    }
}
