using System;
using SandBox.BoardGames.MissionLogics;
using SandBox.ViewModelCollection.BoardGame;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Missions
{
	[OverrideView(typeof(BoardGameView))]
	public class MissionGauntletBoardGameView : MissionView, IBoardGameHandler
	{
		public MissionBoardGameLogic _missionBoardGameHandler { get; private set; }

		public Camera Camera { get; private set; }

		public MissionGauntletBoardGameView()
		{
			this.ViewOrderPriority = 2;
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			base.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("BoardGameHotkeyCategory"));
		}

		public override void OnMissionScreenActivate()
		{
			base.OnMissionScreenActivate();
			this._missionBoardGameHandler = base.Mission.GetMissionBehavior<MissionBoardGameLogic>();
			if (this._missionBoardGameHandler != null)
			{
				this._missionBoardGameHandler.Handler = this;
			}
		}

		void IBoardGameHandler.Activate()
		{
			this._dataSource.Activate();
		}

		void IBoardGameHandler.SwitchTurns()
		{
			BoardGameVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.SwitchTurns();
		}

		void IBoardGameHandler.DiceRoll(int roll)
		{
			BoardGameVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.DiceRoll(roll);
		}

		void IBoardGameHandler.Install()
		{
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._spriteCategory = spriteData.SpriteCategories["ui_boardgame"];
			this._spriteCategory.Load(resourceContext, uiresourceDepot);
			this._dataSource = new BoardGameVM();
			this._dataSource.SetRollDiceKey(HotKeyManager.GetCategory("BoardGameHotkeyCategory").GetHotKey("BoardGameRollDice"));
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "BoardGame", false);
			this._gauntletMovie = this._gauntletLayer.LoadMovie("BoardGame", this._dataSource);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._cameraHolder = base.Mission.Scene.FindEntityWithTag("camera_holder");
			this.CreateCamera();
			if (this._cameraHolder == null)
			{
				this._cameraHolder = base.Mission.Scene.FindEntityWithTag("camera_holder");
			}
			if (this.Camera == null)
			{
				this.CreateCamera();
			}
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._missionMouseVisibilityState = base.MissionScreen.SceneLayer.InputRestrictions.MouseVisibility;
			this._missionInputRestrictions = base.MissionScreen.SceneLayer.InputRestrictions.InputUsageMask;
			base.MissionScreen.SceneLayer.InputRestrictions.SetInputRestrictions(false, 7);
			base.MissionScreen.SceneLayer.IsFocusLayer = true;
			base.MissionScreen.AddLayer(this._gauntletLayer);
			base.MissionScreen.SetLayerCategoriesStateAndDeactivateOthers(new string[] { "SceneLayer", "BoardGame" }, true);
			ScreenManager.TrySetFocus(base.MissionScreen.SceneLayer);
			this.SetStaticCamera();
		}

		void IBoardGameHandler.Uninstall()
		{
			if (this._dataSource != null)
			{
				this._dataSource.OnFinalize();
				this._dataSource = null;
			}
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			base.MissionScreen.SceneLayer.InputRestrictions.SetInputRestrictions(this._missionMouseVisibilityState, this._missionInputRestrictions);
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletMovie = null;
			this._gauntletLayer = null;
			this.Camera = null;
			this._cameraHolder = null;
			base.MissionScreen.CustomCamera = null;
			base.MissionScreen.SetLayerCategoriesStateAndToggleOthers(new string[] { "BoardGame" }, false);
			base.MissionScreen.SetLayerCategoriesState(new string[] { "SceneLayer" }, true);
			this._spriteCategory.Unload();
		}

		private bool IsHotkeyPressedInAnyLayer(string hotkeyID)
		{
			SceneLayer sceneLayer = base.MissionScreen.SceneLayer;
			bool flag = sceneLayer != null && sceneLayer.Input.IsHotKeyPressed(hotkeyID);
			GauntletLayer gauntletLayer = this._gauntletLayer;
			bool flag2 = gauntletLayer != null && gauntletLayer.Input.IsHotKeyPressed(hotkeyID);
			return flag || flag2;
		}

		private bool IsHotkeyDownInAnyLayer(string hotkeyID)
		{
			SceneLayer sceneLayer = base.MissionScreen.SceneLayer;
			bool flag = sceneLayer != null && sceneLayer.Input.IsHotKeyDown(hotkeyID);
			GauntletLayer gauntletLayer = this._gauntletLayer;
			bool flag2 = gauntletLayer != null && gauntletLayer.Input.IsHotKeyDown(hotkeyID);
			return flag || flag2;
		}

		private bool IsGameKeyReleasedInAnyLayer(string hotKeyID)
		{
			SceneLayer sceneLayer = base.MissionScreen.SceneLayer;
			bool flag = sceneLayer != null && sceneLayer.Input.IsHotKeyReleased(hotKeyID);
			GauntletLayer gauntletLayer = this._gauntletLayer;
			bool flag2 = gauntletLayer != null && gauntletLayer.Input.IsHotKeyReleased(hotKeyID);
			return flag || flag2;
		}

		private void CreateCamera()
		{
			this.Camera = Camera.CreateCamera();
			if (this._cameraHolder != null)
			{
				this.Camera.Entity = this._cameraHolder;
			}
			this.Camera.SetFovVertical(0.7853982f, 1.7777778f, 0.01f, 3000f);
		}

		private void SetStaticCamera()
		{
			if (this._cameraHolder != null && this.Camera.Entity != null)
			{
				base.MissionScreen.CustomCamera = this.Camera;
				return;
			}
			Debug.FailedAssert("[DEBUG]Camera entities are null.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.GauntletUI\\Missions\\MissionGauntletBoardGameView.cs", "SetStaticCamera", 189);
		}

		public override void OnMissionScreenTick(float dt)
		{
			MissionBoardGameLogic missionBoardGameHandler = this._missionBoardGameHandler;
			if (missionBoardGameHandler != null && missionBoardGameHandler.IsGameInProgress)
			{
				MissionScreen missionScreen = base.MissionScreen;
				if (missionScreen == null || !missionScreen.IsPhotoModeEnabled)
				{
					base.OnMissionScreenTick(dt);
					if (this._gauntletLayer != null && this._dataSource != null)
					{
						if (this.IsHotkeyPressedInAnyLayer("Exit"))
						{
							this._dataSource.ExecuteForfeit();
						}
						else if (this.IsHotkeyPressedInAnyLayer("BoardGameRollDice") && this._dataSource.IsGameUsingDice)
						{
							this._dataSource.ExecuteRoll();
						}
					}
					if (this._missionBoardGameHandler.Board != null)
					{
						Vec3 vec;
						Vec3 vec2;
						base.MissionScreen.ScreenPointToWorldRay(base.Input.GetMousePositionRanged(), out vec, out vec2);
						this._missionBoardGameHandler.Board.SetUserRay(vec, vec2);
					}
					return;
				}
			}
		}

		public override void OnMissionScreenFinalize()
		{
			if (this._dataSource != null)
			{
				this._dataSource.OnFinalize();
				this._dataSource = null;
			}
			this._gauntletLayer = null;
			this._gauntletMovie = null;
			base.OnMissionScreenFinalize();
		}

		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (this._gauntletLayer != null)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (this._gauntletLayer != null)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		private BoardGameVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private IGauntletMovie _gauntletMovie;

		private GameEntity _cameraHolder;

		private SpriteCategory _spriteCategory;

		private bool _missionMouseVisibilityState;

		private InputUsageMask _missionInputRestrictions;
	}
}
