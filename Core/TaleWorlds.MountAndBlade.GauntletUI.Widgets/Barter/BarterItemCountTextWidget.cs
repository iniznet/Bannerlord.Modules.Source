using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Barter
{
	// Token: 0x0200016E RID: 366
	public class BarterItemCountTextWidget : TextWidget
	{
		// Token: 0x060012B2 RID: 4786 RVA: 0x0003398E File Offset: 0x00031B8E
		public BarterItemCountTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x060012B3 RID: 4787 RVA: 0x00033997 File Offset: 0x00031B97
		// (set) Token: 0x060012B4 RID: 4788 RVA: 0x0003399F File Offset: 0x00031B9F
		[Editor(false)]
		public int Count
		{
			get
			{
				return this._count;
			}
			set
			{
				if (this._count != value)
				{
					this._count = value;
					base.OnPropertyChanged(value, "Count");
					base.IntText = value;
					base.IsVisible = value > 1;
				}
			}
		}

		// Token: 0x04000890 RID: 2192
		private int _count;
	}
}
