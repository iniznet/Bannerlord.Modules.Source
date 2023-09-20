using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002D9 RID: 729
	public class MultiplayerBattleBannerBearersModel : BattleBannerBearersModel
	{
		// Token: 0x0600281F RID: 10271 RVA: 0x0009B6EB File Offset: 0x000998EB
		public override int GetMinimumFormationTroopCountToBearBanners()
		{
			return int.MaxValue;
		}

		// Token: 0x06002820 RID: 10272 RVA: 0x0009B6F2 File Offset: 0x000998F2
		public override float GetBannerInteractionDistance(Agent interactingAgent)
		{
			return float.MaxValue;
		}

		// Token: 0x06002821 RID: 10273 RVA: 0x0009B6F9 File Offset: 0x000998F9
		public override bool CanAgentPickUpAnyBanner(Agent agent)
		{
			return false;
		}

		// Token: 0x06002822 RID: 10274 RVA: 0x0009B6FC File Offset: 0x000998FC
		public override bool CanBannerBearerProvideEffectToFormation(Agent agent, Formation formation)
		{
			return false;
		}

		// Token: 0x06002823 RID: 10275 RVA: 0x0009B6FF File Offset: 0x000998FF
		public override bool CanAgentBecomeBannerBearer(Agent agent)
		{
			return false;
		}

		// Token: 0x06002824 RID: 10276 RVA: 0x0009B702 File Offset: 0x00099902
		public override int GetAgentBannerBearingPriority(Agent agent)
		{
			return 0;
		}

		// Token: 0x06002825 RID: 10277 RVA: 0x0009B705 File Offset: 0x00099905
		public override bool CanFormationDeployBannerBearers(Formation formation)
		{
			return false;
		}

		// Token: 0x06002826 RID: 10278 RVA: 0x0009B708 File Offset: 0x00099908
		public override int GetDesiredNumberOfBannerBearersForFormation(Formation formation)
		{
			return 0;
		}

		// Token: 0x06002827 RID: 10279 RVA: 0x0009B70B File Offset: 0x0009990B
		public override ItemObject GetBannerBearerReplacementWeapon(BasicCharacterObject agentCharacter)
		{
			return null;
		}
	}
}
