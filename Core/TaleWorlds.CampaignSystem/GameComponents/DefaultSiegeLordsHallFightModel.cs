using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000140 RID: 320
	public class DefaultSiegeLordsHallFightModel : SiegeLordsHallFightModel
	{
		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x060017A8 RID: 6056 RVA: 0x0007598E File Offset: 0x00073B8E
		public override float AreaLostRatio
		{
			get
			{
				return 3f;
			}
		}

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x060017A9 RID: 6057 RVA: 0x00075995 File Offset: 0x00073B95
		public override float AttackerDefenderTroopCountRatio
		{
			get
			{
				return 0.7f;
			}
		}

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x060017AA RID: 6058 RVA: 0x0007599C File Offset: 0x00073B9C
		public override float DefenderMaxArcherRatio
		{
			get
			{
				return 0.7f;
			}
		}

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x060017AB RID: 6059 RVA: 0x000759A3 File Offset: 0x00073BA3
		public override int MaxDefenderSideTroopCount
		{
			get
			{
				return 27;
			}
		}

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x060017AC RID: 6060 RVA: 0x000759A7 File Offset: 0x00073BA7
		public override int MaxDefenderArcherCount
		{
			get
			{
				return MathF.Round((float)this.MaxDefenderSideTroopCount * this.DefenderMaxArcherRatio);
			}
		}

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x060017AD RID: 6061 RVA: 0x000759BC File Offset: 0x00073BBC
		public override int MaxAttackerSideTroopCount
		{
			get
			{
				return 19;
			}
		}

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x060017AE RID: 6062 RVA: 0x000759C0 File Offset: 0x00073BC0
		public override int DefenderTroopNumberForSuccessfulPullBack
		{
			get
			{
				return 20;
			}
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x000759C4 File Offset: 0x00073BC4
		public override FlattenedTroopRoster GetPriorityListForLordsHallFightMission(MapEvent playerMapEvent, BattleSideEnum side, int troopCount)
		{
			List<MapEventParty> list = (from x in playerMapEvent.PartiesOnSide(side)
				where x.Party.IsMobile
				select x).ToList<MapEventParty>();
			FlattenedTroopRoster flattenedTroopRoster = new FlattenedTroopRoster(list.Sum((MapEventParty x) => x.Party.MemberRoster.TotalHealthyCount));
			foreach (MapEventParty mapEventParty in list)
			{
				flattenedTroopRoster.Add(mapEventParty.Party.MemberRoster.GetTroopRoster());
			}
			List<FlattenedTroopRosterElement> list2 = flattenedTroopRoster.Where((FlattenedTroopRosterElement x) => !x.Troop.IsHero && x.Troop.IsRanged && !x.IsWounded).ToList<FlattenedTroopRosterElement>();
			list2.Shuffle<FlattenedTroopRosterElement>();
			List<FlattenedTroopRosterElement> list3 = flattenedTroopRoster.Where((FlattenedTroopRosterElement x) => !x.Troop.IsHero && !x.Troop.IsRanged && !x.IsWounded).ToList<FlattenedTroopRosterElement>();
			list3.Shuffle<FlattenedTroopRosterElement>();
			flattenedTroopRoster.RemoveIf((FlattenedTroopRosterElement x) => !x.Troop.IsHero || x.IsWounded);
			int num = troopCount - flattenedTroopRoster.Count<FlattenedTroopRosterElement>();
			if (num > 0)
			{
				int count = list2.Count;
				int count2 = list3.Count;
				int num2 = MathF.Min(count, Campaign.Current.Models.SiegeLordsHallFightModel.MaxDefenderArcherCount);
				int num3 = 0;
				int num4 = 0;
				while (num > 0 && (num3 < num2 || num4 < count2))
				{
					if (num3 < num2)
					{
						FlattenedTroopRosterElement flattenedTroopRosterElement = list2[num3];
						flattenedTroopRoster.Add(flattenedTroopRosterElement.Troop, false, flattenedTroopRosterElement.Xp);
						num--;
					}
					if (num4 < count2 && num > 0)
					{
						FlattenedTroopRosterElement flattenedTroopRosterElement2 = list3[num4];
						flattenedTroopRoster.Add(flattenedTroopRosterElement2.Troop, false, flattenedTroopRosterElement2.Xp);
						num--;
					}
					num3++;
					num4++;
				}
			}
			return flattenedTroopRoster;
		}
	}
}
