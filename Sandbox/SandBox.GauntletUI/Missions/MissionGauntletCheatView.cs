using System;
using System.Collections.Generic;
using SandBox.ViewModelCollection.Map.Cheat;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Missions
{
	[OverrideView(typeof(MissionCheatView))]
	public class MissionGauntletCheatView : MissionCheatView
	{
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this.FinalizeScreen();
		}

		public override bool GetIsCheatsAvailable()
		{
			return true;
		}

		public override void InitializeScreen()
		{
			if (this._isActive)
			{
				return;
			}
			this._isActive = true;
			IEnumerable<GameplayCheatBase> missionCheatList = GameplayCheatsManager.GetMissionCheatList();
			this._dataSource = new GameplayCheatsVM(new Action(this.FinalizeScreen), missionCheatList);
			this.InitializeKeyVisuals();
			this._gauntletLayer = new GauntletLayer(4500, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MapCheats", this._dataSource);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		public override void FinalizeScreen()
		{
			if (!this._isActive)
			{
				return;
			}
			this._isActive = false;
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			GameplayCheatsVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._gauntletLayer = null;
			this._dataSource = null;
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._isActive)
			{
				this.HandleInput();
			}
		}

		private void HandleInput()
		{
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				GameplayCheatsVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.ExecuteClose();
			}
		}

		private void InitializeKeyVisuals()
		{
			this._dataSource.SetCloseInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		}

		private GauntletLayer _gauntletLayer;

		private GameplayCheatsVM _dataSource;

		private bool _isActive;
	}
}
