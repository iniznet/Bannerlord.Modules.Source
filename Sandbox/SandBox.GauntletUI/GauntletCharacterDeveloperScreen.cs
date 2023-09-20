using System;
using SandBox.View;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	[GameStateScreen(typeof(CharacterDeveloperState))]
	public class GauntletCharacterDeveloperScreen : ScreenBase, IGameStateListener, IChangeableScreen, ICharacterDeveloperStateHandler
	{
		public GauntletCharacterDeveloperScreen(CharacterDeveloperState clanState)
		{
			this._characterDeveloperState = clanState;
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			LoadingWindow.DisableGlobalLoadingWindow();
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit") || this._gauntletLayer.Input.IsGameKeyPressed(37))
			{
				if (this._dataSource.CurrentCharacter.IsInspectingAnAttribute)
				{
					this._dataSource.CurrentCharacter.ExecuteStopInspectingCurrentAttribute();
					return;
				}
				if (this._dataSource.CurrentCharacter.PerkSelection.IsActive)
				{
					this._dataSource.CurrentCharacter.PerkSelection.ExecuteDeactivate();
					return;
				}
				this.CloseCharacterDeveloperScreen();
				return;
			}
			else
			{
				if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
				{
					this.ExecuteConfirm();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyReleased("Reset"))
				{
					this.ExecuteReset();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyPressed("SwitchToPreviousTab"))
				{
					this.ExecuteSwitchToPreviousTab();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyPressed("SwitchToNextTab"))
				{
					this.ExecuteSwitchToNextTab();
				}
				return;
			}
		}

		void IGameStateListener.OnActivate()
		{
			base.OnActivate();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._characterdeveloper = spriteData.SpriteCategories["ui_characterdeveloper"];
			this._characterdeveloper.Load(resourceContext, uiresourceDepot);
			this._dataSource = new CharacterDeveloperVM(new Action(this.CloseCharacterDeveloperScreen));
			this._dataSource.SetGetKeyTextFromKeyIDFunc(new Func<string, TextObject>(Game.Current.GameTextManager.GetHotKeyGameTextFromKeyID));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			this._dataSource.SetPreviousCharacterInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
			this._dataSource.SetNextCharacterInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
			if (this._characterDeveloperState.InitialSelectedHero != null)
			{
				this._dataSource.SelectHero(this._characterDeveloperState.InitialSelectedHero);
			}
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", true);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._gauntletLayer.LoadMovie("CharacterDeveloper", this._dataSource);
			base.AddLayer(this._gauntletLayer);
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(3));
			SoundEvent.PlaySound2D("event:/ui/panels/panel_character_open");
			this._gauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterFrames(2, null);
		}

		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
			base.RemoveLayer(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
			this._characterdeveloper.Unload();
		}

		private void CloseCharacterDeveloperScreen()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		private void ExecuteConfirm()
		{
			this._dataSource.ExecuteDone();
		}

		private void ExecuteReset()
		{
			this._dataSource.ExecuteReset();
		}

		private void ExecuteSwitchToPreviousTab()
		{
			this._dataSource.CharacterList.ExecuteSelectPreviousItem();
		}

		private void ExecuteSwitchToNextTab()
		{
			this._dataSource.CharacterList.ExecuteSelectNextItem();
		}

		bool IChangeableScreen.AnyUnsavedChanges()
		{
			return this._dataSource.IsThereAnyChanges();
		}

		bool IChangeableScreen.CanChangesBeApplied()
		{
			return true;
		}

		void IChangeableScreen.ApplyChanges()
		{
			this._dataSource.ApplyAllChanges();
		}

		void IChangeableScreen.ResetChanges()
		{
			this._dataSource.ExecuteReset();
		}

		private const string _panelOpenSound = "event:/ui/panels/panel_character_open";

		private CharacterDeveloperVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private SpriteCategory _characterdeveloper;

		private readonly CharacterDeveloperState _characterDeveloperState;
	}
}
