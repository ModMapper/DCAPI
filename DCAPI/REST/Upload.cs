#nullable enable
using System.Runtime.CompilerServices;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

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

        public static async Task<JsonElement> GalleryWrite(RESTClient rest, string id, string app_id, string mode, string client_token, string subject,
                string? name, string? password, string? user_id, IList<string>? memo_block, IList<File>? upload, IList<long?>? detail_idx, string? fix, int? secret_use) {
            var form = new MultipartFormDataContent {
                { new StringContent(id), "id" },
                { new StringContent(app_id), "app_id" },
                { new StringContent(mode), "mode" },
                { new StringContent(client_token), "client_token" },
                { new StringContent(subject), "subject" } };
            form.AddValue(  "id",           id);
            form.AddValue(  "app_id",       app_id);
            form.AddValue(  "mode",         mode);
            form.AddValue(  "client_token", client_token);
            form.AddValue(  "subject",      subject);
            form.AddNull(   "name",         name);
            form.AddNull(   "password",     password);
            form.AddNull(   "user_id",      user_id);
            if(memo_block != null) 
                for(int i = 0; i < memo_block.Count; i++)
                    form.AddValue($"memo_block[{i}]", memo_block[i]);
            if(upload != null)
                for(int i = 0; i < upload.Count; i++)
                    form.Add(upload[i].ToContent(), $"upload[{i}]");
            if(detail_idx != null)
                for(int i = 0; i < detail_idx.Count; i++)
                    form.AddNull($"detail_idx[{i}]", detail_idx[i].ToString());
            form.AddNull("fix", fix);
            form.AddNull("secret_use", secret_use.ToString());
            return await rest.PostApp("http://upload.dcinside.com/_app_write_api.php", form);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<JsonElement> UploadFile(RESTClient rest,
                string? best_chk, string gall_id, long gall_no, string mode, string file_name, File upfile, string? user_no, string? user_id,
                string? comment_nick, string? password, string client_token, string? comment_txt, string app_id, int? down_chk) {
            var form = new MultipartFormDataContent();
            form.AddNull(   "best_chk",     best_chk);
            form.AddValue(  "gall_id",      gall_id);
            form.AddValue(  "gall_no",      gall_no.ToString());
            form.AddValue(  "mode",         mode);
            form.AddValue(  "file_name",    file_name);
            if(upfile.FileName != null)
                form.Add(upfile.ToContent(), "upfile", upfile.FileName);
            else
                form.Add(upfile.ToContent(), "upfile");
            form.AddNull(   "user_no",      user_no);
            form.AddNull(   "user_id",      user_id);
            form.AddNull(   "comment_nick", comment_nick);
            form.AddNull(   "password",     password);
            form.AddValue(  "client_token", client_token);
            form.AddNull(   "comment_txt",  comment_txt);
            form.AddValue(  "app_id",       app_id);
            form.AddNull(   "down_chk", down_chk.ToString());
            return await rest.PostApp("http://upload.dcinside.com/_app_upload.php", form);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddValue(this MultipartFormDataContent form, string name, string value) 
            =>form.Add(new StringContent(value), name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddNull(this MultipartFormDataContent form, string name, string? value) {
            if(value != null && value.Length != 0)
                form.Add(new StringContent(value), name);
        }
    }
}
