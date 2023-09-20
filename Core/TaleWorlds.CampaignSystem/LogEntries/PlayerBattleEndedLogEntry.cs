﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	public class PlayerBattleEndedLogEntry : LogEntry, IChatNotification
	{
		internal static void AutoGeneratedStaticCollectObjectsPlayerBattleEndedLogEntry(object o, List<object> collectedObjects)
		{
			((PlayerBattleEndedLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._winnerSideHero);
			collectedObjects.Add(this._defeatedSideHero);
			collectedObjects.Add(this._defeatedSideClan);
			collectedObjects.Add(this._winnerSideClan);
			collectedObjects.Add(this._defeatedSidePartyName);
			collectedObjects.Add(this._witnesses);
			collectedObjects.Add(this._capturedSettlement);
		}

		internal static object AutoGeneratedGetMemberValue_winnerSideHero(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._winnerSideHero;
		}

		internal static object AutoGeneratedGetMemberValue_defeatedSideHero(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._defeatedSideHero;
		}

		internal static object AutoGeneratedGetMemberValue_defeatedSideClan(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._defeatedSideClan;
		}

		internal static object AutoGeneratedGetMemberValue_winnerSideClan(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._winnerSideClan;
		}

		internal static object AutoGeneratedGetMemberValue_defeatedSidePartyName(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._defeatedSidePartyName;
		}

		internal static object AutoGeneratedGetMemberValue_defeatedSidePartyIsSettlement(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._defeatedSidePartyIsSettlement;
		}

		internal static object AutoGeneratedGetMemberValue_defeatedSidePartyIsBanditFaction(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._defeatedSidePartyIsBanditFaction;
		}

		internal static object AutoGeneratedGetMemberValue_witnesses(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._witnesses;
		}

		internal static object AutoGeneratedGetMemberValue_isAgainstGreatOdds(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._isAgainstGreatOdds;
		}

		internal static object AutoGeneratedGetMemberValue_isEasyPlayerVictory(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._isEasyPlayerVictory;
		}

		internal static object AutoGeneratedGetMemberValue_isPlayerLastStand(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._isPlayerLastStand;
		}

		internal static object AutoGeneratedGetMemberValue_isAgainstCaravan(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._isAgainstCaravan;
		}

		internal static object AutoGeneratedGetMemberValue_playerVictory(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._playerVictory;
		}

		internal static object AutoGeneratedGetMemberValue_playerWasAttacker(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._playerWasAttacker;
		}

		internal static object AutoGeneratedGetMemberValue_capturedSettlement(object o)
		{
			return ((PlayerBattleEndedLogEntry)o)._capturedSettlement;
		}

		public bool IsVisibleNotification
		{
			get
			{
				return this._defeatedSideHero != null && this._winnerSideHero != null;
			}
		}

		public PlayerBattleEndedLogEntry(MapEvent mapEvent)
		{
			PartyBase leaderParty = ((mapEvent.BattleState == BattleState.AttackerVictory) ? mapEvent.AttackerSide : mapEvent.DefenderSide).LeaderParty;
			PartyBase leaderParty2 = mapEvent.GetMapEventSide(mapEvent.DefeatedSide).LeaderParty;
			this._winnerSideHero = leaderParty.LeaderHero;
			this._defeatedSideHero = leaderParty2.LeaderHero;
			this._defeatedSidePartyIsSettlement = leaderParty2.IsSettlement && (leaderParty2.Settlement.IsTown || leaderParty2.Settlement.IsCastle);
			if (this._defeatedSidePartyIsSettlement)
			{
				this._capturedSettlement = leaderParty2.Settlement;
			}
			this._defeatedSidePartyIsBanditFaction = leaderParty2.MapFaction.IsBanditFaction;
			this._defeatedSidePartyName = leaderParty2.Name;
			this._winnerSideClan = this.GetClanOf(leaderParty);
			this._defeatedSideClan = this.GetClanOf(leaderParty2);
			BattleSideEnum playerSide = PlayerEncounter.Current.PlayerSide;
			MapEventSide mapEventSide = mapEvent.GetMapEventSide(playerSide);
			MapEventSide mapEventSide2 = mapEvent.GetMapEventSide(playerSide.GetOppositeSide());
			float strengthRatio = mapEventSide.StrengthRatio;
			int casualties = mapEventSide.Casualties;
			int casualties2 = mapEventSide2.Casualties;
			this._playerVictory = playerSide == mapEvent.WinningSide;
			this._playerWasAttacker = playerSide == mapEvent.AttackerSide.MissionSide;
			this._isAgainstGreatOdds = strengthRatio > 1.5f;
			this._isEasyPlayerVictory = strengthRatio < 0.5f && casualties * 3 < casualties2 && playerSide == mapEvent.WinningSide;
			if (this._defeatedSidePartyIsSettlement)
			{
				this._isEasyPlayerVictory = strengthRatio < 0.25f && casualties * 3 < casualties2 && playerSide == mapEvent.WinningSide;
			}
			this._isPlayerLastStand = playerSide == mapEvent.DefeatedSide && casualties2 > casualties;
			MobileParty mobileParty = mapEventSide2.Parties[0].Party.MobileParty;
			this._isAgainstCaravan = playerSide == mapEvent.WinningSide && mobileParty != null && mobileParty.IsCaravan;
			Dictionary<Hero, short> dictionary = new Dictionary<Hero, short>();
			if (mapEvent != null)
			{
				foreach (PartyBase partyBase in mapEvent.InvolvedParties)
				{
					foreach (TroopRosterElement troopRosterElement in partyBase.MemberRoster.GetTroopRoster())
					{
						if (troopRosterElement.Character.HeroObject != null && !dictionary.ContainsKey(troopRosterElement.Character.HeroObject))
						{
							dictionary.Add(troopRosterElement.Character.HeroObject, (partyBase.Side == MobileParty.MainParty.Party.Side) ? 1 : -1);
						}
					}
				}
			}
			this._witnesses = dictionary.GetReadOnlyDictionary<Hero, short>();
		}

		private Clan GetClanOf(PartyBase party)
		{
			MobileParty mobileParty = party.MobileParty;
			if (((mobileParty != null) ? mobileParty.ActualClan : null) != null)
			{
				return party.MobileParty.ActualClan;
			}
			if (party.Owner != null)
			{
				if (!party.Owner.IsNotable)
				{
					return party.Owner.Clan;
				}
				return party.Owner.HomeSettlement.OwnerClan;
			}
			else
			{
				if (party.IsSettlement)
				{
					return party.Settlement.OwnerClan;
				}
				if (party.LeaderHero != null)
				{
					return party.LeaderHero.Clan;
				}
				return CampaignData.NeutralFaction;
			}
		}

		public override ImportanceEnum GetImportanceForClan(Clan clan)
		{
			return ImportanceEnum.Zero;
		}

		public override void GetConversationScoreAndComment(Hero talkTroop, bool findString, out string comment, out ImportanceEnum score)
		{
			short num;
			if (!this._witnesses.TryGetValue(talkTroop, out num))
			{
				num = -2;
			}
			score = ImportanceEnum.Zero;
			comment = "";
			bool flag = this._winnerSideHero == Hero.MainHero;
			if (this._playerVictory)
			{
				if (this._capturedSettlement != null)
				{
					if (Hero.MainHero.CurrentSettlement == this._capturedSettlement && Hero.OneToOneConversationHero.IsNotable)
					{
						score = ImportanceEnum.QuiteImportant;
						comment = "str_comment_endplayerbattle_you_stormed_this_city";
						return;
					}
					if (num == 1)
					{
						score = ImportanceEnum.SomewhatImportant;
						if (findString)
						{
							MBTextManager.SetTextVariable("ENEMY_SETTLEMENT", this._defeatedSidePartyName, false);
							if (this._isEasyPlayerVictory)
							{
								comment = "str_comment_endplayerbattle_we_stormed_castle_easy";
								return;
							}
							comment = "str_comment_endplayerbattle_we_stormed_castle";
							return;
						}
					}
					else if (flag && this._defeatedSideClan == talkTroop.Clan)
					{
						score = ImportanceEnum.SomewhatImportant;
						if (findString)
						{
							MBTextManager.SetTextVariable("SETTLEMENT_NAME", this._defeatedSidePartyName, false);
							comment = "str_comment_endplayerbattle_you_captured_our_castle";
							return;
						}
					}
					else if (flag)
					{
						score = ImportanceEnum.SomewhatImportant;
						if (findString)
						{
							MBTextManager.SetTextVariable("SETTLEMENT_NAME", this._defeatedSidePartyName, false);
							comment = "str_comment_endplayerbattle_you_stormed_castle";
							return;
						}
					}
				}
				else if (this._defeatedSideHero != null && this._defeatedSideHero == talkTroop && flag)
				{
					score = ImportanceEnum.SomewhatImportant;
					if (findString)
					{
						comment = "str_comment_endplayerbattle_you_defeated_me";
						return;
					}
				}
				else if (this._defeatedSideHero != null && num == 1 && talkTroop.MapFaction == Hero.MainHero.MapFaction && this._defeatedSideHero.MapFaction != Hero.MainHero.MapFaction)
				{
					score = ImportanceEnum.SomewhatImportant;
					if (findString)
					{
						MBTextManager.SetTextVariable("ENEMY_TERM", FactionHelper.GetTermUsedByOtherFaction(this._defeatedSideHero.MapFaction, talkTroop.Clan, false), false);
						MBTextManager.SetTextVariable("DEFEATED_PARTY_LEADER", this._defeatedSideHero.Name, false);
						comment = "str_comment_endplayerbattle_we_defeated_enemy";
						return;
					}
				}
				else if (this._defeatedSidePartyIsBanditFaction)
				{
					if (num == 1 && this._isEasyPlayerVictory)
					{
						score = ImportanceEnum.SomewhatImportant;
						if (findString)
						{
							comment = "str_comment_endplayerbattle_we_hunted_bandit_easy";
							return;
						}
					}
					else if (num == 1)
					{
						score = ImportanceEnum.SomewhatImportant;
						if (findString)
						{
							comment = "str_comment_endplayerbattle_we_hunted_bandit";
							return;
						}
					}
					else if (talkTroop.IsMerchant)
					{
						score = ImportanceEnum.SomewhatImportant;
						if (findString)
						{
							comment = "str_comment_endplayerbattle_you_hunted_bandit_merchant";
							return;
						}
					}
				}
				else if (this._isAgainstCaravan)
				{
					if (this._defeatedSideClan != null && this._defeatedSideClan.Leader == talkTroop)
					{
						score = ImportanceEnum.SomewhatImportant;
						if (findString)
						{
							comment = "str_comment_endplayerbattle_you_accosted_my_caravan";
							return;
						}
					}
				}
				else
				{
					bool flag2;
					if (this._defeatedSideHero != null)
					{
						IFaction faction2;
						if (this._defeatedSideClan.Kingdom != null)
						{
							IFaction faction = this._defeatedSideClan.Kingdom;
							faction2 = faction;
						}
						else
						{
							IFaction faction = this._defeatedSideClan;
							faction2 = faction;
						}
						if (faction2 == talkTroop.MapFaction)
						{
							flag2 = Hero.MainHero.MapFaction != talkTroop.MapFaction;
							goto IL_284;
						}
					}
					flag2 = false;
					IL_284:
					if (flag2 && flag)
					{
						int num2 = -5;
						num2 -= talkTroop.CharacterObject.GetTraitLevel(DefaultTraits.Mercy) * 5;
						num2 -= talkTroop.CharacterObject.GetTraitLevel(DefaultTraits.Generosity) * 5;
						if (talkTroop.GetRelation(this._defeatedSideHero) < num2)
						{
							score = ImportanceEnum.SomewhatImportant;
							if (findString)
							{
								string text = ConversationHelper.HeroRefersToHero(talkTroop, this._defeatedSideHero, true);
								MBTextManager.SetTextVariable("DEFEATED_LEADER_RELATIONSHIP", text, false);
								this._defeatedSideHero.SetTextVariables();
								MBTextManager.SetTextVariable("DEFEATED_LEADER", this._defeatedSideHero.Name, false);
								comment = "str_comment_endplayerbattle_you_defeated_my_ally_disrespectful";
								return;
							}
						}
						else
						{
							score = ImportanceEnum.SomewhatImportant;
							if (findString)
							{
								string text2 = ConversationHelper.HeroRefersToHero(talkTroop, this._defeatedSideHero, true);
								MBTextManager.SetTextVariable("DEFEATED_LEADER_RELATIONSHIP", text2, false);
								this._defeatedSideHero.SetTextVariables();
								MBTextManager.SetTextVariable("DEFEATED_LEADER", this._defeatedSideHero.Name, false);
								comment = "str_comment_endplayerbattle_you_defeated_my_ally";
								return;
							}
						}
					}
					else
					{
						bool flag3;
						if (this._defeatedSideHero != null && talkTroop.MapFaction == Hero.MainHero.MapFaction)
						{
							IFaction faction3;
							if (this._defeatedSideClan.Kingdom != null)
							{
								IFaction faction = this._defeatedSideClan.Kingdom;
								faction3 = faction;
							}
							else
							{
								IFaction faction = this._defeatedSideClan;
								faction3 = faction;
							}
							flag3 = FactionManager.IsAtWarAgainstFaction(faction3, talkTroop.MapFaction);
						}
						else
						{
							flag3 = false;
						}
						if (flag3 && flag)
						{
							if (this._isAgainstGreatOdds)
							{
								score = ImportanceEnum.SomewhatImportant;
								if (findString)
								{
									MBTextManager.SetTextVariable("DEFEATED_PARTY_LEADER", this._defeatedSideHero.Name, false);
									this._defeatedSideHero.SetTextVariables();
									comment = "str_comment_endplayerbattle_you_defeated_our_enemy_great_battle";
									return;
								}
							}
							else
							{
								score = ImportanceEnum.SomewhatImportant;
								if (findString)
								{
									MBTextManager.SetTextVariable("DEFEATED_PARTY_LEADER", this._defeatedSideHero.Name, false);
									this._defeatedSideHero.SetTextVariables();
									comment = "str_comment_endplayerbattle_you_defeated_our_enemy";
									return;
								}
							}
						}
					}
				}
			}
			else if (!this._playerVictory)
			{
				if (this._winnerSideHero == talkTroop && flag)
				{
					score = ImportanceEnum.SomewhatImportant;
					if (findString)
					{
						comment = "str_comment_endplayerbattle_i_defeated_you";
						return;
					}
				}
				else if (this._winnerSideHero == talkTroop && num == -1)
				{
					score = ImportanceEnum.SomewhatImportant;
					if (findString)
					{
						comment = "str_comment_endplayerbattle_we_defeated_you";
						return;
					}
				}
				else if (this._winnerSideHero != null && flag)
				{
					IFaction faction4;
					if (this._winnerSideClan.Kingdom != null)
					{
						IFaction faction = this._winnerSideClan.Kingdom;
						faction4 = faction;
					}
					else
					{
						IFaction faction = this._winnerSideClan;
						faction4 = faction;
					}
					if (faction4 == talkTroop.MapFaction && talkTroop.MapFaction != Hero.MainHero.MapFaction && flag)
					{
						score = ImportanceEnum.SomewhatImportant;
						if (findString)
						{
							MBTextManager.SetTextVariable("VICTORIOUS_PARTY_LEADER", this._winnerSideHero.Name, false);
							comment = "str_comment_endplayerbattle_my_ally_defeated_you";
							return;
						}
					}
					else if (num == 1 && this._winnerSideHero.MapFaction != Hero.MainHero.MapFaction && talkTroop.MapFaction == Hero.MainHero.MapFaction)
					{
						score = ImportanceEnum.SomewhatImportant;
						if (findString)
						{
							MBTextManager.SetTextVariable("VICTORIOUS_PARTY_LEADER", this._winnerSideHero.Name, false);
							MBTextManager.SetTextVariable("ENEMY_TERM", FactionHelper.GetTermUsedByOtherFaction(this._winnerSideHero.MapFaction, talkTroop.MapFaction, false), false);
							comment = "str_comment_endplayerbattle_we_were_defeated";
							return;
						}
					}
					else
					{
						bool flag4;
						if (talkTroop.MapFaction == Hero.MainHero.MapFaction)
						{
							IFaction mapFaction = talkTroop.MapFaction;
							IFaction faction5;
							if (this._winnerSideClan.Kingdom != null)
							{
								IFaction faction = this._winnerSideClan.Kingdom;
								faction5 = faction;
							}
							else
							{
								IFaction faction = this._winnerSideClan;
								faction5 = faction;
							}
							flag4 = FactionManager.IsAtWarAgainstFaction(mapFaction, faction5);
						}
						else
						{
							flag4 = false;
						}
						if (flag4 && flag)
						{
							if (this._isPlayerLastStand)
							{
								score = ImportanceEnum.SomewhatImportant;
								if (findString)
								{
									MBTextManager.SetTextVariable("VICTORIOUS_PARTY_LEADER", this._winnerSideHero.Name, false);
									MBTextManager.SetTextVariable("ENEMY_TERM", FactionHelper.GetTermUsedByOtherFaction(this._winnerSideHero.MapFaction, talkTroop.MapFaction, false), false);
									comment = "str_comment_endplayerbattle_our_enemy_defeated_you_pyrrhic";
									return;
								}
							}
							else
							{
								score = ImportanceEnum.SomewhatImportant;
								if (findString)
								{
									MBTextManager.SetTextVariable("VICTORIOUS_PARTY_LEADER", this._winnerSideHero.Name, false);
									MBTextManager.SetTextVariable("ENEMY_TERM", FactionHelper.GetTermUsedByOtherFaction(this._winnerSideHero.MapFaction, talkTroop.MapFaction, false), false);
									comment = "str_comment_endplayerbattle_our_enemy_defeated_you";
								}
							}
						}
					}
				}
			}
		}

		public override string ToString()
		{
			return this.GetNotificationText().ToString();
		}

		public TextObject GetNotificationText()
		{
			TextObject textObject = TextObject.Empty;
			if (this._winnerSideHero != null && this._defeatedSideHero != null)
			{
				textObject = GameTexts.FindText("str_destroy_player_party", null);
				StringHelpers.SetCharacterProperties("HERO_1", this._winnerSideHero.CharacterObject, textObject, false);
				StringHelpers.SetCharacterProperties("HERO_2", this._defeatedSideHero.CharacterObject, textObject, false);
			}
			return textObject;
		}

		[SaveableField(280)]
		private readonly Hero _winnerSideHero;

		[SaveableField(281)]
		private readonly Hero _defeatedSideHero;

		[SaveableField(282)]
		private readonly Clan _defeatedSideClan;

		[SaveableField(283)]
		private readonly Clan _winnerSideClan;

		[SaveableField(284)]
		private readonly TextObject _defeatedSidePartyName;

		[SaveableField(285)]
		private readonly bool _defeatedSidePartyIsSettlement;

		[SaveableField(286)]
		private readonly bool _defeatedSidePartyIsBanditFaction;

		[SaveableField(287)]
		private readonly MBReadOnlyDictionary<Hero, short> _witnesses;

		[SaveableField(288)]
		private readonly bool _isAgainstGreatOdds;

		[SaveableField(289)]
		private readonly bool _isEasyPlayerVictory;

		[SaveableField(290)]
		private readonly bool _isPlayerLastStand;

		[SaveableField(291)]
		private readonly bool _isAgainstCaravan;

		[SaveableField(292)]
		private readonly bool _playerVictory;

		[SaveableField(293)]
		private readonly bool _playerWasAttacker;

		[SaveableField(294)]
		private readonly Settlement _capturedSettlement;
	}
}