using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace HealthKitPoc
{
	[Register("GraphWeightView"), DesignTimeVisible(true)]
	public class GraphWeightView : UIView, IComponent
	{
	    public string FontName { get; set; } = "Helvetica";

	    private const int TextRight = 0;
		private const int TextCenter = 1;
		private const int TextLeft = 2;

		private const int PosTop = 0;
		private const int PosCenter = 1;
		private const int PosBottom = 2;

		private string _title = "Biceps";
		private string _totalString = "Total";
		private string _totalValuePre = "Reps";
		private string _totalValueSuj = "";
		private string _axisTitle = "kg";
		private List<WeightData> _data;
		private CGRect[] _zones = { };
		private nfloat _paddingLateral;
		private nfloat _paddingSuperior;
		private nfloat _paddingInferior;
		private nfloat _axisWidth;
		private float _redondeo;
		private nfloat _strokeWidth;
		private nfloat _linePointRadious;
		private nfloat _titleTextSize;
		private nfloat _valueTextSize;
		private UIColor _titleColor;
		private nfloat _headerPadding;
		private nfloat _axisTextSize;
		private UIColor _colorAxis;
		private UIColor _colorSelected;
		private UIColor _colorPrincipal;
		private UIColor _colorBackground;
		private UIColor _colorAxisX;
		private int _lastSelected = -1;
		private int _selected = -1;
		private int _previousSelected = -1;
		
		#region IComponent implementation

		public ISite Site { get; set; }
	    public bool ShouldShowDateRange { get; set; }

	    public event EventHandler Disposed;

		#endregion

		[Export("initWithCoder:")]
		public GraphWeightView(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		public GraphWeightView(IntPtr h) : base(h)
		{
			Initialize();
		}

		public GraphWeightView()
		{
			Initialize();
		}

	    private void Initialize()
		{
			nfloat density = 1;//UIScreen.MainScreen.Scale;
			_paddingLateral = 50 * density;
			_paddingSuperior = 60 * density;
			_paddingInferior = 20 * density;
			_axisTextSize = 10 * density;
			_axisWidth = 1 * density;
			_redondeo = 10;
			_strokeWidth = 3 * density;
			_linePointRadious = 5 * density;

			_colorSelected = UIColor.FromRGB(142, 4, 77);
			_colorPrincipal = UIColor.White;

			_colorAxis = UIColor.White;
			_colorAxisX = UIColor.FromRGB(255, 192, 203);

			_headerPadding = 5 * density;
			_titleTextSize = 14 * density;
			_valueTextSize = 12 * density;
			_titleColor = UIColor.White;

            _colorBackground=UIColor.Gray;

			SetNeedsDisplay();
		}

        public void SetData(string title, List<WeightData> data)
		{
			_title = title;
			_data = data;
			
		    _lastSelected = -1;
		    _selected = -1;

            dates = new List<DateTime>();
            dates.Add(DateTime.Now);
		    SetNeedsDisplay();
		}


        private int SwipePadding = 0;
        private List<DateTime> dates;

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);
			var canvas = UIGraphics.GetCurrentContext();
			canvas.SetShouldAntialias(true);
			canvas.ClearRect(rect);
		    var ancho = Frame.Width;
			var alto = Frame.Height;
			nint numColumnas = 7; //siete dias

			DrawBackground(canvas,ancho,alto,_colorBackground);

			var startX = _paddingLateral;
			var endX = ancho - _paddingLateral;
			var restX = endX - startX;

			var startY = _paddingSuperior;
			var endY = alto - _paddingInferior;
			var restY = endY - startY;

			var columnWidth = restX / numColumnas;
			
			var tempZones = new CGRect[9]; //7+1 por el padding
			for (var i = 0; i < 8; i++)
			{
                var pos = startX + i * (columnWidth) + _axisWidth-SwipePadding;

				tempZones[i] = new CGRect(pos, startY, columnWidth , endY - startX); //TODO
			}
			_zones = tempZones;

			float max = 100;
			/*foreach (var t in _data1)
			{
			    while (t > max1)
			        max1 = max1 + _redondeo;
			}*/
			DrawAxis(canvas, startX, _axisWidth, startY, endY, _colorAxis);
			DrawText(max+"", startX - _axisTextSize / 3, startY, _axisTextSize, canvas, TextRight, _colorAxis, ancho);
			DrawText((max / 2)+"", startX - _axisTextSize / 3, startY + restY / 2, _axisTextSize, canvas, TextRight, _colorAxis, ancho);
			DrawText("0", startX - _axisTextSize / 3, startY + restY, _axisTextSize, canvas, TextRight, _colorAxis, ancho);
			DrawText(_axisTitle, startX - _axisTextSize / 3, startY - _axisTextSize * 1.3f, _axisTextSize, canvas, TextRight, _colorAxis, ancho);
            //for(int i<)
			/*for (var i = 0; i < _data1.Length; i++)
			{
				var pos = startX + columnPadding + i * (2 * columnPadding + columnWidth + _columnSeparatorWidth) + _axisWidth;
				var posSeparator = pos + columnPadding + columnWidth + _columnSeparatorWidth;
				DrawBar(canvas, _data1[i], max1, pos, columnWidth, startY, restY, columnBarRadious, _colorPrincipal1, i == _selected, _colorSelected1);
				if (i < _data1.Length - 1)
					DrawSeparator(canvas, posSeparator, _columnSeparatorWidth, startY, restY, _colorPrincipal1);
			}
            

				float max2 = 0;
				foreach (var t in _data2)
				{
					while (t > max2)
						max2 = max2 + _redondeo;
				}
			if (_drawLines)
			{
				//DrawText(max2.KiloFormat(), endX + _axisTextSize / 3, startY, _axisTextSize, canvas, TextLeft, _colorAxis2, ancho);
				//DrawText((max2 / 2).KiloFormat(), endX + _axisTextSize / 3, startY + restY / 2, _axisTextSize, canvas, TextLeft, _colorAxis2, ancho);
				DrawText("0", endX + _axisTextSize / 3, startY + restY, _axisTextSize, canvas, TextLeft, _colorAxis2, ancho);
				DrawText(_axis2Title, endX + _axisTextSize / 3, startY - _axisTextSize * 1.3f, _axisTextSize, canvas, TextLeft, _colorAxis2, ancho);
				DrawAxis(canvas, endX - _axisWidth, _axisWidth, startY, endY, _colorAxis2);
				DrawLines(canvas, _data2, max2, startY, restY, startX, columnPadding, columnWidth, _colorPrincipal2);
			}
			else
			{
				DrawAxis(canvas, endX - _axisWidth, _axisWidth, startY, endY, _colorAxis);
			}*/

            DrawAxis(canvas, startX, restX , endY, endY+_axisWidth, _colorAxis);

            /*var axisOffset = (endX - startX) / (_axisX.Count() - 1);
            for (var i = 0; i < _axisX.Length; i++)
            {
                var pos = startX + i * (axisOffset);
                //nfloat pos = startX + columnPadding + columnWidth / 2 + i * (2 * columnPadding + columnWidth + columnSeparatorWidth) + axisWidth;
                DrawText(_axisX[i], pos, endY + _axisTextSize * 1.3f, _axisTextSize, canvas, TextCenter, _colorAxisX, ancho, PosBottom);
            }
			
			DrawText(_title, _headerPadding, _headerPadding, _titleTextSize, canvas, TextLeft, _titleColor, ancho, PosTop);

		    if (_selected == -1)
			{
				float total1 = 0;
				float total2 = 0;
				for (var i = 0; i < _data1.Length; i++)
					total1 = total1 + _data1[i];
				for (var i = 0; i < _data2.Length; i++)
					total2 = total2 + _data2[i];
				DrawText(_totalString, ancho / 2, _headerPadding, _axisTextSize, canvas, TextCenter, _titleColor, ancho, PosTop);
				//var temp1 = _totalValuePre1 + " " + total1.KiloFormat() + _totalValueSuj1;
				//var temp2 = _totalValuePre2 + " " + total2.KiloFormat() + _totalValueSuj2;
				DrawText("", ancho / 2 - _headerPadding, _headerPadding + 1.3f * _axisTextSize, _valueTextSize, canvas, TextRight, _titleColor, ancho, PosTop);
				DrawText("", ancho / 2 + _headerPadding, _headerPadding + 1.3f * _axisTextSize, _valueTextSize, canvas, TextLeft, _titleColor, ancho, PosTop);
			}
			else if (_selected < _axisX.Length)
			{
				float total1 = 0; 
				float total2 = 0;
				if (_selected < _data1.Length)
					total1 = _data1[_selected];
				if (_selected < _data2.Length)
					total2 = _data2[_selected];

			    var title = _axisXTitle[_selected]; // ShouldShowDateRange ? _axisXTitle[_selected] : _axisX[_selected];
				DrawText(title, ancho / 2, _headerPadding, _axisTextSize, canvas, TextCenter, _titleColor, ancho, PosTop);

				var temp1 = $"{_totalValuePre1} {total1} {_totalValueSuj1}";
				var temp2 = $"{_totalValuePre2} {total2} {_totalValueSuj2}";
			    var title2 = _drawLines ? $"{temp1}. {temp2}" : temp1;

			    DrawText(title2, ancho / 2, _headerPadding + 1.3f * _axisTextSize, _valueTextSize, canvas, TextCenter, _titleColor, ancho, PosTop);
			}*/
		}

		private void DrawLines(CGContext canvas, float[] values, nfloat max, nfloat startY, nfloat restY, nfloat startX, nfloat columnPadding, nfloat columnWidth, UIColor color)
		{
			canvas.SaveState();
			color.SetFill();
			color.SetStroke();
			canvas.SetLineWidth(_strokeWidth);
			var path = new CGPath();
			/*for (var i = 0; i < values.Length; i++)
			{
				var realValue = (values[i] * restY) / max;
				var pos = startX + columnPadding + columnWidth / 2 + i * (2 * columnPadding + columnWidth + _columnSeparatorWidth) + _axisWidth;
				if (i == 0)
					path.MoveToPoint(pos, startY + (restY - realValue));
				else
					path.AddLineToPoint(pos, startY + (restY - realValue));

			}*/
			canvas.AddPath(path);
			canvas.DrawPath(CGPathDrawingMode.Stroke);

			canvas.RestoreState();
			/*for (var i = 0; i < values.Length; i++)
			{
				color.SetFill();
				color.SetStroke();
				canvas.SetLineWidth(0);
				var realValue = (values[i] * restY) / max;
				var pos = startX + columnPadding + columnWidth / 2 + i * (2 * columnPadding + columnWidth + _columnSeparatorWidth) + _axisWidth;
				canvas.AddArc(pos, startY + (restY - realValue), _linePointRadious, 0, (float)Math.PI * 2, false);
				canvas.DrawPath(CGPathDrawingMode.FillStroke);
				if (i == _selected)
				{
					_colorSelected.SetStroke();
					canvas.SetLineWidth(_strokeWidth);
					canvas.AddArc(pos, startY + (restY - realValue), _linePointRadious, 0, (float)Math.PI * 2, false);
					canvas.DrawPath(CGPathDrawingMode.Stroke);
				}
			}*/
		}

		private static void DrawBar(CGContext canvas, float value, nfloat max, nfloat pos, nfloat columnWidth, nfloat startY, nfloat restY, nfloat columnBarRadious, UIColor color1,  bool mark, UIColor colorMark)
		{
			var realValue = (value * restY) / max;

			canvas.SaveState();
			canvas.SetLineWidth(0);

			var path = RoundedRect(pos, startY + (restY - realValue), pos + columnWidth, startY + restY, columnBarRadious);
			canvas.AddPath(path);

			canvas.Clip();

			using (var rgb = CGColorSpace.CreateDeviceRGB())
			{
				CGGradient gradient;
				if (mark)
					gradient = new CGGradient(rgb, new[]
					{
					new CGColor(colorMark.CGColor,255f/255),
					new CGColor(colorMark.CGColor,255f/255)
					});
				else
					gradient = new CGGradient(rgb, new[]
				{
					new CGColor(color1.CGColor,0.5f),
					new CGColor(color1.CGColor,1f)
				});

				canvas.DrawLinearGradient(gradient,
					new CGPoint(0, startY),
					new CGPoint(0, startY + restY),
					CGGradientDrawingOptions.DrawsBeforeStartLocation);
			}

			canvas.RestoreState();
		}

		private static void DrawBackground(CGContext canvas, nfloat ancho, nfloat alto, UIColor color1)
		{
			canvas.SaveState();
			canvas.SetLineWidth(0);

			//var path = new CGPath();
			//path.AddLines(points.ToArray());
			//path.AddLines(new[] { new CGPoint(0, 0), new CGPoint(0, alto), new CGPoint(ancho, alto), new CGPoint(ancho, 0) });
			//path.CloseSubpath();
			//canvas.AddPath(path);

			//canvas.FillRect(new CGRect(pos, startY, separatorWidth, restY));
			canvas.Clip();

			using (var rgb = CGColorSpace.CreateDeviceRGB())
			{
				var gradient = new CGGradient(rgb, new[]
				{
					new CGColor(color1.CGColor,1f),
					new CGColor(color1.CGColor,0.25f)
				});

				canvas.DrawLinearGradient(gradient,
					//new CGPoint(path.BoundingBox.Left, path.BoundingBox.Top),
					//new CGPoint(path.BoundingBox.Right, path.BoundingBox.Bottom),
					new CGPoint(0, ancho),
					new CGPoint(ancho, 0),
					CGGradientDrawingOptions.DrawsBeforeStartLocation);
			}

			canvas.RestoreState();
		}

		private static void DrawSeparator(CGContext canvas, nfloat pos, nfloat separatorWidth, nfloat startY, nfloat restY, UIColor color1)
		{
			canvas.SaveState();
			canvas.SetLineWidth(0);

			var path = new CGPath();
			//path.AddLines(points.ToArray());
			path.AddLines(new[] { new CGPoint(pos, startY), new CGPoint(pos, startY + restY), new CGPoint(pos + separatorWidth, startY + restY), new CGPoint(pos + separatorWidth, startY) });
			path.CloseSubpath();
			canvas.AddPath(path);

			//canvas.FillRect(new CGRect(pos, startY, separatorWidth, restY));
			canvas.Clip();

			using (var rgb = CGColorSpace.CreateDeviceRGB())
			{
				var gradient = new CGGradient(rgb, new[]
				{
					new CGColor(color1.CGColor,0.25f),
					new CGColor(color1.CGColor,0.5f)
				});

				canvas.DrawLinearGradient(gradient,
					//new CGPoint(path.BoundingBox.Left, path.BoundingBox.Top),
					//new CGPoint(path.BoundingBox.Right, path.BoundingBox.Bottom),
					new CGPoint(0, startY),
					new CGPoint(0, startY + restY),
					CGGradientDrawingOptions.DrawsBeforeStartLocation);
			}

			canvas.RestoreState();
		}

		private static void DrawAxis(CGContext canvas, nfloat pos, nfloat separatorWidth, nfloat startY, nfloat restY, UIColor color1)
		{
			color1.SetFill();
			color1.SetStroke();
			canvas.SetLineWidth(0);
			canvas.FillRect(new CGRect(pos, startY, separatorWidth, restY - startY));
		}

		private static CGPath RoundedRect(nfloat left, nfloat top, nfloat right, nfloat bottom, nfloat r)
		{
			var path = new CGPath();
			if (r < 0) r = 0;
			var width = right - left;
			var height = bottom - top;
			if (r > width / 2) r = width / 2;
			if (r > height / 2) r = height / 2;

			path.MoveToPoint(left, bottom);
			path.AddLineToPoint(left, top - r);
			path.AddArc(left + r, top + r, r, (float)Math.PI, (float)Math.PI * 3 / 2, false);
			path.AddLineToPoint(right - r, top);
			path.AddArc(right - r, top + r, r, (float)Math.PI / 2, 0, false);
			path.AddLineToPoint(right, bottom);
			path.CloseSubpath();

			return path;
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			var touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				var point = touch.LocationInView(this);
				var sel = -1;
				_previousSelected = _selected;
				for (var i = 0; i < _zones.Length; i++)
				{
					if (IsTouched(point.X, point.Y, _zones[i]))
						sel = i;
				}
				_selected = sel;
				if (_selected != _lastSelected)
					_lastSelected = -1;
				SetNeedsDisplay();
			}
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);
			var touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				var point = touch.LocationInView(this);
				var sel = -1;
				for (var i = 0; i < _zones.Length; i++)
				{
					if (IsTouched(point.X, point.Y, _zones[i]))
						sel = i;
				}
				_selected = sel;
				if (_selected != _lastSelected)
					_lastSelected = -1;
				SetNeedsDisplay();
			}
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
			var touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				var point = touch.LocationInView(this);
				if (_selected != _lastSelected && _selected != -1)
				{
					_lastSelected = _selected;
					//if (barGraphListener != null)
					//	barGraphListener.onBarSelected(selected);
					//Toast.MakeText(Context, "Seleccionado: " + selected, ToastLength.Short).Show();
				}
				else {
					_selected = -1;
					_lastSelected = -1;
					//Toast.MakeText(Context, "Quitada", ToastLength.Short).Show();
				}
				SetNeedsDisplay();
			}
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);
			var touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				var point = touch.LocationInView(this);
				if (_selected != _lastSelected && _selected != -1)
				{
					_lastSelected = _selected;
					//if (barGraphListener != null)
					//	barGraphListener.onBarSelected(selected);
					//Toast.MakeText(Context, "Seleccionado: " + selected, ToastLength.Short).Show();
				}
				else {
					_selected = -1;
					_lastSelected = -1;
					//Toast.MakeText(Context, "Quitada", ToastLength.Short).Show();
				}
				SetNeedsDisplay();
			}
		}

		private static bool IsTouched(nfloat posX, nfloat posY, CGRect r)
		    => posX >= r.Left && posX <= r.Right && posY >= r.Top && posY <= r.Bottom;

	    private void DrawText(string t, nfloat x, nfloat y, nfloat fontSize, CGContext gctx, int alignement, UIColor color, nfloat width, int position = PosCenter)
		{
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
		}
	}
}