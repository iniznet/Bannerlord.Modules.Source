using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Inventory
{
	// Token: 0x020000D4 RID: 212
	public class TransferCommandResult
	{
		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06001312 RID: 4882 RVA: 0x000562E5 File Offset: 0x000544E5
		// (set) Token: 0x06001313 RID: 4883 RVA: 0x000562ED File Offset: 0x000544ED
		public CharacterObject TransferCharacter { get; private set; }

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06001314 RID: 4884 RVA: 0x000562F6 File Offset: 0x000544F6
		// (set) Token: 0x06001315 RID: 4885 RVA: 0x000562FE File Offset: 0x000544FE
		public bool IsCivilianEquipment { get; private set; }

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06001316 RID: 4886 RVA: 0x00056307 File Offset: 0x00054507
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

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06001317 RID: 4887 RVA: 0x00056334 File Offset: 0x00054534
		// (set) Token: 0x06001318 RID: 4888 RVA: 0x0005633C File Offset: 0x0005453C
		public InventoryLogic.InventorySide ResultSide { get; private set; }

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06001319 RID: 4889 RVA: 0x00056345 File Offset: 0x00054545
		// (set) Token: 0x0600131A RID: 4890 RVA: 0x0005634D File Offset: 0x0005454D
		public ItemRosterElement EffectedItemRosterElement { get; private set; }

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x0600131B RID: 4891 RVA: 0x00056356 File Offset: 0x00054556
		// (set) Token: 0x0600131C RID: 4892 RVA: 0x0005635E File Offset: 0x0005455E
		public int EffectedNumber { get; private set; }

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x0600131D RID: 4893 RVA: 0x00056367 File Offset: 0x00054567
		// (set) Token: 0x0600131E RID: 4894 RVA: 0x0005636F File Offset: 0x0005456F
		public int FinalNumber { get; private set; }

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x0600131F RID: 4895 RVA: 0x00056378 File Offset: 0x00054578
		// (set) Token: 0x06001320 RID: 4896 RVA: 0x00056380 File Offset: 0x00054580
		public EquipmentIndex EffectedEquipmentIndex { get; private set; }

		// Token: 0x06001321 RID: 4897 RVA: 0x00056389 File Offset: 0x00054589
		public TransferCommandResult()
		{
		}

		// Token: 0x06001322 RID: 4898 RVA: 0x00056391 File Offset: 0x00054591
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
