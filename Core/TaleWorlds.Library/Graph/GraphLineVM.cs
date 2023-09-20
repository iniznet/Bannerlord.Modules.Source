using System;

namespace TaleWorlds.Library.Graph
{
	public class GraphLineVM : ViewModel
	{
		public GraphLineVM(string ID, string name)
		{
			this.Points = new MBBindingList<GraphLinePointVM>();
			this.Name = name;
			this.ID = ID;
		}

		[DataSourceProperty]
		public MBBindingList<GraphLinePointVM> Points
		{
			get
			{
				return this._points;
			}
			set
			{
				if (value != this._points)
				{
					this._points = value;
					base.OnPropertyChangedWithValue<MBBindingList<GraphLinePointVM>>(value, "Points");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public string ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if (value != this._ID)
				{
					this._ID = value;
					base.OnPropertyChangedWithValue<string>(value, "ID");
				}
			}
		}

		private MBBindingList<GraphLinePointVM> _points;

		private string _name;

		private string _ID;
	}
}
