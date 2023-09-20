using System;
using System.Collections.Generic;
using SandBox.View.CharacterCreation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.CharacterCreation
{
	// Token: 0x0200003A RID: 58
	[CharacterCreationStageView(typeof(CharacterCreationFaceGeneratorStage))]
	public class CharacterCreationFaceGeneratorView : CharacterCreationStageViewBase
	{
		// Token: 0x06000217 RID: 535 RVA: 0x0000EF00 File Offset: 0x0000D100
		public CharacterCreationFaceGeneratorView(CharacterCreation characterCreation, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage onRefresh, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction, ControlCharacterCreationStageReturnInt getTotalStageCountAction, ControlCharacterCreationStageReturnInt getFurthestIndexAction, ControlCharacterCreationStageWithInt goToIndexAction)
			: base(affirmativeAction, negativeAction, onRefresh, getTotalStageCountAction, getCurrentStageIndexAction, getFurthestIndexAction, goToIndexAction)
		{
			this._characterCreation = characterCreation;
			MBObjectManager objectManager = Game.Current.ObjectManager;
			string text = "player_char_creation_show_";
			CharacterObject playerCharacter = CharacterObject.PlayerCharacter;
			string text2;
			if (playerCharacter == null)
			{
				text2 = null;
			}
			else
			{
				CultureObject culture = playerCharacter.Culture;
				text2 = ((culture != null) ? culture.StringId : null);
			}
			MBEquipmentRoster @object = objectManager.GetObject<MBEquipmentRoster>(text + text2);
			Equipment equipment = ((@object != null) ? @object.DefaultEquipment : null);
			this._faceGeneratorView = new BodyGeneratorView(new ControlCharacterCreationStage(this.NextStage), affirmativeActionText, new ControlCharacterCreationStage(this.PreviousStage), negativeActionText, Hero.MainHero.CharacterObject, false, null, equipment, getCurrentStageIndexAction, getTotalStageCountAction, getFurthestIndexAction, goToIndexAction);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000EFA7 File Offset: 0x0000D1A7
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._faceGeneratorView.OnFinalize();
			this._faceGeneratorView = null;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000EFC1 File Offset: 0x0000D1C1
		public override IEnumerable<ScreenLayer> GetLayers()
		{
			return new List<ScreenLayer>
			{
				this._faceGeneratorView.SceneLayer,
				this._faceGeneratorView.GauntletLayer
			};
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000EFEA File Offset: 0x0000D1EA
		public override void PreviousStage()
		{
			this._negativeAction.Invoke();
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000EFF8 File Offset: 0x0000D1F8
		public override void NextStage()
		{
			List<FaceGenChar> list = new List<FaceGenChar>
			{
				new FaceGenChar(this._faceGeneratorView.BodyGen.CurrentBodyProperties, this._faceGeneratorView.BodyGen.Race, new Equipment(), this._faceGeneratorView.BodyGen.IsFemale, "act_inventory_idle_start")
			};
			this._characterCreation.ChangeFaceGenChars(list);
			this._affirmativeAction.Invoke();
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000F067 File Offset: 0x0000D267
		public override void Tick(float dt)
		{
			this._faceGeneratorView.OnTick(dt);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000F075 File Offset: 0x0000D275
		public override int GetVirtualStageCount()
		{
			return 1;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000F078 File Offset: 0x0000D278
		public override void GoToIndex(int index)
		{
			this._goToIndexAction.Invoke(index);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000F086 File Offset: 0x0000D286
		public override void LoadEscapeMenuMovie()
		{
			this._escapeMenuDatasource = new EscapeMenuVM(base.GetEscapeMenuItems(this), null);
			this._escapeMenuMovie = this._faceGeneratorView.GauntletLayer.LoadMovie("EscapeMenu", this._escapeMenuDatasource);
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000F0BC File Offset: 0x0000D2BC
		public override void ReleaseEscapeMenuMovie()
		{
			this._faceGeneratorView.GauntletLayer.ReleaseMovie(this._escapeMenuMovie);
			this._escapeMenuDatasource = null;
			this._escapeMenuMovie = null;
		}

		// Token: 0x0400012A RID: 298
		private BodyGeneratorView _faceGeneratorView;

		// Token: 0x0400012B RID: 299
		private readonly CharacterCreation _characterCreation;

		// Token: 0x0400012C RID: 300
		private EscapeMenuVM _escapeMenuDatasource;

		// Token: 0x0400012D RID: 301
		private IGauntletMovie _escapeMenuMovie;
	}
}
