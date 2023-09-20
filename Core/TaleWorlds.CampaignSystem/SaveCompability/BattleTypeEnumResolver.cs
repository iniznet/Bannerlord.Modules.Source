using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Resolvers;

namespace TaleWorlds.CampaignSystem.SaveCompability
{
	// Token: 0x020000C5 RID: 197
	public class BattleTypeEnumResolver : IEnumResolver
	{
		// Token: 0x0600125D RID: 4701 RVA: 0x00054318 File Offset: 0x00052518
		public string ResolveObject(string originalObject)
		{
			if (string.IsNullOrEmpty(originalObject))
			{
				Debug.FailedAssert("EndCaptivityDetail data is null or empty", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\SaveCompability\\BattleTypeEnumResolver.cs", "ResolveObject", 16);
				return MapEvent.BattleTypes.None.ToString();
			}
			if (originalObject.Equals("AlleyFight"))
			{
				return MapEvent.BattleTypes.None.ToString();
			}
			return originalObject;
		}
	}
}
