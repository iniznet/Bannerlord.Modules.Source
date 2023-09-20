using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.ThirdPhase
{
	public class DefeatTheConspiracyQuestBehavior : CampaignBehaviorBase
	{
		private TextObject _empireUnitedText
		{
			get
			{
				return new TextObject("{=zTYd6Qai}The Empire has been united under the {FACTION}!", null);
			}
		}

		private TextObject _empireDefeatedText
		{
			get
			{
				return new TextObject("{=rCX81DDR}The Empire has been destroyed!", null);
			}
		}

		public override void RegisterEvents()
		{
			StoryModeEvents.OnConspiracyActivatedEvent.AddNonSerializedListener(this, new Action(this.OnConspiracyActivated));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
		}

		private void OnConspiracyActivated()
		{
			this.InitializeFinalPhase();
		}

		private void HourlyTick()
		{
			if (StoryModeManager.Current.MainStoryLine.ThirdPhase != null && Extensions.IsEmpty<Kingdom>(StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms) && !this._hasBeenFinalized)
			{
				this._hasBeenFinalized = true;
				object obj = new TextObject("{=R4Gqskgq}Victory", null);
				TextObject textObject = (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? this._empireUnitedText : this._empireDefeatedText);
				textObject.SetTextVariable("FACTION", StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom.Name);
				TextObject textObject2 = new TextObject("{=DM6luo3c}Continue", null);
				InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, false, textObject2.ToString(), "", delegate
				{
					string text;
					string text2;
					string text3;
					if (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine)
					{
						text = "imperial_outro";
						text2 = "imperial_outro";
						text3 = "imperial_outro";
					}
					else
					{
						text = "anti_imperial_outro";
						text2 = "anti_imperial_outro";
						text3 = "anti_imperial_outro";
					}
					Campaign.Current.TimeControlMode = 0;
					DefeatTheConspiracyQuestBehavior.PlayOutroCinematic(text, text2, text3, new Action(this.ShowGameStatistics));
				}, null, "", 0f, null, null, null), true, false);
			}
		}

		protected void InitializeFinalPhase()
		{
			bool isOnImperialQuestLine = StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine;
			List<Kingdom> list = Kingdom.All.Where((Kingdom t) => StoryModeData.IsKingdomImperial(t) && t != StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom && !t.IsEliminated).ToList<Kingdom>();
			List<Kingdom> list2 = Kingdom.All.Where((Kingdom t) => !StoryModeData.IsKingdomImperial(t) && t != StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom && !t.IsEliminated).ToList<Kingdom>();
			List<DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest> list3 = new List<DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest>();
			List<Kingdom> list4;
			List<Kingdom> list5;
			if (isOnImperialQuestLine)
			{
				if (Extensions.IsEmpty<Kingdom>(list2))
				{
					Kingdom randomElementWithPredicate = Extensions.GetRandomElementWithPredicate<Kingdom>(Kingdom.All, (Kingdom t) => !StoryModeData.IsKingdomImperial(t));
					randomElementWithPredicate.ReactivateKingdom();
					list2.Add(randomElementWithPredicate);
				}
				list4 = list2.OrderByDescending((Kingdom t) => t.TotalStrength).Take(3).ToList<Kingdom>();
				list5 = new List<Kingdom> { StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom };
				for (int i = 0; i < list2.Count; i++)
				{
					for (int j = i + 1; j < list2.Count; j++)
					{
						if (!FactionManager.IsAlliedWithFaction(list2[i], list2[j]))
						{
							MakePeaceAction.Apply(list2[i], list2[j], 0);
						}
					}
				}
			}
			else
			{
				if (Extensions.IsEmpty<Kingdom>(list))
				{
					Kingdom randomElementWithPredicate2 = Extensions.GetRandomElementWithPredicate<Kingdom>(Kingdom.All, new Func<Kingdom, bool>(StoryModeData.IsKingdomImperial));
					randomElementWithPredicate2.ReactivateKingdom();
					list.Add(randomElementWithPredicate2);
				}
				list4 = list.ToList<Kingdom>();
				list5 = new List<Kingdom> { StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom };
				for (int k2 = 0; k2 < list.Count; k2++)
				{
					for (int l = k2 + 1; l < list.Count; l++)
					{
						if (!FactionManager.IsAlliedWithFaction(list[k2], list[l]))
						{
							MakePeaceAction.Apply(list[k2], list[l], 0);
						}
					}
				}
			}
			foreach (Kingdom kingdom5 in list5)
			{
				StoryModeManager.Current.MainStoryLine.ThirdPhase.AddAllyKingdom(kingdom5);
			}
			for (int m = 0; m < list4.Count; m++)
			{
				if (!list4[m].IsAtWarWith(StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom))
				{
					ChangeRelationAction.ApplyPlayerRelation(list4[m].Leader, -10, true, true);
					DeclareWarAction.ApplyByPlayerHostility(list4[m], StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom);
				}
			}
			int num = 0;
			foreach (Kingdom kingdom2 in list4)
			{
				StoryModeManager.Current.MainStoryLine.ThirdPhase.AddOppositionKingdom(kingdom2);
				DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest defeatTheConspiracyQuest = new DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest("defeat_the_conspiracy_quest_" + num, kingdom2);
				num++;
				defeatTheConspiracyQuest.StartQuest();
				list3.Add(defeatTheConspiracyQuest);
			}
			Hero leader = list4[Extensions.IndexOfMax<Kingdom>(list4, (Kingdom k) => (int)k.TotalStrength)].Leader;
			SceneNotificationData sceneNotificationData;
			if (StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine)
			{
				sceneNotificationData = new AntiEmpireConspiracyBeginsSceneNotificationItem(leader, StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms.ToList<Kingdom>());
			}
			else
			{
				sceneNotificationData = new ProEmpireConspiracyBeginsSceneNotificationItem(leader);
			}
			MBInformationManager.ShowSceneNotification(sceneNotificationData);
			Dictionary<Kingdom, int> dictionary = new Dictionary<Kingdom, int>();
			float conspiracyStrength = StoryModeManager.Current.MainStoryLine.SecondPhase.ConspiracyStrength;
			List<float> list6 = new List<float> { 0.5f, 0.3f, 0.2f };
			int num2 = 0;
			for (int n = 0; n < list6.Count; n++)
			{
				if (num2 > list4.Count - 1)
				{
					num2 = 0;
				}
				Kingdom kingdom3 = list4[num2];
				if (!dictionary.ContainsKey(kingdom3))
				{
					dictionary.Add(kingdom3, 0);
				}
				Dictionary<Kingdom, int> dictionary2 = dictionary;
				Kingdom kingdom4 = kingdom3;
				dictionary2[kingdom4] += (int)(conspiracyStrength * list6[n]);
				num2++;
			}
			foreach (KeyValuePair<Kingdom, int> keyValuePair in dictionary)
			{
				Kingdom kingdom = keyValuePair.Key;
				MBList<MobileParty> mblist = new MBList<MobileParty>();
				foreach (WarPartyComponent warPartyComponent in kingdom.WarPartyComponents)
				{
					if (warPartyComponent.Leader != null)
					{
						mblist.Add(warPartyComponent.MobileParty);
					}
				}
				MBList<CharacterObject> mblist2 = Extensions.ToMBList<CharacterObject>(CharacterHelper.GetTroopTree(kingdom.Culture.BasicTroop, 3f, 6f));
				int num3 = keyValuePair.Value / 2;
				int num4 = num3 / 200;
				List<Hero> list7 = new List<Hero>();
				Clan clan = null;
				Func<Settlement, bool> <>9__9;
				for (int num5 = 0; num5 < num4; num5++)
				{
					if (clan == null || clan.Lords.Count >= clan.CommanderLimit)
					{
						DefeatTheConspiracyQuestBehavior.<>c__DisplayClass12_1 CS$<>8__locals2 = new DefeatTheConspiracyQuestBehavior.<>c__DisplayClass12_1();
						DefeatTheConspiracyQuestBehavior.<>c__DisplayClass12_1 CS$<>8__locals3 = CS$<>8__locals2;
						NameGenerator nameGenerator = NameGenerator.Current;
						CultureObject culture = kingdom.Culture;
						MBReadOnlyList<Settlement> all = Settlement.All;
						Func<Settlement, bool> func;
						if ((func = <>9__9) == null)
						{
							func = (<>9__9 = (Settlement t) => t.Culture == kingdom.Culture && t.IsVillage);
						}
						CS$<>8__locals3.clanName = nameGenerator.GenerateClanName(culture, Extensions.GetRandomElementWithPredicate<Settlement>(all, func));
						clan = Clan.CreateClan(string.Concat(new object[]
						{
							"main_storyline_clan_",
							CS$<>8__locals2.clanName.ToString(),
							"_",
							Clan.All.Count((Clan t) => t.Name == CS$<>8__locals2.clanName)
						}));
						clan.InitializeClan(CS$<>8__locals2.clanName, CS$<>8__locals2.clanName, kingdom.Culture, Banner.CreateRandomClanBanner(-1), default(Vec2), false);
						clan.IsNoble = true;
					}
					MBList<Settlement> mblist3 = Extensions.ToMBList<Settlement>(kingdom.Settlements.Where((Settlement t) => !t.IsUnderSiege && !t.IsUnderRaid));
					Settlement settlement = null;
					if (!Extensions.IsEmpty<Settlement>(mblist3))
					{
						settlement = Extensions.GetRandomElementWithPredicate<Settlement>(mblist3, (Settlement t) => t.IsTown);
						if (settlement == null)
						{
							settlement = Extensions.GetRandomElement<Settlement>(mblist3);
						}
					}
					Hero hero;
					if (mblist.Count <= 0)
					{
						hero = Extensions.GetRandomElementWithPredicate<Hero>(Hero.AllAliveHeroes, (Hero t) => t.Occupation == 3);
					}
					else
					{
						hero = Extensions.GetRandomElement<MobileParty>(mblist).LeaderHero;
					}
					Hero hero2 = HeroCreator.CreateSpecialHero(hero.CharacterObject, settlement, clan, null, -1);
					GiveGoldAction.ApplyBetweenCharacters(null, hero2, 2000, true);
					hero2.ChangeState(1);
					if (clan.Leader == null)
					{
						clan.SetLeader(hero2);
					}
					if (clan.Kingdom != kingdom)
					{
						ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom, false);
					}
					MobileParty mobileParty;
					if (settlement != null)
					{
						hero2.BornSettlement = settlement;
						EnterSettlementAction.ApplyForCharacterOnly(hero2, settlement);
						mobileParty = clan.CreateNewMobileParty(hero2);
					}
					else
					{
						Clan rulingClan = kingdom.RulingClan;
						Vec2 vec = ((rulingClan != null) ? rulingClan.InitialPosition : kingdom.InitialPosition);
						if (!PartyBase.IsPositionOkForTraveling(vec))
						{
							vec = Campaign.Current.MapSceneWrapper.GetAccessiblePointNearPosition(vec, 50f);
						}
						mobileParty = clan.CreateNewMobilePartyAtPosition(hero2, vec);
						hero2.BornSettlement = Extensions.GetRandomElementWithPredicate<Settlement>(Settlement.All, (Settlement t) => t.IsTown || t.IsVillage);
					}
					mobileParty.MemberRoster.AddToCounts(Extensions.GetRandomElement<CharacterObject>(mblist2), 200, false, 0, 0, true, -1);
					mobileParty.ItemRoster.AddToCounts(DefaultItems.Grain, 10);
					mobileParty.ItemRoster.AddToCounts(DefaultItems.Meat, 5);
					list7.Add(hero2);
				}
				if (Extensions.IsEmpty<MobileParty>(mblist))
				{
					mblist.AddRange(list7.Select((Hero t) => t.PartyBelongedTo));
				}
				if (!Extensions.IsEmpty<MobileParty>(mblist))
				{
					int num6 = keyValuePair.Value - num3;
					int num7 = num3 - list7.Count * 200;
					int num8 = num6 + num7;
					int num9 = num8 / mblist.Count;
					int num10 = num8 % mblist.Count;
					if (num9 > 0)
					{
						foreach (MobileParty mobileParty2 in mblist)
						{
							for (int num11 = 0; num11 < num9; num11++)
							{
								mobileParty2.MemberRoster.AddToCounts(Extensions.GetRandomElement<CharacterObject>(mblist2), 1, false, 0, 0, true, -1);
							}
						}
					}
					if (num10 > 0)
					{
						MobileParty randomElement = Extensions.GetRandomElement<MobileParty>(mblist);
						for (int num12 = 0; num12 < num10; num12++)
						{
							randomElement.MemberRoster.AddToCounts(Extensions.GetRandomElement<CharacterObject>(mblist2), 1, false, 0, 0, true, -1);
						}
					}
				}
			}
			foreach (DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest defeatTheConspiracyQuest2 in list3)
			{
				defeatTheConspiracyQuest2.CalculateReinforcedWarScore();
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_hasBeenFinalized", ref this._hasBeenFinalized);
		}

		private static void PlayOutroCinematic(string videoFile, string audioFile, string subtitleFile, Action onVideoFinished)
		{
			VideoPlaybackState videoPlaybackState = Game.Current.GameStateManager.CreateState<VideoPlaybackState>();
			string text = ModuleHelper.GetModuleFullPath("SandBox") + "Videos/CampaignOutro/";
			string text2 = text + videoFile + ".ivf";
			string text3 = text + audioFile + ".ogg";
			string text4 = text + subtitleFile;
			videoPlaybackState.SetStartingParameters(text2, text3, text4, 30f, true);
			videoPlaybackState.SetOnVideoFinisedDelegate(onVideoFinished);
			Game.Current.GameStateManager.PushState(videoPlaybackState, 0);
		}

		private void ShowGameStatistics()
		{
			GameOverState gameOverState = Game.Current.GameStateManager.CreateState<GameOverState>(new object[] { 2 });
			Game.Current.GameStateManager.PopState(0);
			Game.Current.GameStateManager.PushState(gameOverState, 0);
		}

		private const int TroopCountPerNewLord = 200;

		private bool _hasBeenFinalized;

		internal class OppositionData
		{
			internal static void AutoGeneratedStaticCollectObjectsOppositionData(object o, List<object> collectedObjects)
			{
				((DefeatTheConspiracyQuestBehavior.OppositionData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.QuestLog);
				CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.LastPeaceOfferDate, collectedObjects);
			}

			internal static object AutoGeneratedGetMemberValueInitialWarScore(object o)
			{
				return ((DefeatTheConspiracyQuestBehavior.OppositionData)o).InitialWarScore;
			}

			internal static object AutoGeneratedGetMemberValueReinforcedWarScore(object o)
			{
				return ((DefeatTheConspiracyQuestBehavior.OppositionData)o).ReinforcedWarScore;
			}

			internal static object AutoGeneratedGetMemberValueQuestLog(object o)
			{
				return ((DefeatTheConspiracyQuestBehavior.OppositionData)o).QuestLog;
			}

			internal static object AutoGeneratedGetMemberValueLastPeaceOfferDate(object o)
			{
				return ((DefeatTheConspiracyQuestBehavior.OppositionData)o).LastPeaceOfferDate;
			}

			[SaveableField(10)]
			public float InitialWarScore;

			[SaveableField(20)]
			public float ReinforcedWarScore;

			[SaveableField(30)]
			public JournalLog QuestLog;

			[SaveableField(40)]
			public CampaignTime LastPeaceOfferDate = CampaignTime.Zero;
		}

		public class DefeatTheConspiracyQuestBehaviorTypeDefiner : SaveableTypeDefiner
		{
			public DefeatTheConspiracyQuestBehaviorTypeDefiner()
				: base(16000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(DefeatTheConspiracyQuestBehavior.OppositionData), 1, null);
				base.AddClassDefinition(typeof(DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest), 2, null);
			}
		}

		public class DefeatTheConspiracyQuest : StoryModeQuestBase
		{
			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=Dfmg8stq}Eliminate {FACTION}", null);
					textObject.SetTextVariable("FACTION", this._oppositionKingdom.Name);
					return textObject;
				}
			}

			private TextObject _defeatOpposingKingdomsQuestLogText
			{
				get
				{
					return new TextObject("{=ib2TKPUa}The ruler of the {FACTION} is leading the alliance against you. Defeat their armies or force them to make peace by capturing their settlements and destroying their parties to achieve victory.", null);
				}
			}

			private TextObject _defeatOpposingKingdomsProgressLogText
			{
				get
				{
					return new TextObject("{=3Io5vmOk}War Progress with the {FACTION}", null);
				}
			}

			private TextObject _imperialKingdomDefeatedPopUpTitleText
			{
				get
				{
					return new TextObject("{=XWL3XcIq}{FACTION} Defeated", null);
				}
			}

			private TextObject _imperialKingdomDefeatedPopUpDescriptionText
			{
				get
				{
					return new TextObject("{=4SnDe0rA}The clans of the {FACTION} have defected to surrounding kingdoms as their leader has given up all hopes of restoring the Empire.", null);
				}
			}

			private TextObject _imperialKingdomDefeatedQuestLogText
			{
				get
				{
					return new TextObject("{=OwcgxRXB}Weakened by the war, the {FACTION} has collapsed and its clans have defected to surrounding kingdoms.", null);
				}
			}

			private TextObject _antiImperialKingdomDefeatedPopUpTitleText
			{
				get
				{
					return new TextObject("{=L3Qb6lbp}Peace Offer from the {FACTION}", null);
				}
			}

			private TextObject _antiImperialKingdomDefeatedPopUpKingDescriptionText
			{
				get
				{
					return new TextObject("{=E87miqTI}Exhausted from the war, the clans of the {FACTION} offer to make peace with the {PLAYER_SUPPORTED_FACTION}.", null);
				}
			}

			private TextObject _antiImperialKingdomDefeatedPopUpSubjectDescriptionText
			{
				get
				{
					return new TextObject("{=hGPdLssq}The ruler of the {PLAYER_SUPPORTED_FACTION} has accepted the peace offered by the war-ravaged {FACTION}.", null);
				}
			}

			private TextObject _antiImperialKingdomDefeatedQuestLogText
			{
				get
				{
					return new TextObject("{=weS3DJKA}Weakened by war, the ruler of the {FACTION} offers to make peace with {PLAYER_FACTION}.", null);
				}
			}

			private TextObject _playerSupportedKingdomDestroyedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=eH8z6Fws}Despite its Dragon Banner, the {PLAYER_SUPPORTED_KINGDOM} has been destroyed!", null);
					textObject.SetTextVariable("PLAYER_SUPPORTED_KINGDOM", StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			public DefeatTheConspiracyQuest(string questId, Kingdom oppositionKingdom)
				: base(questId, StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine ? StoryModeHeroes.ImperialMentor : StoryModeHeroes.AntiImperialMentor, CampaignTime.Never)
			{
				this._oppositionKingdom = oppositionKingdom;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			protected override void SetDialogs()
			{
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			private void UpdateWarProgressWithKingdom()
			{
				float num = this.CalculateWarScoreForKingdom(this._oppositionKingdom);
				float reinforcedWarScore = this._oppositionData.ReinforcedWarScore;
				float num2 = this._oppositionData.InitialWarScore / 2f;
				int num3 = (int)MathF.Clamp((reinforcedWarScore - num) / (reinforcedWarScore - num2) * 100f, -100f, 100f);
				this._oppositionData.QuestLog.UpdateCurrentProgress(num3);
				if (num <= this._oppositionData.InitialWarScore / 2f && this._oppositionData.LastPeaceOfferDate.ElapsedDaysUntilNow >= 7f)
				{
					this._oppositionData.LastPeaceOfferDate = CampaignTime.Now;
					this.InitializeKingdomDefeatedPopUp(this._oppositionKingdom);
				}
			}

			private void InitializeKingdomDefeatedPopUp(Kingdom kingdom)
			{
				Kingdom playerSupportedKingdom = StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom;
				TextObject textObject;
				TextObject textObject2;
				if (StoryModeData.IsKingdomImperial(kingdom))
				{
					textObject = this._imperialKingdomDefeatedPopUpTitleText;
					textObject.SetTextVariable("FACTION", kingdom.Name);
					textObject2 = this._imperialKingdomDefeatedPopUpDescriptionText;
					textObject2.SetTextVariable("FACTION", kingdom.Name);
					textObject2.SetTextVariable("PLAYER_SUPPORTED_FACTION", playerSupportedKingdom.Name);
					TextObject textObject3 = new TextObject("{=DM6luo3c}Continue", null);
					InformationManager.ShowInquiry(new InquiryData(textObject.ToString(), textObject2.ToString(), true, false, textObject3.ToString(), "", delegate
					{
						this.OnKingdomDefeated(kingdom);
					}, null, "", 0f, null, null, null), true, false);
					return;
				}
				textObject = this._antiImperialKingdomDefeatedPopUpTitleText;
				textObject.SetTextVariable("FACTION", kingdom.Name);
				if (playerSupportedKingdom.Leader == Hero.MainHero)
				{
					textObject2 = this._antiImperialKingdomDefeatedPopUpKingDescriptionText;
					textObject2.SetTextVariable("FACTION", kingdom.Name);
					textObject2.SetTextVariable("PLAYER_SUPPORTED_FACTION", playerSupportedKingdom.Name);
					TextObject textObject4 = new TextObject("{=Y94H6XnK}Accept", null);
					TextObject textObject5 = new TextObject("{=cOgmdp9e}Decline", null);
					InformationManager.ShowInquiry(new InquiryData(textObject.ToString(), textObject2.ToString(), true, true, textObject4.ToString(), textObject5.ToString(), delegate
					{
						this.OnKingdomDefeated(kingdom);
					}, null, "", 0f, null, null, null), true, false);
					return;
				}
				textObject2 = this._antiImperialKingdomDefeatedPopUpSubjectDescriptionText;
				textObject2.SetTextVariable("FACTION", kingdom.Name);
				textObject2.SetTextVariable("PLAYER_SUPPORTED_FACTION", playerSupportedKingdom.Name);
				TextObject textObject6 = new TextObject("{=DM6luo3c}Continue", null);
				InformationManager.ShowInquiry(new InquiryData(textObject.ToString(), textObject2.ToString(), true, false, textObject6.ToString(), "", delegate
				{
					this.OnKingdomDefeated(kingdom);
				}, null, "", 0f, null, null, null), true, false);
			}

			private void OnKingdomDefeated(Kingdom kingdom)
			{
				if (this._oppositionKingdom == kingdom)
				{
					Kingdom playerSupportedKingdom = StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom;
					if (StoryModeData.IsKingdomImperial(kingdom))
					{
						StoryModeManager.Current.MainStoryLine.ThirdPhase.RemoveOppositionKingdom(kingdom);
						base.RemoveLog(this._oppositionData.QuestLog);
						TextObject imperialKingdomDefeatedQuestLogText = this._imperialKingdomDefeatedQuestLogText;
						imperialKingdomDefeatedQuestLogText.SetTextVariable("FACTION", kingdom.EncyclopediaLinkWithName);
						base.AddLog(imperialKingdomDefeatedQuestLogText, false);
						Kingdom kingdom2 = null;
						if (!Extensions.IsEmpty<Kingdom>(StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms))
						{
							kingdom2 = StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms.OrderBy((Kingdom t) => Campaign.Current.Models.MapDistanceModel.GetDistance(kingdom.FactionMidSettlement, t.FactionMidSettlement)).FirstOrDefault<Kingdom>();
						}
						else
						{
							List<Kingdom> list = Kingdom.All.Where((Kingdom t) => !StoryModeData.IsKingdomImperial(t) && !t.IsEliminated && t != playerSupportedKingdom).ToList<Kingdom>();
							if (Extensions.IsEmpty<Kingdom>(list))
							{
								kingdom2 = playerSupportedKingdom;
							}
							else
							{
								kingdom2 = list.OrderBy((Kingdom t) => Campaign.Current.Models.MapDistanceModel.GetDistance(kingdom.FactionMidSettlement, t.FactionMidSettlement)).FirstOrDefault<Kingdom>();
								foreach (Settlement settlement in new List<Settlement>(kingdom.Settlements))
								{
									ChangeOwnerOfSettlementAction.ApplyByLeaveFaction(playerSupportedKingdom.Leader, settlement);
								}
							}
						}
						if (kingdom2 != null)
						{
							this.DefectClansOfKingdomToKingdom(kingdom, kingdom2);
						}
						else
						{
							Debug.FailedAssert("Kingdom to defect can't be found", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\Quests\\ThirdPhase\\DefeatTheConspiracyQuestBehavior.cs", "OnKingdomDefeated", 336);
						}
					}
					else
					{
						MakePeaceAction.Apply(kingdom, playerSupportedKingdom, 0);
						StoryModeManager.Current.MainStoryLine.ThirdPhase.RemoveOppositionKingdom(kingdom);
						base.RemoveLog(this._oppositionData.QuestLog);
						TextObject antiImperialKingdomDefeatedQuestLogText = this._antiImperialKingdomDefeatedQuestLogText;
						antiImperialKingdomDefeatedQuestLogText.SetTextVariable("FACTION", kingdom.EncyclopediaLinkWithName);
						antiImperialKingdomDefeatedQuestLogText.SetTextVariable("PLAYER_FACTION", playerSupportedKingdom.EncyclopediaLinkWithName);
						base.AddLog(antiImperialKingdomDefeatedQuestLogText, false);
						if (Extensions.IsEmpty<Kingdom>(StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms))
						{
							Kingdom playerSupportedKingdom2 = playerSupportedKingdom;
							foreach (Kingdom kingdom3 in Kingdom.All.Where((Kingdom t) => !t.IsEliminated && StoryModeData.IsKingdomImperial(t)))
							{
								if (kingdom3 != playerSupportedKingdom2)
								{
									this.DefectClansOfKingdomToKingdom(kingdom3, playerSupportedKingdom2);
								}
							}
						}
					}
					base.CompleteQuestWithSuccess();
				}
			}

			private void DefectClansOfKingdomToKingdom(Kingdom defectorKingdom, Kingdom targetKingdom)
			{
				foreach (Clan clan in new List<Clan>(defectorKingdom.Clans))
				{
					if (clan == Clan.PlayerClan)
					{
						ChangeKingdomAction.ApplyByLeaveKingdom(Clan.PlayerClan, true);
					}
					else if (clan.IsUnderMercenaryService)
					{
						ChangeKingdomAction.ApplyByJoinFactionAsMercenary(clan, targetKingdom, clan.MercenaryAwardMultiplier, false);
					}
					else
					{
						ChangeKingdomAction.ApplyByJoinToKingdom(clan, targetKingdom, false);
					}
				}
				DestroyKingdomAction.Apply(defectorKingdom);
			}

			private void OnKingdomDestroyed(Kingdom kingdom)
			{
				if (StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom == kingdom)
				{
					base.AddLog(this._playerSupportedKingdomDestroyedLogText, false);
					base.CompleteQuestWithFail(null);
					TextObject textObject = new TextObject("{=atnUtXdO}The {KINGDOM_NAME} has been defeated. Your quest to restore the Empire has failed.", null);
					if (!StoryModeManager.Current.MainStoryLine.IsOnAntiImperialQuestLine)
					{
						textObject = new TextObject("{=r48aEAbq}The {KINGDOM_NAME} has been defeated. Your quest to destroy the Empire has failed.", null);
					}
					textObject.SetTextVariable("KINGDOM_NAME", StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom.Name);
					MBInformationManager.AddQuickInformation(textObject, 0, null, "");
				}
			}

			private float CalculateWarScoreForKingdom(Kingdom kingdom)
			{
				float num = 0f;
				foreach (Settlement settlement in kingdom.Settlements)
				{
					num += this.GetWarScoreOfSettlement(settlement);
				}
				num += kingdom.TotalStrength;
				return num;
			}

			private float GetWarScoreOfSettlement(Settlement settlement)
			{
				float num = 0f;
				if (settlement.IsTown)
				{
					num = 3000f;
				}
				else if (settlement.IsCastle)
				{
					num = 1000f;
				}
				return num;
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.OnKingdomDestroyed));
				CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnCampaignQuestCompleted));
				CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.FactionStrengthsUpdated));
				CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.SettlementOwnerChanged));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			}

			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (clan == Clan.PlayerClan && oldKingdom == StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom)
				{
					base.CompleteQuestWithCancel(null);
				}
			}

			private void OnCampaignQuestCompleted(QuestBase completedQuest, QuestBase.QuestCompleteDetails detail)
			{
				if (completedQuest != this && completedQuest is DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest)
				{
					this.UpdateWarProgressWithKingdom();
					StoryModeManager.Current.MainStoryLine.ThirdPhase.CompleteThirdPhase(detail);
				}
			}

			private void FactionStrengthsUpdated()
			{
				this.UpdateWarProgressWithKingdom();
			}

			private void SettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
			{
				if (((newOwner != null && newOwner.MapFaction == this._oppositionKingdom) || (oldOwner != null && oldOwner.MapFaction == this._oppositionKingdom)) && this.GetWarScoreOfSettlement(settlement) > 0f)
				{
					this.UpdateWarProgressWithKingdom();
				}
			}

			protected override void OnStartQuest()
			{
				DefeatTheConspiracyQuestBehavior.OppositionData oppositionData = new DefeatTheConspiracyQuestBehavior.OppositionData();
				oppositionData.InitialWarScore = this.CalculateWarScoreForKingdom(this._oppositionKingdom);
				oppositionData.ReinforcedWarScore = 0f;
				TextObject defeatOpposingKingdomsQuestLogText = this._defeatOpposingKingdomsQuestLogText;
				TextObject defeatOpposingKingdomsProgressLogText = this._defeatOpposingKingdomsProgressLogText;
				defeatOpposingKingdomsQuestLogText.SetTextVariable("FACTION", this._oppositionKingdom.EncyclopediaLinkWithName);
				defeatOpposingKingdomsProgressLogText.SetTextVariable("FACTION", this._oppositionKingdom.EncyclopediaLinkWithName);
				oppositionData.QuestLog = base.AddTwoWayContinuousLog(defeatOpposingKingdomsQuestLogText, defeatOpposingKingdomsProgressLogText, 0, 100, false);
				this._oppositionData = oppositionData;
			}

			public void CalculateReinforcedWarScore()
			{
				this._oppositionData.ReinforcedWarScore = this.CalculateWarScoreForKingdom(this._oppositionKingdom);
			}

			internal static void AutoGeneratedStaticCollectObjectsDefeatTheConspiracyQuest(object o, List<object> collectedObjects)
			{
				((DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._oppositionKingdom);
				collectedObjects.Add(this._oppositionData);
			}

			internal static object AutoGeneratedGetMemberValue_oppositionKingdom(object o)
			{
				return ((DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest)o)._oppositionKingdom;
			}

			internal static object AutoGeneratedGetMemberValue_oppositionData(object o)
			{
				return ((DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest)o)._oppositionData;
			}

			private const int ProgressTrackerRange = 100;

			[SaveableField(100)]
			private Kingdom _oppositionKingdom;

			[SaveableField(110)]
			private DefeatTheConspiracyQuestBehavior.OppositionData _oppositionData;
		}
	}
}
