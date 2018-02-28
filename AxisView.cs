using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace HealthKitPoc
{
    [Register("AxisView"), DesignTimeVisible(true)]
	public class AxisView : UIView, IComponent
	{

		#region IComponent implementation

		public ISite Site { get; set; }
		public bool ShouldShowDateRange { get; set; }

		public event EventHandler Disposed;

		#endregion

        private nint paddingTop;
        private nint paddingBottom;
        private List<float> data = new List<float>() { 40, 60, 80, 100.5f };

		[Export("initWithCoder:")]
		public AxisView(NSCoder coder) : base(coder)
        {
			Initialize();
		}

		public AxisView(IntPtr h) : base(h)
        {
			Initialize();
		}

		public AxisView()
		{
			Initialize();
		}

		private void Initialize()
		{
            paddingTop = 8;
            paddingBottom = 18;
            SetNeedsDisplay();
		}

        public void SetData(List<float> data){
            this.data = data;
            SetNeedsDisplay();
        }

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);
			var context = UIGraphics.GetCurrentContext();
			context.SetShouldAntialias(true);
            nfloat restY = Frame.Height - paddingTop - paddingBottom;
            DrawAxis(context, Frame.Width-1,1,paddingTop,restY,UIColor.White);
            float max = data.Max();
            float min = data.Min();
            for (int i = 0; i < data.Count;i++){
                nfloat real = (data[i]-min) * (restY) / (max - min);
                DrawText(data[i].ToString("0.0"),Frame.Width-3,paddingTop+restY-real,12,context,TextRight,UIColor.White,Frame.Width-3,PosCenter);
            }
		}

		private static void DrawAxis(CGContext canvas, nfloat pos, nfloat axisWidth, nfloat startY, nfloat restY, UIColor color)
		{
			color.SetFill();
			color.SetStroke();
			canvas.SetLineWidth(0);
			canvas.FillRect(new CGRect(pos, startY, axisWidth, restY));
		}

		public string FontName { get; set; } = "Helvetica";

		private const int TextRight = 0;
		private const int TextCenter = 1;
		private const int TextLeft = 2;

		private const int PosTop = 0;
		private const int PosCenter = 1;
		private const int PosBottom = 2;

		private void DrawText(string t, nfloat x, nfloat y, nfloat fontSize, CGContext gctx, int alignement, UIColor color, nfloat width, int position = PosCenter)
		{
			gctx.SaveState();
			UIGraphics.PushContext(gctx);
			gctx.SetFillColor(color.CGColor);

			var tfont = UIFont.FromName(FontName, fontSize);

			var nsstr = new NSString(t);
			var sz = nsstr.StringSize(tfont, width, UILineBreakMode.WordWrap);
			while ((int)(sz.Width - width) == 0)
			{
				var temp = sz.Height;
				sz = nsstr.StringSize(tfont, new CGSize(width, sz.Height * 2.5f), UILineBreakMode.WordWrap);
				if (temp == sz.Height)
					break;
			}
			var mitad = sz.Height / 2;
			switch (position)
			{
				case PosTop:
					mitad = 0;
					break;
				case PosCenter:
					mitad = sz.Height / 2;
					break;
				case PosBottom:
					mitad = sz.Height;
					break;
			}
			CGRect rect;
			switch (alignement)
			{
				case TextCenter:
					rect = new CGRect(x - sz.Width / 2, y - mitad, sz.Width, sz.Height * 2.5f);
					nsstr.DrawString(rect, tfont, UILineBreakMode.WordWrap, UITextAlignment.Center);
					break;
				case TextLeft:
					rect = new CGRect(x, y - mitad, sz.Width, sz.Height);
					nsstr.DrawString(rect, tfont, UILineBreakMode.WordWrap, UITextAlignment.Left);
					break;
				case TextRight:
					rect = new CGRect(x - sz.Width, y - mitad, sz.Width, sz.Height);
					nsstr.DrawString(rect, tfont, UILineBreakMode.WordWrap, UITextAlignment.Right);
					break;
			}

			UIGraphics.PopContext();
			gctx.RestoreState();
		}
    }
}
