using System;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	// Token: 0x02000023 RID: 35
	public abstract class MissionGauntletBattleUIBase : MissionView
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600019E RID: 414 RVA: 0x00009017 File Offset: 0x00007217
		// (set) Token: 0x0600019F RID: 415 RVA: 0x0000901F File Offset: 0x0000721F
		private protected bool IsViewActive { protected get; private set; }

		// Token: 0x060001A0 RID: 416
		protected abstract void OnCreateView();

		// Token: 0x060001A1 RID: 417
		protected abstract void OnDestroyView();

		// Token: 0x060001A2 RID: 418 RVA: 0x00009028 File Offset: 0x00007228
		private void OnEnableView()
		{
			this.OnCreateView();
			this.IsViewActive = true;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00009037 File Offset: 0x00007237
		private void OnDisableView()
		{
			this.OnDestroyView();
			this.IsViewActive = false;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00009046 File Offset: 0x00007246
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			if (GameNetwork.IsMultiplayer)
			{
				this.OnEnableView();
			}
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000905C File Offset: 0x0000725C
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (!GameNetwork.IsMultiplayer && !MBCommon.IsPaused)
			{
				if (!this.IsViewActive && !BannerlordConfig.HideBattleUI)
				{
					this.OnEnableView();
					return;
				}
				if (this.IsViewActive && BannerlordConfig.HideBattleUI)
				{
					this.OnDisableView();
				}
			}
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x000090A9 File Offset: 0x000072A9
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			if (this.IsViewActive)
			{
				this.OnDisableView();
			}
		}
	}
}
