using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	// Token: 0x02000039 RID: 57
	public class PopupSceneSwitchItemSequence : PopupSceneSequence
	{
		// Token: 0x060002A2 RID: 674 RVA: 0x00017C91 File Offset: 0x00015E91
		public override void OnInitialState()
		{
			this.AttachItem(this.InitialItem, this.InitialBodyPart);
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00017CA5 File Offset: 0x00015EA5
		public override void OnPositiveState()
		{
			this.AttachItem(this.PositiveItem, this.PositiveBodyPart);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00017CB9 File Offset: 0x00015EB9
		public override void OnNegativeState()
		{
			this.AttachItem(this.NegativeItem, this.NegativeBodyPart);
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x00017CD0 File Offset: 0x00015ED0
		private EquipmentIndex StringToEquipmentIndex(PopupSceneSwitchItemSequence.BodyPartIndex part)
		{
			switch (part)
			{
			case PopupSceneSwitchItemSequence.BodyPartIndex.None:
				return -1;
			case PopupSceneSwitchItemSequence.BodyPartIndex.Weapon0:
				return 0;
			case PopupSceneSwitchItemSequence.BodyPartIndex.Weapon1:
				return 1;
			case PopupSceneSwitchItemSequence.BodyPartIndex.Weapon2:
				return 2;
			case PopupSceneSwitchItemSequence.BodyPartIndex.Weapon3:
				return 3;
			case PopupSceneSwitchItemSequence.BodyPartIndex.ExtraWeaponSlot:
				return 4;
			case PopupSceneSwitchItemSequence.BodyPartIndex.Head:
				return 5;
			case PopupSceneSwitchItemSequence.BodyPartIndex.Body:
				return 6;
			case PopupSceneSwitchItemSequence.BodyPartIndex.Leg:
				return 7;
			case PopupSceneSwitchItemSequence.BodyPartIndex.Gloves:
				return 8;
			case PopupSceneSwitchItemSequence.BodyPartIndex.Cape:
				return 9;
			case PopupSceneSwitchItemSequence.BodyPartIndex.Horse:
				return 10;
			case PopupSceneSwitchItemSequence.BodyPartIndex.HorseHarness:
				return 11;
			default:
				return -1;
			}
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x00017D38 File Offset: 0x00015F38
		private void AttachItem(string itemName, PopupSceneSwitchItemSequence.BodyPartIndex bodyPart)
		{
			if (this._agentVisuals == null)
			{
				return;
			}
			EquipmentIndex equipmentIndex = this.StringToEquipmentIndex(bodyPart);
			if (equipmentIndex != -1)
			{
				AgentVisualsData copyAgentVisualsData = this._agentVisuals.GetCopyAgentVisualsData();
				Equipment equipment = this._agentVisuals.GetEquipment().Clone(false);
				if (itemName == "")
				{
					equipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, default(EquipmentElement));
				}
				else
				{
					equipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>(itemName), null, null, false));
				}
				copyAgentVisualsData.RightWieldedItemIndex(0).LeftWieldedItemIndex(-1).Equipment(equipment);
				this._agentVisuals.Refresh(false, copyAgentVisualsData, false);
			}
		}

		// Token: 0x040001C2 RID: 450
		public string InitialItem;

		// Token: 0x040001C3 RID: 451
		public string PositiveItem;

		// Token: 0x040001C4 RID: 452
		public string NegativeItem;

		// Token: 0x040001C5 RID: 453
		public PopupSceneSwitchItemSequence.BodyPartIndex InitialBodyPart;

		// Token: 0x040001C6 RID: 454
		public PopupSceneSwitchItemSequence.BodyPartIndex PositiveBodyPart;

		// Token: 0x040001C7 RID: 455
		public PopupSceneSwitchItemSequence.BodyPartIndex NegativeBodyPart;

		// Token: 0x020000B3 RID: 179
		public enum BodyPartIndex
		{
			// Token: 0x04000353 RID: 851
			None,
			// Token: 0x04000354 RID: 852
			Weapon0,
			// Token: 0x04000355 RID: 853
			Weapon1,
			// Token: 0x04000356 RID: 854
			Weapon2,
			// Token: 0x04000357 RID: 855
			Weapon3,
			// Token: 0x04000358 RID: 856
			ExtraWeaponSlot,
			// Token: 0x04000359 RID: 857
			Head,
			// Token: 0x0400035A RID: 858
			Body,
			// Token: 0x0400035B RID: 859
			Leg,
			// Token: 0x0400035C RID: 860
			Gloves,
			// Token: 0x0400035D RID: 861
			Cape,
			// Token: 0x0400035E RID: 862
			Horse,
			// Token: 0x0400035F RID: 863
			HorseHarness
		}
	}
}
