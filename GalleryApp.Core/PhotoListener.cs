using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System.Threading.Tasks;

namespace GalleryApp.Core
{
    public class PhotoListener
    {
        private const string Url = "http://192.168.1.103/signalr";
        private HubConnection _connection;
        private IHubProxy _proxy;

        public event EventHandler<IList<string>> NewPhotosReceived;

        public async Task StartListening()
        {
            _connection = new HubConnection(Url);
            _proxy = _connection.CreateHubProxy("gallery");

            _proxy.On<IList<string>>("newPhotosReceived", urls =>
                                     {
                if (NewPhotosReceived != null)
                    NewPhotosReceived.Invoke(this, urls);
            });

            await _connection.Start();
        }
    }
}

