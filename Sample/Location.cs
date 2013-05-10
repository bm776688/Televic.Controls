using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Sample
{
	public class Location : INotifyPropertyChanged
	{
		private double x;
		public double X
		{
			get { return x; }
			set
			{
				if (x != value)
				{
					x = value;
					NotifyPropertyChanged("X");
				}
			}
		}

		private double y;
		public double Y
		{
			get { return y; }
			set
			{
				if (y != value)
				{
					y = value;
					NotifyPropertyChanged("Y");
				}
			}
		}

		private string field1;
		public string Field1
		{
			get { return field1; }
			set
			{
				if (field1 != value)
				{
					field1 = value;
					NotifyPropertyChanged("Field1");
				}
			}
		}

		private string field2;
		public string Field2
		{
			get { return field2; }
			set
			{
				if (field2 != value)
				{
					field2 = value;
					NotifyPropertyChanged("Field2");
				}
			}
		}

		private string field3;
		public string Field3
		{
			get { return field3; }
			set
			{
				if (field3 != value)
				{
					field3 = value;
					NotifyPropertyChanged("Field3");
				}
			}
		}

		private string field4;
		public string Field4
		{
			get { return field4; }
			set
			{
				if (field4 != value)
				{
					field4 = value;
					NotifyPropertyChanged("Field4");
				}
			}
		}

		private Person personInfo;
		public Person PersonInfo
		{
			get { return personInfo; }
			set
			{
				if (personInfo != value)
				{
					personInfo = value;
					NotifyPropertyChanged("PersonInfo");
				}
			}
		}

		private ImageSource pinImage;
		public ImageSource PinImage
		{
			get { return pinImage; }
			set
			{
				if (pinImage != value)
				{
					pinImage = value;
					NotifyPropertyChanged("PinImage");
				}
			}
		}

		public Location(double x, double y, ImageSource pinImage = null) 
		{
			X = x;
			Y = y;
			PinImage = pinImage;
			Field1 = "AAA";
			Field2 = "内容";
			Field3 = "hehe";
			Field4 = "内容十二";
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

	public class LocationCollection : ObservableCollection<Location> 
	{
	}
}
