using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GalleryApp.Core
{
    public class PhotoUploader
    {
        private const string UploadUrl = "http://192.168.1.103/api/photo";

        public async Task UploadPhoto(byte[] photoBytes, string fileExtention)
        {
            var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(photoBytes);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Guid.NewGuid() + "." + fileExtention
            };
            content.Add(fileContent);

            using (var client = new HttpClient())
            {
                await client.PutAsync(UploadUrl, content);
            }
        }
    }
}