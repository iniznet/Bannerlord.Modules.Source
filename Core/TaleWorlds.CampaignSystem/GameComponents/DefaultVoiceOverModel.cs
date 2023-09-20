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
	// Token: 0x0200014C RID: 332
	public class DefaultVoiceOverModel : VoiceOverModel
	{
		// Token: 0x06001811 RID: 6161 RVA: 0x00079DFC File Offset: 0x00077FFC
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
			Debug.Print("[VOICEOVER]Sound path found: " + BasePath.Name + text2, 5, Debug.DebugColor.White, 17592186044416UL);
			text2 = text2.Replace("$PLATFORM", "PC");
			return text2 + ".ogg";
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x0007A1AC File Offset: 0x000783AC
		private void CheckPossibleMatches(VoiceObject voiceObject, List<string> possibleMatches, ref List<string> possibleVoicePaths, bool doubleCheckForGender = false, bool isFemale = false)
		{
			foreach (string text in possibleMatches)
			{
				foreach (string text2 in voiceObject.VoicePaths)
				{
					if (Regex.Match(text2, text, RegexOptions.IgnoreCase).Success && !possibleVoicePaths.Contains(text2))
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

		// Token: 0x06001813 RID: 6163 RVA: 0x0007A29C File Offset: 0x0007849C
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

		// Token: 0x04000881 RID: 2177
		private const string ImperialHighClass = "imperial_high";

		// Token: 0x04000882 RID: 2178
		private const string ImperialLowClass = "imperial_low";

		// Token: 0x04000883 RID: 2179
		private const string VlandianClass = "vlandian";

		// Token: 0x04000884 RID: 2180
		private const string SturgianClass = "sturgian";

		// Token: 0x04000885 RID: 2181
		private const string KhuzaitClass = "khuzait";

		// Token: 0x04000886 RID: 2182
		private const string AseraiClass = "aserai";

		// Token: 0x04000887 RID: 2183
		private const string BattanianClass = "battanian";

		// Token: 0x04000888 RID: 2184
		private const string ForestBanditClass = "forest_bandits";

		// Token: 0x04000889 RID: 2185
		private const string SeaBanditClass = "sea_raiders";

		// Token: 0x0400088A RID: 2186
		private const string MountainBanditClass = "mountain_bandits";

		// Token: 0x0400088B RID: 2187
		private const string DesertBanditClass = "desert_bandits";

		// Token: 0x0400088C RID: 2188
		private const string SteppeBanditClass = "steppe_bandits";

		// Token: 0x0400088D RID: 2189
		private const string LootersClass = "looters";

		// Token: 0x0400088E RID: 2190
		private const string Male = "male";

		// Token: 0x0400088F RID: 2191
		private const string Female = "female";

		// Token: 0x04000890 RID: 2192
		private const string GenericPersonaId = "generic";
	}
}
