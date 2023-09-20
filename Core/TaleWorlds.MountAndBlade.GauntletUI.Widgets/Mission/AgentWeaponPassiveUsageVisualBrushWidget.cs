using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000C4 RID: 196
	public class AgentWeaponPassiveUsageVisualBrushWidget : BrushWidget
	{
		// Token: 0x060009E3 RID: 2531 RVA: 0x0001C365 File Offset: 0x0001A565
		public AgentWeaponPassiveUsageVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x0001C378 File Offset: 0x0001A578
		private void UpdateVisualState()
		{
			if (this._firstUpdate)
			{
				this.RegisterBrushStatesOfWidget();
				this._firstUpdate = false;
			}
			switch (this.CouchLanceState)
			{
			case 0:
				base.IsVisible = false;
				return;
			case 1:
				base.IsVisible = true;
				this.SetState("ConditionsNotMet");
				return;
			case 2:
				base.IsVisible = true;
				this.SetState("Possible");
				return;
			case 3:
				this.SetState("Active");
				base.IsVisible = true;
				return;
			default:
				return;
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x060009E5 RID: 2533 RVA: 0x0001C3F8 File Offset: 0x0001A5F8
		// (set) Token: 0x060009E6 RID: 2534 RVA: 0x0001C400 File Offset: 0x0001A600
		[Editor(false)]
		public int CouchLanceState
		{
			get
			{
				return this._couchLanceState;
			}
			set
			{
				if (this._couchLanceState != value)
				{
					this._couchLanceState = value;
					base.OnPropertyChanged(value, "CouchLanceState");
					this.UpdateVisualState();
				}
			}
		}

		// Token: 0x04000484 RID: 1156
		private bool _firstUpdate;

		// Token: 0x04000485 RID: 1157
		private int _couchLanceState = -1;
	}
}
