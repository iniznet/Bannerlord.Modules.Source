using System;
using TaleWorlds.MountAndBlade;

namespace SandBox.AI
{
	public class UsablePlaceAI : UsableMachineAIBase
	{
		public UsablePlaceAI(UsableMachine usableMachine)
			: base(usableMachine)
		{
		}

		protected override Agent.AIScriptedFrameFlags GetScriptedFrameFlags(Agent agent)
		{
			if (!this.UsableMachine.GameEntity.HasTag("quest_wanderer_target"))
			{
				return 16;
			}
			return 0;
		}
	}
}
