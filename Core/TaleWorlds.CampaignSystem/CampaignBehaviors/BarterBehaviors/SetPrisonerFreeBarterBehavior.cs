using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.BarterBehaviors
{
	// Token: 0x020003FC RID: 1020
	public class SetPrisonerFreeBarterBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003D15 RID: 15637 RVA: 0x00122907 File Offset: 0x00120B07
		public override void RegisterEvents()
		{
			CampaignEvents.BarterablesRequested.AddNonSerializedListener(this, new Action<BarterData>(this.CheckForBarters));
		}

		// Token: 0x06003D16 RID: 15638 RVA: 0x00122920 File Offset: 0x00120B20
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003D17 RID: 15639 RVA: 0x00122924 File Offset: 0x00120B24
		public void CheckForBarters(BarterData args)
		{
			PartyBase offererParty = args.OffererParty;
			PartyBase otherParty = args.OtherParty;
			if (offererParty != null && otherParty != null)
			{
				List<CharacterObject> list = offererParty.PrisonerHeroes.ToList<CharacterObject>();
				if (offererParty.LeaderHero != null)
				{
					Hero leaderHero = offererParty.LeaderHero;
					Hero leaderHero2 = offererParty.LeaderHero;
					object obj;
					if (leaderHero2 == null)
					{
						obj = null;
					}
					else
					{
						Clan clan = leaderHero2.Clan;
						obj = ((clan != null) ? clan.Leader : null);
					}
					if (leaderHero == obj)
					{
						list.AddRange(offererParty.LeaderHero.Clan.DungeonPrisonersOfClan);
					}
				}
				foreach (CharacterObject characterObject in list)
				{
					if (characterObject.IsHero && !FactionManager.IsAtWarAgainstFaction(characterObject.HeroObject.MapFaction, otherParty.MapFaction))
					{
						Barterable barterable = new SetPrisonerFreeBarterable(characterObject.HeroObject, args.OffererHero, args.OffererParty, args.OtherHero);
						args.AddBarterable<PrisonerBarterGroup>(barterable, false);
					}
				}
				List<CharacterObject> list2 = otherParty.PrisonerHeroes.ToList<CharacterObject>();
				if (otherParty.LeaderHero != null)
				{
					Hero leaderHero3 = otherParty.LeaderHero;
					Hero leaderHero4 = otherParty.LeaderHero;
					object obj2;
					if (leaderHero4 == null)
					{
						obj2 = null;
					}
					else
					{
						Clan clan2 = leaderHero4.Clan;
						obj2 = ((clan2 != null) ? clan2.Leader : null);
					}
					if (leaderHero3 == obj2)
					{
						list2.AddRange(otherParty.LeaderHero.Clan.DungeonPrisonersOfClan);
					}
				}
				foreach (CharacterObject characterObject2 in list2)
				{
					if (characterObject2.IsHero && !FactionManager.IsAtWarAgainstFaction(characterObject2.HeroObject.MapFaction, offererParty.MapFaction))
					{
						Barterable barterable2 = new SetPrisonerFreeBarterable(characterObject2.HeroObject, args.OtherHero, args.OtherParty, args.OffererHero);
						args.AddBarterable<PrisonerBarterGroup>(barterable2, false);
					}
				}
			}
		}
	}
}
