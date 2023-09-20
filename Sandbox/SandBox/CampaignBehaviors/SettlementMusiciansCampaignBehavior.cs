using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Objects;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace SandBox.CampaignBehaviors
{
	// Token: 0x020000A2 RID: 162
	public class SettlementMusiciansCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000972 RID: 2418 RVA: 0x0004EED8 File Offset: 0x0004D0D8
		public override void RegisterEvents()
		{
			CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x0004EEF4 File Offset: 0x0004D0F4
		private void OnMissionStarted(IMission mission)
		{
			Mission mission2;
			if ((mission2 = mission as Mission) != null && CampaignMission.Current != null && PlayerEncounter.LocationEncounter != null && PlayerEncounter.LocationEncounter.Settlement != null && CampaignMission.Current.Location != null)
			{
				IEnumerable<MusicianGroup> enumerable = MBExtensions.FindAllWithType<MusicianGroup>(mission2.MissionObjects);
				Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
				foreach (MusicianGroup musicianGroup in enumerable)
				{
					List<SettlementMusicData> list = this.CreateRandomPlayList(settlement);
					musicianGroup.SetPlayList(list);
				}
			}
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x0004EF88 File Offset: 0x0004D188
		private List<SettlementMusicData> CreateRandomPlayList(Settlement settlement)
		{
			List<string> listOfLocationTags = new List<string>();
			string stringId = CampaignMission.Current.Location.StringId;
			if (stringId == "center")
			{
				listOfLocationTags.Add("lordshall");
				listOfLocationTags.Add("tavern");
			}
			else
			{
				listOfLocationTags.Add(stringId);
			}
			Dictionary<CultureObject, float> dictionary = new Dictionary<CultureObject, float>();
			List<CultureObject> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<CultureObject>();
			Town town = settlement.Town;
			float num;
			if (town == null)
			{
				Village village = settlement.Village;
				num = ((village != null) ? village.Bound.Town.Loyalty : 100f);
			}
			else
			{
				num = town.Loyalty;
			}
			float num2 = num * 0.01f;
			float num3 = 0f;
			using (List<CultureObject>.Enumerator enumerator = objectTypeList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CultureObject c = enumerator.Current;
					dictionary.Add(c, 0f);
					float num4 = Kingdom.All.Sum(delegate(Kingdom k)
					{
						if (c != k.Culture)
						{
							return 0f;
						}
						return k.TotalStrength;
					});
					if (num4 > num3)
					{
						num3 = num4;
					}
				}
			}
			foreach (Kingdom kingdom in Kingdom.All)
			{
				float num5 = (Campaign.MapDiagonal - Campaign.Current.Models.MapDistanceModel.GetDistance(kingdom.FactionMidSettlement, settlement.MapFaction.FactionMidSettlement)) / Campaign.MaximumDistanceBetweenTwoSettlements;
				float num6 = num5 * num5 * num5 * 2f;
				num6 += (settlement.MapFaction.IsAtWarWith(kingdom) ? 1f : 2f) * num2;
				dictionary[kingdom.Culture] = MathF.Max(dictionary[kingdom.Culture], num6);
			}
			Dictionary<CultureObject, float> dictionary2;
			CultureObject cultureObject;
			foreach (Kingdom kingdom2 in Kingdom.All)
			{
				dictionary2 = dictionary;
				cultureObject = kingdom2.Culture;
				dictionary2[cultureObject] += kingdom2.TotalStrength / num3 * 0.5f;
			}
			foreach (Town town2 in Town.AllTowns)
			{
				float num7 = (Campaign.MapDiagonal - Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, town2.Settlement)) / Campaign.MapDiagonal;
				float num8 = num7 * num7 * num7;
				num8 *= MathF.Min(settlement.Prosperity, 5000f) * 0.0002f;
				dictionary2 = dictionary;
				cultureObject = town2.Culture;
				dictionary2[cultureObject] += num8;
			}
			dictionary2 = dictionary;
			cultureObject = settlement.Culture;
			dictionary2[cultureObject] += 10f;
			dictionary2 = dictionary;
			cultureObject = settlement.MapFaction.Culture;
			dictionary2[cultureObject] += num2 * 5f;
			List<SettlementMusicData> list = (from x in MBObjectManager.Instance.GetObjectTypeList<SettlementMusicData>()
				where listOfLocationTags.Contains(x.LocationId)
				select x).ToList<SettlementMusicData>();
			KeyValuePair<CultureObject, float> maxWeightedCulture = Extensions.MaxBy<KeyValuePair<CultureObject, float>, float>(dictionary, (KeyValuePair<CultureObject, float> x) => x.Value);
			float num9 = (float)list.Count((SettlementMusicData x) => x.Culture == maxWeightedCulture.Key) / maxWeightedCulture.Value;
			List<SettlementMusicData> list2 = new List<SettlementMusicData>();
			foreach (KeyValuePair<CultureObject, float> keyValuePair in dictionary)
			{
				int num10 = MBRandom.RoundRandomized(num9 * keyValuePair.Value);
				if (num10 > 0)
				{
					this.PopulatePlayList(list2, list, keyValuePair.Key, num10);
				}
			}
			if (Extensions.IsEmpty<SettlementMusicData>(list2))
			{
				list2 = list;
			}
			Extensions.Shuffle<SettlementMusicData>(list2);
			return list2;
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x0004F3DC File Offset: 0x0004D5DC
		private void PopulatePlayList(List<SettlementMusicData> playList, List<SettlementMusicData> settlementMusicDatas, CultureObject culture, int count)
		{
			List<SettlementMusicData> list = settlementMusicDatas.Where((SettlementMusicData x) => x.Culture == culture).ToList<SettlementMusicData>();
			Extensions.Shuffle<SettlementMusicData>(list);
			int num = 0;
			while (num < count && num < list.Count)
			{
				playList.Add(list[num]);
				num++;
			}
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x0004F437 File Offset: 0x0004D637
		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
