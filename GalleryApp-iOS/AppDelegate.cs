using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GalleryAppiOS
{
    [Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
    {
        private UIWindow _window;
        private UINavigationController _controller;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            _controller = new UINavigationController(new MainViewController());

            _window = new UIWindow(UIScreen.MainScreen.Bounds);
            _window.RootViewController = _controller;
            _window.MakeKeyAndVisible();
			
            return true;
        }
    }
}