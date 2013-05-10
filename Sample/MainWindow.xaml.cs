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

namespace Sample
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		MainWindowViewModel viewModel;
		public MainWindow()
		{
			viewModel = new MainWindowViewModel();
			InitializeComponent();
			this.DataContext = viewModel;
			this.calendar.SelectedDate = DateTime.Today;
		}

		private void MapControl_MouseDown(object sender, MouseButtonEventArgs e)
		{
			//map.RenderTransform = new ScaleTransform(0.5, 0.5);
		}

		private void buttonAdd_Click(object sender, RoutedEventArgs e)
		{
			viewModel.Locations.Add(new Location(500, 100));
		}

		private void buttonRemove_Click(object sender, RoutedEventArgs e)
		{
			if(viewModel.Locations.Count > 0)viewModel.Locations.RemoveAt(0);
		}

		private void buttonModify_Click(object sender, RoutedEventArgs e)
		{
			if(viewModel.Locations.Count > 0)viewModel.Locations[0].Y = 400;
		}

		private void search_Click(object sender, RoutedEventArgs e)
		{
			searchTabControl.SearchTabItem("Contact us");
		}

		private void searchTabControl_SearchBarContentChanged(object sender, TextChangedEventArgs e)
		{
			state.Text = searchTabControl.SearchBarText;
		}

		private void GridCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
		{
			string dir = Environment.CurrentDirectory;
			string filename = "";
			if (calendar.SelectedDate.HasValue)
			{
				filename = calendar.SelectedDate.Value.ToString("yyyy-MM-dd");
				viewModel.CallRecordData.Source = new Uri(string.Format("{0}/data/{1}.xml", dir, filename));
			}
		}

		private void dataGridCallRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// doing some Web service functions
		}
	}
}
