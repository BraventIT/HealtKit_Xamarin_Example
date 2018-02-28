using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace HealthKitPoc
{
    [Register("GraphTitleView"), DesignTimeVisible(true)]
	public class GraphTitleView : UIView, IComponent
	{

		#region IComponent implementation

		public ISite Site { get; set; }
		public bool ShouldShowDateRange { get; set; }

		public event EventHandler Disposed;

		#endregion

        private double initial=0;
        private double target=0;
        private String initialText="Peso inicial";
        private String targetText="Objetivo";
        private String units="kg";

		[Export("initWithCoder:")]
		public GraphTitleView(NSCoder coder) : base(coder)
        {
			Initialize();
		}

		public GraphTitleView(IntPtr h) : base(h)
        {
			Initialize();
		}

		public GraphTitleView()
		{
			Initialize();
		}

		private void Initialize()
		{
            SetNeedsDisplay();
		}

        public void SetData(double initial, double target, String initialText, String targetText, String units){
            this.initial = initial;
            this.target = target;
            this.initialText = initialText;
            this.targetText = targetText;
            this.units = units;
            SetNeedsDisplay();
        }

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);
			var context = UIGraphics.GetCurrentContext();
			context.SetShouldAntialias(true);

            var nsstr = new NSString(initialText);
            var tfont = UIFont.FromName(FontName, 12);
            var sz = nsstr.StringSize(tfont, Frame.Width, UILineBreakMode.WordWrap);
            nfloat offset = sz.Width + 5;
            nsstr = new NSString(initial.ToString("0.#"));
			tfont = UIFont.FromName(FontName, 18);
			var sz2 = nsstr.StringSize(tfont, Frame.Width, UILineBreakMode.WordWrap);			
			offset = offset+sz2.Width;
			nsstr = new NSString(units);
			tfont = UIFont.FromName(FontName, 12);
			var sz3 = nsstr.StringSize(tfont, Frame.Width, UILineBreakMode.WordWrap);			
            offset = offset + sz3.Width+15;
            nsstr = new NSString(targetText);
			tfont = UIFont.FromName(FontName, 12);
            var sz4 = nsstr.StringSize(tfont, Frame.Width, UILineBreakMode.WordWrap);
			offset = offset+sz4.Width + 5;
            nsstr = new NSString(target.ToString("0.#"));
			tfont = UIFont.FromName(FontName, 18);
			var sz5 = nsstr.StringSize(tfont, Frame.Width, UILineBreakMode.WordWrap);
			offset = offset + sz5.Width;
			nsstr = new NSString(units);
			tfont = UIFont.FromName(FontName, 12);
			var sz6 = nsstr.StringSize(tfont, Frame.Width, UILineBreakMode.WordWrap);
            nfloat total=offset + sz6.Width;
            DrawText(initialText, (Frame.Width-total)/2, Frame.Height / 2, 12, context, TextLeft, UIColor.White, sz.Width, PosCenter);
			offset = (Frame.Width - total) / 2+sz.Width + 5;
			DrawText(initial.ToString("0.#"), offset, Frame.Height / 2, 18, context, TextLeft, UIColor.White, sz2.Width, PosCenter);
			offset = offset + sz2.Width;
			DrawText(units, offset, Frame.Height / 2, 12, context, TextLeft, UIColor.White, sz3.Width, PosCenter);
			offset = offset + sz3.Width + 15;
			DrawText(targetText, offset, Frame.Height / 2, 12, context, TextLeft, UIColor.Green, sz4.Width, PosCenter);
			offset = offset + sz4.Width + 5;
			DrawText(target.ToString("0.#"), offset, Frame.Height / 2, 18, context, TextLeft, UIColor.Green, sz5.Width, PosCenter);
			offset = offset + sz5.Width;
            DrawText(units, offset, Frame.Height / 2, 12, context, TextLeft, UIColor.Green, sz6.Width, PosCenter);
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
