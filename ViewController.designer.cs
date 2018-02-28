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
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton addButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TableWeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        HealthKitPoc.ScrollableGraphView WeightGraph { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (addButton != null) {
                addButton.Dispose ();
                addButton = null;
            }

            if (TableWeight != null) {
                TableWeight.Dispose ();
                TableWeight = null;
            }

            if (WeightGraph != null) {
                WeightGraph.Dispose ();
                WeightGraph = null;
            }
        }
    }
}