using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.ComponentModel;

namespace HealthKitPoc
{
	[Register("GradientView"), DesignTimeVisible(true)]
	public class GradientView : UIView, IComponent
    {
		#region IComponent implementation

		public ISite Site { get; set; }
		public bool ShouldShowDateRange { get; set; }

		public event EventHandler Disposed;

		#endregion

		[Export("initWithCoder:")]
		public GradientView(NSCoder coder) : base(coder)
        {
			Initialize();
		}

		public GradientView(IntPtr h) : base(h)
        {
			Initialize();
		}

		public GradientView()
		{
			Initialize();
		}

        private void Initialize()
        {
            SetNeedsDisplay();
        }

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);
			var context = UIGraphics.GetCurrentContext();
			context.SetShouldAntialias(true);
            DrawBackground(context, Frame.Width, Frame.Height, UIColor.DarkGray);

		}

		private static void DrawBackground(CGContext context, nfloat ancho, nfloat alto, UIColor color)
		{
			context.SaveState();
			
			using (var rgb = CGColorSpace.CreateDeviceRGB())
			{
				var gradient = new CGGradient(rgb, new[]
				{
					new CGColor(color.CGColor,0.25f),
					new CGColor(color.CGColor,1f)
				});

				context.DrawLinearGradient(gradient,
					new CGPoint(0, ancho),
					new CGPoint(ancho, 0),
					CGGradientDrawingOptions.DrawsBeforeStartLocation);
			}

			context.RestoreState();
		}
    }
}