using System;
using System.Runtime.InteropServices;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using GalleryApp.Core;

namespace GalleryAppiOS
{
    public partial class MainViewController : DialogViewController
    {
        private UIImagePickerController _picker;
        private const string UploadUrl = "http://192.168.1.103/api/photo";
        private readonly PhotoUploader _uploader;
        private readonly PhotoListener _listener;

        public MainViewController() : base (UITableViewStyle.Grouped, null)
        {
            Root = new RootElement("RealTime Gallery")
            {
                new Section ()
                {
                    new StringElement("Upload a picture", UploadPicture)
				}
            };

            _uploader = new PhotoUploader();
            _listener = new PhotoListener();

            _listener.NewPhotosReceived += (sender, urls) =>
            {
                InvokeOnMainThread(() =>
                {
                    foreach (var url in urls)
                        new UIAlertView("New Photo", url, null, "Ok", null).Show();
                });
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _listener.StartListening();
        }

        private void UploadPicture()
        {
            _picker = new UIImagePickerController();
            _picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            _picker.FinishedPickingMedia += (s, e) =>
            {
                _picker.DismissViewController(true, null);
                var image = (UIImage)e.Info.ObjectForKey(new NSString("UIImagePickerControllerOriginalImage"));
                byte[] bytes;
                using (var imageData = image.AsJPEG())
                {
                    bytes = new byte[imageData.Length];
                    Marshal.Copy(imageData.Bytes, bytes, 0, Convert.ToInt32(imageData.Length));
                }

                _uploader.UploadPhoto(bytes, "jpg");
            };

            PresentViewController(_picker, true, null);
        }
    }
}