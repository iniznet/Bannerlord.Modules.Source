using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public interface IMultiplayerInventoryLogic
	{
		int TransferFromEquipmentSlotToChest(int draggedEquipmentIndex);

		bool TransferFromChestToEquipmentSlot(int draggedChestNo, int droppedEquipmentIndex, out int oldEquipmentNewChestIndex);

		bool TransferFromEquipmentSlotToEquipmentSlot(int draggedEquipmentIndex, int droppedEquipmentIndex);

		int CharacterCount { get; }

		int CurrentCharacterIndex { get; }

		void SetNextCharacter();

		void SetPreviousCharacter();

		void OnEditSelectedCharacter(BodyProperties bodyProperties, bool isFemale);

		int FindProperEquipmentSlotIndex(string itemType);

		bool SingleCharacterMode { get; }
	}
}
