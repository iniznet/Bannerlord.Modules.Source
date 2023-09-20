using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001CF RID: 463
	public abstract class VoiceOverModel : GameModel
	{
		// Token: 0x06001B97 RID: 7063
		public abstract string GetSoundPathForCharacter(CharacterObject character, VoiceObject voiceObject);

		// Token: 0x06001B98 RID: 7064
		public abstract string GetAccentClass(CultureObject culture, bool isHighClass);
	}
}
