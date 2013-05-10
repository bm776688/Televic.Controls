using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.ComponentModel;

namespace Televic.Controls
{
	/// <summary> map control item, use to zoom and pan a map </summary>
	[TemplatePart(Name = PART_Presenter, Type = typeof(MapPanel))]
	public class MapControl : ItemsControl
	{
		private const string PART_Presenter = "PART_Presenter";

		#region Dependency Properties

		[Description("get or set the map image")]
		public ImageSource MapImage
		{
			get { return (ImageSource)GetValue(MapImageProperty); }
			set { SetValue(MapImageProperty, value); }
		}
		public static readonly DependencyProperty MapImageProperty =
			DependencyProperty.Register("MapImage", typeof(ImageSource), typeof(MapControl), new UIPropertyMetadata(null));

		[Description("get or set the zoom panel visibility"), Category("Visibility")]
		public Visibility ZoomPanelVisibility
		{
			get { return (Visibility)GetValue(ZoomPanelVisibilityProperty); }
			set { SetValue(ZoomPanelVisibilityProperty, value); }
		}
		public static readonly DependencyProperty ZoomPanelVisibilityProperty =
			DependencyProperty.Register("ZoomPanelVisibility", typeof(Visibility), typeof(MapControl), new UIPropertyMetadata(Visibility.Visible));

		[Bindable(true), Category("Common"), Description("allow zoom in or out the map")]
		public bool AllowZoom
		{
			get { return (bool)GetValue(AllowZoomProperty); }
			set { SetValue(AllowZoomProperty, value); }
		}
		public static readonly DependencyProperty AllowZoomProperty =
			DependencyProperty.Register("AllowZoom", typeof(bool), typeof(MapControl), new UIPropertyMetadata(true));

		[Bindable(true), Category("Common"), Description("allow pan the map")]
		public bool AllowPan
		{
			get { return (bool)GetValue(AllowPanProperty); }
			set { SetValue(AllowPanProperty, value); }
		}
		public static readonly DependencyProperty AllowPanProperty =
			DependencyProperty.Register("AllowPan", typeof(bool), typeof(MapControl), new UIPropertyMetadata(true));

		public double TranslateX
		{
			get { return (double)GetValue(TranslateXProperty); }
			set
			{
				BeginAnimation(TranslateXProperty, null);
				SetValue(TranslateXProperty, value);
			}
		}
		public static readonly DependencyProperty TranslateXProperty =
			DependencyProperty.Register("TranslateX", typeof(double), typeof(MapControl), new UIPropertyMetadata(0.0, TranslateX_PropertyChanged, TranslateX_Coerce));

		public double TranslateY
		{
			get { return (double)GetValue(TranslateYProperty); }
			set
			{
				BeginAnimation(TranslateYProperty, null);
				SetValue(TranslateYProperty, value);
			}
		}
		public static readonly DependencyProperty TranslateYProperty =
			DependencyProperty.Register("TranslateY", typeof(double), typeof(MapControl), new UIPropertyMetadata(0.0, TranslateY_PropertyChanged, TranslateY_Coerce));

		[Bindable(true), Category("Common"), Description("get or set the zoom animation speed")]
		public TimeSpan AnimationLength
		{
			get { return (TimeSpan)GetValue(AnimationLengthProperty); }
			set { SetValue(AnimationLengthProperty, value); }
		}
		public static readonly DependencyProperty AnimationLengthProperty =
			DependencyProperty.Register("AnimationLength", typeof(TimeSpan), typeof(MapControl), new UIPropertyMetadata(TimeSpan.FromMilliseconds(500)));

		[Bindable(true), Category("Common"), Description("the minimum of map can be zoomed in")]
		public double MinZoom
		{
			get { return (double)GetValue(MinZoomProperty); }
			set { SetValue(MinZoomProperty, value); }
		}
		public static readonly DependencyProperty MinZoomProperty =
			DependencyProperty.Register("MinZoom", typeof(double), typeof(MapControl), new UIPropertyMetadata(0.1));

