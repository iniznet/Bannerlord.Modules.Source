using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000F6 RID: 246
	[Serializable]
	public class ClanCreationProgress
	{
		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x0000658C File Offset: 0x0000478C
		public Progress Progress
		{
			get
			{
				int num = 0;
				int num2 = 0;
				foreach (ClanCreationPlayerData clanCreationPlayerData2 in this.ClanCreationPlayerData)
				{
					if (clanCreationPlayerData2.ClanCreationAnswer == ClanCreationAnswer.Accepted)
					{
						num++;
					}
					else if (clanCreationPlayerData2.ClanCreationAnswer == ClanCreationAnswer.Declined)
					{
						num2++;
					}
				}
				if (num == this.ClanCreationPlayerData.Length)
				{
					return Progress.Success;
				}
				if (num2 > 0)
				{
					return Progress.Fail;
				}
				return Progress.Undecided;
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06000455 RID: 1109 RVA: 0x000065E9 File Offset: 0x000047E9
		// (set) Token: 0x06000456 RID: 1110 RVA: 0x000065F1 File Offset: 0x000047F1
		public ClanCreationPlayerData[] ClanCreationPlayerData { get; private set; }

		// Token: 0x06000457 RID: 1111 RVA: 0x000065FA File Offset: 0x000047FA
		public ClanCreationProgress(ClanCreationPlayerData[] clanCreationPlayerData)
		{
			this.ClanCreationPlayerData = clanCreationPlayerData;
		}
	}
}
