using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public class GradientPanel : Panel
	{
		public const string PanelCategory = "GradientPanel";

		private int _borderWidth = 1;

		private int _shadowOffSet = 5;

		private int _roundCornerRadius = 4;

		private Image _image;

		private Point _imageLocation = new Point(4, 4);

		private Color _borderColor = Color.Gray;

		private Color _gradientStartColor = Color.White;

		private Color _gradientEndColor = Color.Gray;

		private IContainer components = null;

		[Browsable(true)]
		[Category("GradientPanel")]
		[DefaultValue(1)]
		public int BorderWidth
		{
			get
			{
				return _borderWidth;
			}
			set
			{
				_borderWidth = value;
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("GradientPanel")]
		[DefaultValue(5)]
		public int ShadowOffSet
		{
			get
			{
				return _shadowOffSet;
			}
			set
			{
				_shadowOffSet = Math.Abs(value);
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("GradientPanel")]
		[DefaultValue(4)]
		public int RoundCornerRadius
		{
			get
			{
				return _roundCornerRadius;
			}
			set
			{
				_roundCornerRadius = Math.Abs(value);
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("GradientPanel")]
		public Image Image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("GradientPanel")]
		[DefaultValue("4,4")]
		public Point ImageLocation
		{
			get
			{
				return _imageLocation;
			}
			set
			{
				_imageLocation = value;
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("GradientPanel")]
		[DefaultValue("Color.Gray")]
		public Color BorderColor
		{
			get
			{
				return _borderColor;
			}
			set
			{
				_borderColor = value;
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("GradientPanel")]
		[DefaultValue("Color.White")]
		public Color GradientStartColor
		{
			get
			{
				return _gradientStartColor;
			}
			set
			{
				_gradientStartColor = value;
				Invalidate();
			}
		}

		[Browsable(true)]
		[Category("GradientPanel")]
		[DefaultValue("Color.Gray")]
		public Color GradientEndColor
		{
			get
			{
				return _gradientEndColor;
			}
			set
			{
				_gradientEndColor = value;
				Invalidate();
			}
		}

		public static GraphicsPath GetRoundPath(Rectangle r, int depth)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(r.X, r.Y, depth, depth, 180f, 90f);
			graphicsPath.AddArc(r.X + r.Width - depth, r.Y, depth, depth, 270f, 90f);
			graphicsPath.AddArc(r.X + r.Width - depth, r.Y + r.Height - depth, depth, depth, 0f, 90f);
			graphicsPath.AddArc(r.X, r.Y + r.Height - depth, depth, depth, 90f, 90f);
			graphicsPath.AddLine(r.X, r.Y + r.Height - depth, r.X, r.Y + depth / 2);
			return graphicsPath;
		}

		public GradientPanel()
		{
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
			InitializeComponent();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
			int num = Math.Min(Math.Min(_shadowOffSet, base.Width - 2), base.Height - 2);
			int num2 = Math.Min(Math.Min(_roundCornerRadius, base.Width - 2), base.Height - 2);
			if (base.Width > 1 && base.Height > 1)
			{
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				Rectangle rectangle = new Rectangle(0, 0, base.Width - num - 1, base.Height - num - 1);
				Rectangle r = new Rectangle(num, num, base.Width - num - 1, base.Height - num - 1);
				GraphicsPath roundPath = GetRoundPath(r, num2);
				GraphicsPath roundPath2 = GetRoundPath(rectangle, num2);
				if (num2 > 0)
				{
					using (PathGradientBrush pathGradientBrush = new PathGradientBrush(roundPath))
					{
						pathGradientBrush.WrapMode = WrapMode.Clamp;
						ColorBlend colorBlend = new ColorBlend(3);
						colorBlend.Colors = new Color[3]
						{
							Color.Transparent,
							Color.FromArgb(180, Color.DimGray),
							Color.FromArgb(180, Color.DimGray)
						};
						colorBlend.Positions = new float[3]
						{
							0f,
							0.1f,
							1f
						};
						pathGradientBrush.InterpolationColors = colorBlend;
						e.Graphics.FillPath(pathGradientBrush, roundPath);
					}
				}
				using (LinearGradientBrush brush = new LinearGradientBrush(rectangle, _gradientStartColor, _gradientEndColor, LinearGradientMode.BackwardDiagonal))
				{
					e.Graphics.FillPath(brush, roundPath2);
					e.Graphics.DrawPath(new Pen(Color.FromArgb(180, _borderColor), _borderWidth), roundPath2);
					if (_image != null)
					{
						e.Graphics.DrawImageUnscaled(_image, _imageLocation);
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
	}
}
