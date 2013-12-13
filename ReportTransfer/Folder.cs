using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ReportTransfer.ReportService;

namespace ReportTransfer
{
	public class Folder : IRSItem, INotifyPropertyChanged
	{
		public Folder()
		{
			Reports = new ObservableCollection<Report>();
			SubFolders = new ObservableCollection<Folder>();
			Selected = true;
		}

		public Folder(CatalogItem item) : this()
		{
			Name = item.Name;
			Path = item.Path;
			string[] parts = Path.Split('/');
			Depth = parts.Length;
		}
		public string Name { get; set; }
		public string Path { get; set; }
		public Folder ParentFolder { get; set; }
		public ObservableCollection<Report> Reports { get; set; }
		public ObservableCollection<Folder> SubFolders { get; set; }
		public int Depth { get; set; }

		public void SetNewRootPath(string newRootFolderName)
		{
			string[] pathParts = Path.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
			pathParts[0] = newRootFolderName;
			Path = "/" + string.Join("/", pathParts);
		}

		private bool _selected;
		public bool Selected
		{
			get { return _selected; }
			set
			{
				_selected = value;
				foreach (var report in Reports)
				{
					report.Selected = value;
				}
				foreach (var subFolder in SubFolders)
				{
					subFolder.Selected = value;
				}
				RaisePropertyChanged("Selected");
			}
		}

		public bool ShouldTransfer
		{
			get { return _selected || Children.Any(c => c.Selected); }
		}

		public ObservableCollection<IRSItem> Children
		{
			get
			{
				var c = SubFolders.Cast<IRSItem>().Union(Reports.Cast<IRSItem>());
				return new ObservableCollection<IRSItem>(c);
			}
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

	public interface IRSItem
	{
		string Name { get; set; }
		string Path { get; set; }
		bool Selected { get; set; }
		ObservableCollection<IRSItem> Children { get; } 

	}
}