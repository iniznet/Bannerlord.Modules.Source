using System;
using TaleWorlds.MountAndBlade;

namespace SandBox.AI
{
	// Token: 0x020000CC RID: 204
	public class UsablePlaceAI : UsableMachineAIBase
	{
		// Token: 0x06000C13 RID: 3091 RVA: 0x0005F3E8 File Offset: 0x0005D5E8
		public UsablePlaceAI(UsableMachine usableMachine)
			: base(usableMachine)
		{
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x0005F3F1 File Offset: 0x0005D5F1
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
