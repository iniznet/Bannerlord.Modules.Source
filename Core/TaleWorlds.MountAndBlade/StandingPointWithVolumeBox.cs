using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000364 RID: 868
	public class StandingPointWithVolumeBox : StandingPointWithWeaponRequirement
	{
		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x06002F59 RID: 12121 RVA: 0x000C10B1 File Offset: 0x000BF2B1
		public override Agent.AIScriptedFrameFlags DisableScriptedFrameFlags
		{
			get
			{
				return Agent.AIScriptedFrameFlags.NoAttack;
			}
		}

		// Token: 0x06002F5A RID: 12122 RVA: 0x000C10B4 File Offset: 0x000BF2B4
		public override bool IsDisabledForAgent(Agent agent)
		{
			return base.IsDisabledForAgent(agent) || MathF.Abs(agent.Position.z - base.GameEntity.GlobalPosition.z) > 2f || agent.Position.DistanceSquared(base.GameEntity.GlobalPosition) > 100f;
		}

		// Token: 0x06002F5B RID: 12123 RVA: 0x000C1114 File Offset: 0x000BF314
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			MBEditor.IsEntitySelected(base.GameEntity);
		}

		// Token: 0x04001368 RID: 4968
		private const float MaxUserAgentDistance = 10f;

		// Token: 0x04001369 RID: 4969
		private const float MaxUserAgentElevation = 2f;

		// Token: 0x0400136A RID: 4970
		public string VolumeBoxTag = "volumebox";
	}
}
