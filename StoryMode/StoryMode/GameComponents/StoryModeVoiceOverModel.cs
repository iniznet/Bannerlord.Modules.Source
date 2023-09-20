using System;
using System.Linq;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Localization;

namespace StoryMode.GameComponents
{
	// Token: 0x02000049 RID: 73
	public class StoryModeVoiceOverModel : DefaultVoiceOverModel
	{
		// Token: 0x060003CB RID: 971 RVA: 0x0001779C File Offset: 0x0001599C
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
