using System;
using StoryMode.StoryModePhases;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000019 RID: 25
	public class CivilianEquipmentTutorial : TutorialItemBase
	{
		// Token: 0x06000075 RID: 117 RVA: 0x00002DB9 File Offset: 0x00000FB9
		public CivilianEquipmentTutorial()
		{
			base.Type = "CivilianEquipment";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "CivilianFilter";
			base.MouseRequired = true;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00002DE5 File Offset: 0x00000FE5
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerFilteredToCivilianEquipment;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00002DED File Offset: 0x00000FED
		public override void OnInventoryEquipmentTypeChange(InventoryEquipmentTypeChangedEvent obj)
		{
			this._playerFilteredToCivilianEquipment = !obj.IsCurrentlyWarSet;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00002DFE File Offset: 0x00000FFE
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 2;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00002E01 File Offset: 0x00001001
		public override bool IsConditionsMetForActivation()
		{
			return TutorialPhase.Instance.IsCompleted && TutorialHelper.CurrentContext == 2;
		}

		// Token: 0x0400001D RID: 29
		private bool _playerFilteredToCivilianEquipment;
	}
}
