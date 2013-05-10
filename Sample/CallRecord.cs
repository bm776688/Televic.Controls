using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Sample
{
	public class CallRecord : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}


		private DateTime date;
		public DateTime Date
		{
			get { return date; }
			set
			{
				if (date != value)
				{
					date = value;
					NotifyPropertyChanged("Date");
				}
			}
		}


		private string wardName;
		public string WardName
		{
			get { return wardName; }
			set
			{
				if (wardName != value)
				{
					wardName = value;
					NotifyPropertyChanged("WardName");
				}
			}
		}


		private string wardNumber;
		public string WardNumber
		{
			get { return wardNumber; }
			set
			{
				if (wardNumber != value)
				{
					wardNumber = value;
					NotifyPropertyChanged("WardNumber");
				}
			}
		}


		private string callSource;
		public string CallSource
		{
			get { return callSource; }
			set
			{
				if (callSource != value)
				{
					callSource = value;
					NotifyPropertyChanged("CallSource");
				}
			}
		}


		private CallType callType;
		public CallType CallType
		{
			get { return callType; }
			set
			{
				if (callType != value)
				{
					callType = value;
					NotifyPropertyChanged("CallType");
				}
			}
		}


		private DateTime callTime;
		public DateTime CallTime
		{
			get { return callTime; }
			set
			{
				if (callTime != value)
				{
					callTime = value;
					NotifyPropertyChanged("CallTime");
				}
			}
		}

	}

	public enum CallType { 
		Normal,
		Emergency,
		Other
	}

	public class CallRecordCollection : ObservableCollection<CallRecord> { }
}
