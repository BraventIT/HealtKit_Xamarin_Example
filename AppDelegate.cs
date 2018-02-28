using Foundation;
using HealthKit;
using UIKit;

namespace HealthKitPoc
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
		// class-level declarations
		private HKHealthStore healthKitStore = new HKHealthStore();


		public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
			//ValidateAuthorization();
			
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        private void ValidateAuthorization()
        {
            var bodyMassType = HKQuantityType.Create(HKQuantityTypeIdentifier.BodyMass);
            var typesToWrite = new NSSet(new[] { bodyMassType });
            var typesToRead = new NSSet(new[] { bodyMassType});
			healthKitStore.RequestAuthorizationToShare(
					typesToWrite,
					typesToRead,
					ReactToHealthCarePermissions);
       
		}

		void ReactToHealthCarePermissions(bool success, NSError error)
		{
            var access = healthKitStore.GetAuthorizationStatus(HKQuantityType.Create(HKQuantityTypeIdentifier.BodyMass));
			if (access.HasFlag(HKAuthorizationStatus.SharingAuthorized))
			{
				BodyMassModel.Instance.Enabled = true;
			}
			else
			{
				BodyMassModel.Instance.Enabled = false;
			}
            string temp=error.Description;
		}
    }
}

