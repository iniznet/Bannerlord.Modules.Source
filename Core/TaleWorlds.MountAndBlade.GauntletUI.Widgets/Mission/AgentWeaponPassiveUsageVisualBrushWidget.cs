using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class AgentWeaponPassiveUsageVisualBrushWidget : BrushWidget
	{
		public AgentWeaponPassiveUsageVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool _firstUpdate;

		private int _couchLanceState = -1;
	}
}
