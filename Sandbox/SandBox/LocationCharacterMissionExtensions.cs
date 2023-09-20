using System;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	// Token: 0x0200000D RID: 13
	public static class LocationCharacterMissionExtensions
	{
		// Token: 0x06000099 RID: 153 RVA: 0x00005395 File Offset: 0x00003595
		public static AgentBuildData GetAgentBuildData(this LocationCharacter locationCharacter)
		{
			return new AgentBuildData(locationCharacter.AgentData);
		}
	}
}
