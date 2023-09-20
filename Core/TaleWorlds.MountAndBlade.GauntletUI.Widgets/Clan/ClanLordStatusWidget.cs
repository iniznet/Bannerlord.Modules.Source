using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	// Token: 0x02000154 RID: 340
	public class ClanLordStatusWidget : Widget
	{
		// Token: 0x0600119E RID: 4510 RVA: 0x00030975 File Offset: 0x0002EB75
		public ClanLordStatusWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x00030988 File Offset: 0x0002EB88
		private void SetVisualState(int type)
		{
			switch (type)
			{
			case 0:
				this.SetState("Dead");
				return;
			case 1:
				this.SetState("Married");
				return;
			case 2:
				this.SetState("Pregnant");
				return;
			case 3:
				this.SetState("InBattle");
				return;
			case 4:
				this.SetState("InSiege");
				return;
			case 5:
				this.SetState("Child");
				return;
			case 6:
				this.SetState("Prisoner");
				return;
			case 7:
				this.SetState("Sick");
				return;
			default:
				return;
			}
		}

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x060011A0 RID: 4512 RVA: 0x00030A1B File Offset: 0x0002EC1B
		// (set) Token: 0x060011A1 RID: 4513 RVA: 0x00030A23 File Offset: 0x0002EC23
		[Editor(false)]
		public int StatusType
		{
			get
			{
				return this._statusType;
			}
			set
			{
				if (this._statusType != value)
				{
					this._statusType = value;
					base.OnPropertyChanged(value, "StatusType");
					this.SetVisualState(value);
				}
			}
		}

		// Token: 0x04000810 RID: 2064
		private int _statusType = -1;
	}
}
