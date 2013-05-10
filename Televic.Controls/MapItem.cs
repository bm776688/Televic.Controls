using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.ComponentModel;


namespace Televic.Controls
{
	public class MapItem : Button
	{
		public event MapItemChangedHandler MapItemChanged;

		public event MapItemPopedUpHandler MapItemPopedUp;

		public event MapItemPopedDownHandler MapItemPopedDowm;

		private Grid grid;

		static MapItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MapItem), new FrameworkPropertyMetadata(typeof(MapItem)));
		}

		#region dependency properties

		[Bindable(false), Description("Map tag pop up panel template")]
		public ControlTemplate PopPanelTemplate
		{
			get { return (ControlTemplate)GetValue(PopPanelTemplateProperty); }
			set { SetValue(PopPanelTemplateProperty, value); }
		}
		public static readonly DependencyProperty PopPanelTemplateProperty =
			DependencyProperty.Register("PopPanelTemplate", typeof(ControlTemplate), typeof(MapItem), new UIPropertyMetadata(null));

		[Bindable(true), Category("common"), Description("Get or set the pop panel offset")]
		public Thickness PopPanelOffset
		{
			get { return (Thickness)GetValue(PopPanelOffsetProperty); }
			set { SetValue(PopPanelOffsetProperty, value); }
		}
		public static readonly DependencyProperty PopPanelOffsetProperty =
			DependencyProperty.Register("PopPanelOffset", typeof(Thickness), typeof(MapItem), new UIPropertyMetadata(new Thickness(0)));


		public ImageSource Image
		{
			get { return (ImageSource)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(ImageSource), typeof(MapItem), new UIPropertyMetadata(null, Image_PropertyChanged));

		[Bindable(true), Category("common"), Description("Get or set the x-coordinate of the map")]
		public double X
		{
			get { return (double)GetValue(XProperty); }
			set { 
				SetValue(XProperty, value); 
			}
		}
		public static readonly DependencyProperty XProperty =
			DependencyProperty.Register("X", typeof(double), typeof(MapItem), new UIPropertyMetadata(0.0, X_PropertyChanged));

		[Bindable(true), Category("common"), Description("Get or set the y-coordinate of the map")]
		public double Y
		{
			get { return (double)GetValue(YProperty); }
			set { SetValue(YProperty, value); }
		}
		public static readonly DependencyProperty YProperty =
			DependencyProperty.Register("Y", typeof(double), typeof(MapItem), new UIPropertyMetadata(0.0, Y_PropertyChanged));


		public double ImageWidth
		{
			get { return (double)GetValue(ImageWidthProperty); }
			set { SetValue(ImageWidthProperty, value); }
		}
		public static readonly DependencyProperty ImageWidthProperty =
			DependencyProperty.Register("ImageWidth", typeof(double), typeof(MapItem), new UIPropertyMetadata(33.0));

		public double ImageHeight
		{
			get { return (double)GetValue(ImageHeightProperty); }
			set { SetValue(ImageHeightProperty, value); }
		}
		public static readonly DependencyProperty ImageHeightProperty =
			DependencyProperty.Register("ImageHeight", typeof(double), typeof(MapItem), new UIPropertyMetadata(40.5));

		
		private static void X_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MapItem mi = d as MapItem;
			if (mi.MapItemChanged != null)
				mi.MapItemChanged(mi, new Point(mi.X, mi.Y));
		}

		private static void Y_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MapItem mi = d as MapItem;
			if (mi.MapItemChanged != null)
				mi.MapItemChanged(mi, new Point(mi.X, mi.Y));
		}
		
		private static void Image_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MapItem mi = d as MapItem;
		}

		public double ImageOffsetX { get { return ImageWidth / 2; } }

		#endregion

		protected override void OnClick()
		{
			base.OnClick();

			if (grid != null) {
				grid.Visibility = Visibility.Visible;
				if (MapItemPopedUp != null) MapItemPopedUp(this);
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			//get the presenter, and initialize
			grid = GetTemplateChild("PART_grid") as Grid;
			ContentControl cc = GetTemplateChild("PART_content") as ContentControl;
			if (cc != null)
			{
				cc.Content = (PopPanelTemplate.LoadContent() as FrameworkElement);
			}
			Image close = GetTemplateChild("PART_close") as Image;
			if (close != null) 
			{
				close.MouseLeftButtonDown += (s, e) =>
				{
					grid.Visibility = Visibility.Hidden;
					e.Handled = true;
					if (MapItemPopedDowm != null) MapItemPopedDowm(this);
				};
			}
		}

	}

}
