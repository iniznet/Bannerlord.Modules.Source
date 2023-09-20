using System;
using SandBox.View.Map;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Recruitment;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Menu
{
	// Token: 0x0200001B RID: 27
	[OverrideView(typeof(MenuRecruitVolunteersView))]
	public class GauntletMenuRecruitVolunteersView : MenuView
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00008DAD File Offset: 0x00006FAD
		public override bool ShouldUpdateMenuAfterRemoved
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00008DB0 File Offset: 0x00006FB0
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._dataSource = new RecruitmentVM();
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			this._dataSource.SetRecruitAllInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("TakeAll"));
			this._dataSource.SetGetKeyTextFromKeyIDFunc(new Func<string, TextObject>(Game.Current.GameTextManager.GetHotKeyGameTextFromKeyID));
			base.Layer = new GauntletLayer(206, "GauntletLayer", false)
			{
				Name = "RecuritLayer"
			};
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			base.MenuViewContext.AddLayer(base.Layer);
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._movie = this._layerAsGauntletLayer.LoadMovie("RecruitmentPopup", this._dataSource);
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(base.Layer);
			this._dataSource.RefreshScreen();
			this._dataSource.Enabled = true;
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(5));
			MapScreen mapScreen;
			if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null)
			{
				mapScreen.SetIsInRecruitment(true);
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00008F68 File Offset: 0x00007168
		protected override void OnFinalize()
		{
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			base.MenuViewContext.RemoveLayer(base.Layer);
			this._movie = null;
			base.Layer = null;
			this._layerAsGauntletLayer = null;
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(4));
			MapScreen mapScreen;
			if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null)
			{
				mapScreen.SetIsInRecruitment(false);
			}
			base.OnFinalize();
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00009008 File Offset: 0x00007208
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (base.Layer.Input.IsHotKeyReleased("Exit"))
			{
				this._dataSource.ExecuteForceQuit();
			}
			else if (base.Layer.Input.IsHotKeyReleased("Confirm"))
			{
				this._dataSource.ExecuteDone();
			}
			else if (base.Layer.Input.IsHotKeyReleased("Reset"))
			{
				this._dataSource.ExecuteReset();
			}
			else if (base.Layer.Input.IsHotKeyReleased("TakeAll"))
			{
				this._dataSource.ExecuteRecruitAll();
			}
			else if (base.Layer.Input.IsGameKeyReleased(39))
			{
				if (this._dataSource.FocusedVolunteerOwner != null)
				{
					this._dataSource.FocusedVolunteerOwner.ExecuteOpenEncyclopedia();
				}
				else if (this._dataSource.FocusedVolunteerTroop != null)
				{
					this._dataSource.FocusedVolunteerTroop.ExecuteOpenEncyclopedia();
				}
			}
			if (!this._dataSource.Enabled)
			{
				base.MenuViewContext.CloseRecruitVolunteers();
			}
		}

		// Token: 0x04000080 RID: 128
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x04000081 RID: 129
		private RecruitmentVM _dataSource;

		// Token: 0x04000082 RID: 130
		private IGauntletMovie _movie;
	}
}
