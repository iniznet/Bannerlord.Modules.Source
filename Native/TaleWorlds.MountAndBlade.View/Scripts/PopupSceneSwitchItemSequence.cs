using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	public class PopupSceneSwitchItemSequence : PopupSceneSequence
	{
		public override void OnInitialState()
		{
			this.AttachItem(this.InitialItem, this.InitialBodyPart);
		}

		public override void OnPositiveState()
		{
			this.AttachItem(this.PositiveItem, this.PositiveBodyPart);
		}

		public override void OnNegativeState()
		{
			this.AttachItem(this.NegativeItem, this.NegativeBodyPart);
		}

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

		public string InitialItem;

		public string PositiveItem;

		public string NegativeItem;

		public PopupSceneSwitchItemSequence.BodyPartIndex InitialBodyPart;

		public PopupSceneSwitchItemSequence.BodyPartIndex PositiveBodyPart;

		public PopupSceneSwitchItemSequence.BodyPartIndex NegativeBodyPart;

		public enum BodyPartIndex
		{
			None,
			Weapon0,
			Weapon1,
			Weapon2,
			Weapon3,
			ExtraWeaponSlot,
			Head,
			Body,
			Leg,
			Gloves,
			Cape,
			Horse,
			HorseHarness
		}
	}
}
