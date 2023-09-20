using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	// Token: 0x02000158 RID: 344
	public class ClanWorkshopTypeVisualBrushWidget : BrushWidget
	{
		// Token: 0x060011B0 RID: 4528 RVA: 0x00030D07 File Offset: 0x0002EF07
		public ClanWorkshopTypeVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00030D1B File Offset: 0x0002EF1B
		private void SetVisualState(string type)
		{
			this.RegisterBrushStatesOfWidget();
			this.SetState(type);
		}

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x060011B2 RID: 4530 RVA: 0x00030D2A File Offset: 0x0002EF2A
		// (set) Token: 0x060011B3 RID: 4531 RVA: 0x00030D32 File Offset: 0x0002EF32
		[Editor(false)]
		public string WorkshopType
		{
			get
			{
				return this._workshopType;
			}
			set
			{
				if (this._workshopType != value)
				{
					this._workshopType = value;
					base.OnPropertyChanged<string>(value, "WorkshopType");
					this.SetVisualState(value);
				}
			}
		}

		// Token: 0x04000815 RID: 2069
		private string _workshopType = "";
	}
}