		[Bindable(true), Category("Common"), Description("the maximum of map can be zoomed in")]
		public double MaxZoom
		{
			get { return (double)GetValue(MaxZoomProperty); }
			set { SetValue(MaxZoomProperty, value); }
		}
		public static readonly DependencyProperty MaxZoomProperty =
			DependencyProperty.Register("MaxZoom", typeof(double), typeof(MapControl), new UIPropertyMetadata(100.0));

		[Bindable(true), Category("Common"), Description("the maximum of once the map can be zoomed in")]
		public double MaxZoomDelta
		{
			get { return (double)GetValue(MaxZoomDeltaProperty); }
			set { SetValue(MaxZoomDeltaProperty, value); }
		}
		public static readonly DependencyProperty MaxZoomDeltaProperty =
			DependencyProperty.Register("MaxZoomDelta", typeof(double), typeof(MapControl), new UIPropertyMetadata(1.8));

		public double ZoomDeltaMultiplier
		{
			get { return (double)GetValue(ZoomDeltaMultiplierProperty); }
			set { SetValue(ZoomDeltaMultiplierProperty, value); }
		}
		public static readonly DependencyProperty ZoomDeltaMultiplierProperty =
			DependencyProperty.Register("ZoomDeltaMultiplier", typeof(double), typeof(MapControl), new UIPropertyMetadata(50.0));

		[Bindable(true), Category("Common"), Description("map current zoom")]
		public double Zoom
		{
			get { return (double)GetValue(ZoomProperty); }
			set
			{
				if (value == (double)GetValue(ZoomProperty))
					return;
				BeginAnimation(ZoomProperty, null);
				SetValue(ZoomProperty, value);
			}
		}
		public static readonly DependencyProperty ZoomProperty =
			DependencyProperty.Register("Zoom", typeof(double), typeof(MapControl), new UIPropertyMetadata(1.0, Zoom_PropertyChanged));

		public MapViewModifierMode ModifierMode
		{
			get { return (MapViewModifierMode)GetValue(ModifierModeProperty); }
			set { SetValue(ModifierModeProperty, value); }
		}
		public static readonly DependencyProperty ModifierModeProperty =
			DependencyProperty.Register("ModifierMode", typeof(MapViewModifierMode), typeof(MapControl), new UIPropertyMetadata(MapViewModifierMode.None));

		public MapControlMode Mode
		{
			get { return (MapControlMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}
		public static readonly DependencyProperty ModeProperty =
			DependencyProperty.Register("Mode", typeof(MapControlMode), typeof(MapControl), new UIPropertyMetadata(MapControlMode.Custom, Mode_PropertyChanged));


		#endregion

		static MapControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MapControl),
													 new FrameworkPropertyMetadata(typeof(MapControl)));
		}

		private Point _mouseDownPos;
		private ScaleTransform _scaleTransform;
		private TranslateTransform _translateTransform;
		private Vector _startTranslate;
		private TransformGroup _transformGroup;

		private int _zoomAnimCount;
		private bool _isZooming = false;
		private List<MapItem> containers = new List<MapItem>();

		private int curZIndex = 0;
		private int maxZIndex = 1000000000;

		private MapPanel _presenter;
		protected MapPanel Presenter
		{
			get { return _presenter; }
			set
			{
				_presenter = value;
				if (_presenter == null)
					return;

				//add the ScaleTransform to the presenter
				_transformGroup = new TransformGroup();
				_scaleTransform = new ScaleTransform();
				_translateTransform = new TranslateTransform();
				_transformGroup.Children.Add(_scaleTransform);
				_transformGroup.Children.Add(_translateTransform);
				_presenter.RenderTransform = _transformGroup;
				_presenter.RenderTransformOrigin = new Point(0.5, 0.5);
			}
		}

