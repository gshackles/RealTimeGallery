using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Gallery.Storage;
using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Gallery.Controllers
{
    public class PhotoController : ApiController
    {
        private readonly CloudBlobContainer _container;

        public PhotoController()
        {
            _container = GetContainer();
        }

        public async Task Put()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
            }

            var provider = new BlobStorageProvider(_container);
            await Request.Content.ReadAsMultipartAsync(provider);

            var context = GlobalHost.ConnectionManager.GetHubContext<Hubs.Gallery>();

            context.Clients.All.newPhotosReceived(provider.Urls);
        }

        private CloudBlobContainer GetContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("CloudStorageConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("weddingpictures");

            container.CreateIfNotExists();

            var permissions = container.GetPermissions();
            if (permissions.PublicAccess == BlobContainerPublicAccessType.Off)
            {
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                container.SetPermissions(permissions);
            }

            return container;
        }
    }
}