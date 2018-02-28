using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace HealthKitPoc
{
    public partial class WeightController : UIViewController
    {
        public event WeightHandler OnAddWeight;
        public event WeightHandler OnDeleteWeight;
        public static WeightData Weight;
        private DateTime Date;

        public WeightController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            addButton.TouchUpInside += delegate
            {
                if (Weight == null)
                {
                    WeightData data = new WeightData();
                    String v = WeightField.Text;
                    try
                    {
                        data.Value = Double.Parse(v);
                        data.Unit = "kg";
                        data.Date = Date;
                        OnAddWeight?.Invoke(this, new WeightEventArgs(data));
                        DismissViewController(false, null);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    OnDeleteWeight?.Invoke(this, new WeightEventArgs(Weight));
                    DismissViewController(false, null);
                }
            };
            CancelButton.TouchUpInside += delegate
            {
                DismissViewController(false, null);
            };
			DateButton.TouchUpInside += (sender, e) =>
			{
                WeightField.ResignFirstResponder();
				OpenDatePicker();
			};
            if (Weight != null)
            {
                Date = Weight.Date;
                WeightField.Text = Weight.Value.ToString("F");
                addButton.SetTitle("Borrar", UIControlState.Normal);
            }
            else
            {
                Date = DateTime.Now;
            }
            DateButton.SetTitle(Date.ToShortDateString() + " " + Date.ToShortTimeString(), UIControlState.Normal);
        }

        public delegate void WeightHandler(Object sender, WeightEventArgs args);

        public class WeightEventArgs : EventArgs
        {
            public WeightData Weight;

            public WeightEventArgs(WeightData w)
            {
                this.Weight = w;
            }
        }

        private void OpenDatePicker()
        {
            UIView dateView = new UIView();
            dateView.BackgroundColor = UIColor.White;

            UIToolbar pickerToolbar = new UIToolbar(new CGRect(0, 0, View.Bounds.Width, 44));
            pickerToolbar.BarStyle = UIBarStyle.Default;
            pickerToolbar.Translucent = true;
            pickerToolbar.SizeToFit();

            UIDatePicker datePicker = new UIDatePicker();
            datePicker.Frame = new CGRect(0, 44, datePicker.Bounds.Width, datePicker.Bounds.Height);
            datePicker.Mode = UIDatePickerMode.DateAndTime;
            datePicker.Date = DateUtil.DateTimeToNSDate(Date);
            datePicker.BackgroundColor = UIColor.White;

            pickerToolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                new UIBarButtonItem("OK", UIBarButtonItemStyle.Done, (sender, eventArgs) => {
                    Date = DateUtil.NSDateToDateTime(datePicker.Date);
                    dateView.RemoveFromSuperview();
                    DateButton.SetTitle(Date.ToShortDateString()+" "+Date.ToShortTimeString(),UIControlState.Normal);
                })
            };

			dateView.Frame = new CGRect(0, View.Frame.Height - pickerToolbar.Bounds.Height - datePicker.Bounds.Height, View.Frame.Width, pickerToolbar.Bounds.Height + datePicker.Bounds.Height);
			dateView.AddSubview(pickerToolbar);
			dateView.AddSubview(datePicker);

			Add(dateView);

        }
    }
}