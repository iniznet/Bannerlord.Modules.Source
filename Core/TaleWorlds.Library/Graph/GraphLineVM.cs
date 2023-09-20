using System;

namespace TaleWorlds.Library.Graph
{
	// Token: 0x020000AC RID: 172
	public class GraphLineVM : ViewModel
	{
		// Token: 0x0600062E RID: 1582 RVA: 0x000133A7 File Offset: 0x000115A7
		public GraphLineVM(string ID, string name)
		{
			this.Points = new MBBindingList<GraphLinePointVM>();
			this.Name = name;
			this.ID = ID;
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x0600062F RID: 1583 RVA: 0x000133C8 File Offset: 0x000115C8
		// (set) Token: 0x06000630 RID: 1584 RVA: 0x000133D0 File Offset: 0x000115D0
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

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000631 RID: 1585 RVA: 0x000133EE File Offset: 0x000115EE
		// (set) Token: 0x06000632 RID: 1586 RVA: 0x000133F6 File Offset: 0x000115F6
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

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000633 RID: 1587 RVA: 0x00013419 File Offset: 0x00011619
		// (set) Token: 0x06000634 RID: 1588 RVA: 0x00013421 File Offset: 0x00011621
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

		// Token: 0x040001DB RID: 475
		private MBBindingList<GraphLinePointVM> _points;

		// Token: 0x040001DC RID: 476
		private string _name;

		// Token: 0x040001DD RID: 477
		private string _ID;
	}
}
