using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	// Token: 0x0200031D RID: 797
	public class RevenueFarmingIssueBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000AD5 RID: 2773
		// (get) Token: 0x06002D54 RID: 11604 RVA: 0x000BD32A File Offset: 0x000BB52A
		private float IncidentChance
		{
			get
			{
				return (100f - RevenueFarmingIssueBehavior.Instance.TargetSettlement.Town.Loyalty * 0.8f) * 0.01f;
			}
		}

		// Token: 0x17000AD6 RID: 2774
		// (get) Token: 0x06002D55 RID: 11605 RVA: 0x000BD354 File Offset: 0x000BB554
		private static RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest Instance
		{
			get
			{
				RevenueFarmingIssueBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<RevenueFarmingIssueBehavior>();
				if (campaignBehavior._cachedQuest != null && campaignBehavior._cachedQuest.IsOngoing)
				{
					return campaignBehavior._cachedQuest;
				}
				using (List<QuestBase>.Enumerator enumerator = Campaign.Current.QuestManager.Quests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest revenueFarmingIssueQuest;
						if ((revenueFarmingIssueQuest = enumerator.Current as RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest) != null)
						{
							campaignBehavior._cachedQuest = revenueFarmingIssueQuest;
							return campaignBehavior._cachedQuest;
						}
					}
				}
				return null;
			}
		}

		// Token: 0x06002D56 RID: 11606 RVA: 0x000BD3EC File Offset: 0x000BB5EC
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunchedEvent));
		}

		// Token: 0x06002D57 RID: 11607 RVA: 0x000BD440 File Offset: 0x000BB640
		private void OnAfterSessionLaunchedEvent(CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenuOption("town_guard", "talk_to_steward_for_revenue_town", "{=voXpzZdH}Hand over the revenue", new GameMenuOption.OnConditionDelegate(this.talk_to_steward_on_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_steward_on_consequence), false, 2, false, null);
			gameStarter.AddGameMenuOption("town", "talk_to_steward_for_revenue_town", "{=voXpzZdH}Hand over the revenue", new GameMenuOption.OnConditionDelegate(this.talk_to_steward_on_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_steward_on_consequence), false, 9, false, null);
			gameStarter.AddGameMenuOption("castle_guard", "talk_to_steward_for_revenue_castle", "{=voXpzZdH}Hand over the revenue", new GameMenuOption.OnConditionDelegate(this.talk_to_steward_on_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_steward_on_consequence), false, 2, false, null);
		}

		// Token: 0x06002D58 RID: 11608 RVA: 0x000BD4E4 File Offset: 0x000BB6E4
		private void OnSessionLaunched(CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenuOption("village", "revenue_farming_quest_collect_tax_menu_button", "{=mcrjFxDQ}Collect revenue", new GameMenuOption.OnConditionDelegate(this.collect_revenue_menu_condition), new GameMenuOption.OnConsequenceDelegate(this.collect_revenue_menu_consequence), false, 5, false, null);
			gameStarter.AddWaitGameMenu("village_collect_revenue", "{=p6swAFWn}Your men started collecting the revenues...", new OnInitDelegate(this.collecting_menu_on_init), null, null, new OnTickDelegate(this.collection_menu_on_tick), GameMenu.MenuAndOptionType.WaitMenuShowOnlyProgressOption, GameOverlays.MenuOverlayType.None, 10f, GameMenu.MenuFlags.None, null);
			gameStarter.AddGameMenuOption("village_collect_revenue", "village_collect_revenue_back", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(this.leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.leave_consequence), true, -1, false, null);
			this.AddVillageEvents(gameStarter);
		}

		// Token: 0x06002D59 RID: 11609 RVA: 0x000BD590 File Offset: 0x000BB790
		private bool talk_to_steward_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Manage;
			args.OptionQuestData = GameMenuOption.IssueQuestFlags.ActiveIssue;
			if (RevenueFarmingIssueBehavior.Instance != null)
			{
				if (Hero.MainHero.Gold < RevenueFarmingIssueBehavior.Instance._totalRequestedDenars)
				{
					args.IsEnabled = false;
					args.Tooltip = new TextObject("{=QOWyEJrm}You don't have enough denars.", null);
				}
				if (!RevenueFarmingIssueBehavior.Instance._allRevenuesAreCollected)
				{
					args.IsEnabled = false;
					args.Tooltip = new TextObject("{=QrAowQ5f}You have to collect the revenues first.", null);
				}
				return Settlement.CurrentSettlement.OwnerClan == RevenueFarmingIssueBehavior.Instance.QuestGiver.Clan;
			}
			return false;
		}

		// Token: 0x06002D5A RID: 11610 RVA: 0x000BD622 File Offset: 0x000BB822
		private void talk_to_steward_on_consequence(MenuCallbackArgs args)
		{
			RevenueFarmingIssueBehavior.Instance.RevenuesAreDeliveredToSteward();
			if (Settlement.CurrentSettlement.IsCastle)
			{
				GameMenu.SwitchToMenu("castle");
				return;
			}
			GameMenu.SwitchToMenu("town");
		}

		// Token: 0x06002D5B RID: 11611 RVA: 0x000BD650 File Offset: 0x000BB850
		private void AddVillageEvents(CampaignGameStarter gameStarter)
		{
			this._villageEvents = new List<RevenueFarmingIssueBehavior.VillageEvent>();
			string text = "{=RabC7Wzm}The headman tells you that most of the villagers can't afford the rest of the tax. They offer crops and other goods as payment in kind.";
			TextObject textObject = new TextObject("{=5hgc03yZ}While your men were collecting revenues, a headman came and told you that most of the villagers couldn't afford to pay what they owe. They offered to pay the rest with their products.", null);
			List<RevenueFarmingIssueBehavior.VillageEventOptionData> list = new List<RevenueFarmingIssueBehavior.VillageEventOptionData>();
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=XVzQ7MXQ}Refuse the offer, break into their homes and collect all rents and taxes by force.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 10)
				{
					args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
					args.IsEnabled = false;
				}
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				TraitLevelingHelper.OnIssueSolvedThroughQuest(RevenueFarmingIssueBehavior.Instance.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Mercy, -50)
				});
				int num = MBRandom.RandomInt(2, 4);
				TextObject textObject2 = new TextObject("{=3vFxRKja}You refused his offer and decided to collect the rest of the revenues by force. Your action upset the village notables and made villagers angry. Some villagers tried to resist. In the brawl, {WOUNDED_COUNT} of your men got wounded.", null);
				textObject2.SetTextVariable("WOUNDED_COUNT", num);
				RevenueFarmingIssueBehavior.Instance.AddLog(textObject2, false);
				this.ChangeRelationWithNotables(-5);
				int num2 = MBRandom.RandomInt(2, 4);
				MobileParty.MainParty.MemberRoster.WoundNumberOfTroopsRandomly(num2);
				TextObject textObject3 = new TextObject("{=o27lTMD4}Some villagers tried to resist. In the brawl, {WOUNDED_NUMBER} of your men got wounded.", null);
				textObject3.SetTextVariable("WOUNDED_NUMBER", num2);
				MBInformationManager.AddQuickInformation(textObject3, 0, null, "");
				this.collect_revenue_menu_consequence(args);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=buKXELE3}Accept the offer.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Trade;
				RevenueFarmingIssueBehavior.RevenueVillage revenueVillage = RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage();
				int num3 = (int)((float)revenueVillage.TargetAmount * 0.5f / (float)revenueVillage.Village.VillageType.PrimaryProduction.Value);
				TextObject textObject4 = new TextObject("{=wZfbYfoH}They will give you {PRODUCT_COUNT} {.%}{?(PRODUCT_COUNT > 1)}{PLURAL(PRODUCT)}{?}{PRODUCT}{\\?}{.%}.", null);
				textObject4.SetTextVariable("PRODUCT", revenueVillage.Village.VillageType.PrimaryProduction.Name);
				textObject4.SetTextVariable("PRODUCT_COUNT", num3);
				args.Tooltip = textObject4;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				int num4;
				this.GiveVillageGoods(out num4);
				TextObject textObject5 = new TextObject("{=b5InObbq}You accepted the headman's offer. The village's notables and villagers were happy with your decision and they gave you {PRODUCT_COUNT} {.%}{?(PRODUCT_COUNT > 1)}{PLURAL(PRODUCT)}{?}{PRODUCT}{\\?}{.%}.", null);
				textObject5.SetTextVariable("PRODUCT", RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage().Village.VillageType.PrimaryProduction.Name);
				textObject5.SetTextVariable("PRODUCT_COUNT", num4);
				RevenueFarmingIssueBehavior.Instance.AddLog(textObject5, false);
				this.ChangeRelationWithNotables(1);
				this.CompleteCurrentRevenueCollection(true);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=jULnw6F1}Leave the village, telling the villagers that they are exempted from payment this year.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=a3WpsFTM}You decided to exempt the rest of the villagers from payment and left the village. The village's notables and farmers were grateful to you.", null), false);
				this.ChangeRelationWithNotables(3);
				this.CompleteCurrentRevenueCollection(true);
			}, false));
			this._villageEvents.Add(new RevenueFarmingIssueBehavior.VillageEvent("offer_goods_and_troops", text, textObject, list));
			text = "{=tVYLzFwu}Suddenly a brawl starts between your troops and some of the village youth.";
			textObject = new TextObject("{=vKaeKPJ5}Revenue collection was interrupted by a sudden brawl between your troops and young men of the village.", null);
			list = new List<RevenueFarmingIssueBehavior.VillageEventOptionData>();
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=eJegI0iX}Order the rest of your troops to put the village youth to flight.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Mission;
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 10)
				{
					args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
					args.IsEnabled = false;
				}
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=Zx1ZEl6Q}You ordered your troops to fight back. In the heat of the brawl, one of the young men was struck on the head and killed. His death greatly upset the villagers.", null), false);
				MBInformationManager.AddQuickInformation(new TextObject("{=xfEVlh7v}Your men beat some of youngsters to the death.", null), 0, null, "");
				this.ChangeRelationWithNotables(-5);
				this.collect_revenue_menu_consequence(args);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=Z6IoX4MH}Order your troops to try not to hurt the youth and try to separate the two sides.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 10)
				{
					args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
					args.IsEnabled = false;
				}
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				int num5 = MBRandom.RandomInt(6, 10);
				TextObject textObject6 = new TextObject("{=YRocrk78}You ordered your troops to disengage. When the dust settled, {WOUNDED} of them had been injured. But the village notables understood that you wanted a peaceful solution.", null);
				textObject6.SetTextVariable("WOUNDED", num5);
				RevenueFarmingIssueBehavior.Instance.AddLog(textObject6, false);
				TextObject textObject7 = new TextObject("{=00Qvwelb}{WOUNDED_NUMBER} of your men got wounded while they were trying to separate the two sides.", null);
				textObject7.SetTextVariable("WOUNDED_NUMBER", num5);
				MBInformationManager.AddQuickInformation(textObject7, 0, null, "");
				MobileParty.MainParty.MemberRoster.WoundNumberOfTroopsRandomly(num5);
				this.ChangeRelationWithNotables(2);
				this.collect_revenue_menu_consequence(args);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=Xl5JTBJE}Leave the village, telling them you've collected enough.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Leave;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=T0feOigD}You decided to stop collecting revenues and leave the village. You told the villagers that they had paid enough, and they were grateful to you.", null), false);
				this.ChangeRelationWithNotables(4);
				this.CompleteCurrentRevenueCollection(true);
			}, true));
			this._villageEvents.Add(new RevenueFarmingIssueBehavior.VillageEvent("brawl_breaks_out", text, textObject, list));
			text = "{=cOlZvnal}A landlord says that his retainers, who help keep order in the village, have gone unpaid and are starting to get mutinous. He says that if you can't help him out with a small sum of money to pay them while you collect the revenues from the rest of the village, he can't guarantee that things will go peacefully.";
			textObject = new TextObject("{=HK4pwetq}A few hours after the revenue collection started, a landlord came and told you that his retainers were getting mutinuous. He asked you for some money to pay them their back wages.", null);
			list = new List<RevenueFarmingIssueBehavior.VillageEventOptionData>();
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=0p0jXXIa}Reject the landlord's request for money and collect revenues as before.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 5)
				{
					args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
					args.IsEnabled = false;
				}
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				this.ChangeRelationWithNotables(-5);
				if (MBRandom.RandomFloat < this.IncidentChance)
				{
					RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=bS7IAgJS}You told the notable that this was not your affair. A few hours later, the mutineers ambushed and killed some of your men on the outskirts of the village, and the revenues stolen.", null), false);
					GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage().CollectedAmount, true);
					TextObject textObject8 = GameTexts.FindText("str_quest_collect_debt_quest_gold_removed", null);
					textObject8.SetTextVariable("GOLD_AMOUNT", RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage().CollectedAmount);
					InformationManager.DisplayMessage(new InformationMessage(textObject8.ToString(), "event:/ui/notification/coins_negative"));
					this.CompleteCurrentRevenueCollection(false);
					int num6 = MBRandom.RandomInt(2, 5);
					TextObject textObject9 = new TextObject("{=mosHZG3b}The mutineers ambushed and killed {KILLED_NUMBER} of your men.", null);
					textObject9.SetTextVariable("KILLED_NUMBER", num6);
					MBInformationManager.AddQuickInformation(textObject9, 0, null, "");
					MobileParty.MainParty.MemberRoster.KillNumberOfMenRandomly(num6, false);
					return;
				}
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=KQ8AU8Bz}You told the notable that this was not your affair. He did not like to hear this.", null), false);
				this.collect_revenue_menu_consequence(args);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=EmJDw5xP}Give the landlord a small bribe for his men, and continue to collect revenues with their help.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Trade;
				int num7 = RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage().TargetAmount / 3;
				if (Hero.MainHero.Gold < num7)
				{
					args.Tooltip = new TextObject("{=m6uSOtE4}You don't have enough money.", null);
					args.IsEnabled = false;
				}
				else
				{
					TextObject textObject10 = new TextObject("{=hCavIm4G}You will pay {AMOUNT}{GOLD_ICON} denars.", null);
					textObject10.SetTextVariable("AMOUNT", num7);
					args.Tooltip = textObject10;
				}
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=kp19y5Hh}You paid off the landlords' retainers to forestall a mutiny. The village notables were grateful to you.", null), false);
				GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage().TargetAmount / 3, false);
				this.ChangeRelationWithNotables(2);
				this.collect_revenue_menu_consequence(args);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=DhrjR8bs}Announce that the villagers have paid enough, and leave the village.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Leave;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=1yCeyK4I}You declared that the village had paid enough, and departed.", null), false);
				this.ChangeRelationWithNotables(4);
				this.CompleteCurrentRevenueCollection(true);
			}, true));
			this._villageEvents.Add(new RevenueFarmingIssueBehavior.VillageEvent("landlord_asks_for_money", text, textObject, list));
			text = "{=pBII35Fa}As your man were collecting the tax, the headman shouted out to you across the fields that there has been an outbreak of the flux in the village. He warns you, for your own good, to stay away.";
			textObject = new TextObject("{=fn59IIUf}As your man were collecting the tax, the headman shouted out to you that the village had seen an outbreak of the flux, and that you should stay away.", null);
			list = new List<RevenueFarmingIssueBehavior.VillageEventOptionData>();
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=CbapENnw}Tell your men that the headman is probably lying, and to go ahead and collect the revenues.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 5)
				{
					args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
					args.IsEnabled = false;
				}
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				if (MBRandom.RandomFloat < this.IncidentChance)
				{
					RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=9AyDNhQj}You told your men to ignore the warning. Several hours after you left, some of your men started experiencing chills, then diarrhea. This does not appear to be a particularly virulent strain, as no one died, but about half your men are incapacitated.", null), false);
					int num8 = MobileParty.MainParty.MemberRoster.TotalHealthyCount / 2;
					TextObject textObject11 = new TextObject("{=rmmZayCT}{WOUNDED_COUNT} of your men got wounded because of illness.", null);
					textObject11.SetTextVariable("WOUNDED_COUNT", num8);
					MBInformationManager.AddQuickInformation(textObject11, 0, null, "");
					MobileParty.MainParty.MemberRoster.WoundNumberOfTroopsRandomly(num8);
				}
				else
				{
					RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=obhcbsQT}You told your men to ignore the warning.", null), false);
				}
				this.ChangeRelationWithNotables(-4);
				this.collect_revenue_menu_consequence(args);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=iE5vWYj2}Tell your men to be careful, and to touch nothing in a house where anyone has been sick.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				RevenueFarmingIssueBehavior.RevenueVillage revenueVillage2 = RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage();
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=b3GrvocA}You told your men to go carefully, but still collect the revenues. The village notables seemed upset with your decision.", null), false);
				revenueVillage2.SetAdditionalProgress(0.35f);
				this.ChangeRelationWithNotables(2);
				this.collect_revenue_menu_consequence(args);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=YZZ4zjxU}Tell the villagers that, in light of their hardship, they are forgiven what they owe.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Leave;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=JSI0FVZ1}You decided to forgive the villagers' back payments. The headman thanked you, as the villagers were already suffering.", null), false);
				this.ChangeRelationWithNotables(4);
				this.CompleteCurrentRevenueCollection(true);
			}, true));
			this._villageEvents.Add(new RevenueFarmingIssueBehavior.VillageEvent("village_is_under_quarantine", text, textObject, list));
			text = "{=yPkHn74X}When you enter the village commons, you find a crowd of villagers has gathered to resist you. They call the rents and taxes owed 'unlawful' and refuse to pay them at all. They pelt your men with rotten vegetables.";
			textObject = new TextObject("{=yPkHn74X}When you enter the village commons, you find a crowd of villagers has gathered to resist you. They call the rents and taxes owed 'unlawful' and refuse to pay them at all. They pelt your men with rotten vegetables.", null);
			list = new List<RevenueFarmingIssueBehavior.VillageEventOptionData>();
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=aZ9bME9C}Order your men to break up the crowd by force", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount <= 9)
				{
					args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
					args.IsEnabled = false;
				}
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				if (MBRandom.RandomFloat < this.IncidentChance)
				{
					RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=ztY2Nf0N}You ordered your men to break up the crowd. There was some scuffling, and some of your men were wounded.", null), false);
					int num9 = MBRandom.RandomInt(6, 8);
					TextObject textObject12 = new TextObject("{=xJwo7eBh}{WOUNDED_NUMBER} of your men got wounded while they were breaking up the crowd.", null);
					textObject12.SetTextVariable("WOUNDED_NUMBER", num9);
					MBInformationManager.AddQuickInformation(textObject12, 0, null, "");
					MobileParty.MainParty.MemberRoster.WoundNumberOfTroopsRandomly(num9);
				}
				else
				{
					RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=ObYvBt0e}You ordered your men to break up the crowd. The attempt was successful and your men continued collecting taxes as usual.", null), false);
				}
				this.ChangeRelationWithNotables(-5);
				this.collect_revenue_menu_consequence(args);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=4MPhLYcT}Bargain with the group, agreeing to forgive the debts of the poorest villagers", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.DefendAction;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage().SetAdditionalProgress(0.5f);
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=54RyKzPJ}After your attempts to bargain, a deal has been made to forgive the debts of the poorest villagers.", null), false);
				this.ChangeRelationWithNotables(2);
				this.collect_revenue_menu_consequence(args);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=tZw45isr}Tell the villagers that they made their point and that you're leaving", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Leave;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=6TYsIQav}After observing the villagers' hardships, you called back your men so as not put any more burden on them.", null), false);
				this.ChangeRelationWithNotables(4);
				this.CompleteCurrentRevenueCollection(true);
			}, true));
			this._villageEvents.Add(new RevenueFarmingIssueBehavior.VillageEvent("refuse_to_pay_what_they_owe", text, textObject, list));
			text = "{=Tl4yagLi}The headman tells you that some households have suffered particularly hard this year from crop failures and bandit depredations. He asks you to forgive their back payments entirely. He hints that they are so desperate that they might resist by force.";
			textObject = new TextObject("{=Tl4yagLi}The headman tells you that some households have suffered particularly hard this year from crop failures and bandit depredations. He asks you to forgive their back payments entirely. He hints that they are so desperate that they might resist by force.", null);
			list = new List<RevenueFarmingIssueBehavior.VillageEventOptionData>();
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=agMtRiru}Refuse to exempt anyone", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 5)
				{
					args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
					args.IsEnabled = false;
				}
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				if (MBRandom.RandomFloat < this.IncidentChance)
				{
					RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=VsriS0iI}You refused to exempt anyone, but the residents attacked and killed some of your troops who were separated from the rest, and then ran away.", null), false);
					int num10 = MBRandom.RandomInt(2, 4);
					TextObject textObject13 = new TextObject("{=MGD8Ka2o}The residents attacked and killed {KILLED_NUMBER} of your troops who were separated from the rest.", null);
					textObject13.SetTextVariable("KILLED_NUMBER", num10);
					MBInformationManager.AddQuickInformation(textObject13, 0, null, "");
					MobileParty.MainParty.MemberRoster.KillNumberOfMenRandomly(num10, false);
				}
				else
				{
					RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=Rz1kkvbK}You refused to exempt anyone. Fortunately the villagers were sufficiently cowed by your men, and did not raise a hand.", null), false);
				}
				this.ChangeRelationWithNotables(-5);
				this.collect_revenue_menu_consequence(args);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=WDp5EAl3}Agree to exempt the poor households", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage().SetAdditionalProgress(0.35f);
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=o70h6xqb}You showed mercy and exempted the poor households from the tax collection", null), false);
				this.ChangeRelationWithNotables(2);
				this.collect_revenue_menu_consequence(args);
			}, false));
			list.Add(new RevenueFarmingIssueBehavior.VillageEventOptionData("{=aMleZjlG}Tell the villagers that they have all paid enough, and depart", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Leave;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				RevenueFarmingIssueBehavior.Instance.AddLog(new TextObject("{=rsQhD7sC}You thought that the villagers have paid enough, so departed from the settlement", null), false);
				this.ChangeRelationWithNotables(4);
				this.CompleteCurrentRevenueCollection(true);
			}, true));
			this._villageEvents.Add(new RevenueFarmingIssueBehavior.VillageEvent("relief_for_the_poorest", text, textObject, list));
			foreach (RevenueFarmingIssueBehavior.VillageEvent villageEvent in this._villageEvents)
			{
				this.AddVillageEventMenus(villageEvent, gameStarter);
			}
		}

		// Token: 0x06002D5C RID: 11612 RVA: 0x000BDC0C File Offset: 0x000BBE0C
		private void AddVillageEventMenus(RevenueFarmingIssueBehavior.VillageEvent villageEvent, CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenu(villageEvent.Id, villageEvent.MainEventText, null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			for (int i = 0; i < villageEvent.OptionConditionsAndConsequences.Count; i++)
			{
				RevenueFarmingIssueBehavior.VillageEventOptionData villageEventOptionData = villageEvent.OptionConditionsAndConsequences[i];
				gameStarter.AddGameMenuOption(villageEvent.Id, "Id_option" + i, villageEventOptionData.Text, villageEventOptionData.OnCondition, villageEventOptionData.OnConsequence, villageEventOptionData.IsLeave, -1, false, null);
			}
		}

		// Token: 0x06002D5D RID: 11613 RVA: 0x000BDC8C File Offset: 0x000BBE8C
		private bool collect_revenue_menu_condition(MenuCallbackArgs args)
		{
			if (RevenueFarmingIssueBehavior.Instance == null || !RevenueFarmingIssueBehavior.Instance.IsOngoing)
			{
				return false;
			}
			args.optionLeaveType = GameMenuOption.LeaveType.Manage;
			args.OptionQuestData = GameMenuOption.IssueQuestFlags.ActiveIssue;
			RevenueFarmingIssueBehavior.RevenueVillage revenueVillage = RevenueFarmingIssueBehavior.Instance.RevenueVillages.FirstOrDefault((RevenueFarmingIssueBehavior.RevenueVillage x) => x.Village == Settlement.CurrentSettlement.Village);
			if (revenueVillage != null && !revenueVillage.GetIsCompleted())
			{
				bool flag = MobileParty.MainParty.MemberRoster.TotalHealthyCount >= 20;
				TextObject textObject = new TextObject("{=CfCsGTfb}Villagers are not taking you seriously, as you do not have enough soldiers to carry out the process. At least 20 men are needed to continue.", null);
				return MenuHelper.SetOptionProperties(args, flag, !flag, textObject);
			}
			return false;
		}

		// Token: 0x06002D5E RID: 11614 RVA: 0x000BDD2C File Offset: 0x000BBF2C
		private void collecting_menu_on_init(MenuCallbackArgs args)
		{
			if (RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage().CollectedAmount == 0)
			{
				TextObject textObject = new TextObject("{=VktwHCN6}Your men have started to collect the tax of {VILLAGE}", null);
				textObject.SetTextVariable("VILLAGE", Settlement.CurrentSettlement.Name);
				MBInformationManager.AddQuickInformation(textObject, 0, null, "");
			}
			RevenueFarmingIssueBehavior.Instance.CollectingRevenues = true;
			args.MenuContext.GameMenu.StartWait();
		}

		// Token: 0x06002D5F RID: 11615 RVA: 0x000BDD92 File Offset: 0x000BBF92
		private bool leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x06002D60 RID: 11616 RVA: 0x000BDD9D File Offset: 0x000BBF9D
		private void leave_consequence(MenuCallbackArgs args)
		{
			RevenueFarmingIssueBehavior.Instance.CollectingRevenues = false;
			GameMenu.SwitchToMenu("village");
		}

		// Token: 0x06002D61 RID: 11617 RVA: 0x000BDDB4 File Offset: 0x000BBFB4
		private void collection_menu_on_tick(MenuCallbackArgs args, CampaignTime dt)
		{
			RevenueFarmingIssueBehavior.RevenueVillage revenueVillage = RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage();
			args.MenuContext.GameMenu.SetProgressOfWaitingInMenu(revenueVillage.CollectProgress);
		}

		// Token: 0x06002D62 RID: 11618 RVA: 0x000BDDE2 File Offset: 0x000BBFE2
		private void collect_revenue_menu_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("village_collect_revenue");
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppablePlay;
		}

		// Token: 0x06002D63 RID: 11619 RVA: 0x000BDDF9 File Offset: 0x000BBFF9
		[GameMenuInitializationHandler("village_collect_revenue")]
		private static void village_collect_revenue_game_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.SettlementComponent.WaitMeshName);
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppablePlay;
		}

		// Token: 0x06002D64 RID: 11620 RVA: 0x000BDE20 File Offset: 0x000BC020
		[GameMenuInitializationHandler("offer_goods_and_troops")]
		[GameMenuInitializationHandler("brawl_breaks_out")]
		[GameMenuInitializationHandler("landlord_asks_for_money")]
		[GameMenuInitializationHandler("village_is_under_quarantine")]
		[GameMenuInitializationHandler("refuse_to_pay_what_they_owe")]
		[GameMenuInitializationHandler("relief_for_the_poorest")]
		private static void village_event_common_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.SettlementComponent.WaitMeshName);
		}

		// Token: 0x06002D65 RID: 11621 RVA: 0x000BDE3C File Offset: 0x000BC03C
		private void ChangeRelationWithNotables(int relation)
		{
			foreach (Hero hero in Settlement.CurrentSettlement.Notables)
			{
				hero.SetHasMet();
				ChangeRelationAction.ApplyPlayerRelation(hero, relation, false, false);
			}
			TextObject textObject = TextObject.Empty;
			if (relation > 0)
			{
				textObject = new TextObject("{=IwS1qeq9}Your relation is increased by {MAGNITUDE} with village notables.", null);
			}
			else
			{
				textObject = new TextObject("{=r5Netxy0}Your relation is decreased by {MAGNITUDE} with village notables.", null);
			}
			textObject.SetTextVariable("MAGNITUDE", relation);
			MBInformationManager.AddQuickInformation(textObject, 0, null, "");
		}

		// Token: 0x06002D66 RID: 11622 RVA: 0x000BDED8 File Offset: 0x000BC0D8
		private void CompleteCurrentRevenueCollection(bool addLog = true)
		{
			RevenueFarmingIssueBehavior.RevenueVillage revenueVillage = RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage();
			RevenueFarmingIssueBehavior.Instance.SetVillageAsCompleted(revenueVillage, addLog);
			if (RevenueFarmingIssueBehavior.Instance.IsTracked(revenueVillage.Village.Settlement))
			{
				RevenueFarmingIssueBehavior.Instance.RemoveTrackedObject(revenueVillage.Village.Settlement);
			}
			RevenueFarmingIssueBehavior.Instance.CollectingRevenues = false;
			PlayerEncounter.Finish(true);
		}

		// Token: 0x06002D67 RID: 11623 RVA: 0x000BDF3C File Offset: 0x000BC13C
		private void GiveVillageGoods(out int amount)
		{
			RevenueFarmingIssueBehavior.RevenueVillage revenueVillage = RevenueFarmingIssueBehavior.Instance.FindCurrentRevenueVillage();
			amount = (int)((float)revenueVillage.TargetAmount * 0.5f / (float)revenueVillage.Village.VillageType.PrimaryProduction.Value);
			MobileParty.MainParty.ItemRoster.AddToCounts(revenueVillage.Village.VillageType.PrimaryProduction, amount);
		}

		// Token: 0x06002D68 RID: 11624 RVA: 0x000BDFA0 File Offset: 0x000BC1A0
		public void OnVillageEventWithIdSpawned(string Id)
		{
			RevenueFarmingIssueBehavior.VillageEvent villageEvent = this._villageEvents.FirstOrDefault((RevenueFarmingIssueBehavior.VillageEvent x) => x.Id == Id);
			RevenueFarmingIssueBehavior.Instance.AddLog(villageEvent.MainLog, false);
		}

		// Token: 0x06002D69 RID: 11625 RVA: 0x000BDFE4 File Offset: 0x000BC1E4
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06002D6A RID: 11626 RVA: 0x000BDFE8 File Offset: 0x000BC1E8
		private bool ConditionsHold(Hero issueGiver, out Settlement targetSettlement)
		{
			targetSettlement = null;
			if (issueGiver.IsLord && issueGiver.Clan.Leader == issueGiver && issueGiver.Clan.Settlements.Count > 0)
			{
				targetSettlement = issueGiver.Clan.Settlements.Where((Settlement x) => x.IsTown).GetRandomElementInefficiently<Settlement>();
			}
			return targetSettlement != null;
		}

		// Token: 0x06002D6B RID: 11627 RVA: 0x000BE05C File Offset: 0x000BC25C
		public void OnCheckForIssue(Hero hero)
		{
			Settlement settlement;
			if (this.ConditionsHold(hero, out settlement))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnSelected), typeof(RevenueFarmingIssueBehavior.RevenueFarmingIssue), IssueBase.IssueFrequency.VeryCommon, settlement));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(RevenueFarmingIssueBehavior.RevenueFarmingIssue), IssueBase.IssueFrequency.VeryCommon));
		}

		// Token: 0x06002D6C RID: 11628 RVA: 0x000BE0C4 File Offset: 0x000BC2C4
		private IssueBase OnSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			PotentialIssueData potentialIssueData = pid;
			return new RevenueFarmingIssueBehavior.RevenueFarmingIssue(issueOwner, potentialIssueData.RelatedObject as Settlement);
		}

		// Token: 0x04000DA9 RID: 3497
		private const int CollectAllVillageTaxesAfterHours = 10;

		// Token: 0x04000DAA RID: 3498
		private const IssueBase.IssueFrequency RevenueFarmingIssueFrequency = IssueBase.IssueFrequency.VeryCommon;

		// Token: 0x04000DAB RID: 3499
		private List<RevenueFarmingIssueBehavior.VillageEvent> _villageEvents;

		// Token: 0x04000DAC RID: 3500
		private RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest _cachedQuest;

		// Token: 0x0200065E RID: 1630
		public class RevenueFarmingIssue : IssueBase
		{
			// Token: 0x0600504F RID: 20559 RVA: 0x00162E1D File Offset: 0x0016101D
			internal static void AutoGeneratedStaticCollectObjectsRevenueFarmingIssue(object o, List<object> collectedObjects)
			{
				((RevenueFarmingIssueBehavior.RevenueFarmingIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06005050 RID: 20560 RVA: 0x00162E2B File Offset: 0x0016102B
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._targetSettlement);
			}

			// Token: 0x06005051 RID: 20561 RVA: 0x00162E40 File Offset: 0x00161040
			internal static object AutoGeneratedGetMemberValue_targetSettlement(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueFarmingIssue)o)._targetSettlement;
			}

			// Token: 0x17001150 RID: 4432
			// (get) Token: 0x06005052 RID: 20562 RVA: 0x00162E4D File Offset: 0x0016104D
			protected override int RewardGold
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x17001151 RID: 4433
			// (get) Token: 0x06005053 RID: 20563 RVA: 0x00162E50 File Offset: 0x00161050
			protected int TotalRequestedDenars
			{
				get
				{
					int num = 0;
					foreach (Village village in this._targetSettlement.BoundVillages)
					{
						if (!village.Settlement.IsRaided && !village.Settlement.IsUnderRaid)
						{
							num += (int)(village.Hearth * 4f);
						}
					}
					return num / 3;
				}
			}

			// Token: 0x17001152 RID: 4434
			// (get) Token: 0x06005054 RID: 20564 RVA: 0x00162ED0 File Offset: 0x001610D0
			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=j5fS9zaa}Yes, there is something. I have been on campaign for much of this year, and I have not been able to go around to my estates collecting the rents that are owed to me and the taxes that are owed to the realm. I need some help collecting these revenues.", null);
				}
			}

			// Token: 0x17001153 RID: 4435
			// (get) Token: 0x06005055 RID: 20565 RVA: 0x00162EDD File Offset: 0x001610DD
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=AXy26AFb}Maybe I can help. What are the terms of agreement.", null);
				}
			}

			// Token: 0x17001154 RID: 4436
			// (get) Token: 0x06005056 RID: 20566 RVA: 0x00162EEA File Offset: 0x001610EA
			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=F540oIed}I can designate you as my official revenue farmer, and give you a list of everyone's holdings and how much they owe. All you need to do is visit all my villages and collect what you can. I don't expect you to be able to get every denar. Some of the people around here are genuinely hard - up, but they'll all try to get out of paying. Let's just keep it simple: I will take {TOTAL_REQUESTED_DENARS}{GOLD_ICON} denars and you can keep whatever else you can squeeze out of them. Are you interested?", null);
					textObject.SetTextVariable("TOTAL_REQUESTED_DENARS", this.TotalRequestedDenars);
					return textObject;
				}
			}

			// Token: 0x17001155 RID: 4437
			// (get) Token: 0x06005057 RID: 20567 RVA: 0x00162F09 File Offset: 0x00161109
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=dAmK7rKG}All right. I will visit your villages and collect your rent.", null);
				}
			}

			// Token: 0x17001156 RID: 4438
			// (get) Token: 0x06005058 RID: 20568 RVA: 0x00162F16 File Offset: 0x00161116
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17001157 RID: 4439
			// (get) Token: 0x06005059 RID: 20569 RVA: 0x00162F19 File Offset: 0x00161119
			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17001158 RID: 4440
			// (get) Token: 0x0600505A RID: 20570 RVA: 0x00162F1C File Offset: 0x0016111C
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=zqrn2beP}Revenue Farming", null);
				}
			}

			// Token: 0x17001159 RID: 4441
			// (get) Token: 0x0600505B RID: 20571 RVA: 0x00162F29 File Offset: 0x00161129
			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=U8izV2lM}A {?ISSUE_GIVER.GENDER}lady{?}lord{\\?} is looking for someone to collect back rents that {?ISSUE_GIVER.GENDER}she{?}he{\\?} says are owed to {?ISSUE_GIVER.GENDER}her{?}him{\\?}.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, null, false);
					return textObject;
				}
			}

			// Token: 0x0600505C RID: 20572 RVA: 0x00162F4E File Offset: 0x0016114E
			public RevenueFarmingIssue(Hero issueOwner, Settlement targetSettlement)
				: base(issueOwner, CampaignTime.DaysFromNow(20f))
			{
				this._targetSettlement = targetSettlement;
			}

			// Token: 0x0600505D RID: 20573 RVA: 0x00162F68 File Offset: 0x00161168
			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.ClanInfluence)
				{
					return -0.2f;
				}
				return 0f;
			}

			// Token: 0x0600505E RID: 20574 RVA: 0x00162F7D File Offset: 0x0016117D
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.VeryCommon;
			}

			// Token: 0x0600505F RID: 20575 RVA: 0x00162F80 File Offset: 0x00161180
			public override bool IssueStayAliveConditions()
			{
				if (this._targetSettlement.OwnerClan == base.IssueOwner.Clan)
				{
					return this._targetSettlement.BoundVillages.Any((Village x) => !x.Settlement.IsRaided && !x.Settlement.IsUnderRaid);
				}
				return false;
			}

			// Token: 0x06005060 RID: 20576 RVA: 0x00162FD8 File Offset: 0x001611D8
			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flags, out Hero relationHero, out SkillObject skill)
			{
				flags = IssueBase.PreconditionFlags.None;
				relationHero = null;
				skill = null;
				if (issueGiver.GetRelationWithPlayer() < -10f)
				{
					flags |= IssueBase.PreconditionFlags.Relation;
					relationHero = issueGiver;
				}
				if (FactionManager.IsAtWarAgainstFaction(issueGiver.MapFaction, Hero.MainHero.MapFaction))
				{
					flags |= IssueBase.PreconditionFlags.AtWar;
				}
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 40)
				{
					flags |= IssueBase.PreconditionFlags.NotEnoughTroops;
				}
				return flags == IssueBase.PreconditionFlags.None;
			}

			// Token: 0x06005061 RID: 20577 RVA: 0x00163045 File Offset: 0x00161245
			protected override void OnGameLoad()
			{
			}

			// Token: 0x06005062 RID: 20578 RVA: 0x00163048 File Offset: 0x00161248
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				List<RevenueFarmingIssueBehavior.RevenueVillage> list = new List<RevenueFarmingIssueBehavior.RevenueVillage>();
				foreach (Village village in this._targetSettlement.BoundVillages)
				{
					if (!village.Settlement.IsUnderRaid && !village.Settlement.IsRaided)
					{
						list.Add(new RevenueFarmingIssueBehavior.RevenueVillage(village, (int)(village.Hearth * 4f)));
					}
				}
				return new RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(20f), list);
			}

			// Token: 0x06005063 RID: 20579 RVA: 0x001630EC File Offset: 0x001612EC
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x04001AB6 RID: 6838
			private const int IssueAndQuestDuration = 20;

			// Token: 0x04001AB7 RID: 6839
			private const int MinimumRequiredMenCount = 40;

			// Token: 0x04001AB8 RID: 6840
			[SaveableField(1)]
			private Settlement _targetSettlement;
		}

		// Token: 0x0200065F RID: 1631
		public class RevenueFarmingIssueQuest : QuestBase
		{
			// Token: 0x06005064 RID: 20580 RVA: 0x001630EE File Offset: 0x001612EE
			internal static void AutoGeneratedStaticCollectObjectsRevenueFarmingIssueQuest(object o, List<object> collectedObjects)
			{
				((RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06005065 RID: 20581 RVA: 0x001630FC File Offset: 0x001612FC
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._revenueVillages);
				collectedObjects.Add(this._currentVillageEvents);
				collectedObjects.Add(this._questProgressLog);
			}

			// Token: 0x06005066 RID: 20582 RVA: 0x00163129 File Offset: 0x00161329
			internal static object AutoGeneratedGetMemberValue_totalRequestedDenars(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest)o)._totalRequestedDenars;
			}

			// Token: 0x06005067 RID: 20583 RVA: 0x0016313B File Offset: 0x0016133B
			internal static object AutoGeneratedGetMemberValueCollectingRevenues(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest)o).CollectingRevenues;
			}

			// Token: 0x06005068 RID: 20584 RVA: 0x0016314D File Offset: 0x0016134D
			internal static object AutoGeneratedGetMemberValue_allRevenuesAreCollected(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest)o)._allRevenuesAreCollected;
			}

			// Token: 0x06005069 RID: 20585 RVA: 0x0016315F File Offset: 0x0016135F
			internal static object AutoGeneratedGetMemberValue_revenueVillages(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest)o)._revenueVillages;
			}

			// Token: 0x0600506A RID: 20586 RVA: 0x0016316C File Offset: 0x0016136C
			internal static object AutoGeneratedGetMemberValue_currentVillageEvents(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest)o)._currentVillageEvents;
			}

			// Token: 0x0600506B RID: 20587 RVA: 0x00163179 File Offset: 0x00161379
			internal static object AutoGeneratedGetMemberValue_questProgressLog(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest)o)._questProgressLog;
			}

			// Token: 0x1700115A RID: 4442
			// (get) Token: 0x0600506C RID: 20588 RVA: 0x00163186 File Offset: 0x00161386
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=zqrn2beP}Revenue Farming", null);
				}
			}

			// Token: 0x1700115B RID: 4443
			// (get) Token: 0x0600506D RID: 20589 RVA: 0x00163193 File Offset: 0x00161393
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x1700115C RID: 4444
			// (get) Token: 0x0600506E RID: 20590 RVA: 0x00163196 File Offset: 0x00161396
			public List<RevenueFarmingIssueBehavior.RevenueVillage> RevenueVillages
			{
				get
				{
					return this._revenueVillages;
				}
			}

			// Token: 0x1700115D RID: 4445
			// (get) Token: 0x0600506F RID: 20591 RVA: 0x0016319E File Offset: 0x0016139E
			// (set) Token: 0x06005070 RID: 20592 RVA: 0x001631A6 File Offset: 0x001613A6
			public Settlement TargetSettlement { get; private set; }

			// Token: 0x1700115E RID: 4446
			// (get) Token: 0x06005071 RID: 20593 RVA: 0x001631B0 File Offset: 0x001613B0
			private TextObject QuestStartedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=b0WQfzNb}{QUEST_GIVER.LINK} the {?QUEST_GIVER.GENDER}lady{?}lord{\\?} of {TARGET_SETTLEMENT} told you that {?QUEST_GIVER.GENDER}she{?}he{\\?} wanted to grant revenue collection rights to a commander of good reputation who has enough men to suppress any resistance. {?QUEST_GIVER.GENDER}She{?}He{\\?} asked you to visit all the villages that are bound to {TARGET_SETTLEMENT} and collect taxes and rents. You have agreed to collect the revenues after paying {QUEST_GIVER.LINK}'s share, {REQUESTED_DENARS}{GOLD_ICON} denars.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("TARGET_SETTLEMENT", this.TargetSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("REQUESTED_DENARS", this._totalRequestedDenars);
					return textObject;
				}
			}

			// Token: 0x1700115F RID: 4447
			// (get) Token: 0x06005072 RID: 20594 RVA: 0x0016320C File Offset: 0x0016140C
			private TextObject QuestCanceledWarDeclaredLog
			{
				get
				{
					TextObject textObject = new TextObject("{=vW6kBki9}Your clan is now at war with {QUEST_GIVER.LINK}'s realm. Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17001160 RID: 4448
			// (get) Token: 0x06005073 RID: 20595 RVA: 0x00163240 File Offset: 0x00161440
			private TextObject PlayerDeclaredWarQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=bqeWVVEE}Your actions have started a war with {QUEST_GIVER.LINK}'s faction. {?QUEST_GIVER.GENDER}She{?}He{\\?} cancels your agreement and the quest is a failure.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17001161 RID: 4449
			// (get) Token: 0x06005074 RID: 20596 RVA: 0x00163274 File Offset: 0x00161474
			private TextObject QuestSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=CEQhyvzj}You have completed the collection of revenues and paid {QUEST_GIVER.LINK} a fix sum in advance, as agreed.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17001162 RID: 4450
			// (get) Token: 0x06005075 RID: 20597 RVA: 0x001632A8 File Offset: 0x001614A8
			private TextObject QuestBetrayedLog
			{
				get
				{
					TextObject textObject = new TextObject("{=5ky3voFY}You have rejected handing over the revenue to the {QUEST_GIVER.LINK}. The {?QUEST_GIVER.GENDER}lady{?}lord{\\?} is deeply disappointed in you.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17001163 RID: 4451
			// (get) Token: 0x06005076 RID: 20598 RVA: 0x001632DC File Offset: 0x001614DC
			private TextObject QuestFailedWithTimeOutLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=RNdrvZJQ}You have failed to bring the revenues to the {?QUEST_GIVER.GENDER}lady{?}lord{\\?} in time.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17001164 RID: 4452
			// (get) Token: 0x06005077 RID: 20599 RVA: 0x00163310 File Offset: 0x00161510
			private TextObject AllRevenuesAreCollectedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=ywlzjQfN}{QUEST_GIVER.LINK} wants {TOTAL_REQUESTED_DENARS}{GOLD_ICON} that you have collected from {?QUEST_GIVER.GENDER}her{?}his{\\?} fiefs. You can either give the denars to the {?QUEST_GIVER.GENDER}lady{?}lord{\\?} yourself, or hand them over to a steward of the {?QUEST_GIVER.GENDER}lady{?}lord{\\?}, which can be found in the castles and towns that belong to the {?QUEST_GIVER.GENDER}lady{?}lord{\\?}.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("TOTAL_REQUESTED_DENARS", this._totalRequestedDenars);
					return textObject;
				}
			}

			// Token: 0x06005078 RID: 20600 RVA: 0x00163354 File Offset: 0x00161554
			public RevenueFarmingIssueQuest(string questId, Hero giverHero, CampaignTime duration, List<RevenueFarmingIssueBehavior.RevenueVillage> revenueVillages)
				: base(questId, giverHero, duration, 0)
			{
				this._revenueVillages = revenueVillages;
				this.TargetSettlement = this._revenueVillages[0].Village.Bound;
				foreach (RevenueFarmingIssueBehavior.VillageEvent villageEvent in Campaign.Current.GetCampaignBehavior<RevenueFarmingIssueBehavior>()._villageEvents)
				{
					this._currentVillageEvents.Add(villageEvent.Id, false);
				}
				foreach (RevenueFarmingIssueBehavior.RevenueVillage revenueVillage in revenueVillages)
				{
					this._totalRequestedDenars += revenueVillage.TargetAmount / 3;
				}
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			// Token: 0x06005079 RID: 20601 RVA: 0x0016344C File Offset: 0x0016164C
			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				this._questProgressLog = base.AddDiscreteLog(this.QuestStartedLogText, new TextObject("{=bC5aMfG0}Villages", null), 0, this._revenueVillages.Count, null, true);
				foreach (RevenueFarmingIssueBehavior.RevenueVillage revenueVillage in this._revenueVillages)
				{
					base.AddTrackedObject(revenueVillage.Village.Settlement);
				}
			}

			// Token: 0x0600507A RID: 20602 RVA: 0x001634DC File Offset: 0x001616DC
			protected override void SetDialogs()
			{
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=PXigJyMs}Excellent. You are acting in my name now. Try to be polite but you have every right to use force if they don't cough up what's owed. Good luck.", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=tthBNejU}Have you collected the revenues?", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=jQsr4vDO}I'm still working on it.", null), null)
					.NpcLine(new TextObject("{=BI1UnHaB}Good, good. This takes time, I know, but don't keep me waiting too long.", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(MapEventHelper.OnConversationEnd))
					.CloseDialog()
					.PlayerOption(new TextObject("{=ORl6qiOj}Yes, here is your share.", null), null)
					.Condition(() => this._allRevenuesAreCollected)
					.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.TurnQuestInClickableCondition))
					.NpcLine(new TextObject("{=MKYzHFKB}Thank you for your help.", null), null, null)
					.Consequence(delegate
					{
						this.QuestCompletedWithSuccess();
						MapEventHelper.OnConversationEnd();
					})
					.CloseDialog()
					.PlayerOption(new TextObject("{=kj3WQY1V}Maybe I should keep this to myself.", null), null)
					.Condition(() => this._allRevenuesAreCollected)
					.NpcLine(new TextObject("{=82aiVoV9}You will regret this in the long run...", null), null, null)
					.Consequence(delegate
					{
						this.QuestCompletedWithBetray();
						MapEventHelper.OnConversationEnd();
					})
					.CloseDialog()
					.PlayerOption(new TextObject("{=G5tyQj6N}Not yet.", null), null)
					.NpcLine(new TextObject("{=UXCjNTjF}Hurry up. I don't have that much time.", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(MapEventHelper.OnConversationEnd))
					.CloseDialog()
					.EndPlayerOptions();
			}

			// Token: 0x0600507B RID: 20603 RVA: 0x0016368A File Offset: 0x0016188A
			private bool TurnQuestInClickableCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				if (Hero.MainHero.Gold < RevenueFarmingIssueBehavior.Instance._totalRequestedDenars)
				{
					explanation = new TextObject("{=QOWyEJrm}You don't have enough denars.", null);
					return false;
				}
				return true;
			}

			// Token: 0x0600507C RID: 20604 RVA: 0x001636BC File Offset: 0x001618BC
			protected override void OnBeforeTimedOut(ref bool completeWithSuccess, ref bool doNotResolveTheQuest)
			{
				this.RelationshipChangeWithQuestGiver = -5;
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -30)
				});
				if (Hero.MainHero.Gold >= this._totalRequestedDenars)
				{
					this.ShowQuestResolvePopUp();
					doNotResolveTheQuest = true;
				}
			}

			// Token: 0x0600507D RID: 20605 RVA: 0x0016370C File Offset: 0x0016190C
			protected override void OnTimedOut()
			{
				base.AddLog(this.QuestFailedWithTimeOutLogText, false);
			}

			// Token: 0x0600507E RID: 20606 RVA: 0x0016371C File Offset: 0x0016191C
			protected override void RegisterEvents()
			{
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
				CampaignEvents.VillageBeingRaided.AddNonSerializedListener(this, new Action<Village>(this.OnVillageRaid));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
				CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			}

			// Token: 0x0600507F RID: 20607 RVA: 0x001637B4 File Offset: 0x001619B4
			private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
			{
				if (settlement == this.TargetSettlement && oldOwner == base.QuestGiver)
				{
					TextObject textObject = new TextObject("{=1m68Nsze}{QUEST_GIVER.LINK} has lost {SETTLEMENT} and your agreement with {?QUEST_GIVER.GENDER}her{?}him{\\?} has been canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this.TargetSettlement.EncyclopediaLinkWithName);
					base.CompleteQuestWithCancel(textObject);
				}
			}

			// Token: 0x06005080 RID: 20608 RVA: 0x00163816 File Offset: 0x00161A16
			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			// Token: 0x06005081 RID: 20609 RVA: 0x0016382C File Offset: 0x00161A2C
			private void OnVillageRaid(Village village)
			{
				RevenueFarmingIssueBehavior.RevenueVillage revenueVillage = this._revenueVillages.FirstOrDefault((RevenueFarmingIssueBehavior.RevenueVillage x) => x.Village.Id == village.Id);
				if (revenueVillage != null && !revenueVillage.IsRaided)
				{
					TextObject textObject = new TextObject("{=k8U0928J}{VILLAGE} has been raided. {QUEST_GIVER.LINK} asks you to exempt them, but still wants you to collect {AMOUNT}{GOLD_ICON} denars from rest of {?QUEST_GIVER.GENDER}her{?}his{\\?} villages.", null);
					textObject.SetTextVariable("VILLAGE", village.Settlement.EncyclopediaLinkWithName);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					this._totalRequestedDenars -= revenueVillage.TargetAmount / 3;
					textObject.SetTextVariable("AMOUNT", this._totalRequestedDenars);
					revenueVillage.SetDone();
					revenueVillage.IsRaided = true;
					base.AddLog(textObject, false);
					this._questProgressLog.UpdateCurrentProgress(this._questProgressLog.CurrentProgress + 1);
					if (this.CollectingRevenues)
					{
						this.CollectingRevenues = false;
					}
					if (this._revenueVillages.All((RevenueFarmingIssueBehavior.RevenueVillage x) => x.IsRaided))
					{
						TextObject textObject2 = new TextObject("{=44f1ff0q}All the villages of {QUEST_GIVER.LINK} has been raided and your agreement with {?QUEST_GIVER.GENDER}her{?}him{\\?} has been canceled.", null);
						base.CompleteQuestWithCancel(textObject2);
					}
				}
			}

			// Token: 0x06005082 RID: 20610 RVA: 0x00163954 File Offset: 0x00161B54
			private void OnHourlyTick()
			{
				if (base.IsOngoing)
				{
					if (!this._allRevenuesAreCollected)
					{
						if (this._revenueVillages.All((RevenueFarmingIssueBehavior.RevenueVillage x) => x.GetIsCompleted()))
						{
							this.OnAllRevenuesAreCollected();
						}
					}
					if (this.CollectingRevenues)
					{
						this.ProgressRevenueCollectionForVillage();
					}
				}
			}

			// Token: 0x06005083 RID: 20611 RVA: 0x001639B1 File Offset: 0x00161BB1
			private void OnAllRevenuesAreCollected()
			{
				this._allRevenuesAreCollected = true;
				base.AddLog(this.AllRevenuesAreCollectedLogText, false);
			}

			// Token: 0x06005084 RID: 20612 RVA: 0x001639C8 File Offset: 0x00161BC8
			public void RevenuesAreDeliveredToSteward()
			{
				MBInformationManager.AddQuickInformation(new TextObject("{=RCa0DpAo}You have handed over the revenue to the steward", null), 0, null, "");
				this.QuestCompletedWithSuccess();
			}

			// Token: 0x06005085 RID: 20613 RVA: 0x001639E8 File Offset: 0x00161BE8
			private void ShowQuestResolvePopUp()
			{
				TextObject textObject = new TextObject("{=I9GYdYZx}{?QUEST_GIVER.GENDER}Lady{?}Lord{\\?} {QUEST_GIVER.NAME} wants {TOTAL_REQUESTED_DENARS}{GOLD_ICON} denars that you have collected from {?QUEST_GIVER.GENDER}her{?}his{\\?} fiefs. {?QUEST_GIVER.GENDER}She{?}He{\\?} has sent {?QUEST_GIVER.GENDER}her{?}his{\\?} steward to you to collect it. If you refuse this will be counted as a crime and {?QUEST_GIVER.GENDER}her{?}his{\\?} faction may declare war on you.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				textObject.SetTextVariable("TOTAL_REQUESTED_DENARS", this._totalRequestedDenars);
				InformationManager.ShowInquiry(new InquiryData(this.Title.ToString(), textObject.ToString(), true, true, new TextObject("{=plZVwdlL}Send the revenue", null).ToString(), new TextObject("{=asa9HaIQ}Keep the revenue", null).ToString(), new Action(this.QuestCompletedWithSuccess), new Action(this.QuestCompletedWithBetray), "", 0f, null, null, null), false, false);
				Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
				if (this.CollectingRevenues)
				{
					this.CollectingRevenues = false;
				}
			}

			// Token: 0x06005086 RID: 20614 RVA: 0x00163AAC File Offset: 0x00161CAC
			private void QuestCompletedWithSuccess()
			{
				GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, this._totalRequestedDenars, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 30)
				});
				this.RelationshipChangeWithQuestGiver = 5;
				base.AddLog(this.QuestSuccessLog, false);
				base.CompleteQuestWithSuccess();
			}

			// Token: 0x06005087 RID: 20615 RVA: 0x00163B08 File Offset: 0x00161D08
			private void QuestCompletedWithBetray()
			{
				ChangeCrimeRatingAction.Apply(base.QuestGiver.MapFaction, 45f, true);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -100)
				});
				this.RelationshipChangeWithQuestGiver = -15;
				base.AddLog(this.QuestBetrayedLog, false);
				base.CompleteQuestWithBetrayal(null);
			}

			// Token: 0x06005088 RID: 20616 RVA: 0x00163B68 File Offset: 0x00161D68
			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this.QuestCanceledWarDeclaredLog);
				}
			}

			// Token: 0x06005089 RID: 20617 RVA: 0x00163B92 File Offset: 0x00161D92
			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this.PlayerDeclaredWarQuestLogText, this.QuestCanceledWarDeclaredLog);
			}

			// Token: 0x0600508A RID: 20618 RVA: 0x00163BAC File Offset: 0x00161DAC
			protected override void OnFinalize()
			{
				if (Campaign.Current.CurrentMenuContext != null)
				{
					if (this._currentVillageEvents.Any((KeyValuePair<string, bool> x) => x.Key == Campaign.Current.CurrentMenuContext.GameMenu.StringId) || Campaign.Current.CurrentMenuContext.GameMenu.StringId == "village_collect_revenue")
					{
						if (Game.Current.GameStateManager.ActiveState is MapState)
						{
							PlayerEncounter.Finish(true);
							return;
						}
						GameMenu.SwitchToMenu("village_outside");
					}
				}
			}

			// Token: 0x0600508B RID: 20619 RVA: 0x00163C38 File Offset: 0x00161E38
			protected override void InitializeQuestOnGameLoad()
			{
				this.TargetSettlement = this._revenueVillages[0].Village.Bound;
				this.SetDialogs();
			}

			// Token: 0x0600508C RID: 20620 RVA: 0x00163C5C File Offset: 0x00161E5C
			public RevenueFarmingIssueBehavior.RevenueVillage FindCurrentRevenueVillage()
			{
				foreach (RevenueFarmingIssueBehavior.RevenueVillage revenueVillage in this._revenueVillages)
				{
					if (revenueVillage.Village.Id == Settlement.CurrentSettlement.Village.Id)
					{
						return revenueVillage;
					}
				}
				return null;
			}

			// Token: 0x0600508D RID: 20621 RVA: 0x00163CD0 File Offset: 0x00161ED0
			private void ProgressRevenueCollectionForVillage()
			{
				RevenueFarmingIssueBehavior.RevenueVillage revenueVillage = this.FindCurrentRevenueVillage();
				if (!revenueVillage.EventOccurred && revenueVillage.CollectProgress >= 0.3f)
				{
					RevenueFarmingIssueBehavior behavior = Campaign.Current.GetCampaignBehavior<RevenueFarmingIssueBehavior>();
					KeyValuePair<string, bool> randomElementInefficiently = this._currentVillageEvents.Where((KeyValuePair<string, bool> x) => !x.Value && behavior._villageEvents.Any((RevenueFarmingIssueBehavior.VillageEvent y) => y.Id == x.Key)).GetRandomElementInefficiently<KeyValuePair<string, bool>>();
					this._currentVillageEvents[randomElementInefficiently.Key] = true;
					behavior.OnVillageEventWithIdSpawned(randomElementInefficiently.Key);
					revenueVillage.EventOccurred = true;
					GameMenu.SwitchToMenu(randomElementInefficiently.Key);
					Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
					return;
				}
				revenueVillage.CollectedAmount += revenueVillage.HourlyGain;
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, revenueVillage.HourlyGain, false);
				if (revenueVillage.GetIsCompleted())
				{
					this.SetVillageAsCompleted(revenueVillage, true);
				}
			}

			// Token: 0x0600508E RID: 20622 RVA: 0x00163DA8 File Offset: 0x00161FA8
			public void SetVillageAsCompleted(RevenueFarmingIssueBehavior.RevenueVillage village, bool addLog = true)
			{
				this.CollectingRevenues = false;
				village.SetDone();
				base.RemoveTrackedObject(village.Village.Settlement);
				GameMenu.SwitchToMenu("village");
				this._questProgressLog.UpdateCurrentProgress(this._questProgressLog.CurrentProgress + 1);
				if (addLog)
				{
					TextObject textObject = new TextObject("{=mQqN8Fg0}Your men have collected {TOTAL_COLLECTED_FROM_VILLAGE}{GOLD_ICON} denars and completed the revenue collection from {VILLAGE}.", null);
					textObject.SetTextVariable("TOTAL_COLLECTED_FROM_VILLAGE", village.CollectedAmount);
					textObject.SetTextVariable("VILLAGE", village.Village.Settlement.EncyclopediaLinkWithName);
					base.AddLog(textObject, false);
				}
				if (!this._allRevenuesAreCollected)
				{
					if (this._revenueVillages.All((RevenueFarmingIssueBehavior.RevenueVillage x) => x.GetIsCompleted()))
					{
						this.OnAllRevenuesAreCollected();
					}
				}
			}

			// Token: 0x04001AB9 RID: 6841
			[SaveableField(10)]
			internal int _totalRequestedDenars;

			// Token: 0x04001ABA RID: 6842
			[SaveableField(20)]
			private readonly List<RevenueFarmingIssueBehavior.RevenueVillage> _revenueVillages;

			// Token: 0x04001ABC RID: 6844
			[SaveableField(30)]
			public bool CollectingRevenues;

			// Token: 0x04001ABD RID: 6845
			[SaveableField(40)]
			private readonly Dictionary<string, bool> _currentVillageEvents = new Dictionary<string, bool>();

			// Token: 0x04001ABE RID: 6846
			[SaveableField(50)]
			internal bool _allRevenuesAreCollected;

			// Token: 0x04001ABF RID: 6847
			[SaveableField(60)]
			private JournalLog _questProgressLog;
		}

		// Token: 0x02000660 RID: 1632
		public class VillageEvent
		{
			// Token: 0x06005095 RID: 20629 RVA: 0x00163EBD File Offset: 0x001620BD
			internal static void AutoGeneratedStaticCollectObjectsVillageEvent(object o, List<object> collectedObjects)
			{
				((RevenueFarmingIssueBehavior.VillageEvent)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06005096 RID: 20630 RVA: 0x00163ECB File Offset: 0x001620CB
			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
			}

			// Token: 0x06005097 RID: 20631 RVA: 0x00163ECD File Offset: 0x001620CD
			public VillageEvent(string id, string mainEventText, TextObject mainLog, List<RevenueFarmingIssueBehavior.VillageEventOptionData> optionConditionsAndConsequences)
			{
				this.Id = id;
				this.MainEventText = mainEventText;
				this.MainLog = mainLog;
				this.OptionConditionsAndConsequences = optionConditionsAndConsequences;
			}

			// Token: 0x04001AC0 RID: 6848
			public readonly string Id;

			// Token: 0x04001AC1 RID: 6849
			public readonly string MainEventText;

			// Token: 0x04001AC2 RID: 6850
			public TextObject MainLog;

			// Token: 0x04001AC3 RID: 6851
			public List<RevenueFarmingIssueBehavior.VillageEventOptionData> OptionConditionsAndConsequences;
		}

		// Token: 0x02000661 RID: 1633
		public class RevenueVillage
		{
			// Token: 0x06005098 RID: 20632 RVA: 0x00163EF2 File Offset: 0x001620F2
			internal static void AutoGeneratedStaticCollectObjectsRevenueVillage(object o, List<object> collectedObjects)
			{
				((RevenueFarmingIssueBehavior.RevenueVillage)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06005099 RID: 20633 RVA: 0x00163F00 File Offset: 0x00162100
			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.Village);
			}

			// Token: 0x0600509A RID: 20634 RVA: 0x00163F0E File Offset: 0x0016210E
			internal static object AutoGeneratedGetMemberValueVillage(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueVillage)o).Village;
			}

			// Token: 0x0600509B RID: 20635 RVA: 0x00163F1B File Offset: 0x0016211B
			internal static object AutoGeneratedGetMemberValueTargetAmount(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueVillage)o).TargetAmount;
			}

			// Token: 0x0600509C RID: 20636 RVA: 0x00163F2D File Offset: 0x0016212D
			internal static object AutoGeneratedGetMemberValueCollectedAmount(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueVillage)o).CollectedAmount;
			}

			// Token: 0x0600509D RID: 20637 RVA: 0x00163F3F File Offset: 0x0016213F
			internal static object AutoGeneratedGetMemberValueHourlyGain(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueVillage)o).HourlyGain;
			}

			// Token: 0x0600509E RID: 20638 RVA: 0x00163F51 File Offset: 0x00162151
			internal static object AutoGeneratedGetMemberValueEventOccurred(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueVillage)o).EventOccurred;
			}

			// Token: 0x0600509F RID: 20639 RVA: 0x00163F63 File Offset: 0x00162163
			internal static object AutoGeneratedGetMemberValueIsRaided(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueVillage)o).IsRaided;
			}

			// Token: 0x060050A0 RID: 20640 RVA: 0x00163F75 File Offset: 0x00162175
			internal static object AutoGeneratedGetMemberValue_isDone(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueVillage)o)._isDone;
			}

			// Token: 0x060050A1 RID: 20641 RVA: 0x00163F87 File Offset: 0x00162187
			internal static object AutoGeneratedGetMemberValue_customProgress(object o)
			{
				return ((RevenueFarmingIssueBehavior.RevenueVillage)o)._customProgress;
			}

			// Token: 0x17001165 RID: 4453
			// (get) Token: 0x060050A2 RID: 20642 RVA: 0x00163F99 File Offset: 0x00162199
			public float CollectProgress
			{
				get
				{
					return ((this.CollectedAmount == 0) ? 0f : ((float)this.CollectedAmount / (float)this.TargetAmount)) + this._customProgress;
				}
			}

			// Token: 0x060050A3 RID: 20643 RVA: 0x00163FC0 File Offset: 0x001621C0
			public void SetDone()
			{
				this._isDone = true;
			}

			// Token: 0x060050A4 RID: 20644 RVA: 0x00163FCC File Offset: 0x001621CC
			public RevenueVillage(Village village, int targetAmount)
			{
				this.Village = village;
				this.TargetAmount = targetAmount;
				this.CollectedAmount = 0;
				this.HourlyGain = targetAmount / 10;
				this._isDone = false;
				this.EventOccurred = false;
				this.IsRaided = false;
				this._customProgress = 0f;
			}

			// Token: 0x060050A5 RID: 20645 RVA: 0x0016401E File Offset: 0x0016221E
			public void SetAdditionalProgress(float progress)
			{
				this._customProgress = progress;
			}

			// Token: 0x060050A6 RID: 20646 RVA: 0x00164027 File Offset: 0x00162227
			public bool GetIsCompleted()
			{
				return this._isDone || this.CollectProgress >= 1f || this.CollectedAmount >= this.TargetAmount;
			}

			// Token: 0x04001AC4 RID: 6852
			[SaveableField(1)]
			public readonly Village Village;

			// Token: 0x04001AC5 RID: 6853
			[SaveableField(2)]
			public readonly int TargetAmount;

			// Token: 0x04001AC6 RID: 6854
			[SaveableField(3)]
			public int CollectedAmount;

			// Token: 0x04001AC7 RID: 6855
			[SaveableField(4)]
			public int HourlyGain;

			// Token: 0x04001AC8 RID: 6856
			[SaveableField(5)]
			private bool _isDone;

			// Token: 0x04001AC9 RID: 6857
			[SaveableField(6)]
			public bool EventOccurred;

			// Token: 0x04001ACA RID: 6858
			[SaveableField(7)]
			public bool IsRaided;

			// Token: 0x04001ACB RID: 6859
			[SaveableField(8)]
			private float _customProgress;
		}

		// Token: 0x02000662 RID: 1634
		public class RevenueFarmingIssueBehaviorTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x060050A7 RID: 20647 RVA: 0x00164051 File Offset: 0x00162251
			public RevenueFarmingIssueBehaviorTypeDefiner()
				: base(850000)
			{
			}

			// Token: 0x060050A8 RID: 20648 RVA: 0x00164060 File Offset: 0x00162260
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(RevenueFarmingIssueBehavior.RevenueFarmingIssue), 1, null);
				base.AddClassDefinition(typeof(RevenueFarmingIssueBehavior.RevenueFarmingIssueQuest), 2, null);
				base.AddClassDefinition(typeof(RevenueFarmingIssueBehavior.VillageEvent), 3, null);
				base.AddClassDefinition(typeof(RevenueFarmingIssueBehavior.RevenueVillage), 4, null);
			}

			// Token: 0x060050A9 RID: 20649 RVA: 0x001640B5 File Offset: 0x001622B5
			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(List<RevenueFarmingIssueBehavior.RevenueVillage>));
				base.ConstructContainerDefinition(typeof(List<RevenueFarmingIssueBehavior.VillageEvent>));
				base.ConstructContainerDefinition(typeof(Dictionary<string, bool>));
			}
		}

		// Token: 0x02000663 RID: 1635
		public struct VillageEventOptionData
		{
			// Token: 0x060050AA RID: 20650 RVA: 0x001640E7 File Offset: 0x001622E7
			public VillageEventOptionData(string text, GameMenuOption.OnConditionDelegate onCondition, GameMenuOption.OnConsequenceDelegate onConsequence, bool isLeave = false)
			{
				this.Text = text;
				this.OnCondition = onCondition;
				this.OnConsequence = onConsequence;
				this.IsLeave = isLeave;
			}

			// Token: 0x04001ACC RID: 6860
			public readonly string Text;

			// Token: 0x04001ACD RID: 6861
			public readonly GameMenuOption.OnConditionDelegate OnCondition;

			// Token: 0x04001ACE RID: 6862
			public readonly GameMenuOption.OnConsequenceDelegate OnConsequence;

			// Token: 0x04001ACF RID: 6863
			public readonly bool IsLeave;
		}
	}
}
