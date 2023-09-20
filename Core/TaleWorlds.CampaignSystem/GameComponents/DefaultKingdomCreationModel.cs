using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000114 RID: 276
	public class DefaultKingdomCreationModel : KingdomCreationModel
	{
		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x060015D2 RID: 5586 RVA: 0x00067518 File Offset: 0x00065718
		public override int MinimumClanTierToCreateKingdom
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x060015D3 RID: 5587 RVA: 0x0006751B File Offset: 0x0006571B
		public override int MinimumNumberOfSettlementsOwnedToCreateKingdom
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x060015D4 RID: 5588 RVA: 0x0006751E File Offset: 0x0006571E
		public override int MinimumTroopCountToCreateKingdom
		{
			get
			{
				return 100;
			}
		}

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x060015D5 RID: 5589 RVA: 0x00067522 File Offset: 0x00065722
		public override int MaximumNumberOfInitialPolicies
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x00067528 File Offset: 0x00065728
		public override bool IsPlayerKingdomCreationPossible(out List<TextObject> explanations)
		{
			bool flag = true;
			explanations = new List<TextObject>();
			if (Hero.MainHero.MapFaction.IsKingdomFaction)
			{
				flag = false;
				TextObject textObject = new TextObject("{=w5b79MmE}Player clan should be independent.", null);
				explanations.Add(textObject);
			}
			if (Clan.PlayerClan.Tier < this.MinimumClanTierToCreateKingdom)
			{
				flag = false;
				TextObject textObject2 = new TextObject("{=j0UDi2AN}Clan tier should be at least {TIER}.", null);
				textObject2.SetTextVariable("TIER", this.MinimumClanTierToCreateKingdom);
				explanations.Add(textObject2);
			}
			if (Clan.PlayerClan.Settlements.Count((Settlement t) => t.IsTown || t.IsCastle) < this.MinimumNumberOfSettlementsOwnedToCreateKingdom)
			{
				flag = false;
				TextObject textObject3 = new TextObject("{=YsGSgaba}Number of towns or castles you own should be at least {SETTLEMENT_COUNT}.", null);
				textObject3.SetTextVariable("SETTLEMENT_COUNT", this.MinimumNumberOfSettlementsOwnedToCreateKingdom);
				explanations.Add(textObject3);
			}
			if (Clan.PlayerClan.Fiefs.Sum(delegate(Town t)
			{
				MobileParty garrisonParty = t.GarrisonParty;
				int? num;
				if (garrisonParty == null)
				{
					num = null;
				}
				else
				{
					TroopRoster memberRoster = garrisonParty.MemberRoster;
					num = ((memberRoster != null) ? new int?(memberRoster.TotalHealthyCount) : null);
				}
				int? num2 = num;
				if (num2 == null)
				{
					return 0;
				}
				return num2.GetValueOrDefault();
			}) + Clan.PlayerClan.WarPartyComponents.Sum((WarPartyComponent t) => t.MobileParty.MemberRoster.TotalHealthyCount) < this.MinimumTroopCountToCreateKingdom)
			{
				flag = false;
				TextObject textObject4 = new TextObject("{=K2txLdOS}You should have at least {TROOP_COUNT} men ready to fight.", null);
				textObject4.SetTextVariable("TROOP_COUNT", this.MinimumTroopCountToCreateKingdom);
				explanations.Add(textObject4);
			}
			return flag;
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x00067690 File Offset: 0x00065890
		public override bool IsPlayerKingdomAbdicationPossible(out List<TextObject> explanations)
		{
			explanations = new List<TextObject>();
			object obj = Clan.PlayerClan.Kingdom != null && Clan.PlayerClan.Kingdom.RulingClan == Clan.PlayerClan;
			bool flag = MobileParty.MainParty.MapEvent != null || MobileParty.MainParty.SiegeEvent != null;
			object obj2 = obj;
			bool flag2 = obj2 != null && !Clan.PlayerClan.Kingdom.UnresolvedDecisions.IsEmpty<KingdomDecision>();
			if (obj2 == null)
			{
				explanations.Add(new TextObject("{=s1ERZ4ZR}You must be the king", null));
			}
			if (flag)
			{
				explanations.Add(new TextObject("{=uaMmmhRV}You must conclude your current encounter", null));
			}
			if (flag2)
			{
				explanations.Add(new TextObject("{=etKrpcHe}You must resolve pending decisions", null));
			}
			return obj2 != null && !flag && !flag2;
		}

		// Token: 0x060015D8 RID: 5592 RVA: 0x0006774E File Offset: 0x0006594E
		public override IEnumerable<CultureObject> GetAvailablePlayerKingdomCultures()
		{
			List<CultureObject> list = new List<CultureObject>();
			list.Add(Clan.PlayerClan.Culture);
			foreach (Settlement settlement in Clan.PlayerClan.Settlements.Where((Settlement t) => t.IsTown || t.IsCastle))
			{
				if (!list.Contains(settlement.Culture))
				{
					list.Add(settlement.Culture);
				}
			}
			foreach (CultureObject cultureObject in list)
			{
				yield return cultureObject;
			}
			List<CultureObject>.Enumerator enumerator2 = default(List<CultureObject>.Enumerator);
			yield break;
			yield break;
		}
	}
}
