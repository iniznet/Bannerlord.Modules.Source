using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x02000384 RID: 900
	public class CompanionGrievanceBehavior : CampaignBehaviorBase
	{
		// Token: 0x060034AD RID: 13485 RVA: 0x000E0EC8 File Offset: 0x000DF0C8
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
			CampaignEvents.VillageLooted.AddNonSerializedListener(this, new Action<Village>(this.OnVillageRaided));
			CampaignEvents.PlayerDesertedBattleEvent.AddNonSerializedListener(this, new Action<int>(this.OnPlayerDesertedBattle));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
		}

		// Token: 0x060034AE RID: 13486 RVA: 0x000E0F48 File Offset: 0x000DF148
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Hero, CompanionGrievanceBehavior.Grievance>>("_heroGrievances", ref this._heroGrievances);
			dataStore.SyncData<CompanionGrievanceBehavior.Grievance>("_currentGrievance", ref this._currentGrievance);
			dataStore.SyncData<CampaignTime[]>("_nextGrievableTimeForComplaintType", ref this._nextGrievableTimeForComplaintType);
		}

		// Token: 0x060034AF RID: 13487 RVA: 0x000E0F80 File Offset: 0x000DF180
		private void AddDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("companion_start_grievance", "start", "grievance_received", "{=PVyZ9vNB}{TITLE}, there is something I wish to discuss with you.", new ConversationSentence.OnConditionDelegate(this.companion_start_grievance_condition), null, 120, null);
			campaignGameStarter.AddPlayerLine("grievance_requested", "grievance_received", "grievance_noticed", "{=m72wpzG2}Go on, I'm listening.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLineWithVariation("companion_continue_grievance_desert_softspoken", "grievance_noticed", "grievance_listened", new ConversationSentence.OnConditionDelegate(this.companion_grievance_desert_battle_condition), null, 100, "", "", "", "", null).Variation(new object[] { "{=7ldEMGn6}I don't like running away from a battle like that.", "DefaultTag", 0 }).Variation(new object[] { "{=EqLQtlca}The way we just ran from the enemy back there... I don't want to get a name for being a coward.", "PersonaSoftspokenTag", 1 })
				.Variation(new object[] { "{=sXwOLNo9}I don't like turning my back on the enemy like that. For me, death in battle is better than dishonor.", "PersonaCurtTag", 1 })
				.Variation(new object[] { "{=TnW3i5ul}We ran back there. I despise running. I prefer to be the wolf, not a rabbit.", "PersonaCurtTag", 1, "KhuzaitTag", 1 })
				.Variation(new object[] { "{=IDJbtGks}We ran back there. It is shameful to turn your back on the enemy.", "PersonaCurtTag", 1, "SturgianTag", 1 })
				.Variation(new object[] { "{=PTj1WtJx}The way we ran back there... It shames me to think of it. Next time, let us fight and die rather than let men call us cowards!", "PersonaEarnestTag", 1 })
				.Variation(new object[] { "{=UTRvswWE}As I recall, when you first hired me, it was to fight, not run away. Now, I'm sure what you did back there was sensible, but still, I've earned a bit of a reputation for bravery and I don't care to be called a coward. Those foes we can't beat - let's try to stay a little more clear of them next time, shall we?", "PersonaIronicTag", 1 })
				.Variation(new object[] { "{=v7OCHday}I suppose back there we had to run away to fight another day, as the hero Cathalac once did. If you remember the story, though, for the next three years he sat by himself in a bog, unable to look anyone else in the eye. So let's not do that too often, shall we?", "PersonaIronicTag", 1, "BattanianTag", 1 })
				.Variation(new object[] { "{=u9tAQLUf}We ran away back there. I hope word does not get around. Not looking forward to seeing the snickers and grins on people's faces the next time we walk into a tavern. Stings worse than arrows, that does.", "PersonaIronicTag", 1, "VlandianTag", 1 })
				.Variation(new object[] { "{=gfoqoGTn}The way we ran away back there... I may have told you that I hoped one day the poets would write odes about me. I had intended that they would praise my heroism, not my ability to scamper to safety.", "PersonaIronicTag", 1, "AseraiTag", 1 });
			campaignGameStarter.AddDialogLineWithVariation("companion_continue_grievance_wage_softspoken", "grievance_noticed", "grievance_listened", new ConversationSentence.OnConditionDelegate(this.companion_grievance_wage_condition), null, 100, "", "", "", "", null).Variation(new object[] { "{=yuqLzmL9}I should remind you that I expect to be paid as you had promised.", "DefaultTag", 1 }).Variation(new object[] { "{=zBfB5vw8}I hope you don't mind me saying this, but... Your men shed their blood for you. It bothers me to hear that their wages are late.", "PersonaSoftspokenTag", 1 })
				.Variation(new object[] { "{=Bhtt6XPv}Your men's wages are late. That's not the kind of company I like to fight in.", "PersonaCurtTag", 1 })
				.Variation(new object[] { "{=0mVwnCES}I must say something. Your men say their wages are late. We should take care that this doesn't happen.", "PersonaEarnestTag", 1 })
				.Variation(new object[] { "{=TzqxgiQl}The men say their wages are late. Best uphold your end of the bargain with them, or they might not keep theirs. It would especially be tricky if they chose to void their contract during a battle, wouldn't you say?", "PersonaIronicTag", 1 });
			campaignGameStarter.AddDialogLineWithVariation("companion_continue_grievance_starve_softspoken", "grievance_noticed", "grievance_listened", new ConversationSentence.OnConditionDelegate(this.companion_grievance_starve_condition), null, 100, "", "", "", "", null).Variation(new object[] { "{=IPLyqdVX}I hear we're running low on food. We should watch our stocks better.", "DefaultTag", 1 }).Variation(new object[] { "{=ITboR6C1}The men say we're running low on food. We should be more careful of that.", "PersonaSoftspokenTag", 1 })
				.Variation(new object[] { "{=HkjaCc44}Your men say there's little to eat. They march, they fight. They deserve to eat.", "PersonaCurtTag", 1 })
				.Variation(new object[] { "{=acOOsQaC}The food's running out. That's not fair to the men. We should take care that the food doesn't run out.", "PersonaEarnestTag", 1 })
				.Variation(new object[] { "{=6UKdUrPs}The men say the food's running out. We expect them to die for us if needed. Least we can do is let them die on a full belly.", "PersonaIronicTag", 1 })
				.Variation(new object[] { "{=gChji1JO}About the food... These men are ready to spill their blood for you, but there won't be much blood in their veins to shed if their bellies are empty.", "PersonaIronicTag", 1, "BattanianTag", 1 })
				.Variation(new object[] { "{=IgGQUms4}About our food situation... The general Aricaros used to say that an army marches on its stomach. Can't get far on an empty one.", "PersonaIronicTag", 1, "EmpireTag", 1 })
				.Variation(new object[] { "{=ZNaQrIaP}About our food situation... We shouldn't let the men go hungry. A man's courage comes from his stomach, they say.", "PersonaIronicTag", 1, "PersonaIronicTag", 1 });
			campaignGameStarter.AddDialogLineWithVariation("companion_continue_grievance_raid_softspoken", "grievance_noticed", "grievance_listened", new ConversationSentence.OnConditionDelegate(this.companion_grievance_raid_condition), null, 100, "", "", "", "", null).Variation(new object[] { "{=zNvjSFaC}Pillaging villages is not what I signed up for.", "DefaultTag", 1 }).Variation(new object[] { "{=bpXgcBCp}What we did to that village... I don't like it. Those farmers, they're a lot like my people. I want to know it won't happen again.", "PersonaSoftspokenTag", 1 })
				.Variation(new object[] { "{=4bkLDxIU}What we did back there, to that village... I don't do that. I want no part of it.", "PersonaCurtTag", 1 })
				.Variation(new object[] { "{=VldAzBo5}I need to say something. What we did to that village - it was wrong. They're innocent farmers and they shouldn't have their homes and fields ransacked and burned like that. I won't do that again.", "PersonaEarnestTag", 1 })
				.Variation(new object[] { "{=pDa7kOja}I know war is cruel, but I don't want to make it crueler than necessary. I'd rather not have the blood of innocents on my conscience, if you don't mind. Let's not raid villages like that.", "PersonaIronicTag", 1 });
			campaignGameStarter.AddPlayerLine("grievance", "grievance_listened", "close_window", "{=OVeSBrhv}Very well, I will consider this when taking such actions.", null, new ConversationSentence.OnConsequenceDelegate(this.companion_grievance_accepted_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("grievance", "grievance_listened", "close_window", "{=2wmKs6Is}As your leader I am able to decide the best course of action.", null, new ConversationSentence.OnConsequenceDelegate(this.companion_grievance_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("grievance", "grievance_listened", "close_window", "{=fzKFQuFT}Perhaps you are not suitable for this party after all.", null, new ConversationSentence.OnConsequenceDelegate(this.companion_grievance_rejected_consequence), 100, null, null);
			campaignGameStarter.AddDialogLine("companion_repeat_grievance", "start", "grievance_repeated", "{=baeO5Zkk}{TITLE}... {GRIEVANCE_SHORT_DESCRIPTION}", new ConversationSentence.OnConditionDelegate(this.companion_repeat_grievance_condition), null, 120, null);
			campaignGameStarter.AddDialogLine("companion_grievance_repetition_desert", "grievance_repeated", "close_window", "{=!}{GRIEVANCE_REPETITION}", new ConversationSentence.OnConditionDelegate(this.companion_grievance_desert_battle_condition), new ConversationSentence.OnConsequenceDelegate(this.companion_grievance_consequence), 100, null);
			campaignGameStarter.AddDialogLine("companion_grievance_repetition_wage", "grievance_repeated", "close_window", "{=!}{GRIEVANCE_REPETITION}", new ConversationSentence.OnConditionDelegate(this.companion_grievance_wage_condition), new ConversationSentence.OnConsequenceDelegate(this.companion_grievance_consequence), 100, null);
			campaignGameStarter.AddDialogLine("companion_grievance_repetition_starve", "grievance_repeated", "close_window", "{=!}{GRIEVANCE_REPETITION}", new ConversationSentence.OnConditionDelegate(this.companion_grievance_starve_condition), new ConversationSentence.OnConsequenceDelegate(this.companion_grievance_consequence), 100, null);
			campaignGameStarter.AddDialogLine("companion_grievance_repetition_raid", "grievance_repeated", "close_window", "{=!}{GRIEVANCE_REPETITION}", new ConversationSentence.OnConditionDelegate(this.companion_grievance_raid_condition), new ConversationSentence.OnConsequenceDelegate(this.companion_grievance_consequence), 100, null);
		}

		// Token: 0x060034B0 RID: 13488 RVA: 0x000E16C0 File Offset: 0x000DF8C0
		private bool companion_grievance_raid_condition()
		{
			return this._currentGrievance != null && this._currentGrievance.TypeOfGrievance == CompanionGrievanceBehavior.GrievanceType.VillageRaided;
		}

		// Token: 0x060034B1 RID: 13489 RVA: 0x000E16DA File Offset: 0x000DF8DA
		private bool companion_grievance_starve_condition()
		{
			return this._currentGrievance != null && this._currentGrievance.TypeOfGrievance == CompanionGrievanceBehavior.GrievanceType.Starvation;
		}

		// Token: 0x060034B2 RID: 13490 RVA: 0x000E16F4 File Offset: 0x000DF8F4
		private bool companion_grievance_desert_battle_condition()
		{
			return this._currentGrievance != null && this._currentGrievance.TypeOfGrievance == CompanionGrievanceBehavior.GrievanceType.DesertedBattle;
		}

		// Token: 0x060034B3 RID: 13491 RVA: 0x000E170E File Offset: 0x000DF90E
		private bool companion_grievance_wage_condition()
		{
			return this._currentGrievance != null && this._currentGrievance.TypeOfGrievance == CompanionGrievanceBehavior.GrievanceType.NoWage;
		}

		// Token: 0x060034B4 RID: 13492 RVA: 0x000E1728 File Offset: 0x000DF928
		private bool companion_start_grievance_condition()
		{
			MBTextManager.SetTextVariable("TITLE", Hero.MainHero.IsFemale ? GameTexts.FindText("str_my_lady", null) : GameTexts.FindText("str_my_lord", null), false);
			return this._currentGrievance != null && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == this._currentGrievance.GrievingHero && this._currentGrievance.Count <= 1 && !this._currentGrievance.HasBeenSettled;
		}

		// Token: 0x060034B5 RID: 13493 RVA: 0x000E17A4 File Offset: 0x000DF9A4
		private bool companion_repeat_grievance_condition()
		{
			if (this._currentGrievance != null && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == this._currentGrievance.GrievingHero)
			{
				MBTextManager.SetTextVariable("TITLE", ConversationHelper.HeroRefersToHero(Hero.OneToOneConversationHero, Hero.MainHero, true), false);
				MBTextManager.SetTextVariable("GRIEVANCE_SHORT_DESCRIPTION", "{=scJ2eVhS}What I said to you before... [default, should not appear]", false);
				if (this._currentGrievance.TypeOfGrievance == CompanionGrievanceBehavior.GrievanceType.DesertedBattle)
				{
					MBTextManager.SetTextVariable("GRIEVANCE_SHORT_DESCRIPTION", "{=1G5M9nn2}What I mentioned about running from battle...", false);
				}
				else if (this._currentGrievance.TypeOfGrievance == CompanionGrievanceBehavior.GrievanceType.NoWage)
				{
					MBTextManager.SetTextVariable("GRIEVANCE_SHORT_DESCRIPTION", "{=p78FaTqe}What I said about our wages being paid on time...", false);
				}
				else if (this._currentGrievance.TypeOfGrievance == CompanionGrievanceBehavior.GrievanceType.Starvation)
				{
					MBTextManager.SetTextVariable("GRIEVANCE_SHORT_DESCRIPTION", "{=zfPQlDbQ}What I said about our food...", false);
				}
				else if (this._currentGrievance.TypeOfGrievance == CompanionGrievanceBehavior.GrievanceType.VillageRaided)
				{
					MBTextManager.SetTextVariable("GRIEVANCE_SHORT_DESCRIPTION", "{=pQmUIjOQ}What I said about raiding villagers...", false);
				}
				MBTextManager.SetTextVariable("GRIEVANCE_REPETITION", "{=qNSOb7pJ}Once again, this is not something I'm happy with...", false);
				if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaEarnest)
				{
					MBTextManager.SetTextVariable("GRIEVANCE_REPETITION", "{=YWu5Xfgz}I don't feel you're taking my complaint seriously.", false);
				}
				else if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaIronic)
				{
					MBTextManager.SetTextVariable("GRIEVANCE_REPETITION", "{=wScKLt7F}Let me put things this way... It's not grown on me at all since the last time it happened.", false);
				}
				else if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaCurt)
				{
					MBTextManager.SetTextVariable("GRIEVANCE_REPETITION", "{=dpzbyUCa}I don't care for it any more than I did before.", false);
				}
				return Hero.OneToOneConversationHero == this._currentGrievance.GrievingHero && this._currentGrievance.Count > 1;
			}
			return false;
		}

		// Token: 0x060034B6 RID: 13494 RVA: 0x000E1930 File Offset: 0x000DFB30
		private void companion_grievance_accepted_consequence()
		{
			CompanionGrievanceBehavior.Grievance value = this._heroGrievances.FirstOrDefault((KeyValuePair<Hero, CompanionGrievanceBehavior.Grievance> t) => t.Value == this._currentGrievance && t.Key == this._currentGrievance.GrievingHero).Value;
			if (value != null)
			{
				value.HasBeenSettled = true;
				if (value.Count <= 1)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, this._currentGrievance.GrievingHero, 10, true);
				}
			}
			this._currentGrievance = null;
		}

		// Token: 0x060034B7 RID: 13495 RVA: 0x000E1990 File Offset: 0x000DFB90
		private void companion_grievance_consequence()
		{
			if (this._currentGrievance.Count > 1)
			{
				int num = (this._currentGrievance.HasBeenSettled ? (-5) : (-2));
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, this._currentGrievance.GrievingHero, num, true);
			}
			this._currentGrievance = null;
		}

		// Token: 0x060034B8 RID: 13496 RVA: 0x000E19DD File Offset: 0x000DFBDD
		private void companion_grievance_rejected_consequence()
		{
			ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, this._currentGrievance.GrievingHero, -15, true);
			this._currentGrievance = null;
		}

		// Token: 0x060034B9 RID: 13497 RVA: 0x000E19FE File Offset: 0x000DFBFE
		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		// Token: 0x060034BA RID: 13498 RVA: 0x000E1A08 File Offset: 0x000DFC08
		private void OnHourlyTick()
		{
			foreach (KeyValuePair<Hero, CompanionGrievanceBehavior.Grievance> keyValuePair in this._heroGrievances)
			{
				CompanionGrievanceBehavior.Grievance value = keyValuePair.Value;
				if (value.GrievingHero.PartyBelongedTo == MobileParty.MainParty && value.HaveGrievance && GameStateManager.Current.ActiveState is MapState && MobileParty.MainParty.MapEvent == null && PlayerEncounter.Current == null)
				{
					this._currentGrievance = value;
					CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject, null, false, false, false, false, false, false), new ConversationCharacterData(value.GrievingHero.CharacterObject, null, false, false, false, false, false, false));
					value.HaveGrievance = false;
					value.NextGrievanceTime = CampaignTime.HoursFromNow(100f + (float)MBRandom.RandomInt(100));
					break;
				}
			}
		}

		// Token: 0x060034BB RID: 13499 RVA: 0x000E1B00 File Offset: 0x000DFD00
		private void DecideCompanionGrievances(CompanionGrievanceBehavior.GrievanceType eventType)
		{
			if (this._nextGrievableTimeForComplaintType[(int)eventType].IsFuture)
			{
				return;
			}
			foreach (Hero hero in Hero.MainHero.CompanionsInParty)
			{
				CompanionGrievanceBehavior.Grievance grievance;
				this._heroGrievances.TryGetValue(hero, out grievance);
				if (grievance == null)
				{
					CompanionGrievanceBehavior.GrievanceType grievanceTypeForCompanion = this.GetGrievanceTypeForCompanion(hero, eventType);
					if (grievanceTypeForCompanion != CompanionGrievanceBehavior.GrievanceType.Invalid)
					{
						grievance = new CompanionGrievanceBehavior.Grievance(hero, CampaignTime.Now, grievanceTypeForCompanion);
						this._heroGrievances.Add(hero, grievance);
						this._nextGrievableTimeForComplaintType[(int)eventType] = CampaignTime.HoursFromNow(100f);
						break;
					}
				}
				if (grievance != null && grievance.TypeOfGrievance == eventType && !grievance.HaveGrievance && grievance.NextGrievanceTime.IsPast)
				{
					grievance.HaveGrievance = true;
					this._nextGrievableTimeForComplaintType[(int)eventType] = CampaignTime.HoursFromNow(100f);
					grievance.Count++;
					break;
				}
			}
		}

		// Token: 0x060034BC RID: 13500 RVA: 0x000E1C04 File Offset: 0x000DFE04
		private CompanionGrievanceBehavior.GrievanceType GetGrievanceTypeForCompanion(Hero companionHero, CompanionGrievanceBehavior.GrievanceType type)
		{
			if ((type == CompanionGrievanceBehavior.GrievanceType.DesertedBattle && companionHero.GetTraitLevel(DefaultTraits.Valor) > 0) || (type == CompanionGrievanceBehavior.GrievanceType.Starvation && companionHero.GetTraitLevel(DefaultTraits.Generosity) > 0) || (type == CompanionGrievanceBehavior.GrievanceType.NoWage && companionHero.GetTraitLevel(DefaultTraits.Generosity) > 0) || (type == CompanionGrievanceBehavior.GrievanceType.VillageRaided && companionHero.GetTraitLevel(DefaultTraits.Mercy) > 0))
			{
				return type;
			}
			return CompanionGrievanceBehavior.GrievanceType.Invalid;
		}

		// Token: 0x060034BD RID: 13501 RVA: 0x000E1C5C File Offset: 0x000DFE5C
		private void OnPlayerDesertedBattle(int sacrificedMenCount)
		{
			this.DecideCompanionGrievances(CompanionGrievanceBehavior.GrievanceType.DesertedBattle);
		}

		// Token: 0x060034BE RID: 13502 RVA: 0x000E1C68 File Offset: 0x000DFE68
		private void OnVillageRaided(Village village)
		{
			PartyBase party = village.Settlement.Party;
			MapEvent mapEvent = ((party != null) ? party.MapEvent : null);
			if (mapEvent != null)
			{
				using (IEnumerator<PartyBase> enumerator = mapEvent.InvolvedParties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == PartyBase.MainParty)
						{
							this.DecideCompanionGrievances(CompanionGrievanceBehavior.GrievanceType.VillageRaided);
						}
					}
				}
			}
		}

		// Token: 0x060034BF RID: 13503 RVA: 0x000E1CD8 File Offset: 0x000DFED8
		private void OnDailyTick()
		{
			if (PartyBase.MainParty.IsStarving)
			{
				this.DecideCompanionGrievances(CompanionGrievanceBehavior.GrievanceType.Starvation);
			}
			if (MobileParty.MainParty.HasUnpaidWages > 0f)
			{
				this.DecideCompanionGrievances(CompanionGrievanceBehavior.GrievanceType.NoWage);
			}
			foreach (KeyValuePair<Hero, CompanionGrievanceBehavior.Grievance> keyValuePair in this._heroGrievances)
			{
				CompanionGrievanceBehavior.Grievance value = keyValuePair.Value;
				if (value.NextGrievanceTime.ElapsedWeeksUntilNow >= 8f)
				{
					value.HasBeenSettled = false;
					value.Count = 0;
				}
			}
		}

		// Token: 0x0400111D RID: 4381
		private Dictionary<Hero, CompanionGrievanceBehavior.Grievance> _heroGrievances = new Dictionary<Hero, CompanionGrievanceBehavior.Grievance>();

		// Token: 0x0400111E RID: 4382
		private CampaignTime[] _nextGrievableTimeForComplaintType = new CampaignTime[Enum.GetValues(typeof(CompanionGrievanceBehavior.GrievanceType)).Length];

		// Token: 0x0400111F RID: 4383
		private CompanionGrievanceBehavior.Grievance _currentGrievance;

		// Token: 0x04001120 RID: 4384
		private const float _baseGrievanceFrequencyInHours = 100f;

		// Token: 0x04001121 RID: 4385
		private const float _grievanceObsolescenceDurationInWeeks = 8f;

		// Token: 0x020006C5 RID: 1733
		public class CompanionGrievanceBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			// Token: 0x06005447 RID: 21575 RVA: 0x0016AA09 File Offset: 0x00168C09
			public CompanionGrievanceBehaviorTypeDefiner()
				: base(80000)
			{
			}

			// Token: 0x06005448 RID: 21576 RVA: 0x0016AA16 File Offset: 0x00168C16
			protected override void DefineEnumTypes()
			{
				base.AddEnumDefinition(typeof(CompanionGrievanceBehavior.GrievanceType), 1, null);
			}

			// Token: 0x06005449 RID: 21577 RVA: 0x0016AA2A File Offset: 0x00168C2A
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(CompanionGrievanceBehavior.Grievance), 10, null);
			}

			// Token: 0x0600544A RID: 21578 RVA: 0x0016AA3F File Offset: 0x00168C3F
			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(Dictionary<Hero, CompanionGrievanceBehavior.Grievance>));
			}
		}

		// Token: 0x020006C6 RID: 1734
		internal class Grievance
		{
			// Token: 0x17001360 RID: 4960
			// (get) Token: 0x0600544B RID: 21579 RVA: 0x0016AA51 File Offset: 0x00168C51
			// (set) Token: 0x0600544C RID: 21580 RVA: 0x0016AA59 File Offset: 0x00168C59
			[SaveableProperty(4)]
			public bool HaveGrievance { get; set; }

			// Token: 0x0600544D RID: 21581 RVA: 0x0016AA62 File Offset: 0x00168C62
			public Grievance(Hero hero, CampaignTime time, CompanionGrievanceBehavior.GrievanceType type)
			{
				this.GrievingHero = hero;
				this.NextGrievanceTime = time;
				this.TypeOfGrievance = type;
				this.HasBeenSettled = false;
				this.Count = 1;
				this.HaveGrievance = true;
			}

			// Token: 0x0600544E RID: 21582 RVA: 0x0016AA94 File Offset: 0x00168C94
			internal static void AutoGeneratedStaticCollectObjectsGrievance(object o, List<object> collectedObjects)
			{
				((CompanionGrievanceBehavior.Grievance)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x0600544F RID: 21583 RVA: 0x0016AAA2 File Offset: 0x00168CA2
			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.GrievingHero);
				CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.NextGrievanceTime, collectedObjects);
			}

			// Token: 0x06005450 RID: 21584 RVA: 0x0016AAC1 File Offset: 0x00168CC1
			internal static object AutoGeneratedGetMemberValueHaveGrievance(object o)
			{
				return ((CompanionGrievanceBehavior.Grievance)o).HaveGrievance;
			}

			// Token: 0x06005451 RID: 21585 RVA: 0x0016AAD3 File Offset: 0x00168CD3
			internal static object AutoGeneratedGetMemberValueGrievingHero(object o)
			{
				return ((CompanionGrievanceBehavior.Grievance)o).GrievingHero;
			}

			// Token: 0x06005452 RID: 21586 RVA: 0x0016AAE0 File Offset: 0x00168CE0
			internal static object AutoGeneratedGetMemberValueNextGrievanceTime(object o)
			{
				return ((CompanionGrievanceBehavior.Grievance)o).NextGrievanceTime;
			}

			// Token: 0x06005453 RID: 21587 RVA: 0x0016AAF2 File Offset: 0x00168CF2
			internal static object AutoGeneratedGetMemberValueTypeOfGrievance(object o)
			{
				return ((CompanionGrievanceBehavior.Grievance)o).TypeOfGrievance;
			}

			// Token: 0x06005454 RID: 21588 RVA: 0x0016AB04 File Offset: 0x00168D04
			internal static object AutoGeneratedGetMemberValueHasBeenSettled(object o)
			{
				return ((CompanionGrievanceBehavior.Grievance)o).HasBeenSettled;
			}

			// Token: 0x06005455 RID: 21589 RVA: 0x0016AB16 File Offset: 0x00168D16
			internal static object AutoGeneratedGetMemberValueCount(object o)
			{
				return ((CompanionGrievanceBehavior.Grievance)o).Count;
			}

			// Token: 0x04001BE0 RID: 7136
			[SaveableField(1)]
			public Hero GrievingHero;

			// Token: 0x04001BE1 RID: 7137
			[SaveableField(2)]
			public CampaignTime NextGrievanceTime;

			// Token: 0x04001BE2 RID: 7138
			[SaveableField(3)]
			public CompanionGrievanceBehavior.GrievanceType TypeOfGrievance;

			// Token: 0x04001BE4 RID: 7140
			[SaveableField(5)]
			public bool HasBeenSettled;

			// Token: 0x04001BE5 RID: 7141
			[SaveableField(6)]
			public int Count;
		}

		// Token: 0x020006C7 RID: 1735
		internal enum GrievanceType
		{
			// Token: 0x04001BE7 RID: 7143
			Invalid,
			// Token: 0x04001BE8 RID: 7144
			NoWage,
			// Token: 0x04001BE9 RID: 7145
			Starvation,
			// Token: 0x04001BEA RID: 7146
			VillageRaided,
			// Token: 0x04001BEB RID: 7147
			DesertedBattle
		}
	}
}
