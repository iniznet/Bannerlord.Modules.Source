﻿using System;
using System.Text.RegularExpressions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Helpers
{
	public static class StringHelpers
	{
		public static string SplitCamelCase(string text)
		{
			return Regex.Replace(text, "((?<=\\p{Ll})\\p{Lu})|((?!\\A)\\p{Lu}(?>\\p{Ll}))", " $0");
		}

		public static string CamelCaseToSnakeCase(string camelCaseString)
		{
			string text = "((?<=.)[A-Z][a-zA-Z]*)|((?<=[a-zA-Z])\\d+)";
			string text2 = "_$1$2";
			return new Regex(text).Replace(camelCaseString, text2).ToLower();
		}

		public static void SetSettlementProperties(string tag, Settlement settlement, TextObject parent = null, bool isRepeatable = false)
		{
			TextObject textObject = new TextObject("", null);
			textObject.SetTextVariable("NAME", settlement.Name);
			textObject.SetTextVariable("LINK", settlement.EncyclopediaLinkWithName);
			if (isRepeatable)
			{
				ConversationSentence.SelectedRepeatLine.SetTextVariable(tag, textObject);
				return;
			}
			if (parent != null)
			{
				parent.SetTextVariable(tag, textObject);
				return;
			}
			MBTextManager.SetTextVariable(tag, textObject, false);
		}

		public static void SetRepeatableCharacterProperties(string tag, CharacterObject character, bool includeDetails = false)
		{
			TextObject characterProperties = StringHelpers.GetCharacterProperties(character, includeDetails);
			ConversationSentence.SelectedRepeatLine.SetTextVariable(tag, characterProperties);
		}

		private static TextObject GetCharacterProperties(CharacterObject character, bool includeDetails)
		{
			TextObject textObject = new TextObject("", null);
			textObject.SetTextVariable("NAME", character.Name);
			textObject.SetTextVariable("GENDER", character.IsFemale ? 1 : 0);
			textObject.SetTextVariable("LINK", character.EncyclopediaLinkWithName);
			if (character.IsHero)
			{
				if (character.HeroObject.FirstName != null)
				{
					textObject.SetTextVariable("FIRSTNAME", character.HeroObject.FirstName);
				}
				else
				{
					textObject.SetTextVariable("FIRSTNAME", character.Name);
				}
				if (includeDetails)
				{
					textObject.SetTextVariable("AGE", (int)MathF.Round(character.Age, 2));
					if (character.HeroObject.MapFaction != null)
					{
						textObject.SetTextVariable("FACTION", character.HeroObject.MapFaction.Name);
					}
					else
					{
						textObject.SetTextVariable("FACTION", character.Culture.Name);
					}
					if (character.HeroObject.Clan != null)
					{
						textObject.SetTextVariable("CLAN", character.HeroObject.Clan.Name);
					}
					else
					{
						textObject.SetTextVariable("CLAN", character.Culture.Name);
					}
				}
			}
			return textObject;
		}

		public static TextObject SetCharacterProperties(string tag, CharacterObject character, TextObject parent = null, bool includeDetails = false)
		{
			TextObject characterProperties = StringHelpers.GetCharacterProperties(character, includeDetails);
			if (parent != null)
			{
				parent.SetTextVariable(tag, characterProperties);
			}
			else
			{
				MBTextManager.SetTextVariable(tag, characterProperties, false);
			}
			return characterProperties;
		}
	}
}
