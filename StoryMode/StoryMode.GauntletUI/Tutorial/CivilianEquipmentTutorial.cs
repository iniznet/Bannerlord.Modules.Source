using System;
using StoryMode.StoryModePhases;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class CivilianEquipmentTutorial : TutorialItemBase
	{
		public CivilianEquipmentTutorial()
		{
			base.Type = "CivilianEquipment";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "CivilianFilter";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerFilteredToCivilianEquipment;
		}

		public override void OnInventoryEquipmentTypeChange(InventoryEquipmentTypeChangedEvent obj)
		{
			this._playerFilteredToCivilianEquipment = !obj.IsCurrentlyWarSet;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 2;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialPhase.Instance.IsCompleted && TutorialHelper.CurrentContext == 2;
		}

		private bool _playerFilteredToCivilianEquipment;
	}
}
