﻿using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class VoiceOverModel : GameModel
	{
		public abstract string GetSoundPathForCharacter(CharacterObject character, VoiceObject voiceObject);

		public abstract string GetAccentClass(CultureObject culture, bool isHighClass);
	}
}
