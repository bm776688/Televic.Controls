using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;

namespace Sample
{
	class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel() 
		{
			Locations = new LocationCollection();
			TestContent = "Wahaha";
			Locations.Add(new Location(0, 0));
			Locations.Add(new Location(101, 110));
			Locations.Add(new Location(390, 450));
			Person person = new Person();
			person.Name = "Steve Ballmer";
			person.Image = new BitmapImage(new Uri("image/photo.png", UriKind.Relative));
			Location loc = new Location(500, 583);
			loc.PersonInfo = person;
			Locations.Add(loc);

			string dataPath = Environment.CurrentDirectory;
			CallRecordData = new XmlDataProvider();
			CallRecordData.Source = new Uri(dataPath + "/data/2013-03-16.xml");
			CallRecordData.InitialLoad();
		}


		private XmlDataProvider callRecordData;
		public XmlDataProvider CallRecordData
		{
			get { return callRecordData; }
			set
			{
				if (callRecordData != value)
				{
					callRecordData = value;
					NotifyPropertyChanged("CallRecordData");
				}
			}
		}


		private LocationCollection locations;
		public LocationCollection Locations
		{
			get { return locations; }
			set
			{
				if (locations != value)
				{
					locations = value;
					NotifyPropertyChanged("Locations");
				}
			}
		}

		private string testContent;
		public string TestContent
		{
			get { return testContent; }
			set
			{
				if (testContent != value)
				{
					testContent = value;
					NotifyPropertyChanged("TestContent");
				}
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
