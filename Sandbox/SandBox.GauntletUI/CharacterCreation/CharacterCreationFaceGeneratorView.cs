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
	[CharacterCreationStageView(typeof(CharacterCreationFaceGeneratorStage))]
	public class CharacterCreationFaceGeneratorView : CharacterCreationStageViewBase
	{
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

		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._faceGeneratorView.OnFinalize();
			this._faceGeneratorView = null;
		}

		public override IEnumerable<ScreenLayer> GetLayers()
		{
			return new List<ScreenLayer>
			{
				this._faceGeneratorView.SceneLayer,
				this._faceGeneratorView.GauntletLayer
			};
		}

		public override void PreviousStage()
		{
			this._negativeAction.Invoke();
		}

		public override void NextStage()
		{
			List<FaceGenChar> list = new List<FaceGenChar>
			{
				new FaceGenChar(this._faceGeneratorView.BodyGen.CurrentBodyProperties, this._faceGeneratorView.BodyGen.Race, new Equipment(), this._faceGeneratorView.BodyGen.IsFemale, "act_inventory_idle_start")
			};
			this._characterCreation.ChangeFaceGenChars(list);
			this._affirmativeAction.Invoke();
		}

		public override void Tick(float dt)
		{
			this._faceGeneratorView.OnTick(dt);
		}

		public override int GetVirtualStageCount()
		{
			return 1;
		}

		public override void GoToIndex(int index)
		{
			this._goToIndexAction.Invoke(index);
		}

		public override void LoadEscapeMenuMovie()
		{
			this._escapeMenuDatasource = new EscapeMenuVM(base.GetEscapeMenuItems(this), null);
			this._escapeMenuMovie = this._faceGeneratorView.GauntletLayer.LoadMovie("EscapeMenu", this._escapeMenuDatasource);
		}

		public override void ReleaseEscapeMenuMovie()
		{
			this._faceGeneratorView.GauntletLayer.ReleaseMovie(this._escapeMenuMovie);
			this._escapeMenuDatasource = null;
			this._escapeMenuMovie = null;
		}

		private BodyGeneratorView _faceGeneratorView;

		private readonly CharacterCreation _characterCreation;

		private EscapeMenuVM _escapeMenuDatasource;

		private IGauntletMovie _escapeMenuMovie;
	}
}
