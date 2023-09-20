using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000C1 RID: 193
	public class AgentAmmoTextWidget : TextWidget
	{
		// Token: 0x060009C7 RID: 2503 RVA: 0x0001BED4 File Offset: 0x0001A0D4
		public AgentAmmoTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x0001BEDD File Offset: 0x0001A0DD
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

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x060009C9 RID: 2505 RVA: 0x0001BF05 File Offset: 0x0001A105
		// (set) Token: 0x060009CA RID: 2506 RVA: 0x0001BF0D File Offset: 0x0001A10D
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

		// Token: 0x04000477 RID: 1143
		private bool _isAlertEnabled;
	}
}
