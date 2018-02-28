// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace HealthKitPoc
{
	[Register ("AddExerciseViewController")]
	partial class AddExerciseViewController
	{
		[Outlet]
		UIKit.UIButton btnSabe { get; set; }

		[Outlet]
		UIKit.UITextField dateEnd { get; set; }

		[Outlet]
		UIKit.UITextField dateInit { get; set; }

		[Outlet]
		UIKit.UITextView txfData { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (dateInit != null) {
				dateInit.Dispose ();
				dateInit = null;
			}

			if (dateEnd != null) {
				dateEnd.Dispose ();
				dateEnd = null;
			}

			if (btnSabe != null) {
				btnSabe.Dispose ();
				btnSabe = null;
			}

			if (txfData != null) {
				txfData.Dispose ();
				txfData = null;
			}
		}
	}
}
