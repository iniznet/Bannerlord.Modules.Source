using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	[OverrideView(typeof(PhotoModeView))]
	public class MissionGauntletPhotoMode : MissionView
	{
		private Scene _missionScene
		{
			get
			{
				return base.MissionScreen.Mission.Scene;
			}
		}

		private InputContext _input
		{
			get
			{
				return base.MissionScreen.SceneLayer.Input;
			}
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._photoModeCategory = spriteData.SpriteCategories["ui_photomode"];
			this._photoModeCategory.Load(resourceContext, uiresourceDepot);
			this._dataSource = new PhotoModeVM(this._missionScene, () => this._vignetteMode, () => this._hideAgentsMode);
			this._cameraRoll = 0f;
			this._photoModeOrbitState = this._missionScene.GetPhotoModeOrbit();
			this._vignetteMode = false;
			this._hideAgentsMode = false;
			this._saveAmbientOcclusionPass = false;
			this._saveObjectIdPass = false;
			this._saveShadowPass = false;
			this._dataSource.AddHotkeyWithForcedName(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("ToggleEscapeMenu"), new TextObject("{=3CsACce8}Exit", null));
			this._dataSource.AddHotkey(HotKeyManager.GetCategory("PhotoModeHotKeyCategory").GetHotKey("FasterCamera"));
			this._dataSource.AddKey(HotKeyManager.GetCategory("PhotoModeHotKeyCategory").GetGameKey(93));
			if (Utilities.EditModeEnabled)
			{
				this._dataSource.AddKey(HotKeyManager.GetCategory("PhotoModeHotKeyCategory").GetGameKey(94));
			}
			this._dataSource.AddKey(HotKeyManager.GetCategory("PhotoModeHotKeyCategory").GetGameKey(90));
			this._dataSource.AddKey(HotKeyManager.GetCategory("PhotoModeHotKeyCategory").GetGameKey(96));
			this._dataSource.AddKey(HotKeyManager.GetCategory("PhotoModeHotKeyCategory").GetGameKey(95));
			this._dataSource.AddKey(HotKeyManager.GetCategory("PhotoModeHotKeyCategory").GetGameKey(97));
			this._dataSource.AddKey(HotKeyManager.GetCategory("PhotoModeHotKeyCategory").GetGameKey(98));
			this._dataSource.AddKey(HotKeyManager.GetCategory("PhotoModeHotKeyCategory").GetGameKey(91));
			this._dataSource.AddKey(HotKeyManager.GetCategory("PhotoModeHotKeyCategory").GetGameKey(92));
			this._dataSource.AddKey(HotKeyManager.GetCategory("PhotoModeHotKeyCategory").GetGameKey(105));
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._takePhoto != -1)
			{
				if (Utilities.GetNumberOfShaderCompilationsInProgress() > 0)
				{
					this._takePhoto++;
				}
				else if (this._takePhoto > 6)
				{
					if (this._saveObjectIdPass)
					{
						string text = this._missionScene.TakePhotoModePicture(false, true, false);
						MBDebug.DisableAllUI = this._prevUIDisabled;
						this._screenShotTakenMessage.SetTextVariable("PATH", text);
						InformationManager.DisplayMessage(new InformationMessage(this._screenShotTakenMessage.ToString()));
						Utilities.SetForceDrawEntityID(false);
						Utilities.SetRenderMode(0);
					}
					this._takePhoto = -1;
				}
				else if (this._takePhoto == 2)
				{
					string text2 = this._missionScene.TakePhotoModePicture(this._saveAmbientOcclusionPass, false, this._saveShadowPass);
					this._screenShotTakenMessage.SetTextVariable("PATH", text2);
					InformationManager.DisplayMessage(new InformationMessage(this._screenShotTakenMessage.ToString()));
					if (this._saveObjectIdPass)
					{
						Utilities.SetForceDrawEntityID(true);
						Utilities.SetRenderMode(14);
						this._takePhoto++;
					}
					else
					{
						MBDebug.DisableAllUI = this._prevUIDisabled;
						this._takePhoto = -1;
					}
				}
				else
				{
					this._takePhoto++;
				}
			}
			if (base.MissionScreen.IsPhotoModeEnabled)
			{
				if (this._takePhoto == -1)
				{
					if (!this._registered)
					{
						GameKeyContext category = HotKeyManager.GetCategory("GenericPanelGameKeyCategory");
						if (!this._input.IsCategoryRegistered(category))
						{
							this._input.RegisterHotKeyCategory(category);
						}
						GameKeyContext category2 = HotKeyManager.GetCategory("PhotoModeHotKeyCategory");
						if (!this._input.IsCategoryRegistered(category2))
						{
							this._input.RegisterHotKeyCategory(category2);
						}
						this._registered = true;
					}
					if (this._suspended)
					{
						this._suspended = false;
						this._gauntletLayer = new GauntletLayer(2147483645, "GauntletLayer", false);
						this._dataSource.RefreshValues();
						this._gauntletLayer.LoadMovie("PhotoMode", this._dataSource);
						this._gauntletLayer.InputRestrictions.SetInputRestrictions(false, 3);
						base.MissionScreen.AddLayer(this._gauntletLayer);
						GauntletChatLogView.Current.SetCanFocusWhileInMission(false);
					}
					if (this._input.IsGameKeyPressed(93) && this.GetCanTakePicture())
					{
						this._prevUIDisabled = MBDebug.DisableAllUI;
						MBDebug.DisableAllUI = true;
						this._saveAmbientOcclusionPass = false;
						this._saveObjectIdPass = false;
						this._saveShadowPass = false;
						this._takePhoto = 0;
					}
					else if (Utilities.EditModeEnabled && this._input.IsGameKeyPressed(94))
					{
						this._prevUIDisabled = MBDebug.DisableAllUI;
						MBDebug.DisableAllUI = true;
						this._saveAmbientOcclusionPass = true;
						this._saveObjectIdPass = Utilities.EditModeEnabled;
						this._saveShadowPass = true;
						this._takePhoto = 0;
					}
					else if (this._input.IsGameKeyPressed(90))
					{
						MBDebug.DisableAllUI = !MBDebug.DisableAllUI;
					}
					else if (this._input.IsGameKeyPressed(95))
					{
						this._photoModeOrbitState = !this._photoModeOrbitState;
						this._missionScene.SetPhotoModeOrbit(this._photoModeOrbitState);
					}
					else if (this._input.IsGameKeyPressed(96))
					{
						base.MissionScreen.SetPhotoModeRequiresMouse(!base.MissionScreen.PhotoModeRequiresMouse);
					}
					else if (this._input.IsGameKeyPressed(97))
					{
						this._vignetteMode = !this._vignetteMode;
						this._missionScene.SetPhotoModeVignette(this._vignetteMode);
					}
					else if (this._input.IsGameKeyPressed(98))
					{
						this._hideAgentsMode = !this._hideAgentsMode;
						Utilities.SetRenderAgents(!this._hideAgentsMode);
					}
					else if (this._input.IsGameKeyPressed(105))
					{
						this.ResetChanges();
					}
					else if (base.MissionScreen.SceneLayer.Input.IsKeyPressed(225))
					{
						this._prevMouseEnabled = base.MissionScreen.PhotoModeRequiresMouse;
						base.MissionScreen.SetPhotoModeRequiresMouse(false);
					}
					else if (base.MissionScreen.SceneLayer.Input.IsKeyReleased(225))
					{
						base.MissionScreen.SetPhotoModeRequiresMouse(this._prevMouseEnabled);
					}
					if (this._input.IsGameKeyDown(91))
					{
						this._cameraRoll -= 0.1f;
						this._missionScene.SetPhotoModeRoll(this._cameraRoll);
						return;
					}
					if (this._input.IsGameKeyDown(92))
					{
						this._cameraRoll += 0.1f;
						this._missionScene.SetPhotoModeRoll(this._cameraRoll);
						return;
					}
				}
			}
			else if (!this._suspended)
			{
				this._suspended = true;
				if (this._gauntletLayer != null)
				{
					base.MissionScreen.RemoveLayer(this._gauntletLayer);
					this._gauntletLayer = null;
				}
				GauntletChatLogView.Current.SetCanFocusWhileInMission(true);
			}
		}

		private bool GetCanTakePicture()
		{
			return !this._gauntletLayer._gauntletUIContext.EventManager.IsControllerActive || !base.MissionScreen.PhotoModeRequiresMouse;
		}

		private void ResetChanges()
		{
			this._photoModeOrbitState = false;
			this._missionScene.SetPhotoModeOrbit(this._photoModeOrbitState);
			this._vignetteMode = false;
			this._hideAgentsMode = false;
			this._saveAmbientOcclusionPass = false;
			this._saveObjectIdPass = false;
			this._saveShadowPass = false;
			this._missionScene.SetPhotoModeFocus(0f, 0f, 0f, 0f);
			this._missionScene.SetPhotoModeVignette(this._vignetteMode);
			Utilities.SetRenderAgents(!this._hideAgentsMode);
			this._cameraRoll = 0f;
			this._missionScene.SetPhotoModeRoll(this._cameraRoll);
			this._dataSource.Reset();
		}

		public override bool OnEscape()
		{
			if (base.MissionScreen.IsPhotoModeEnabled)
			{
				base.MissionScreen.SetPhotoModeEnabled(false);
				base.Mission.IsInPhotoMode = false;
				MBDebug.DisableAllUI = false;
				this.ResetChanges();
				return true;
			}
			return false;
		}

		public override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			return !base.MissionScreen.IsPhotoModeEnabled;
		}

		public override void OnMissionScreenFinalize()
		{
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._photoModeCategory.Unload();
			base.OnMissionScreenFinalize();
		}

		private readonly TextObject _screenShotTakenMessage = new TextObject("{=1e12bdjj}Screenshot has been saved in {PATH}", null);

		private const float _cameraRollAmount = 0.1f;

		private const float _cameraFocusAmount = 0.1f;

		private GauntletLayer _gauntletLayer;

		private PhotoModeVM _dataSource;

		private bool _registered;

		private SpriteCategory _photoModeCategory;

		private float _cameraRoll;

		private bool _photoModeOrbitState;

		private bool _suspended = true;

		private bool _vignetteMode;

		private bool _hideAgentsMode;

		private int _takePhoto = -1;

		private bool _saveAmbientOcclusionPass;

		private bool _saveObjectIdPass;

		private bool _saveShadowPass;

		private bool _prevUIDisabled;

		private bool _prevMouseEnabled;
	}
}
