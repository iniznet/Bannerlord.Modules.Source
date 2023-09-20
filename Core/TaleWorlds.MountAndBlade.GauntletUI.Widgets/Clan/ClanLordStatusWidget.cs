using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	public class ClanLordStatusWidget : Widget
	{
		public ClanLordStatusWidget(UIContext context)
			: base(context)
		{
		}

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

		private int _statusType = -1;
	}
}
