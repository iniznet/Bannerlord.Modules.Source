using System;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Helpers
{
	public static class PersuasionHelper
	{
		public static TextObject ShowSuccess(PersuasionOptionArgs optionArgs, bool showToPlayer = true)
		{
			return TextObject.Empty;
		}

		public static TextObject GetDefaultPersuasionOptionReaction(PersuasionOptionResult optionResult)
		{
			TextObject textObject;
			if (optionResult == PersuasionOptionResult.CriticalSuccess)
			{
				textObject = new TextObject("{=yNSqDwse}Well... I can't argue with that.", null);
			}
			else if (optionResult == PersuasionOptionResult.Failure || optionResult == PersuasionOptionResult.Miss)
			{
				textObject = new TextObject("{=mZmCmC6q}I don't think so.", null);
			}
			else if (optionResult == PersuasionOptionResult.CriticalFailure)
			{
				textObject = new TextObject("{=zqapPfSK}No.. No.", null);
			}
			else
			{
				textObject = ((MBRandom.RandomFloat > 0.5f) ? new TextObject("{=AmBEgOyq}I see...", null) : new TextObject("{=hq13B7Ok}Yes.. You might be correct.", null));
			}
			return textObject;
		}
	}
}
