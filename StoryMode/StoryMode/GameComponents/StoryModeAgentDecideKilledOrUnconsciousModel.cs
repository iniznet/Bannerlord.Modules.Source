using System;
using SandBox.GameComponents;
using StoryMode.StoryModeObjects;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace StoryMode.GameComponents
{
	// Token: 0x02000039 RID: 57
	public class StoryModeAgentDecideKilledOrUnconsciousModel : SandboxAgentDecideKilledOrUnconsciousModel
	{
		// Token: 0x060003A4 RID: 932 RVA: 0x0001700C File Offset: 0x0001520C
		public override float GetAgentStateProbability(Agent affectorAgent, Agent effectedAgent, DamageTypes damageType, out float useSurgeryProbability)
		{
			useSurgeryProbability = 1f;
			if (effectedAgent.Character.IsHero && (effectedAgent.Character == StoryModeHeroes.ElderBrother.CharacterObject || effectedAgent.Character == StoryModeHeroes.Radagos.CharacterObject || effectedAgent.Character == StoryModeHeroes.RadagosHencman.CharacterObject) && !StoryModeManager.Current.MainStoryLine.IsCompleted)
			{
				return 0f;
			}
			if (!StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted && Mission.Current.GetMemberCountOfSide(effectedAgent.Team.Side) > 4)
			{
				return 0f;
			}
			return base.GetAgentStateProbability(affectorAgent, effectedAgent, damageType, out useSurgeryProbability);
		}
	}
}
