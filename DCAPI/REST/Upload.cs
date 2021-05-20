#nullable enable
using System.Runtime.CompilerServices;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace DCAPI.REST {
    public static class Upload {
        public struct File {
            public string? FileName;
            public string? MediaType;
            public Stream Stream;

            public File(Stream stream, string? filename = null, string? mediatype = null)
                => (FileName, MediaType, Stream) = (filename, mediatype, stream);

            public StreamContent ToContent() {
                var content = new StreamContent(Stream);
                if(MediaType != null)
                    content.Headers.Add("Content-Type", MediaType);
                return content;
            }
        }

        public static async Task<JsonElement> GalleryWrite(RESTClient rest, string id, string app_id, string mode, string client_token,
                string subject, string? name, string? password, string? user_id, string[]? memo_block, File[]? upload, long?[]? detail_idx) {
            var form = new MultipartFormDataContent {
                { new StringContent(id), "id" },
                { new StringContent(app_id), "app_id" },
                { new StringContent(mode), "mode" },
                { new StringContent(client_token), "client_token" },
                { new StringContent(subject), "subject" } };
            form.AddNotNull(nameof(id), id);
            form.AddNotNull(nameof(app_id), app_id);
            form.AddNotNull(nameof(mode), mode);
            form.AddNotNull(nameof(client_token), client_token);
            form.AddNotNull(nameof(subject), subject);
            form.AddNotNull(nameof(name), name);
            form.AddNotNull(nameof(password), password);
            form.AddNotNull(nameof(user_id), user_id);
            if(memo_block != null) 
                for(int i = 0; i < memo_block.Length; i++)
                    form.AddNotNull($"memo_block[{i}]", memo_block[i]);
            if(upload != null)
                for(int i = 0; i < upload.Length; i++)
                    form.Add(upload[i].ToContent(), $"upload[{i}]");
            if(detail_idx != null)
                for(int i = 0; i < detail_idx.Length; i++)
                    form.AddNotNull($"detail_idx[{i}]", detail_idx[i].ToString());
            return await rest.PostApp("http://upload.dcinside.com/_app_write_api.php", form);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<JsonElement> UploadFile(RESTClient rest,
                string? best_chk, string gall_id, long gall_no, string mode, string? file_name, File upfile, string? user_no, string? user_id,
                string? comment_nick, string? password, string client_token, string? comment_txt, string app_id, int? down_chk) {
            var form = new MultipartFormDataContent();
            form.AddNotNull(nameof(best_chk), best_chk);
            form.AddNotNull(nameof(gall_id), gall_id);
            form.AddNotNull(nameof(gall_no), gall_no.ToString());
            form.AddNotNull(nameof(mode), mode);
            form.AddNotNull(nameof(file_name), file_name);
            if(upfile.FileName != null)
                form.Add(upfile.ToContent(), "upfile", upfile.FileName);
            else
                form.Add(upfile.ToContent(), "upfile");
            form.AddNotNull(nameof(user_no), user_no);
            form.AddNotNull(nameof(user_id), user_id);
            form.AddNotNull(nameof(comment_nick), comment_nick);
            form.AddNotNull(nameof(password), password);
            form.AddNotNull(nameof(client_token), client_token);
            form.AddNotNull(nameof(comment_txt), comment_txt);
            form.AddNotNull(nameof(app_id), app_id);
            form.AddNotNull(nameof(down_chk), down_chk.ToString());

            return await rest.PostApp("http://upload.dcinside.com/_app_upload.php", form);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddNotNull(this MultipartFormDataContent form, string name, string? value) {
            if(value != null && value.Length != 0)
                form.Add(new StringContent(value), name);
        }

    }
}
