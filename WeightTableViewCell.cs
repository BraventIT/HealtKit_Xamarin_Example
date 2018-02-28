using System;

using Foundation;
using UIKit;

namespace HealthKitPoc
{
    public partial class WeightTableViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("WeightTableViewCell");
        public static readonly UINib Nib;

		static WeightTableViewCell()
		{
			Nib = UINib.FromName("WeightTableViewCell", NSBundle.MainBundle);
		}

		public static WeightTableViewCell Create()
		{
			return (WeightTableViewCell)Nib.Instantiate(null, null)[0];
		}

		protected WeightTableViewCell(IntPtr handle) : base(handle)
        {
			// Note: this .ctor should not contain any initialization logic.
		}

        public void SetWeight(WeightData h, int pos)
		{
            LabelDate.Text = h.Date.ToString("dd/MM/yy HH:mm");
            WeightLabel.Text = h.Value.ToString("0.0") + " " + h.Unit;
            if (pos == 0)
			{
				line1.Hidden = false;
			}
			else
			{
				line1.Hidden = true;
			}
		}

	}
}
