using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Inventory
{
	public class TransferCommandResult
	{
		public CharacterObject TransferCharacter { get; private set; }

		public bool IsCivilianEquipment { get; private set; }

		public Equipment TransferEquipment
		{
			get
			{
				if (!this.IsCivilianEquipment)
				{
					CharacterObject transferCharacter = this.TransferCharacter;
					if (transferCharacter == null)
					{
						return null;
					}
					return transferCharacter.FirstBattleEquipment;
				}
				else
				{
					CharacterObject transferCharacter2 = this.TransferCharacter;
					if (transferCharacter2 == null)
					{
						return null;
					}
					return transferCharacter2.FirstCivilianEquipment;
				}
			}
		}

		public InventoryLogic.InventorySide ResultSide { get; private set; }

		public ItemRosterElement EffectedItemRosterElement { get; private set; }

		public int EffectedNumber { get; private set; }

		public int FinalNumber { get; private set; }

		public EquipmentIndex EffectedEquipmentIndex { get; private set; }

		public TransferCommandResult()
		{
		}

		public TransferCommandResult(InventoryLogic.InventorySide resultSide, ItemRosterElement effectedItemRosterElement, int effectedNumber, int finalNumber, EquipmentIndex effectedEquipmentIndex, CharacterObject transferCharacter, bool isCivilianEquipment)
		{
			this.ResultSide = resultSide;
			this.EffectedItemRosterElement = effectedItemRosterElement;
			this.EffectedNumber = effectedNumber;
			this.FinalNumber = finalNumber;
			this.EffectedEquipmentIndex = effectedEquipmentIndex;
			this.TransferCharacter = transferCharacter;
			this.IsCivilianEquipment = isCivilianEquipment;
		}
	}
}
