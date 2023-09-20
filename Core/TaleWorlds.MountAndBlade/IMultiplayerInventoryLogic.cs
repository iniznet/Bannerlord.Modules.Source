using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200031E RID: 798
	public interface IMultiplayerInventoryLogic
	{
		// Token: 0x06002B04 RID: 11012
		int TransferFromEquipmentSlotToChest(int draggedEquipmentIndex);

		// Token: 0x06002B05 RID: 11013
		bool TransferFromChestToEquipmentSlot(int draggedChestNo, int droppedEquipmentIndex, out int oldEquipmentNewChestIndex);

		// Token: 0x06002B06 RID: 11014
		bool TransferFromEquipmentSlotToEquipmentSlot(int draggedEquipmentIndex, int droppedEquipmentIndex);

		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x06002B07 RID: 11015
		int CharacterCount { get; }

		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x06002B08 RID: 11016
		int CurrentCharacterIndex { get; }

		// Token: 0x06002B09 RID: 11017
		void SetNextCharacter();

		// Token: 0x06002B0A RID: 11018
		void SetPreviousCharacter();

		// Token: 0x06002B0B RID: 11019
		void OnEditSelectedCharacter(BodyProperties bodyProperties, bool isFemale);

		// Token: 0x06002B0C RID: 11020
		int FindProperEquipmentSlotIndex(string itemType);

		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x06002B0D RID: 11021
		bool SingleCharacterMode { get; }
	}
}
