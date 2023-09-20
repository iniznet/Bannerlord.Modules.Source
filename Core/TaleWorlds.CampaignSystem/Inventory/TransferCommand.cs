using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Inventory
{
	// Token: 0x020000D5 RID: 213
	public struct TransferCommand
	{
		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06001323 RID: 4899 RVA: 0x000563CE File Offset: 0x000545CE
		// (set) Token: 0x06001324 RID: 4900 RVA: 0x000563D6 File Offset: 0x000545D6
		public TransferCommand.CommandType TypeOfCommand { get; private set; }

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06001325 RID: 4901 RVA: 0x000563DF File Offset: 0x000545DF
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

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06001326 RID: 4902 RVA: 0x0005640C File Offset: 0x0005460C
		// (set) Token: 0x06001327 RID: 4903 RVA: 0x00056414 File Offset: 0x00054614
		public InventoryLogic.InventorySide FromSide { get; private set; }

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06001328 RID: 4904 RVA: 0x0005641D File Offset: 0x0005461D
		// (set) Token: 0x06001329 RID: 4905 RVA: 0x00056425 File Offset: 0x00054625
		public InventoryLogic.InventorySide ToSide { get; private set; }

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x0600132A RID: 4906 RVA: 0x0005642E File Offset: 0x0005462E
		// (set) Token: 0x0600132B RID: 4907 RVA: 0x00056436 File Offset: 0x00054636
		public EquipmentIndex FromEquipmentIndex { get; private set; }

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x0600132C RID: 4908 RVA: 0x0005643F File Offset: 0x0005463F
		// (set) Token: 0x0600132D RID: 4909 RVA: 0x00056447 File Offset: 0x00054647
		public EquipmentIndex ToEquipmentIndex { get; private set; }

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x0600132E RID: 4910 RVA: 0x00056450 File Offset: 0x00054650
		// (set) Token: 0x0600132F RID: 4911 RVA: 0x00056458 File Offset: 0x00054658
		public int Amount { get; private set; }

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06001330 RID: 4912 RVA: 0x00056461 File Offset: 0x00054661
		// (set) Token: 0x06001331 RID: 4913 RVA: 0x00056469 File Offset: 0x00054669
		public ItemRosterElement ElementToTransfer { get; private set; }

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06001332 RID: 4914 RVA: 0x00056472 File Offset: 0x00054672
		// (set) Token: 0x06001333 RID: 4915 RVA: 0x0005647A File Offset: 0x0005467A
		public CharacterObject Character { get; private set; }

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x06001334 RID: 4916 RVA: 0x00056483 File Offset: 0x00054683
		// (set) Token: 0x06001335 RID: 4917 RVA: 0x0005648B File Offset: 0x0005468B
		public bool IsCivilianEquipment { get; private set; }

		// Token: 0x06001336 RID: 4918 RVA: 0x00056494 File Offset: 0x00054694
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

		// Token: 0x020004EA RID: 1258
		public enum CommandType
		{
			// Token: 0x04001522 RID: 5410
			Transfer
		}
	}
}
