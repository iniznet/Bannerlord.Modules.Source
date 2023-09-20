using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x02000391 RID: 913
	public class FindingItemOnMapBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600367A RID: 13946 RVA: 0x000F2C6E File Offset: 0x000F0E6E
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
		}

		// Token: 0x0600367B RID: 13947 RVA: 0x000F2C87 File Offset: 0x000F0E87
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600367C RID: 13948 RVA: 0x000F2C8C File Offset: 0x000F0E8C
		public void DailyTickParty(MobileParty party)
		{
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(party.CurrentNavigationFace);
			if ((faceTerrainType == TerrainType.Steppe || faceTerrainType == TerrainType.Plain) && party.HasPerk(DefaultPerks.Scouting.BeastWhisperer, false) && MBRandom.RandomFloat < DefaultPerks.Scouting.BeastWhisperer.PrimaryBonus)
			{
				ItemObject randomElementWithPredicate = Items.All.GetRandomElementWithPredicate((ItemObject x) => x.IsMountable);
				if (randomElementWithPredicate != null)
				{
					party.ItemRoster.AddToCounts(randomElementWithPredicate, 1);
					if (party.IsMainParty)
					{
						TextObject textObject = new TextObject("{=vl9bawa7}{COUNT} {?(COUNT > 1)}{PLURAL(ANIMAL_NAME)} are{?}{ANIMAL_NAME} is{\\?} added to your party.", null);
						textObject.SetTextVariable("COUNT", 1);
						textObject.SetTextVariable("ANIMAL_NAME", randomElementWithPredicate.Name);
						InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
					}
				}
			}
		}
	}
}
