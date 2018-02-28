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
    [Register ("ScrollableGraphView")]
    partial class ScrollableGraphView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        HealthKitPoc.AxisView Axis { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView Scroll { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        HealthKitPoc.GraphTitleView Title { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Axis != null) {
                Axis.Dispose ();
                Axis = null;
            }

            if (Scroll != null) {
                Scroll.Dispose ();
                Scroll = null;
            }

            if (Title != null) {
                Title.Dispose ();
                Title = null;
            }
        }
    }
}