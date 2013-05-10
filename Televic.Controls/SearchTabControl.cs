using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace Televic.Controls
{
	/// <summary> Tab control </summary>
	public class SearchTabControl : TabControl
	{
		private Path navigateShape;
		private TextBox searchBar;

		/// <summary> Occurs when the searchbar content has been changed </summary>
		public event TextChangedEventHandler SearchBarContentChanged;

		#region dependency properties

		[Bindable(false)]
		public double NavigateLeftPosition
		{
			get { return (double)GetValue(NavigateLeftPositionProperty); }
			set { SetValue(NavigateLeftPositionProperty, value); }
		}
		public static readonly DependencyProperty NavigateLeftPositionProperty =
			DependencyProperty.Register("NavigateLeftPosition", typeof(double), typeof(SearchTabControl), new UIPropertyMetadata(0.0));

		[Bindable(true),Category("Visibility"),Description("Get or set the visibility of search bar")]
		public Visibility SearchBarVisibility
		{
			get { return (Visibility)GetValue(SearchBarVisibilityProperty); }
			set { SetValue(SearchBarVisibilityProperty, value); }
		}
		public static readonly DependencyProperty SearchBarVisibilityProperty =
			DependencyProperty.Register("SearchBarVisibility", typeof(Visibility), typeof(SearchTabControl), new UIPropertyMetadata(Visibility.Visible));

		[Bindable(true),Category("Layout"),Description("Get or set the search bar width")]
		public double SearchBarWidth
		{
			get { return (double)GetValue(SearchBarWidthProperty); }
			set { SetValue(SearchBarWidthProperty, value); }
		}
		public static readonly DependencyProperty SearchBarWidthProperty =
			DependencyProperty.Register("SearchBarWidth", typeof(double), typeof(SearchTabControl), new UIPropertyMetadata(150.0));

		[Bindable(true),Category("Common"),Description("Get or set search bar text box content")]
		public string SearchBarText
		{
			get { return (string)GetValue(SearchBarTextProperty); }
			set { SetValue(SearchBarTextProperty, value); }
		}
		public static readonly DependencyProperty SearchBarTextProperty =
			DependencyProperty.Register("SearchBarText", typeof(string), typeof(SearchTabControl), new UIPropertyMetadata(""));

		[Bindable(true),Category("Brushes"),Description("tab panel background")]
		public Brush TabPanelBackground
		{
			get { return (Brush)GetValue(TabPanelBackgroundProperty); }
			set { SetValue(TabPanelBackgroundProperty, value); }
		}
		public static readonly DependencyProperty TabPanelBackgroundProperty =
			DependencyProperty.Register("TabPanelBackground", typeof(Brush), typeof(SearchTabControl), new UIPropertyMetadata(null));

		
		#endregion

		static SearchTabControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchTabControl), new FrameworkPropertyMetadata(typeof(SearchTabControl)));
		}

		#region override methods

		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			if (e.AddedItems != null && e.AddedItems.Count > 0) UpdateNavigateShapePosition();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			navigateShape = this.GetTemplateChild("navigateShape") as Path;
			searchBar = this.GetTemplateChild("PART_searchBar") as TextBox;
			if (searchBar != null)
				searchBar.TextChanged += (s, e) =>
				{
					if (SearchBarContentChanged != null) SearchBarContentChanged(this, e);
				};
			Canvas searchIcon = this.GetTemplateChild("searchIcon") as Canvas;
			if(searchIcon != null)
				searchIcon.MouseLeftButtonDown += (s,e) =>
				{
					SearchTabItem(this.SearchBarText);
				};
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			Size size = base.ArrangeOverride(arrangeBounds);
			this.UpdateNavigateShapePosition();
			return size;
		}

		#endregion

		// update the small triangle's location
		private void UpdateNavigateShapePosition()
		{
			if (navigateShape == null) return;
			TabItem item = this.SelectedItem as TabItem;
			double widthOffset;
			if (item == null) widthOffset = 2000;
			else widthOffset = item.ActualWidth / 2;
			double left = 0;
			for (int i = 0; i < SelectedIndex; i++)
			{
				TabItem leftItem = this.Items[i] as TabItem;
				if (leftItem != null) left += leftItem.ActualWidth;
			}
			double destPosition = widthOffset + left - navigateShape.ActualWidth / 2;
			StartAnimation(NavigateLeftPositionProperty, destPosition, new Duration(TimeSpan.FromMilliseconds(500)));
		}

		// start the animation of an dependency property
		private void StartAnimation(DependencyProperty dp, double toValue, Duration duration)
		{
			if (double.IsNaN(toValue) || double.IsInfinity(toValue)) return;
			DoubleAnimation animation = new DoubleAnimation(toValue, duration);
			BeginAnimation(dp, animation, HandoffBehavior.Compose);
		}

		/// <summary>
		/// search a table item which header contains the given string
		/// </summary>
		/// <param name="header">given search string</param>
		public void SearchTabItem(string header) 
		{
			foreach (TabItem item in this.Items) 
			{
				if (item.HasHeader && item.Header is string && item.Header.ToString().Contains(header) && item.IsEnabled)
				{
					this.SelectedItem = item;
					break;
				}
			}
		}
	}
}
