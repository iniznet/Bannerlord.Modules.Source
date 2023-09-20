using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class StandingPointWithVolumeBox : StandingPointWithWeaponRequirement
	{
		public override Agent.AIScriptedFrameFlags DisableScriptedFrameFlags
		{
			get
			{
				return Agent.AIScriptedFrameFlags.NoAttack;
			}
		}

		public override bool IsDisabledForAgent(Agent agent)
		{
			return base.IsDisabledForAgent(agent) || MathF.Abs(agent.Position.z - base.GameEntity.GlobalPosition.z) > 2f || agent.Position.DistanceSquared(base.GameEntity.GlobalPosition) > 100f;
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			MBEditor.IsEntitySelected(base.GameEntity);
		}

		private const float MaxUserAgentDistance = 10f;

		private const float MaxUserAgentElevation = 2f;

		public string VolumeBoxTag = "volumebox";
	}
}
