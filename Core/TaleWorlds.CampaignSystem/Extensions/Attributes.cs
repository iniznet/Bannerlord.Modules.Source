using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Extensions
{
	// Token: 0x02000153 RID: 339
	public static class Attributes
	{
		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x06001837 RID: 6199 RVA: 0x0007B007 File Offset: 0x00079207
		public static MBReadOnlyList<CharacterAttribute> All
		{
			get
			{
				return Campaign.Current.AllCharacterAttributes;
			}
		}
	}
}
