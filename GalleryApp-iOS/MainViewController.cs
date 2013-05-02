using System;
using System.Runtime.InteropServices;
using GalleryApp.Core;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GalleryAppiOS
{
    public partial class MainViewController : DialogViewController
    {
        private const string UploadUrl = "http://192.168.1.103/api/photo";

        private UIImagePickerController _picker;
        private PhotoUploader _uploader;
        private PhotoListener _listener;
        private Section _imageSection;

        public MainViewController() 
            : base (UITableViewStyle.Grouped, null)
        {
            _imageSection = new Section();

            Root = new RootElement("RealTime Gallery")
            {
                _imageSection
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add);
            NavigationItem.RightBarButtonItem.Clicked += delegate { UploadPicture(); };

            _uploader = new PhotoUploader();
            _listener = new PhotoListener();

            _listener.NewPhotosReceived += (sender, urls) =>
            {
                InvokeOnMainThread(() =>
                {
                    foreach (var url in urls)
                    {
                        _imageSection.Add(
                            new ImageStringElement(DateTime.Now.ToString(), 
                                                   UIImage.LoadFromData(NSData.FromUrl(new NSUrl(url))))
                        );
                    }
                });
            };

            _listener.StartListening();
        }

        private void UploadPicture()
        {
            _picker = new UIImagePickerController();
            _picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            _picker.Canceled += delegate { _picker.DismissViewController(true, null); };
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