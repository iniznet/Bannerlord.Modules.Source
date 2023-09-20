using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Inventory
{
	public struct TransferCommand
	{
		public TransferCommand.CommandType TypeOfCommand { get; private set; }

		public Equipment CharacterEquipment
		{
			get
			{
				if (!this.IsCivilianEquipment)
				{
					CharacterObject character = this.Character;
					if (character == null)
					{
						return null;
					}
					return character.FirstBattleEquipment;
				}
				else
				{
					CharacterObject character2 = this.Character;
					if (character2 == null)
					{
						return null;
					}
					return character2.FirstCivilianEquipment;
				}
			}
		}

		public InventoryLogic.InventorySide FromSide { get; private set; }

		public InventoryLogic.InventorySide ToSide { get; private set; }

		public EquipmentIndex FromEquipmentIndex { get; private set; }

		public EquipmentIndex ToEquipmentIndex { get; private set; }

		public int Amount { get; private set; }

		public ItemRosterElement ElementToTransfer { get; private set; }

		public CharacterObject Character { get; private set; }

		public bool IsCivilianEquipment { get; private set; }

		public static TransferCommand Transfer(int amount, InventoryLogic.InventorySide fromSide, InventoryLogic.InventorySide toSide, ItemRosterElement elementToTransfer, EquipmentIndex fromEquipmentIndex, EquipmentIndex toEquipmentIndex, CharacterObject character, bool civilianEquipment)
		{
			return new TransferCommand
			{
				TypeOfCommand = TransferCommand.CommandType.Transfer,
				FromSide = fromSide,
				ToSide = toSide,
				ElementToTransfer = elementToTransfer,
				FromEquipmentIndex = fromEquipmentIndex,
				ToEquipmentIndex = toEquipmentIndex,
				Character = character,
				Amount = amount,
				IsCivilianEquipment = civilianEquipment
			};
		}

		public enum CommandType
		{
			Transfer
		}
	}
}
