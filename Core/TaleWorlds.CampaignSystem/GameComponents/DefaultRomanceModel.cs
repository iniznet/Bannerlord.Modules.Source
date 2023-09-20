using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000133 RID: 307
	public class DefaultRomanceModel : RomanceModel
	{
		// Token: 0x060016E9 RID: 5865 RVA: 0x00070A0C File Offset: 0x0006EC0C
		public override int GetAttractionValuePercentage(Hero potentiallyInterestedCharacter, Hero heroOfInterest)
		{
			return MathF.Abs((potentiallyInterestedCharacter.StaticBodyProperties.GetHashCode() + heroOfInterest.StaticBodyProperties.GetHashCode()) % 100);
		}
	}
}
