using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	[GameStateScreen(typeof(CustomBattleState))]
	public class CustomBattleScreen : ScreenBase, IGameStateListener
	{
		public CustomBattleScreen(CustomBattleState customBattleState)
		{
			this._customBattleState = customBattleState;
		}

		void IGameStateListener.OnActivate()
		{
		}

		void IGameStateListener.OnDeactivate()
		{
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
			this._dataSource.OnFinalize();
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._dataSource = new CustomBattleMenuVM(this._customBattleState);
			this._dataSource.SetStartInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			this._dataSource.SetRandomizeInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Randomize"));
			TroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this._dataSource.TroopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp != null)
			{
				troopTypeSelectionPopUp.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			}
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", true);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this.LoadMovie();
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._dataSource.SetActiveState(true);
			base.AddLayer(this._gauntletLayer);
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._isFirstFrameCounter >= 0)
			{
				if (this._isFirstFrameCounter == 0)
				{
					LoadingWindow.DisableGlobalLoadingWindow();
				}
				else
				{
					this._shouldTickLayersThisFrame = false;
				}
				this._isFirstFrameCounter--;
			}
			if (!this._gauntletLayer.IsFocusedOnInput())
			{
				TroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this._dataSource.TroopTypeSelectionPopUp;
				if (troopTypeSelectionPopUp != null && troopTypeSelectionPopUp.IsOpen)
				{
					if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Exit"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._dataSource.TroopTypeSelectionPopUp.ExecuteCancel();
						return;
					}
					if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._dataSource.TroopTypeSelectionPopUp.ExecuteDone();
						return;
					}
					if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Reset"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._dataSource.TroopTypeSelectionPopUp.ExecuteReset();
						return;
					}
				}
				else
				{
					if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Exit"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._dataSource.ExecuteBack();
						return;
					}
					if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Randomize"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._dataSource.ExecuteRandomize();
						return;
					}
					if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._dataSource.ExecuteStart();
					}
				}
			}
		}

		protected override void OnFinalize()
		{
			this.UnloadMovie();
			base.RemoveLayer(this._gauntletLayer);
			this._dataSource = null;
			this._gauntletLayer = null;
			base.OnFinalize();
		}

		protected override void OnActivate()
		{
			this.LoadMovie();
			CustomBattleMenuVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.SetActiveState(true);
			}
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			this._isFirstFrameCounter = 2;
			base.OnActivate();
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this.UnloadMovie();
			CustomBattleMenuVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.SetActiveState(false);
		}

		public override void UpdateLayout()
		{
			base.UpdateLayout();
			if (!this._isMovieLoaded)
			{
				CustomBattleMenuVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.RefreshValues();
			}
		}

		private void LoadMovie()
		{
			if (!this._isMovieLoaded)
			{
				this._gauntletMovie = this._gauntletLayer.LoadMovie("CustomBattleScreen", this._dataSource);
				this._isMovieLoaded = true;
			}
		}

		private void UnloadMovie()
		{
			if (this._isMovieLoaded)
			{
				this._gauntletLayer.ReleaseMovie(this._gauntletMovie);
				this._gauntletMovie = null;
				this._isMovieLoaded = false;
				this._gauntletLayer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(this._gauntletLayer);
			}
		}

		private CustomBattleState _customBattleState;

		private GauntletLayer _gauntletLayer;

		private IGauntletMovie _gauntletMovie;

		private CustomBattleMenuVM _dataSource;

		private bool _isMovieLoaded;

		private int _isFirstFrameCounter;
	}
}
