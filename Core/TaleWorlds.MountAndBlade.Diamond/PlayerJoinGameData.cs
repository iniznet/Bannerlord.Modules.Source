using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000123 RID: 291
	[Serializable]
	public class PlayerJoinGameData
	{
		// Token: 0x1700024A RID: 586
		// (get) Token: 0x0600068A RID: 1674 RVA: 0x0000B021 File Offset: 0x00009221
		public PlayerData PlayerData { get; }

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x0600068B RID: 1675 RVA: 0x0000B029 File Offset: 0x00009229
		public PlayerId PlayerId
		{
			get
			{
				return this.PlayerData.PlayerId;
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x0600068C RID: 1676 RVA: 0x0000B036 File Offset: 0x00009236
		public string Name { get; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x0000B03E File Offset: 0x0000923E
		public Guid? PartyId { get; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x0000B046 File Offset: 0x00009246
		public Dictionary<string, List<string>> UsedCosmetics { get; }

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x0600068F RID: 1679 RVA: 0x0000B04E File Offset: 0x0000924E
		public string IpAddress { get; }

		// Token: 0x06000690 RID: 1680 RVA: 0x0000B056 File Offset: 0x00009256
		public PlayerJoinGameData(PlayerData playerData, string name, Guid? partyId, Dictionary<string, List<string>> usedCosmetics, string ipAddress)
		{
			this.PlayerData = playerData;
			this.Name = name;
			this.PartyId = partyId;
			this.UsedCosmetics = usedCosmetics;
			this.IpAddress = ipAddress;
		}
	}
}
