using Foundation;
using System;
using UIKit;
using System.ComponentModel;
using ObjCRuntime;
using System.Collections.Generic;
using System.Linq;

namespace HealthKitPoc
{
    public partial class ScrollableGraphView : UIView, System.ComponentModel.IComponent
    {
        #region IComponent implementation
        public ISite Site { get; set; }
        public event EventHandler Disposed;
        #endregion

        [Export("initWithCoder:")]
        public ScrollableGraphView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        public ScrollableGraphView(IntPtr h) : base(h)
        {
            Initialize();
        }

        public ScrollableGraphView()
        {
            Initialize();
        }

        void Initialize()
        {
            var arr = NSBundle.MainBundle.LoadNib("ScrollableGraphView", this, null);
            var v = Runtime.GetNSObject<UIView>(arr.ValueAt(0));
            v.Frame = new CoreGraphics.CGRect(0, 0, Frame.Width, Frame.Height);
            AddSubview(v);

            Scroll.ShowsHorizontalScrollIndicator = false;
            /*Scroll.Scrolled+=delegate {
                dataView?.SetNeedsDisplay();    
            };*/
        }
            GraphDataView dataView;
        public void SetData(List<WeightData> data,double initial,double target)
        {
            Title.SetData(initial,target,"Peso inicial","Objetivo","kg");
            double min = data.Min(x => x.Value);
            if (initial < min)
                min = initial;
            if (target < min)
                min = target;
            min = min - min % 5-5;
			double max = data.Max(x => x.Value);
			if (initial > max)
				max = initial;
			if (target > max)
				max = target;
            max = max +(5- max % 5) + 5;
            //while ((max - min) <30)
            //    min = min - 5;
            List<float> axisValues = new List<float>() { (float)(min), (float)((max - min) / 4 + min), (float)(2 * (max - min) / 4 + min),(float)(3 * (max - min) / 4 + min), (float)( max) };
            Axis.SetData(axisValues);

            int visibleDays = 60;
			DateTime minDate = data.Min(x => x.Date);
			minDate = minDate.AddHours(-minDate.Hour).AddMinutes(-minDate.Minute).AddSeconds(-minDate.Second).AddMilliseconds(-minDate.Millisecond);
            DateTime maxDate = DateTime.Now.AddDays(1);
			maxDate = maxDate.AddHours(-maxDate.Hour).AddMinutes(-maxDate.Minute).AddSeconds(-maxDate.Second).AddMilliseconds(-maxDate.Millisecond);
            while (maxDate.AddDays(1).Day > 1)
                maxDate = maxDate.AddDays(1);
            while (maxDate.Ticks - minDate.Ticks < visibleDays * TimeSpan.TicksPerDay)
				minDate=minDate.AddDays(-1);
            while(minDate.Day>1)
                minDate=minDate.AddDays(-1);
			int days = (int)((maxDate.Ticks - minDate.Ticks) / TimeSpan.TicksPerDay)+1;
            nfloat dayWidth = Scroll.Frame.Width / visibleDays;
            if (dataView != null)
                dataView.RemoveFromSuperview();
			dataView = new GraphDataView();
			dataView.Frame = new CoreGraphics.CGRect(0, 0, days * dayWidth, Scroll.Frame.Height);		
            dataView.SetData(minDate,days,data,max,min,initial,target);
            Scroll.ContentSize = new CoreGraphics.CGSize(days * dayWidth, Scroll.Frame.Height);
			Scroll.ScrollRectToVisible(new CoreGraphics.CGRect(days * dayWidth - Scroll.Frame.Width, 0, Scroll.Frame.Width, Scroll.Frame.Height), false);			
            Scroll.Add(dataView);

                SetNeedsDisplay();
            DateTime maxDataDate = data.Max(x => x.Date);
            nfloat pos = dataView.GetPositionForDate(maxDataDate.AddDays(-visibleDays+1));
            Scroll.ScrollRectToVisible(new CoreGraphics.CGRect(pos, 0, Scroll.Frame.Width, Scroll.Frame.Height), false);
		}
    }
}