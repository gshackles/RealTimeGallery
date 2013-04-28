using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Gallery.Storage
{
    public class BlobStorageProvider : MultipartFileStreamProvider
    {
        private readonly CloudBlobContainer _container;

        public BlobStorageProvider(CloudBlobContainer container)
            : base(Path.GetTempPath())
        {
            _container = container;
            Urls = new List<string>();
        }

        public IList<string> Urls { get; private set; } 

        public override Task ExecutePostProcessingAsync()
        {
            foreach (var file in FileData)
            {
                string fileName = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));
                var blob = _container.GetBlockBlobReference(fileName);

                using (var stream = File.OpenRead(file.LocalFileName))
                {
                    blob.UploadFromStream(stream);
                }

                File.Delete(file.LocalFileName);
                Urls.Add(blob.Uri.AbsoluteUri);
            }

            return base.ExecutePostProcessingAsync();
        }
    }
}