		/// <summary> the origion point, default to be the center point (width/2, height/2) </summary>
		public Point OrigionPosition
		{
			get { return new Point(ActualWidth / 2, ActualHeight / 2); }
		}

		/// <summary> zoom map to fill the parent panel size</summary>
		public void ZoomToFill()
		{
			Mode = MapControlMode.Fill;
		}

		#region dependency property methods

		private static void Mode_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MapControl mc = d as MapControl;
			MapControlMode mode = (MapControlMode)e.NewValue;
			switch (mode)
			{
				case MapControlMode.Fill:
					mc.DoZoomToFill();
					break;
				case MapControlMode.Original:
					mc.DoZoomToOriginal();
					break;
				case MapControlMode.Custom:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static void TranslateX_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MapControl mc = d as MapControl;
			if (mc._translateTransform == null)
				return;
			mc._translateTransform.X = (double)e.NewValue;
			if (!mc._isZooming)
				mc.Mode = MapControlMode.Custom;
			mc.UpdateLocation();
		}

		private static void TranslateY_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MapControl mc = d as MapControl;
			if (mc._translateTransform == null)
				return;
			mc._translateTransform.Y = (double)e.NewValue;
			if (!mc._isZooming)
				mc.Mode = MapControlMode.Custom;
			mc.UpdateLocation();
		}

		private static void Zoom_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MapControl mc = d as MapControl;
			if (mc._scaleTransform == null)
				return;

			double zoom = (double)e.NewValue;
			mc._scaleTransform.ScaleX = zoom;
			mc._scaleTransform.ScaleY = zoom;
			if (!mc._isZooming)
			{
				double delta = (double)e.NewValue / (double)e.OldValue;
				mc.TranslateX *= delta;
				mc.TranslateY *= delta;
				mc.Mode = MapControlMode.Custom;
			}
			mc.UpdateLocation();
		}

		private static object TranslateX_Coerce(DependencyObject d, object basevalue)
		{
			MapControl mc = d as MapControl;
			return mc.GetCoercedTranslateX((double)basevalue, mc.Zoom);
		}

		private double GetCoercedTranslateX(double baseValue, double zoom)
		{
			if (_presenter == null)
				return 0.0;

			return GetCoercedTranslate(baseValue, zoom,
									   _presenter.ContentSize.Width,
									   _presenter.DesiredSize.Width,
									   ActualWidth);
		}

		private static object TranslateY_Coerce(DependencyObject d, object basevalue)
		{
			MapControl mc = d as MapControl;
			return mc.GetCoercedTranslateY((double)basevalue, mc.Zoom);
		}

		private double GetCoercedTranslateY(double baseValue, double zoom)
		{
			if (_presenter == null)
				return 0.0;

			return GetCoercedTranslate(baseValue, zoom,
									   _presenter.ContentSize.Height,
									   _presenter.DesiredSize.Height,
									   ActualHeight);
		}

		private double GetCoercedTranslate(double translate, double zoom, double contentSize, double desiredSize, double actualSize)
		{
			return translate;
		}

		#endregion

		#region private methods

		private void DoZoom(double deltaZoom, Point origoPosition, Point startHandlePosition, Point targetHandlePosition)
		{
			double startZoom = Zoom;
			double currentZoom = startZoom * deltaZoom;
			currentZoom = Math.Max(MinZoom, Math.Min(MaxZoom, currentZoom));

			Vector startTranslate = new Vector(TranslateX, TranslateY);

			Vector v = (startHandlePosition - origoPosition);
			Vector vTarget = (targetHandlePosition - origoPosition);

			Vector targetPoint = (v - startTranslate) / startZoom;
			Vector zoomedTargetPointPos = targetPoint * currentZoom + startTranslate;
			Vector endTranslate = vTarget - zoomedTargetPointPos;

			double transformX = GetCoercedTranslateX(TranslateX + endTranslate.X, currentZoom);
			double transformY = GetCoercedTranslateY(TranslateY + endTranslate.Y, currentZoom);

			DoZoomAnimation(currentZoom, transformX, transformY);
			Mode = MapControlMode.Custom;
		}

		private void DoZoomAnimation(double targetZoom, double transformX, double transformY)
		{
			_isZooming = true;
			Duration duration = new Duration(AnimationLength);
			StartAnimation(TranslateXProperty, transformX, duration);
			StartAnimation(TranslateYProperty, transformY, duration);
			StartAnimation(ZoomProperty, targetZoom, duration);
		}

		private void StartAnimation(DependencyProperty dp, double toValue, Duration duration)
		{
			if (double.IsNaN(toValue) || double.IsInfinity(toValue))
			{
				if (dp == ZoomProperty)
				{
					_isZooming = false;
				}
				return;
			}
			DoubleAnimation animation = new DoubleAnimation(toValue, duration);
			if (dp == ZoomProperty)
			{
				_zoomAnimCount++;
				animation.Completed += (s, args) =>
				{
					_zoomAnimCount--;
					if (_zoomAnimCount > 0)
						return;
					double zoom = Zoom;
					BeginAnimation(ZoomProperty, null);
					SetValue(ZoomProperty, zoom);
					_isZooming = false;
				};
			}
			BeginAnimation(dp, animation, HandoffBehavior.Compose);
		}

		public void ZoomToOriginal()
		{
			Mode = MapControlMode.Original;
		}

		private void DoZoomToOriginal()
		{
			if (_presenter == null)
				return;

			Vector initialTranslate = GetInitialTranslate();
			DoZoomAnimation(1.0, initialTranslate.X, initialTranslate.Y);
		}

		private Vector GetInitialTranslate()
		{
			if (_presenter == null)
				return new Vector(0.0, 0.0);
			double w = _presenter.ContentSize.Width - _presenter.DesiredSize.Width;
			double h = _presenter.ContentSize.Height - _presenter.DesiredSize.Height;
			double tX = -w / 2.0;
			double tY = -h / 2.0;

			return new Vector(tX, tY);
		}

		private void DoZoomToFill()
		{
			if (_presenter == null)
				return;

			double deltaZoom = Math.Min(
				ActualWidth / _presenter.ContentSize.Width,
				ActualHeight / _presenter.ContentSize.Height);

			Vector initialTranslate = GetInitialTranslate();
			DoZoomAnimation(deltaZoom, initialTranslate.X * deltaZoom, initialTranslate.Y * deltaZoom);
		}

		private void StartMove(MouseButtonEventArgs e, bool isPreview)
		{
			if (AllowPan == false || ModifierMode != MapViewModifierMode.None) return;

			if (!isPreview) ModifierMode = MapViewModifierMode.Pan;

			if (ModifierMode == MapViewModifierMode.None)
				return;

			_mouseDownPos = e.GetPosition(this);
			_startTranslate = new Vector(TranslateX, TranslateY);
			Mouse.Capture(this);
		}

		private Point TranslateLocation(MapItem i) 
		{
			Point origionPoint = new Point(i.X, i.Y);
			Point newPoint = origionPoint;
			Point center = new Point(ActualWidth / 2, ActualHeight / 2);
			newPoint.X = (newPoint.X - center.X) * Zoom + center.X;
			newPoint.Y = (newPoint.Y - center.Y) * Zoom + center.Y;
			newPoint.X += TranslateX;
			newPoint.Y += TranslateY;
			Vector offset = new Vector(-i.ImageOffsetX, -i.ActualHeight);
			return newPoint + offset;
		}

		private void UpdateLocation() 
		{
			foreach (MapItem item in containers) 
			{
				Point p = TranslateLocation(item);
				Canvas.SetLeft(item, p.X);
				Canvas.SetTop(item, p.Y);
			}
			this.UpdateLayout();
		}

		#endregion

		#region override methods

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is UIElement;
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			base.GetContainerForItemOverride();
			MapItem container = ItemTemplate.LoadContent() as MapItem;
			if (container != null) containers.Add(container);
			container.MapItemChanged += (s, a) =>
			{
				UpdateLocation();
			};
			container.MapItemPopedUp += (s) =>
			{
				curZIndex++;
				if (curZIndex > maxZIndex) 
				{
					foreach (MapItem item in containers) Canvas.SetZIndex(item, 0);
					curZIndex = 1;
				}
				Canvas.SetZIndex(s as UIElement, curZIndex);
			};
			container.MapItemPopedDowm += (s) =>
			{ 
				Canvas.SetZIndex(s as UIElement, 0);
			};
			return container;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			return base.ArrangeOverride(finalSize);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return base.MeasureOverride(availableSize);
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			MapItem container = element as MapItem;
			if (!Object.ReferenceEquals(element, item) && container != null)
			{
				if (item is UIElement) return;

			}
		}

		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			base.ClearContainerForItemOverride(element, item);

			MapItem container = element as MapItem;
			if (container != null && containers.Contains(container))containers.Remove(container);
			if (!container.Equals(item))
			{
				// release the dependency property of border
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			//get the presenter, and initialize
			Presenter = GetTemplateChild(PART_Presenter) as MapPanel;
			if (Presenter != null)
			{
				Presenter.SizeChanged += (s, a) =>
				{
					if (Mode == MapControlMode.Fill)
						DoZoomToFill();
					UpdateLocation();
				};
				Presenter.ContentSizeChanged += (s, a) =>
				{
					if (Mode == MapControlMode.Fill)
						DoZoomToFill();
					UpdateLocation();
				};
			}
			ZoomToFill();
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			if (AllowZoom == false || ModifierMode != MapViewModifierMode.None) return;
			e.Handled = true;
			Point origoPosition = new Point(ActualWidth / 2, ActualHeight / 2);
			Point mousePosition = e.GetPosition(this);

			double max = MaxZoomDelta;
			double min = 1 / MaxZoomDelta;
			double delta = (e.Delta + 120) / 240.0 * (max - min) + min;
			delta = Math.Max(min, Math.Min(max, delta));

			DoZoom(
				delta, //Math.Max(1 / MaxZoomDelta, Math.Min(MaxZoomDelta, e.Delta / 10000.0 * ZoomDeltaMultiplier + 1)),
				origoPosition,
				mousePosition,
				mousePosition);
		}

		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			if (ModifierMode == MapViewModifierMode.Pan)
			{
				Vector translate = _startTranslate + (e.GetPosition(this) - _mouseDownPos);
				TranslateX = translate.X;
				TranslateY = translate.Y;
			}
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			StartMove(e, false);
		}

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			StartMove(e, true);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			switch (ModifierMode)
			{
				case MapViewModifierMode.None:
					return;
				case MapViewModifierMode.Pan:
					break;
				case MapViewModifierMode.ZoomIn:
					break;
				case MapViewModifierMode.ZoomOut:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			ModifierMode = MapViewModifierMode.None;
			ReleaseMouseCapture();
		}

		#endregion
	}

	public enum MapControlMode
	{
		/// <summary>
		/// The content should fill the given space.
		/// </summary>
		Fill,

		/// <summary>
		/// The content will be represented in its original size.
		/// </summary>
		Original,

		/// <summary>
		/// The content will be zoomed with a custom percent.
		/// </summary>
		Custom
	}

	public enum MapViewModifierMode
	{
		/// <summary>
		/// It does nothing at all.
		/// </summary>
		None,

		/// <summary>
		/// You can pan the view with the mouse in this mode.
		/// </summary>
		Pan,

		/// <summary>
		/// You can zoom in with the mouse in this mode.
		/// </summary>
		ZoomIn,

		/// <summary>
		/// You can zoom out with the mouse in this mode.
		/// </summary>
		ZoomOut,
	}
}
