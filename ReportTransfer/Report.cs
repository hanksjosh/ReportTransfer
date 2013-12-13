using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ReportTransfer.ReportService;

namespace ReportTransfer
{
	public class Report : IRSItem, INotifyPropertyChanged
	{
		private ObservableCollection<IRSItem> _children; 
		public Report()
		{
			_children = new ObservableCollection<IRSItem>();
			Selected = true;
		}
		public Report(CatalogItem item) : this()
		{
			Name = item.Name;
			Path = item.Path;
		}
		public string Name { get; set; }
		public string Path { get; set; }
		public Folder ParentFolder { get; set; }

		private bool _selected;
		public bool Selected
		{
			get { return _selected; }
			set
			{
				_selected = value; 
				RaisePropertyChanged("Selected");
			}
		}


		public ObservableCollection<IRSItem> Children
		{
			get { return _children; }
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}

			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

	}
}