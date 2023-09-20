using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation.Tags;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultVoiceOverModel : VoiceOverModel
	{
		public override string GetSoundPathForCharacter(CharacterObject character, VoiceObject voiceObject)
		{
			if (voiceObject == null)
			{
				return "";
			}
			foreach (string text in voiceObject.VoicePaths)
			{
				Debug.Print("[VOICEOVER]Sound path search for: " + text, 0, Debug.DebugColor.White, 17592186044416UL);
			}
			string text2 = "";
			string text3 = character.StringId + "_" + (CharacterObject.PlayerCharacter.IsFemale ? "female" : "male");
			foreach (string text4 in voiceObject.VoicePaths)
			{
				if (text4.Contains(text3))
				{
					text2 = text4;
					break;
				}
				if (text4.Contains(character.StringId + "_"))
				{
					text2 = text4;
				}
			}
			if (string.IsNullOrEmpty(text2))
			{
				string accentClass = this.GetAccentClass(character.Culture, ConversationTagHelper.UsesHighRegister(character));
				Debug.Print("accentClass: " + accentClass, 0, Debug.DebugColor.White, 17592186044416UL);
				string text5 = (character.IsFemale ? "female" : "male");
				string stringId = character.GetPersona().StringId;
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				list2.Add(string.Concat(new string[] { ".+\\\\", accentClass, "_", text5, "_", stringId, "_.+" }));
				list2.Add(string.Concat(new string[] { ".+\\\\", accentClass, "_", text5, "_generic_.+" }));
				this.CheckPossibleMatches(voiceObject, list2, ref list, false, false);
				if (list.IsEmpty<string>())
				{
					list2.Clear();
					list2.Add(string.Concat(new string[] { ".+\\\\", accentClass, "_", stringId, "_.+" }));
					list2.Add(".+\\\\" + accentClass + "_generic_.+");
					list2.Add(string.Concat(new string[] { ".+\\\\", text5, "_", stringId, "_.+" }));
					list2.Add(".+\\\\" + text5 + "_generic_.+");
					this.CheckPossibleMatches(voiceObject, list2, ref list, false, false);
					if (list.IsEmpty<string>())
					{
						list2.Clear();
						list2.Add(".+\\\\" + stringId + "_.+");
						list2.Add(".+\\\\generic_.+");
						list2.Add(".+" + accentClass + "_.+");
						this.CheckPossibleMatches(voiceObject, list2, ref list, true, character.IsFemale);
					}
				}
				if (!list.IsEmpty<string>())
				{
					if (character.IsHero)
					{
						text2 = list[character.HeroObject.RandomInt(list.Count)];
					}
					else if (MobileParty.ConversationParty != null)
					{
						text2 = list[MobileParty.ConversationParty.RandomInt(list.Count)];
					}
					else
					{
						text2 = list.GetRandomElement<string>();
					}
				}
			}
			if (string.IsNullOrEmpty(text2))
			{
				return "";
			}
			Debug.Print("[VOICEOVER]Sound path found: " + BasePath.Name + text2, 0, Debug.DebugColor.White, 17592186044416UL);
			text2 = text2.Replace("$PLATFORM", "PC");
			return text2 + ".ogg";
		}

		private void CheckPossibleMatches(VoiceObject voiceObject, List<string> possibleMatches, ref List<string> possibleVoicePaths, bool doubleCheckForGender = false, bool isFemale = false)
		{
			foreach (string text in possibleMatches)
			{
				Regex regex = new Regex(text, RegexOptions.IgnoreCase);
				foreach (string text2 in voiceObject.VoicePaths)
				{
					if (regex.Match(text2).Success && !possibleVoicePaths.Contains(text2))
					{
						if (doubleCheckForGender && (text2.Contains("_male") || text2.Contains("_female")))
						{
							string text3 = (isFemale ? "_female" : "_male");
							if (text2.Contains(text3))
							{
								possibleVoicePaths.Add(text2);
							}
						}
						else
						{
							possibleVoicePaths.Add(text2);
						}
					}
				}
			}
		}

		public override string GetAccentClass(CultureObject culture, bool isHighClass)
		{
			if (culture.StringId == "empire")
			{
				if (isHighClass)
				{
					return "imperial_high";
				}
				return "imperial_low";
			}
			else
			{
				if (culture.StringId == "vlandia")
				{
					return "vlandian";
				}
				if (culture.StringId == "sturgia")
				{
					return "sturgian";
				}
				if (culture.StringId == "khuzait")
				{
					return "khuzait";
				}
				if (culture.StringId == "aserai")
				{
					return "aserai";
				}
				if (culture.StringId == "battania")
				{
					return "battanian";
				}
				if (culture.StringId == "forest_bandits")
				{
					return "forest_bandits";
				}
				if (culture.StringId == "sea_raiders")
				{
					return "sea_raiders";
				}
				if (culture.StringId == "mountain_bandits")
				{
					return "mountain_bandits";
				}
				if (culture.StringId == "desert_bandits")
				{
					return "desert_bandits";
				}
				if (culture.StringId == "steppe_bandits")
				{
					return "steppe_bandits";
				}
				if (culture.StringId == "looters")
				{
					return "looters";
				}
				return "";
			}
		}

		private const string ImperialHighClass = "imperial_high";

		private const string ImperialLowClass = "imperial_low";

		private const string VlandianClass = "vlandian";

		private const string SturgianClass = "sturgian";

		private const string KhuzaitClass = "khuzait";

		private const string AseraiClass = "aserai";

		private const string BattanianClass = "battanian";

		private const string ForestBanditClass = "forest_bandits";

		private const string SeaBanditClass = "sea_raiders";

		private const string MountainBanditClass = "mountain_bandits";

		private const string DesertBanditClass = "desert_bandits";

		private const string SteppeBanditClass = "steppe_bandits";

		private const string LootersClass = "looters";

		private const string Male = "male";

		private const string Female = "female";

		private const string GenericPersonaId = "generic";
	}
}
