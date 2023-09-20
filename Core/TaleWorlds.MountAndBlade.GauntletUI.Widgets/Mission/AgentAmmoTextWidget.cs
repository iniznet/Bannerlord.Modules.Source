using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class AgentAmmoTextWidget : TextWidget
	{
		public AgentAmmoTextWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.IsAlertEnabled)
			{
				this.SetState("Alert");
				return;
			}
			this.SetState("Default");
		}

		public bool IsAlertEnabled
		{
			get
			{
				return this._isAlertEnabled;
			}
			set
			{
				if (this._isAlertEnabled != value)
				{
					this._isAlertEnabled = value;
					base.OnPropertyChanged(value, "IsAlertEnabled");
				}
			}
		}

		private bool _isAlertEnabled;
	}
}
