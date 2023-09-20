using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class PlayerCaptivityCampaignBehavior : CampaignBehaviorBase, ICaptivityCampaignBehavior
	{
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_isPlayerExecuted", ref this._isMainHeroExecuted);
			dataStore.SyncData<Hero>("_mainHeroExecuter", ref this._mainHeroExecuter);
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnPrisonerTaken));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
		}

		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddGameMenus(campaignGameStarter);
		}

		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (this._isMainHeroExecuted)
			{
				this._isMainHeroExecuted = false;
				KillCharacterAction.ApplyByExecution(Hero.MainHero, this._mainHeroExecuter, true, false);
			}
		}

		private void OnPrisonerTaken(PartyBase capturer, Hero prisoner)
		{
			if (prisoner == Hero.MainHero && capturer.LeaderHero != null && (float)capturer.LeaderHero.GetRelation(prisoner) < -30f && MBRandom.RandomFloat <= 0.02f)
			{
				this._isMainHeroExecuted = true;
				this._mainHeroExecuter = capturer.LeaderHero;
			}
		}

		private Hero FindEnemyPrisonerToSwapWithPlayer()
		{
			IFaction mapFaction = Hero.MainHero.MapFaction;
			IFaction mapFaction2 = PlayerCaptivity.CaptorParty.MapFaction;
			foreach (Settlement settlement in mapFaction.Settlements)
			{
				foreach (CharacterObject characterObject in settlement.Party.PrisonerHeroes)
				{
					if (characterObject.HeroObject.MapFaction == mapFaction2)
					{
						return characterObject.HeroObject;
					}
				}
			}
			return null;
		}

		private void AddGameMenus(CampaignGameStarter gameSystemInitializer)
		{
			gameSystemInitializer.AddGameMenu("menu_captivity_end_no_more_enemies", "{=gOsori1b}Your captors have no more use for you and aren't in a murderous mood, so they let you go.", new OnInitDelegate(this.game_menu_captivity_escape_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_end_no_more_enemies", "mno_continue", "{=veWOovVv}Continue...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				PlayerCaptivity.EndCaptivity();
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_captivity_end_by_ally_party_saved", "{=J2Iok9lT}An ally has paid your ransom.", new OnInitDelegate(this.game_menu_captivity_escape_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_end_by_ally_party_saved", "mno_continue", "{=veWOovVv}Continue...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				PlayerCaptivity.EndCaptivity();
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_captivity_end_by_party_removed", "{=8gF5qYw5}Your captors have been dispersed, and you are able to escape.", new OnInitDelegate(this.game_menu_captivity_escape_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_end_by_party_removed", "mno_continue", "{=veWOovVv}Continue...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				PlayerCaptivity.EndCaptivity();
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_captivity_end_wilderness_escape", "{=EVODEPGw}After painful days of being dragged about as a prisoner, you find a chance and escape from your captors!", new OnInitDelegate(this.game_menu_captivity_escape_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_end_wilderness_escape", "mno_continue", "{=veWOovVv}Continue...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				PlayerCaptivity.EndCaptivity();
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_escape_captivity_during_battle", "{=HYGKcgh6}Your captors engage in a battle. You take advantage of the confusion and escape.", new OnInitDelegate(this.game_menu_captivity_escape_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_escape_captivity_during_battle", "mno_continue", "{=veWOovVv}Continue...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				PlayerCaptivity.EndCaptivity();
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_released_after_battle", "{=GeoTk5b9}Your captors engage in a battle and they lost, you are released after battle.", new OnInitDelegate(this.game_menu_captivity_escape_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_released_after_battle", "mno_continue", "{=veWOovVv}Continue...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				Hero.MainHero.PartyBelongedToAsPrisoner.MobileParty.Ai.SetDoNotAttackMainParty(12);
				PlayerEncounter.ProtectPlayerSide(1f);
				GameMenu.ExitToLast();
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_captivity_end_propose_ransom_wilderness", "{=j5OqFCa6}After painful days of being dragged about as a prisoner, suddenly one of your captors comes near you with an offer; he proposes to free you in return for {MONEY_AMOUNT}{GOLD_ICON} of your hidden wealth. You decide to...", new OnInitDelegate(this.menu_captivity_end_propose_ransom_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_end_propose_ransom_wilderness", "mno_captivity_end_ransom_accept", "{=buKXELE3}Accept the offer.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Escape;
				return true;
			}, new GameMenuOption.OnConsequenceDelegate(this.game_menu_captivity_end_by_ransom_on_init), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_end_propose_ransom_wilderness", "captivity_end_ransom_deny", "{=L4Se89I6}Refuse him, wait for a better offer.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Wait;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				GameMenu.ExitToLast();
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_captivity_transfer_to_town", "{=ZEvChv7b}Your captors take you to {TOWN_NAME}. You are thrown into the dungeon there...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				MBTextManager.SetTextVariable("TOWN_NAME", PlayerCaptivity.CaptorParty.Settlement.Name, false);
				PlayerCaptivity.CaptorParty.SetAsCameraFollowParty();
			}, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_transfer_to_town", "mno_continue", "{=veWOovVv}Continue...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				GameMenu.ExitToLast();
				GameMenu.ActivateGameMenu("settlement_wait");
				Campaign.Current.TimeControlMode = Campaign.Current.LastTimeControlMode;
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_captivity_end_exchanged_with_prisoner", "{=qoqbaHCE}After days of imprisonment, you are finally set free when your captors exchange you with {PRISONER_LORD.LINK}.", new OnInitDelegate(this.game_menu_captivity_end_by_deal_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_end_exchanged_with_prisoner", "mno_continue", "{=veWOovVv}Continue...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				Hero.MainHero.PartyBelongedToAsPrisoner.MobileParty.Ai.SetDoNotAttackMainParty(12);
				PlayerEncounter.ProtectPlayerSide(1f);
				GameMenu.ExitToLast();
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_captivity_end_propose_ransom_in_prison", "{=pFzj52dE}You spend long hours in the sunless dank of the dungeon, more than you can count. Suddenly one of your captors enters your cell with an offer; he proposes to free you in return for {MONEY_AMOUNT}{GOLD_ICON} of your hidden wealth. You decide to...", new OnInitDelegate(this.menu_captivity_end_propose_ransom_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_end_propose_ransom_in_prison", "mno_captivity_end_ransom_accept", "{=buKXELE3}Accept the offer.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Escape;
				return true;
			}, new GameMenuOption.OnConsequenceDelegate(this.game_menu_captivity_end_by_ransom_on_init), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_end_propose_ransom_in_prison", "captivity_end_ransom_deny", "{=L4Se89I6}Refuse him, wait for a better offer.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Wait;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				GameMenu.ExitToLast();
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_captivity_castle_remain", "{=BLrwIj7s}More days pass in the darkness of your cell. You get through them as best you can, enduring the kicks and curses of the guards, watching your underfed body waste away more and more...", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_castle_remain", "mno_continue", "{=veWOovVv}Continue...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				GameMenu.ExitToLast();
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_captivity_end_prison_escape", "{=85kgOyBj}After painful days of being imprisoned in dungeon, you find a chance break free and escape from the settlement!", new OnInitDelegate(this.game_menu_captivity_escape_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_end_prison_escape", "mno_continue", "{=veWOovVv}Continue...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Continue;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				PlayerCaptivity.EndCaptivity();
			}, false, -1, false, null);
		}

		private void game_menu_captivity_escape_on_init(MenuCallbackArgs args)
		{
		}

		private void game_menu_captivity_end_by_deal_on_init(MenuCallbackArgs args)
		{
			StringHelpers.SetCharacterProperties("PRISONER_LORD", this.FindEnemyPrisonerToSwapWithPlayer().CharacterObject, null, false);
		}

		private void game_menu_captivity_end_by_ransom_on_init(MenuCallbackArgs args)
		{
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, Campaign.Current.PlayerCaptivity.CurrentRansomAmount, false);
			PlayerCaptivity.EndCaptivity();
		}

		private void menu_captivity_end_propose_ransom_on_init(MenuCallbackArgs args)
		{
			MBTextManager.SetTextVariable("MONEY_AMOUNT", Campaign.Current.PlayerCaptivity.CurrentRansomAmount);
		}

		public void CheckCaptivityChange(float dt)
		{
			if (PlayerCaptivity.CaptorParty.IsMobile && !PlayerCaptivity.CaptorParty.MobileParty.IsActive)
			{
				GameMenu.SwitchToMenu("menu_captivity_end_by_party_removed");
				return;
			}
			if (PlayerCaptivity.CaptorParty.IsMobile && PlayerCaptivity.CaptorParty.MapFaction == Hero.MainHero.Clan)
			{
				GameMenu.SwitchToMenu("menu_captivity_end_by_ally_party_saved");
				return;
			}
			if (!FactionManager.IsAtWarAgainstFaction(PlayerCaptivity.CaptorParty.MapFaction, MobileParty.MainParty.MapFaction) && (PlayerCaptivity.CaptorParty.MapFaction == MobileParty.MainParty.MapFaction || (!Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingModerate(PlayerCaptivity.CaptorParty.MapFaction) && !Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingSevere(PlayerCaptivity.CaptorParty.MapFaction))))
			{
				GameMenu.SwitchToMenu("menu_captivity_end_no_more_enemies");
				return;
			}
			if (PlayerCaptivity.CaptorParty.IsMobile && PlayerCaptivity.CaptorParty.MobileParty.CurrentSettlement != null && PlayerCaptivity.CaptorParty.MobileParty.CurrentSettlement.IsTown && PlayerCaptivity.CaptorParty.MapFaction == PlayerCaptivity.CaptorParty.MobileParty.CurrentSettlement.MapFaction)
			{
				PlayerCaptivity.LastCheckTime = CampaignTime.Now;
				if (Game.Current.GameStateManager.ActiveState is MapState)
				{
					Campaign.Current.LastTimeControlMode = Campaign.Current.TimeControlMode;
				}
				PlayerCaptivity.CaptorParty = PlayerCaptivity.CaptorParty.MobileParty.CurrentSettlement.Party;
				GameMenu.SwitchToMenu("menu_captivity_transfer_to_town");
				return;
			}
			float playerProgress = Campaign.Current.PlayerProgress;
			float num = (0.4f + playerProgress * 0.4f) * 24f;
			num *= (Hero.MainHero.PartyBelongedToAsPrisoner.IsSettlement ? 2f : ((Hero.MainHero.PartyBelongedToAsPrisoner.IsMobile && Hero.MainHero.PartyBelongedToAsPrisoner.LeaderHero != null) ? 1.5f : 1f));
			if (this.CheckTimeElapsedMoreThanHours(PlayerCaptivity.LastCheckTime, num))
			{
				PlayerCaptivity.LastCheckTime = CampaignTime.Now;
				if (Campaign.Current.PlayerCaptivity.CountOfOffers == 0)
				{
					Campaign.Current.PlayerCaptivity.SetRansomAmount();
				}
				else
				{
					Campaign.Current.PlayerCaptivity.CurrentRansomAmount = MathF.Max((int)((float)Campaign.Current.PlayerCaptivity.CurrentRansomAmount * 0.8f - (float)Campaign.Current.PlayerCaptivity.CountOfOffers * 0.05f), 1);
				}
				float randomFloat = MBRandom.RandomFloat;
				float num2 = 0f;
				if (PlayerCaptivity.CaptorParty.IsMobile && PlayerCaptivity.CaptorParty.MapEvent != null)
				{
					int num3 = 0;
					int num4 = 0;
					foreach (PartyBase partyBase in PlayerCaptivity.CaptorParty.MapEvent.InvolvedParties)
					{
						if (partyBase.Side == PlayerCaptivity.CaptorParty.Side)
						{
							num3 += partyBase.MemberRoster.TotalManCount;
						}
						else
						{
							num4 += partyBase.MemberRoster.TotalManCount;
						}
					}
					if ((float)num3 < (float)num4 * 3f + 1f)
					{
						num2 = 1f - (float)num3 / ((float)num4 * 3f + 1f);
						num2 /= 2f;
					}
				}
				float num5 = ((float)Campaign.Current.PlayerCaptivity.CountOfOffers + 1f) / 8f;
				if (num2 > 0f)
				{
					num5 = MathF.Pow(num5, 1f - num2);
				}
				if (Hero.MainHero.PartyBelongedToAsPrisoner != null)
				{
					if (Hero.MainHero.PartyBelongedToAsPrisoner.IsMobile && Hero.MainHero.PartyBelongedToAsPrisoner.LeaderHero != null)
					{
						num5 *= MathF.Sqrt(num5);
					}
					else if (Hero.MainHero.PartyBelongedToAsPrisoner.IsSettlement)
					{
						if (Hero.MainHero.PartyBelongedToAsPrisoner.Settlement.IsHideout)
						{
							num5 *= MathF.Sqrt(num5);
						}
						else
						{
							num5 *= num5;
						}
					}
					if (Hero.MainHero.PartyBelongedToAsPrisoner.IsMobile && Hero.MainHero.GetPerkValue(DefaultPerks.Roguery.FleetFooted))
					{
						num5 *= 1f + DefaultPerks.Roguery.FleetFooted.SecondaryBonus * 0.01f;
					}
				}
				if (randomFloat < num5)
				{
					if (PlayerCaptivity.CaptorParty.IsMobile && PlayerCaptivity.CaptorParty.MapEvent != null)
					{
						GameMenu.SwitchToMenu("menu_escape_captivity_during_battle");
						return;
					}
					if (Hero.MainHero.CurrentSettlement == null)
					{
						GameMenu.SwitchToMenu("menu_captivity_end_wilderness_escape");
						return;
					}
					GameMenu.SwitchToMenu("menu_captivity_end_prison_escape");
					return;
				}
				else
				{
					Campaign.Current.PlayerCaptivity.CountOfOffers++;
					if (randomFloat < 0.5f && Campaign.Current.PlayerCaptivity.CurrentRansomAmount <= Hero.MainHero.Gold)
					{
						PartyBase partyBelongedToAsPrisoner = Hero.MainHero.PartyBelongedToAsPrisoner;
						if (((partyBelongedToAsPrisoner != null) ? partyBelongedToAsPrisoner.MapEvent : null) == null)
						{
							if (Hero.MainHero.CurrentSettlement != null)
							{
								GameMenu.SwitchToMenu("menu_captivity_end_propose_ransom_in_prison");
								return;
							}
							GameMenu.SwitchToMenu("menu_captivity_end_propose_ransom_wilderness");
						}
					}
				}
			}
		}

		private bool CheckTimeElapsedMoreThanHours(CampaignTime eventBeginTime, float hoursToWait)
		{
			float elapsedHoursUntilNow = eventBeginTime.ElapsedHoursUntilNow;
			float randomNumber = PlayerCaptivity.RandomNumber;
			return (double)hoursToWait * (0.5 + (double)randomNumber) < (double)elapsedHoursUntilNow;
		}

		private const float PlayerExecutionProbability = 0.02f;

		private const float PlayerExecutionRelationLimit = -30f;

		private const int MaxDaysOfImprisonment = 7;

		private bool _isMainHeroExecuted;

		private Hero _mainHeroExecuter;
	}
}
