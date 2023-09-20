using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Tutorial
{
	// Token: 0x0200000F RID: 15
	public class ElementNotificationVM : ViewModel
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x0000320C File Offset: 0x0000140C
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x00003214 File Offset: 0x00001414
		[DataSourceProperty]
		public string ElementID
		{
			get
			{
				return this._elementID;
			}
			set
			{
				if (value != this._elementID)
				{
					this._elementID = value;
					base.OnPropertyChangedWithValue<string>(value, "ElementID");
				}
			}
		}

		// Token: 0x04000047 RID: 71
		private string _elementID = string.Empty;
	}
}
