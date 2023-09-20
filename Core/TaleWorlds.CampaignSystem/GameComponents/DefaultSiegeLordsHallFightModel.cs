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
	public class DefaultSiegeLordsHallFightModel : SiegeLordsHallFightModel
	{
		public override float AreaLostRatio
		{
			get
			{
				return 3f;
			}
		}

		public override float AttackerDefenderTroopCountRatio
		{
			get
			{
				return 0.7f;
			}
		}

		public override float DefenderMaxArcherRatio
		{
			get
			{
				return 0.7f;
			}
		}

		public override int MaxDefenderSideTroopCount
		{
			get
			{
				return 27;
			}
		}

		public override int MaxDefenderArcherCount
		{
			get
			{
				return MathF.Round((float)this.MaxDefenderSideTroopCount * this.DefenderMaxArcherRatio);
			}
		}

		public override int MaxAttackerSideTroopCount
		{
			get
			{
				return 19;
			}
		}

		public override int DefenderTroopNumberForSuccessfulPullBack
		{
			get
			{
				return 20;
			}
		}

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
