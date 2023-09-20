using System;
using System.Linq;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
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
				return voiceObject.VoicePaths.First<string>().Replace("$PLATFORM", "PC") + ".ogg";
			}
			return base.GetSoundPathForCharacter(character, voiceObject);
		}
	}
}
