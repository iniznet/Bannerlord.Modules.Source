using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.View.CharacterCreation;
using SandBox.View.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.CharacterCreation
{
	// Token: 0x02000039 RID: 57
	[CharacterCreationStageView(typeof(CharacterCreationCultureStage))]
	public class CharacterCreationCultureStageView : CharacterCreationStageViewBase
	{
		// Token: 0x0600020C RID: 524 RVA: 0x0000EB24 File Offset: 0x0000CD24
		public CharacterCreationCultureStageView(CharacterCreation characterCreation, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage onRefresh, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction, ControlCharacterCreationStageReturnInt getTotalStageCountAction, ControlCharacterCreationStageReturnInt getFurthestIndexAction, ControlCharacterCreationStageWithInt goToIndexAction)
			: base(affirmativeAction, negativeAction, onRefresh, getCurrentStageIndexAction, getTotalStageCountAction, getFurthestIndexAction, goToIndexAction)
		{
			this._characterCreation = characterCreation;
			this.GauntletLayer = new GauntletLayer(1, "GauntletLayer", true)
			{
				IsFocusLayer = true
			};
			this.GauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this.GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			ScreenManager.TrySetFocus(this.GauntletLayer);
			this._dataSource = new CharacterCreationCultureStageVM(this._characterCreation, new Action(this.NextStage), affirmativeActionText, new Action(this.PreviousStage), negativeActionText, getCurrentStageIndexAction.Invoke(), getTotalStageCountAction.Invoke(), getFurthestIndexAction.Invoke(), new Action<int>(this.GoToIndex), new Action<CultureObject>(this.OnCultureSelected));
			this._movie = this.GauntletLayer.LoadMovie("CharacterCreationCultureStage", this._dataSource);
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._characterCreationCategory = spriteData.SpriteCategories["ui_charactercreation"];
			this._characterCreationCategory.Load(resourceContext, uiresourceDepot);
			CharacterCreationContentBase instance = CharacterCreationContentBase.Instance;
			bool flag;
			if (instance == null)
			{
				flag = false;
			}
			else
			{
				flag = instance.CharacterCreationStages.Any((Type c) => c.IsEquivalentTo(typeof(CharacterCreationBannerEditorStage)));
			}
			if (flag)
			{
				this._bannerEditorCategory = spriteData.SpriteCategories["ui_bannericons"];
				this._bannerEditorCategory.Load(resourceContext, uiresourceDepot);
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000ECE0 File Offset: 0x0000CEE0
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this.GauntletLayer = null;
			CharacterCreationCultureStageVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = null;
			this._characterCreationCategory.Unload();
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000ED14 File Offset: 0x0000CF14
		private void HandleLayerInput()
		{
			if (this.GauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				this._dataSource.OnPreviousStage();
				return;
			}
			if (this.GauntletLayer.Input.IsHotKeyReleased("Confirm") && this._dataSource.CanAdvance)
			{
				this._dataSource.OnNextStage();
			}
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000ED73 File Offset: 0x0000CF73
		public override void Tick(float dt)
		{
			base.Tick(dt);
			if (this._dataSource.IsActive)
			{
				base.HandleEscapeMenu(this, this.GauntletLayer);
				this.HandleLayerInput();
			}
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000ED9C File Offset: 0x0000CF9C
		public override void NextStage()
		{
			this._characterCreation.Name = NameGenerator.Current.GenerateFirstNameForPlayer(this._dataSource.CurrentSelectedCulture.Culture, Hero.MainHero.IsFemale).ToString();
			this._affirmativeAction.Invoke();
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000EDE8 File Offset: 0x0000CFE8
		private void OnCultureSelected(CultureObject culture)
		{
			MissionSoundParametersView.SoundParameterMissionCulture soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.None;
			if (culture.StringId == "aserai")
			{
				soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Aserai;
			}
			else if (culture.StringId == "khuzait")
			{
				soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Khuzait;
			}
			else if (culture.StringId == "vlandia")
			{
				soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Vlandia;
			}
			else if (culture.StringId == "sturgia")
			{
				soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Sturgia;
			}
			else if (culture.StringId == "battania")
			{
				soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Battania;
			}
			else if (culture.StringId == "empire")
			{
				soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Empire;
			}
			SoundManager.SetGlobalParameter("MissionCulture", (float)soundParameterMissionCulture);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000EE85 File Offset: 0x0000D085
		public override void PreviousStage()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000EE97 File Offset: 0x0000D097
		public override int GetVirtualStageCount()
		{
			return 1;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000EE9A File Offset: 0x0000D09A
		public override IEnumerable<ScreenLayer> GetLayers()
		{
			return new List<ScreenLayer> { this.GauntletLayer };
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000EEAD File Offset: 0x0000D0AD
		public override void LoadEscapeMenuMovie()
		{
			this._escapeMenuDatasource = new EscapeMenuVM(base.GetEscapeMenuItems(this), null);
			this._escapeMenuMovie = this.GauntletLayer.LoadMovie("EscapeMenu", this._escapeMenuDatasource);
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000EEDE File Offset: 0x0000D0DE
		public override void ReleaseEscapeMenuMovie()
		{
			this.GauntletLayer.ReleaseMovie(this._escapeMenuMovie);
			this._escapeMenuDatasource = null;
			this._escapeMenuMovie = null;
		}

		// Token: 0x04000121 RID: 289
		private const string CultureParameterId = "MissionCulture";

		// Token: 0x04000122 RID: 290
		private readonly IGauntletMovie _movie;

		// Token: 0x04000123 RID: 291
		private GauntletLayer GauntletLayer;

		// Token: 0x04000124 RID: 292
		private CharacterCreationCultureStageVM _dataSource;

		// Token: 0x04000125 RID: 293
		private SpriteCategory _characterCreationCategory;

		// Token: 0x04000126 RID: 294
		private SpriteCategory _bannerEditorCategory;

		// Token: 0x04000127 RID: 295
		private readonly CharacterCreation _characterCreation;

		// Token: 0x04000128 RID: 296
		private EscapeMenuVM _escapeMenuDatasource;

		// Token: 0x04000129 RID: 297
		private IGauntletMovie _escapeMenuMovie;
	}
}
