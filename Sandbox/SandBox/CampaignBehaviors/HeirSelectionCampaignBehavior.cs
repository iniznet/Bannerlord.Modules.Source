using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.CampaignBehaviors
{
	public class HeirSelectionCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnBeforeMainCharacterDiedEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnBeforeMainCharacterDied));
			CampaignEvents.OnBeforePlayerCharacterChangedEvent.AddNonSerializedListener(this, new Action<Hero, Hero>(this.OnBeforePlayerCharacterChanged));
			CampaignEvents.OnPlayerCharacterChangedEvent.AddNonSerializedListener(this, new Action<Hero, Hero, MobileParty, bool>(this.OnPlayerCharacterChanged));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void OnBeforePlayerCharacterChanged(Hero oldPlayer, Hero newPlayer)
		{
			foreach (ItemRosterElement itemRosterElement in MobileParty.MainParty.ItemRoster)
			{
				this._itemsThatWillBeInherited.Add(itemRosterElement);
			}
			for (int i = 0; i < 12; i++)
			{
				if (!oldPlayer.BattleEquipment[i].IsEmpty)
				{
					this._equipmentsThatWillBeInherited.AddToCounts(oldPlayer.BattleEquipment[i], 1);
				}
				if (!oldPlayer.CivilianEquipment[i].IsEmpty)
				{
					this._equipmentsThatWillBeInherited.AddToCounts(oldPlayer.CivilianEquipment[i], 1);
				}
			}
		}

		private void OnPlayerCharacterChanged(Hero oldPlayer, Hero newPlayer, MobileParty newMainParty, bool isMainPartyChanged)
		{
			if (isMainPartyChanged)
			{
				newMainParty.ItemRoster.Add(this._itemsThatWillBeInherited);
			}
			newMainParty.ItemRoster.Add(this._equipmentsThatWillBeInherited);
			this._itemsThatWillBeInherited.Clear();
			this._equipmentsThatWillBeInherited.Clear();
		}

		private void OnBeforeMainCharacterDied(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			Dictionary<Hero, int> heirApparents = Hero.MainHero.Clan.GetHeirApparents();
			if (heirApparents.Count == 0)
			{
				if (PlayerEncounter.Current != null && (PlayerEncounter.Battle == null || !PlayerEncounter.Battle.IsFinalized))
				{
					PlayerEncounter.Finish(true);
				}
				Dictionary<TroopRosterElement, int> dictionary = new Dictionary<TroopRosterElement, int>();
				foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.Party.MemberRoster.GetTroopRoster())
				{
					if (troopRosterElement.Character != CharacterObject.PlayerCharacter)
					{
						dictionary.Add(troopRosterElement, troopRosterElement.Number);
					}
				}
				foreach (KeyValuePair<TroopRosterElement, int> keyValuePair in dictionary)
				{
					MobileParty.MainParty.Party.MemberRoster.RemoveTroop(keyValuePair.Key.Character, keyValuePair.Value, default(UniqueTroopDescriptor), 0);
				}
				Hero.MainHero.AddDeathMark(null, detail);
				CampaignEventDispatcher.Instance.OnGameOver();
				this.GameOverCleanup();
				this.ShowGameStatistics();
				Campaign.Current.OnGameOver();
				return;
			}
			if (Hero.MainHero.IsPrisoner)
			{
				EndCaptivityAction.ApplyByDeath(Hero.MainHero);
			}
			if (PlayerEncounter.Current != null && (PlayerEncounter.Battle == null || !PlayerEncounter.Battle.IsFinalized))
			{
				PlayerEncounter.Finish(true);
			}
			List<InquiryElement> list = new List<InquiryElement>();
			foreach (KeyValuePair<Hero, int> keyValuePair2 in heirApparents.OrderBy((KeyValuePair<Hero, int> x) => x.Value))
			{
				TextObject textObject = new TextObject("{=!}{HERO.NAME}", null);
				StringHelpers.SetCharacterProperties("HERO", keyValuePair2.Key.CharacterObject, textObject, false);
				textObject.SetTextVariable("POINT", keyValuePair2.Value);
				string heroPropertiesHint = HeirSelectionCampaignBehavior.GetHeroPropertiesHint(keyValuePair2.Key);
				list.Add(new InquiryElement(keyValuePair2.Key, textObject.ToString(), new ImageIdentifier(CharacterCode.CreateFrom(keyValuePair2.Key.CharacterObject)), true, heroPropertiesHint));
			}
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=iHYAEEfv}SELECT AN HEIR", null).ToString(), string.Empty, list, false, 1, 1, GameTexts.FindText("str_done", null).ToString(), string.Empty, new Action<List<InquiryElement>>(HeirSelectionCampaignBehavior.OnHeirSelectionOver), null, ""), false, false);
			Campaign.Current.TimeControlMode = 0;
		}

		private static string GetHeroPropertiesHint(Hero hero)
		{
			GameTexts.SetVariable("newline", "\n");
			string text = hero.Name.ToString();
			TextObject textObject = GameTexts.FindText("str_STR1_space_STR2", null);
			textObject.SetTextVariable("STR1", GameTexts.FindText("str_enc_sf_age", null).ToString());
			textObject.SetTextVariable("STR2", ((int)hero.Age).ToString());
			string text2 = GameTexts.FindText("str_attributes", null).ToString();
			foreach (CharacterAttribute characterAttribute in Attributes.All)
			{
				GameTexts.SetVariable("LEFT", characterAttribute.Name.ToString());
				GameTexts.SetVariable("RIGHT", hero.GetAttributeValue(characterAttribute));
				string text3 = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
				GameTexts.SetVariable("STR1", text2);
				GameTexts.SetVariable("STR2", text3);
				text2 = GameTexts.FindText("str_string_newline_string", null).ToString();
			}
			int num = 0;
			string text4 = GameTexts.FindText("str_skills", null).ToString();
			foreach (SkillObject skillObject in Skills.All)
			{
				int skillValue = hero.GetSkillValue(skillObject);
				if (skillValue > 50)
				{
					GameTexts.SetVariable("LEFT", skillObject.Name.ToString());
					GameTexts.SetVariable("RIGHT", skillValue);
					string text5 = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
					GameTexts.SetVariable("STR1", text4);
					GameTexts.SetVariable("STR2", text5);
					text4 = GameTexts.FindText("str_string_newline_string", null).ToString();
					num++;
				}
			}
			GameTexts.SetVariable("STR1", text);
			GameTexts.SetVariable("STR2", textObject.ToString());
			string text6 = GameTexts.FindText("str_string_newline_string", null).ToString();
			GameTexts.SetVariable("newline", "\n \n");
			GameTexts.SetVariable("STR1", text6);
			GameTexts.SetVariable("STR2", text2);
			text6 = GameTexts.FindText("str_string_newline_string", null).ToString();
			if (num > 0)
			{
				GameTexts.SetVariable("STR1", text6);
				GameTexts.SetVariable("STR2", text4);
				text6 = GameTexts.FindText("str_string_newline_string", null).ToString();
			}
			GameTexts.SetVariable("newline", "\n");
			return text6;
		}

		private static void OnHeirSelectionOver(List<InquiryElement> element)
		{
			ApplyHeirSelectionAction.ApplyByDeath(element[0].Identifier as Hero);
		}

		private void ShowGameStatistics()
		{
			object obj = new TextObject("{=oxb2FVz5}Clan Destroyed", null);
			TextObject textObject = new TextObject("{=T2GbF6lK}With no suitable heirs, the {CLAN_NAME} clan is no more. Your journey ends here.", null);
			textObject.SetTextVariable("CLAN_NAME", Clan.PlayerClan.Name);
			TextObject textObject2 = new TextObject("{=DM6luo3c}Continue", null);
			InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, false, textObject2.ToString(), "", delegate
			{
				GameOverState gameOverState = Game.Current.GameStateManager.CreateState<GameOverState>(new object[] { 1 });
				Game.Current.GameStateManager.CleanAndPushState(gameOverState, 0);
			}, null, "", 0f, null, null, null), true, false);
		}

		private void GameOverCleanup()
		{
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, Hero.MainHero.Gold, true);
			Campaign.Current.MainParty.Party.ItemRoster.Clear();
			Campaign.Current.MainParty.Party.MemberRoster.Clear();
			Campaign.Current.MainParty.Party.PrisonRoster.Clear();
			Campaign.Current.MainParty.IsVisible = false;
			Campaign.Current.CameraFollowParty = null;
			Campaign.Current.MainParty.IsActive = false;
			PartyBase.MainParty.SetVisualAsDirty();
			if (Hero.MainHero.MapFaction.IsKingdomFaction && Clan.PlayerClan.Kingdom.Leader == Hero.MainHero)
			{
				DestroyKingdomAction.ApplyByKingdomLeaderDeath(Clan.PlayerClan.Kingdom);
			}
		}

		private readonly ItemRoster _itemsThatWillBeInherited = new ItemRoster();

		private readonly ItemRoster _equipmentsThatWillBeInherited = new ItemRoster();
	}
}
