using System;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	public abstract class MissionGauntletBattleUIBase : MissionView
	{
		private protected bool IsViewActive { protected get; private set; }

		protected abstract void OnCreateView();

		protected abstract void OnDestroyView();

		private void OnEnableView()
		{
			this.OnCreateView();
			this.IsViewActive = true;
		}

		private void OnDisableView()
		{
			this.OnDestroyView();
			this.IsViewActive = false;
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			if (GameNetwork.IsMultiplayer)
			{
				this.OnEnableView();
			}
		}

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
