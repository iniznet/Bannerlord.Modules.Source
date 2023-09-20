using System;
using Helpers;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Extensions
{
	// Token: 0x02000157 RID: 343
	public static class TextObjectExtensions
	{
		// Token: 0x0600183B RID: 6203 RVA: 0x0007B033 File Offset: 0x00079233
		public static void SetCharacterProperties(this TextObject to, string tag, CharacterObject character, bool includeDetails = false)
		{
			StringHelpers.SetCharacterProperties(tag, character, to, includeDetails);
		}
	}
}
