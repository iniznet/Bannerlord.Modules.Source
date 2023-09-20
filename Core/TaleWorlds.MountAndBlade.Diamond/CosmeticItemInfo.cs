using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000105 RID: 261
	[Serializable]
	public class CosmeticItemInfo
	{
		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x00006A72 File Offset: 0x00004C72
		// (set) Token: 0x060004B5 RID: 1205 RVA: 0x00006A7A File Offset: 0x00004C7A
		public string TroopId { get; private set; }

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x060004B6 RID: 1206 RVA: 0x00006A83 File Offset: 0x00004C83
		// (set) Token: 0x060004B7 RID: 1207 RVA: 0x00006A8B File Offset: 0x00004C8B
		public string CosmeticIndex { get; private set; }

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x060004B8 RID: 1208 RVA: 0x00006A94 File Offset: 0x00004C94
		// (set) Token: 0x060004B9 RID: 1209 RVA: 0x00006A9C File Offset: 0x00004C9C
		public bool IsEquipped { get; private set; }

		// Token: 0x060004BA RID: 1210 RVA: 0x00006AA5 File Offset: 0x00004CA5
		public CosmeticItemInfo(string troopId, string cosmeticIndex, bool isEquipped)
		{
			this.TroopId = troopId;
			this.CosmeticIndex = cosmeticIndex;
			this.IsEquipped = isEquipped;
		}
	}
}
