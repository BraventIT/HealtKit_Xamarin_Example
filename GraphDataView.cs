using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace HealthKitPoc
{
	[Register("GraphDataView"), DesignTimeVisible(true)]
	public class GraphDataView : UIView, IComponent
	{
        private nfloat _strokeWidth;
        private nfloat _linePointRadious;
        private List<nfloat> positions;
        private int selected = -1;
        private nint paddingTop;
        private nint paddingBottom;
        private DateTime initDate=DateTime.Now.AddDays(-14);
        private int days = 14;
        List<WeightData> weigths;
        private double maxWeight=110;
        private double minWeight=40;
        private double initWeight = 75;
        private double targetWeight = 80;

		#region IComponent implementation

		public ISite Site { get; set; }
		public bool ShouldShowDateRange { get; set; }

		public event EventHandler Disposed;

		#endregion

		[Export("initWithCoder:")]
		public GraphDataView(NSCoder coder) : base(coder)
        {
			Initialize();
		}

		public GraphDataView(IntPtr h) : base(h)
        {
			Initialize();
		}

		public GraphDataView()
		{
			Initialize();

		}

        private void Initialize()
        {
			_strokeWidth = 2;
			_linePointRadious = 5;
            paddingTop = 8;
            paddingBottom = 18;
            BackgroundColor = UIColor.Clear;
            UserInteractionEnabled = true;
			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer((obj) =>
			{
                CGPoint position= obj.LocationInView(this);
                nfloat min = positions.OrderBy(x => Math.Abs(position.X - x)).FirstOrDefault();
                selected = positions.IndexOf(min);
                SetNeedsDisplay();
			});
            AddGestureRecognizer(tapGesture);
			SetNeedsDisplay();
        }

        public void SetData(DateTime initDate,int days,List<WeightData> weigths,double maxWeight,double minWeight,double initWeight,double targetWeight)
        {
            this.initDate = initDate;
            this.days = days;
            this.weigths = weigths;
            this.maxWeight = maxWeight;
            this.minWeight = minWeight;
            this.initWeight = initWeight;
            this.targetWeight = targetWeight;
            SetNeedsDisplay();
        }

        public nfloat GetPositionForDate(DateTime date){
			positions = new List<nfloat>();
            nfloat pixelPerTicks = Frame.Width / (days * TimeSpan.TicksPerDay);				
            nfloat pos = 0 + (date.Ticks - initDate.Ticks) * pixelPerTicks;
            return pos;
		}

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            var canvas = UIGraphics.GetCurrentContext();
            canvas.SetShouldAntialias(true);
            canvas.ClearRect(rect);
            var ancho = Frame.Width;
            var alto = Frame.Height;
            nfloat restY = alto - paddingBottom - paddingTop;
            DrawDays(canvas,paddingTop,restY,0,ancho);
            nfloat realTargetweight = (float)((targetWeight - minWeight) / (maxWeight - minWeight) * restY);
            DrawDashedLine(canvas,0,Frame.Width,paddingTop+restY-realTargetweight,1,UIColor.Green);
            nfloat realMinweight = (float)((initWeight - minWeight) / (maxWeight - minWeight) * restY);
            DrawDashedLine(canvas, 0, Frame.Width, paddingTop + restY-realMinweight, 1, UIColor.White);

            DrawLines(canvas,paddingTop,restY,0,ancho,UIColor.Red);
            if (selected >= 0)
                DrawMark(canvas,paddingTop, restY,0,ancho);
        }



        private void DrawDays(CGContext canvas, nfloat startY, nfloat restY,nfloat startX, nfloat restX){
			UIColor.FromRGBA(255, 255, 255, 64).SetFill();
			UIColor.FromRGBA(255, 255, 255, 64).SetStroke();
			canvas.SetLineWidth(0);
			nfloat ancho = Frame.Width / days;
			/*for (int i = 0; i < days+1;i++){
                
                var pos = ancho * i;
                canvas.FillRect(new CGRect(pos-1, startY, 1, restY));
                if (i > 0 && i < days)
                {
                    DateTime Date = initDate.AddDays(i);
                    String text = Date.ToString("dd/MM");
                    DrawText(text, pos, Frame.Height - paddingBottom + 3, 12, canvas, TextCenter, UIColor.White, 0, PosTop);
                    if (text.StartsWith("01"))
                        DrawText(Date.ToString("yyyy"), pos, Frame.Height - paddingBottom + 16, 8, canvas, TextCenter, UIColor.White, 0, PosTop);
                }
            }*/
            nfloat pixelPerTicks = restX / (days * TimeSpan.TicksPerDay);
            for (int i = 0; i < days+1;i++){
                DateTime Date = initDate.AddDays(i);
                if (i == 0 || Date.ToString("dd/MM").StartsWith("01"))
                {                    
                    var pos = startX + (Date.Ticks - initDate.Ticks) * pixelPerTicks;
                    if(Date.ToString("dd/MM").StartsWith("01"))
                        canvas.FillRect(new CGRect(pos - 1, startY, 1, restY));
                    int daysInMonth = DateTime.DaysInMonth(Date.Year, Date.Month);
                    String text = Date.ToString("MMM yyyy").Replace(".","");
                    nfloat width = daysInMonth*TimeSpan.TicksPerDay * pixelPerTicks;
                    DrawText(text, pos +width/2, Frame.Height - paddingBottom + 3, 12, canvas, TextCenter, UIColor.White, width, PosTop);
                }
            }
		}

		private void DrawLines(CGContext canvas, nfloat startY, nfloat restY, nfloat startX,nfloat restX , UIColor color)
		{
			canvas.SaveState();
            UIColor.FromRGBA(255,255,255,32).SetFill();
			color.SetStroke();
			canvas.SetLineWidth(_strokeWidth);
			var path = new CGPath();
            var path2 = new CGPath();
            nfloat ancho = Frame.Width / days;
            positions = new List<nfloat>();
            nfloat pixelPerTicks =  restX/ (days*TimeSpan.TicksPerDay);			
            for (var i = 0; i < weigths.Count; i++)
            {

                nfloat realValue = (float)((weigths[i].Value-minWeight)/(maxWeight-minWeight)*restY);
                var pos = startX + (weigths[i].Date.Ticks-initDate.Ticks)*pixelPerTicks;
				if (i == 0)
				{
                    path.MoveToPoint(pos, startY+ restY);
                    path2.MoveToPoint(pos,startY + restY - realValue);
				}
                 path.AddLineToPoint(pos, startY + restY - realValue);
                path2.AddLineToPoint(pos,startY + restY - realValue);
                positions.Add(pos);
                if(i==weigths.Count-1){
                    path.AddLineToPoint(pos, startY + restY);
                    path.CloseSubpath();
                }

            }
			canvas.AddPath(path2);
            canvas.DrawPath(CGPathDrawingMode.Stroke);
			canvas.AddPath(path);
            canvas.DrawPath(CGPathDrawingMode.Fill);
			canvas.RestoreState();
            for (var i = 0; i < weigths.Count; i++)
            {
                color.SetFill();
                color.SetStroke();
                canvas.SetLineWidth(0);
				nfloat realValue = (float)((weigths[i].Value - minWeight) / (maxWeight - minWeight) * restY);
				var pos = startX + (weigths[i].Date.Ticks - initDate.Ticks) * pixelPerTicks;
                canvas.AddArc(pos, startY + restY - realValue, _linePointRadious / 2.5f, 0, (float)Math.PI * 2, false);
                canvas.DrawPath(CGPathDrawingMode.FillStroke);
            }
            if (selected > -1)
            {
				color.SetFill();
				color.SetStroke();
				canvas.SetLineWidth(0);
                nfloat realValue = (float)((weigths[selected].Value - minWeight) / (maxWeight - minWeight) * restY);
                var pos = startX + (weigths[selected].Date.Ticks - initDate.Ticks) * pixelPerTicks;
				canvas.AddArc(pos, startY + restY - realValue, _linePointRadious, 0, (float)Math.PI * 2, false);
				canvas.DrawPath(CGPathDrawingMode.FillStroke);
                color.SetFill();
                UIColor.White.SetStroke();
                canvas.SetLineWidth(_strokeWidth);
                canvas.AddArc(pos, startY + restY - realValue, _linePointRadious, 0, (float)Math.PI * 2, false);
                canvas.DrawPath(CGPathDrawingMode.Stroke);

            }
			
		}

        private void DrawMark(CGContext canvas, nfloat startY, nfloat restY, nfloat startX, nfloat restX){
            nfloat pixelPerTicks = restX / (days * TimeSpan.TicksPerDay);
            nfloat realValue = (float)((weigths[selected].Value - minWeight) / (maxWeight - minWeight) * restY);
            var pos = startX + (weigths[selected].Date.Ticks - initDate.Ticks) * pixelPerTicks;
            String valor = weigths[selected].Value.ToString("0.#");
            String unidades = weigths[selected].Unit;
            var tfont = UIFont.FromName(FontName, 12);
			var nsstr = new NSString(valor);
            var sz = nsstr.StringSize(tfont, Frame.Width, UILineBreakMode.WordWrap);
			tfont = UIFont.FromName(FontName, 10);
            nsstr = new NSString(unidades);
            var sz2 = nsstr.StringSize(tfont, Frame.Width, UILineBreakMode.WordWrap);
            String date = " - "+weigths[selected].Date.ToString("dd/MM/yy");
			tfont = UIFont.FromName(FontName, 10);
			nsstr = new NSString(date);
			var sz3 = nsstr.StringSize(tfont, Frame.Width, UILineBreakMode.WordWrap);
            nfloat width = sz.Width + sz2.Width+sz3.Width;
			if (pos - width / 2 < startX)
				pos = 0 + width / 2 + 3;
			if (pos + width / 2 > startX + restX)
				pos = startX + restX - width / 2 - 3;
			DrawAxis(canvas, pos - width / 2 - 3, width + 6, startY + restY - realValue - 10, startY + restY - realValue - 10 - sz.Height - 6, UIColor.Red);
            DrawText(valor, pos - width / 2, startY + restY - realValue - 13, 12, canvas, TextLeft, UIColor.White, sz.Width, PosBottom);
            DrawText(unidades, pos - width / 2 + sz.Width, startY + restY - realValue - 13, 10, canvas, TextLeft, UIColor.White, sz2.Width, PosBottom);
            DrawText(date, pos - width / 2 + sz.Width+ sz2.Width, startY + restY - realValue - 13, 10, canvas, TextLeft, UIColor.White, sz3.Width, PosBottom);
            /*nfloat maxWidth = width;
            if (sz3.Width > width)
                maxWidth = sz3.Width;
            if (pos - maxWidth / 2 < startX)
                pos = 0+maxWidth/2+3;
            if (pos + maxWidth / 2 > startX+restX)
                pos = startX+restX- maxWidth / 2-3;
            DrawAxis(canvas, pos-maxWidth/2-3, maxWidth+6, startY+ restY - realValue - 10, startY+restY - realValue - 10-sz.Height-sz3.Height-6, UIColor.Red);
            DrawText(valor,pos-width/2,startY+restY - realValue-13-sz3.Height,12,canvas,TextLeft,UIColor.White,width,PosBottom);
            DrawText(unidades, pos + width / 2, startY+restY - realValue - 13-sz3.Height, 10, canvas, TextRight, UIColor.White, width, PosBottom);
            DrawText(date, pos , startY + restY - realValue - 13, 10, canvas, TextCenter, UIColor.White, sz3.Width, PosBottom);*/
            pos = startX + (weigths[selected].Date.Ticks - initDate.Ticks) * pixelPerTicks;//Pintar triangulo en su sitio
			canvas.SaveState();
			UIColor.Red.SetFill();
			UIColor.Red.SetStroke();
			canvas.SetLineWidth(0);
			var path = new CGPath();
			path.MoveToPoint(pos - 4f, startY +restY- realValue - 10f);
			path.AddLineToPoint(pos + 4f, startY + restY - realValue - 10f);
            path.AddLineToPoint(pos, startY + restY - realValue-6);
            path.CloseSubpath();
			canvas.AddPath(path);
            canvas.DrawPath(CGPathDrawingMode.Fill);
			canvas.RestoreState();
        }

		private static void DrawAxis(CGContext canvas, nfloat pos, nfloat axisWidth, nfloat startY, nfloat restY, UIColor color)
		{
			color.SetFill();
			color.SetStroke();
			canvas.SetLineWidth(0);
			canvas.FillRect(new CGRect(pos, startY, axisWidth, restY - startY));

		}

		private static void DrawDashedLine(CGContext canvas, nfloat pos, nfloat axisWidth, nfloat startY, nfloat restY, UIColor color)
		{
            canvas.SaveState();
			color.SetFill();
			color.SetStroke();
            canvas.SetLineDash(0,new nfloat[]{3,3});
			canvas.SetLineWidth(1);			
			var path = new CGPath();
            path.MoveToPoint(pos,startY);
            path.AddLineToPoint(pos+axisWidth,startY);
			canvas.AddPath(path);
			canvas.DrawPath(CGPathDrawingMode.Stroke);
            canvas.RestoreState();
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
