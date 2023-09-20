using System;
using System.Collections.Generic;
using SandBox.GauntletUI.BannerEditor;
using SandBox.View.CharacterCreation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.CharacterCreation
{
	// Token: 0x02000037 RID: 55
	[CharacterCreationStageView(typeof(CharacterCreationBannerEditorStage))]
	public class CharacterCreationBannerEditorView : CharacterCreationStageViewBase
	{
		// Token: 0x060001E8 RID: 488 RVA: 0x0000DD44 File Offset: 0x0000BF44
		public CharacterCreationBannerEditorView(CharacterCreation characterCreation, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage onRefresh = null, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction = null, ControlCharacterCreationStageReturnInt getTotalStageCountAction = null, ControlCharacterCreationStageReturnInt getFurthestIndexAction = null, ControlCharacterCreationStageWithInt goToIndexAction = null)
			: this(CharacterObject.PlayerCharacter, Clan.PlayerClan.Banner, affirmativeAction, affirmativeActionText, negativeAction, negativeActionText, onRefresh, getCurrentStageIndexAction, getTotalStageCountAction, getFurthestIndexAction, goToIndexAction)
		{
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000DD78 File Offset: 0x0000BF78
		public CharacterCreationBannerEditorView(BasicCharacterObject character, Banner banner, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage onRefresh = null, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction = null, ControlCharacterCreationStageReturnInt getTotalStageCountAction = null, ControlCharacterCreationStageReturnInt getFurthestIndexAction = null, ControlCharacterCreationStageWithInt goToIndexAction = null)
			: base(affirmativeAction, negativeAction, onRefresh, getTotalStageCountAction, getCurrentStageIndexAction, getFurthestIndexAction, goToIndexAction)
		{
			this._bannerEditorView = new BannerEditorView(character, banner, new ControlCharacterCreationStage(this.AffirmativeAction), affirmativeActionText, negativeAction, negativeActionText, onRefresh, getCurrentStageIndexAction, getTotalStageCountAction, getFurthestIndexAction, goToIndexAction);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000DDC1 File Offset: 0x0000BFC1
		public override IEnumerable<ScreenLayer> GetLayers()
		{
			return new List<ScreenLayer>
			{
				this._bannerEditorView.SceneLayer,
				this._bannerEditorView.GauntletLayer
			};
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000DDEA File Offset: 0x0000BFEA
		public override void PreviousStage()
		{
			this._bannerEditorView.Exit(true);
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000DDF8 File Offset: 0x0000BFF8
		public override void NextStage()
		{
			this._bannerEditorView.Exit(false);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000DE06 File Offset: 0x0000C006
		public override void Tick(float dt)
		{
			if (!this._isFinalized)
			{
				this._bannerEditorView.OnTick(dt);
				if (this._isFinalized)
				{
					return;
				}
				base.HandleEscapeMenu(this, this._bannerEditorView.SceneLayer);
			}
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000DE37 File Offset: 0x0000C037
		public override int GetVirtualStageCount()
		{
			return 1;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000DE3A File Offset: 0x0000C03A
		public override void GoToIndex(int index)
		{
			this._bannerEditorView.GoToIndex(index);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000DE48 File Offset: 0x0000C048
		protected override void OnFinalize()
		{
			this._bannerEditorView.OnDeactivate();
			this._bannerEditorView.OnFinalize();
			this._isFinalized = true;
			base.OnFinalize();
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000DE70 File Offset: 0x0000C070
		private void AffirmativeAction()
		{
			uint primaryColor = this._bannerEditorView.Banner.GetPrimaryColor();
			uint firstIconColor = this._bannerEditorView.Banner.GetFirstIconColor();
			Clan playerClan = Clan.PlayerClan;
			playerClan.Color = primaryColor;
			playerClan.Color2 = firstIconColor;
			playerClan.UpdateBannerColor(primaryColor, firstIconColor);
			(GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent.SetPlayerBanner(this._bannerEditorView.Banner);
			this._affirmativeAction.Invoke();
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000DEE8 File Offset: 0x0000C0E8
		public override void LoadEscapeMenuMovie()
		{
			this._escapeMenuDatasource = new EscapeMenuVM(base.GetEscapeMenuItems(this), null);
			this._escapeMenuMovie = this._bannerEditorView.GauntletLayer.LoadMovie("EscapeMenu", this._escapeMenuDatasource);
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000DF1E File Offset: 0x0000C11E
		public override void ReleaseEscapeMenuMovie()
		{
			this._bannerEditorView.GauntletLayer.ReleaseMovie(this._escapeMenuMovie);
			this._escapeMenuDatasource = null;
			this._escapeMenuMovie = null;
		}

		// Token: 0x04000105 RID: 261
		private readonly BannerEditorView _bannerEditorView;

		// Token: 0x04000106 RID: 262
		private bool _isFinalized;

		// Token: 0x04000107 RID: 263
		private EscapeMenuVM _escapeMenuDatasource;

		// Token: 0x04000108 RID: 264
		private IGauntletMovie _escapeMenuMovie;
	}
}
