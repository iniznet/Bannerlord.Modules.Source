using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Resolvers;

namespace TaleWorlds.CampaignSystem.SaveCompability
{
	// Token: 0x020000C6 RID: 198
	public class EndCaptivityDetailEnumResolver : IEnumResolver
	{
		// Token: 0x0600125F RID: 4703 RVA: 0x0005437C File Offset: 0x0005257C
		public string ResolveObject(string originalObject)
		{
			if (string.IsNullOrEmpty(originalObject))
			{
				Debug.FailedAssert("EndCaptivityDetail data is null or empty", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\SaveCompability\\EndCaptivityDetailEnumResolver.cs", "ResolveObject", 15);
				return EndCaptivityDetail.ReleasedByChoice.ToString();
			}
			if (originalObject.Equals("EscapeFromLootedParty"))
			{
				return EndCaptivityDetail.ReleasedAfterEscape.ToString();
			}
			if (originalObject.Equals("ReleasedFromPartyScreen"))
			{
				return EndCaptivityDetail.ReleasedByChoice.ToString();
			}
			if (originalObject.Equals("RemovedParty"))
			{
				return EndCaptivityDetail.ReleasedByChoice.ToString();
			}
			return originalObject;
		}
	}
}
