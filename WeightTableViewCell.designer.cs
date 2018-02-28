// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace HealthKitPoc
{
    [Register ("WeightTableViewCell")]
    partial class WeightTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelDate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView line1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView line2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel WeightLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (LabelDate != null) {
                LabelDate.Dispose ();
                LabelDate = null;
            }

            if (line1 != null) {
                line1.Dispose ();
                line1 = null;
            }

            if (line2 != null) {
                line2.Dispose ();
                line2 = null;
            }

            if (WeightLabel != null) {
                WeightLabel.Dispose ();
                WeightLabel = null;
            }
        }
    }
}