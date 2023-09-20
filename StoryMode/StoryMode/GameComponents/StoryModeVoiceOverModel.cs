using System;
using System.Linq;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.GameComponents
{
	public class StoryModeVoiceOverModel : DefaultVoiceOverModel
	{
		public override string GetSoundPathForCharacter(CharacterObject character, VoiceObject voiceObject)
		{
			if (voiceObject == null)
			{
				return "";
			}
			if (!TutorialPhase.Instance.IsCompleted && TutorialPhase.Instance.TutorialVillageHeadman.CharacterObject == character)
			{
				string text = voiceObject.VoicePaths.First<string>();
				Debug.Print("[VOICEOVER]Sound path found: " + BasePath.Name + text, 0, 12, 17592186044416UL);
				text = text.Replace("$PLATFORM", "PC");
				return text + ".ogg";
			}
			if (StoryModeHeroes.ElderBrother.CharacterObject != character)
			{
				return base.GetSoundPathForCharacter(character, voiceObject);
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
				return text2;
			}
			Debug.Print("[VOICEOVER]Sound path found: " + BasePath.Name + text2, 0, 12, 17592186044416UL);
			text2 = text2.Replace("$PLATFORM", "PC");
			return text2 + ".ogg";
		}

		private const string Male = "male";

		private const string Female = "female";
	}
}
