using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003AD RID: 941
	public class MilitiasCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003851 RID: 14417 RVA: 0x000FFCBE File Offset: 0x000FDEBE
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
		}

		// Token: 0x06003852 RID: 14418 RVA: 0x000FFCD8 File Offset: 0x000FDED8
		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter starter, int i)
		{
			int count = Town.AllTowns.Count;
			int count2 = Town.AllCastles.Count;
			int count3 = Village.All.Count;
			int num = count / 100 + ((count % 100 > i) ? 1 : 0);
			int num2 = count2 / 100 + ((count2 % 100 > i) ? 1 : 0);
			int num3 = count3 / 100 + ((count3 % 100 > i) ? 1 : 0);
			int num4 = count / 100 * i;
			int num5 = count2 / 100 * i;
			int num6 = count3 / 100 * i;
			for (int j = 0; j < i; j++)
			{
				num4 += ((count % 100 > j) ? 1 : 0);
				num5 += ((count2 % 100 > j) ? 1 : 0);
				num6 += ((count3 % 100 > j) ? 1 : 0);
			}
			for (int k = 0; k < num; k++)
			{
				Town.AllTowns[num4 + k].Settlement.Militia = Town.AllTowns[num4 + k].Settlement.Town.MilitiaChange * 45f;
			}
			for (int l = 0; l < num2; l++)
			{
				Town.AllCastles[num5 + l].Settlement.Militia = Town.AllCastles[num5 + l].Settlement.Town.MilitiaChange * 45f;
			}
			for (int m = 0; m < num3; m++)
			{
				Village.All[num6 + m].Settlement.Militia = Village.All[num6 + m].Settlement.Village.MilitiaChange * 45f;
			}
		}

		// Token: 0x06003853 RID: 14419 RVA: 0x000FFE7F File Offset: 0x000FE07F
		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
