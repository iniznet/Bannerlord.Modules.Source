using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.BarterBehaviors
{
	// Token: 0x020003F6 RID: 1014
	public class DiplomaticBartersBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CF7 RID: 15607 RVA: 0x00121C97 File Offset: 0x0011FE97
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.DailyTickClan));
		}

		// Token: 0x06003CF8 RID: 15608 RVA: 0x00121CB0 File Offset: 0x0011FEB0
		private void DailyTickClan(Clan clan)
		{
			bool flag = false;
			using (List<WarPartyComponent>.Enumerator enumerator = clan.WarPartyComponents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.MobileParty.MapEvent != null)
					{
						flag = true;
						break;
					}
				}
			}
			MBList<Clan> mblist = Clan.NonBanditFactions.ToMBList<Clan>();
			if (clan == Clan.PlayerClan || clan.TotalStrength <= 0f || clan.IsEliminated)
			{
				return;
			}
			if (clan.IsBanditFaction || clan == CampaignData.NeutralFaction || clan.IsRebelClan)
			{
				return;
			}
			if (clan.Kingdom == null && MBRandom.RandomFloat < 0.5f)
			{
				Clan randomElement = mblist.GetRandomElement<Clan>();
				if (randomElement.Kingdom == null && randomElement != Clan.PlayerClan && clan.IsAtWarWith(randomElement) && !clan.IsMinorFaction && !randomElement.IsMinorFaction)
				{
					this.ConsiderPeace(clan, randomElement);
					return;
				}
			}
			else if (MBRandom.RandomFloat < 0.2f && !clan.IsUnderMercenaryService && clan.Kingdom != null && !clan.IsClanTypeMercenary)
			{
				if (MBRandom.RandomFloat < 0.1f)
				{
					Clan clan2 = mblist.GetRandomElement<Clan>();
					int num = 0;
					while (clan2.Kingdom == null || clan.Kingdom == clan2.Kingdom || clan2.IsEliminated)
					{
						clan2 = mblist.GetRandomElement<Clan>();
						num++;
						if (num >= 20)
						{
							break;
						}
					}
					if (clan2.Kingdom != null && clan.Kingdom != clan2.Kingdom && !clan.GetStanceWith(clan2.Kingdom).IsAtConstantWar && !flag && clan2.MapFaction.IsKingdomFaction && !clan2.IsEliminated && clan2 != Clan.PlayerClan && clan2.MapFaction.Leader != Hero.MainHero)
					{
						if (clan.WarPartyComponents.All((WarPartyComponent x) => x.MobileParty.MapEvent == null))
						{
							this.ConsiderDefection(clan, clan2.MapFaction as Kingdom);
							return;
						}
					}
				}
			}
			else if (MBRandom.RandomFloat < ((clan.MapFaction.Leader == Hero.MainHero) ? 0.2f : 0.4f))
			{
				Kingdom kingdom = Kingdom.All[MBRandom.RandomInt(Kingdom.All.Count)];
				int num2 = 0;
				using (List<Kingdom>.Enumerator enumerator2 = Kingdom.All.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Culture == clan.Culture)
						{
							num2 += 10;
						}
						else
						{
							num2++;
						}
					}
				}
				int num3 = (int)(MBRandom.RandomFloat * (float)num2);
				foreach (Kingdom kingdom2 in Kingdom.All)
				{
					if (kingdom2.Culture == clan.Culture)
					{
						num3 -= 10;
					}
					else
					{
						num3--;
					}
					if (num3 < 0)
					{
						kingdom = kingdom2;
						break;
					}
				}
				if (kingdom.Leader != Hero.MainHero && !kingdom.IsEliminated && (clan.Kingdom == null || clan.IsUnderMercenaryService) && clan.MapFaction != kingdom && !clan.MapFaction.IsAtWarWith(kingdom) && !clan.GetStanceWith(kingdom).IsAtConstantWar)
				{
					if (clan.WarPartyComponents.All((WarPartyComponent x) => x.MobileParty.MapEvent == null))
					{
						bool flag2 = true;
						if (!clan.IsMinorFaction)
						{
							foreach (Kingdom kingdom3 in Kingdom.All)
							{
								if (kingdom3 != kingdom && clan.IsAtWarWith(kingdom3) && !kingdom3.IsAtWarWith(kingdom) && kingdom.TotalStrength <= 10f * kingdom3.TotalStrength)
								{
									flag2 = false;
									break;
								}
							}
						}
						if (flag2)
						{
							if (clan.IsMinorFaction)
							{
								this.ConsiderClanJoinAsMercenary(clan, kingdom);
								return;
							}
							this.ConsiderClanJoin(clan, kingdom);
							return;
						}
					}
				}
			}
			else if (MBRandom.RandomFloat < 0.4f)
			{
				if (clan.Kingdom != null && !flag && clan.Kingdom.RulingClan != clan && clan != Clan.PlayerClan)
				{
					if (clan.WarPartyComponents.All((WarPartyComponent x) => x.MobileParty.MapEvent == null))
					{
						if (clan.IsMinorFaction)
						{
							this.ConsiderClanLeaveAsMercenary(clan);
							return;
						}
						this.ConsiderClanLeaveKingdom(clan);
						return;
					}
				}
			}
			else if (MBRandom.RandomFloat < 0.7f)
			{
				Clan randomElement2 = mblist.GetRandomElement<Clan>();
				IFaction mapFaction = randomElement2.MapFaction;
				if (!clan.IsMinorFaction && (!mapFaction.IsMinorFaction || mapFaction == Clan.PlayerClan) && clan.Kingdom == null && randomElement2 != clan && !mapFaction.IsEliminated && mapFaction.WarPartyComponents.Count > 0 && clan.WarPartyComponents.Count > 0 && !clan.IsAtWarWith(mapFaction) && clan != Clan.PlayerClan)
				{
					this.ConsiderWar(clan, mapFaction);
				}
			}
		}

		// Token: 0x06003CF9 RID: 15609 RVA: 0x0012222C File Offset: 0x0012042C
		private void ConsiderClanLeaveKingdom(Clan clan)
		{
			LeaveKingdomAsClanBarterable leaveKingdomAsClanBarterable = new LeaveKingdomAsClanBarterable(clan.Leader, null);
			if (leaveKingdomAsClanBarterable.GetValueForFaction(clan) > 0)
			{
				leaveKingdomAsClanBarterable.Apply();
			}
		}

		// Token: 0x06003CFA RID: 15610 RVA: 0x00122258 File Offset: 0x00120458
		private void ConsiderClanLeaveAsMercenary(Clan clan)
		{
			LeaveKingdomAsClanBarterable leaveKingdomAsClanBarterable = new LeaveKingdomAsClanBarterable(clan.Leader, null);
			if (leaveKingdomAsClanBarterable.GetValueForFaction(clan) > 500)
			{
				leaveKingdomAsClanBarterable.Apply();
			}
		}

		// Token: 0x06003CFB RID: 15611 RVA: 0x00122288 File Offset: 0x00120488
		private void ConsiderClanJoin(Clan clan, Kingdom kingdom)
		{
			JoinKingdomAsClanBarterable joinKingdomAsClanBarterable = new JoinKingdomAsClanBarterable(clan.Leader, kingdom, false);
			if (joinKingdomAsClanBarterable.GetValueForFaction(clan) + joinKingdomAsClanBarterable.GetValueForFaction(kingdom) > 0)
			{
				Campaign.Current.BarterManager.ExecuteAiBarter(clan, kingdom, clan.Leader, kingdom.Leader, joinKingdomAsClanBarterable);
			}
		}

		// Token: 0x06003CFC RID: 15612 RVA: 0x001222D4 File Offset: 0x001204D4
		private void ConsiderClanJoinAsMercenary(Clan clan, Kingdom kingdom)
		{
			MercenaryJoinKingdomBarterable mercenaryJoinKingdomBarterable = new MercenaryJoinKingdomBarterable(clan.Leader, null, kingdom);
			if (mercenaryJoinKingdomBarterable.GetValueForFaction(clan) + mercenaryJoinKingdomBarterable.GetValueForFaction(kingdom) > 0)
			{
				Campaign.Current.BarterManager.ExecuteAiBarter(clan, kingdom, clan.Leader, kingdom.Leader, mercenaryJoinKingdomBarterable);
			}
		}

		// Token: 0x06003CFD RID: 15613 RVA: 0x00122320 File Offset: 0x00120520
		private void ConsiderDefection(Clan clan1, Kingdom kingdom)
		{
			JoinKingdomAsClanBarterable joinKingdomAsClanBarterable = new JoinKingdomAsClanBarterable(clan1.Leader, kingdom, false);
			int valueForFaction = joinKingdomAsClanBarterable.GetValueForFaction(clan1);
			int valueForFaction2 = joinKingdomAsClanBarterable.GetValueForFaction(kingdom);
			int num = valueForFaction + valueForFaction2;
			int num2 = 0;
			if (valueForFaction < 0)
			{
				num2 = -valueForFaction;
			}
			if (num > 0 && (float)num2 <= (float)kingdom.Leader.Gold * 0.5f)
			{
				clan1.Leader.GetRelation(clan1.MapFaction.Leader);
				clan1.Leader.GetRelation(kingdom.Leader);
				Campaign.Current.BarterManager.ExecuteAiBarter(clan1, kingdom, clan1.Leader, kingdom.Leader, joinKingdomAsClanBarterable);
			}
		}

		// Token: 0x06003CFE RID: 15614 RVA: 0x001223B8 File Offset: 0x001205B8
		private void ConsiderPeace(Clan clan1, Clan clan2)
		{
			PeaceBarterable peaceBarterable = new PeaceBarterable(clan1.Leader, clan1.MapFaction, clan2.MapFaction, CampaignTime.Years(1f));
			if (peaceBarterable.GetValueForFaction(clan1) + peaceBarterable.GetValueForFaction(clan2) > 0)
			{
				Campaign.Current.BarterManager.ExecuteAiBarter(clan1, clan2, clan1.Leader, clan2.Leader, peaceBarterable);
			}
		}

		// Token: 0x06003CFF RID: 15615 RVA: 0x00122418 File Offset: 0x00120618
		private void ConsiderWar(Clan clan, IFaction otherMapFaction)
		{
			DeclareWarBarterable declareWarBarterable = new DeclareWarBarterable(clan, otherMapFaction);
			if (declareWarBarterable.GetValueForFaction(clan) > 1000)
			{
				declareWarBarterable.Apply();
			}
		}

		// Token: 0x06003D00 RID: 15616 RVA: 0x00122441 File Offset: 0x00120641
		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
