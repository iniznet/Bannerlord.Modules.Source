using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	[OverrideView(typeof(MissionBoundaryCrossingView))]
	public class MissionGauntletBoundaryCrossingView : MissionGauntletBattleUIBase
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
		}

		protected override void OnCreateView()
		{
			this._dataSource = new BoundaryCrossingVM(base.Mission, new Action<bool>(this.OnEscapeMenuToggled));
			this._gauntletLayer = new GauntletLayer(47, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("BoundaryCrossing", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		protected override void OnDestroyView()
		{
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		private void OnEscapeMenuToggled(bool isOpened)
		{
			if (base.IsViewActive)
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, !isOpened);
			}
		}

		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		private GauntletLayer _gauntletLayer;

		private BoundaryCrossingVM _dataSource;
	}
}
