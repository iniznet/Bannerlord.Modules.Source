using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	// Token: 0x020000B6 RID: 182
	public class ServerStatusItemBrushWidget : BrushWidget
	{
		// Token: 0x06000977 RID: 2423 RVA: 0x0001B14C File Offset: 0x0001934C
		public ServerStatusItemBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x0001B15C File Offset: 0x0001935C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.RegisterBrushStatesOfWidget();
				this._initialized = true;
				this.OnStatusChange(this.Status);
			}
			if (Math.Abs(base.ReadOnlyBrush.GlobalAlphaFactor - this._currentAlphaTarget) > 0.001f)
			{
				this.SetGlobalAlphaRecursively(MathF.Lerp(base.ReadOnlyBrush.GlobalAlphaFactor, this._currentAlphaTarget, dt * 5f, 1E-05f));
			}
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0001B1D7 File Offset: 0x000193D7
		private void OnStatusChange(int value)
		{
			this.SetState(value.ToString());
			if (value == 0)
			{
				this._currentAlphaTarget = 0f;
				return;
			}
			if (value - 1 > 1)
			{
				return;
			}
			this._currentAlphaTarget = 1f;
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x0600097A RID: 2426 RVA: 0x0001B207 File Offset: 0x00019407
		// (set) Token: 0x0600097B RID: 2427 RVA: 0x0001B20F File Offset: 0x0001940F
		public int Status
		{
			get
			{
				return this._status;
			}
			set
			{
				if (value != this._status)
				{
					this._status = value;
					base.OnPropertyChanged(value, "Status");
					this.OnStatusChange(value);
				}
			}
		}

		// Token: 0x0400045B RID: 1115
		private float _currentAlphaTarget;

		// Token: 0x0400045C RID: 1116
		private bool _initialized;

		// Token: 0x0400045D RID: 1117
		private int _status = -1;
	}
}
