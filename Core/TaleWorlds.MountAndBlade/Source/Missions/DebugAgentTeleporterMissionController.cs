using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	// Token: 0x020003F1 RID: 1009
	public class DebugAgentTeleporterMissionController : MissionLogic
	{
		// Token: 0x060034C7 RID: 13511 RVA: 0x000DB710 File Offset: 0x000D9910
		public override void AfterStart()
		{
		}

		// Token: 0x060034C8 RID: 13512 RVA: 0x000DB714 File Offset: 0x000D9914
		public override void OnMissionTick(float dt)
		{
			Agent agent = null;
			int debugAgent = base.Mission.GetDebugAgent();
			foreach (Agent agent2 in base.Mission.Agents)
			{
				if (debugAgent == agent2.Index)
				{
					agent = agent2;
					break;
				}
			}
			if (agent == null && base.Mission.Agents.Count > 0)
			{
				int num = MBRandom.RandomInt(base.Mission.Agents.Count);
				int count = base.Mission.Agents.Count;
				int num2 = num;
				Agent agent3;
				for (;;)
				{
					agent3 = base.Mission.Agents[num2];
					if (agent3 != Agent.Main && agent3.IsActive())
					{
						break;
					}
					num2 = (num2 + 1) % count;
					if (num2 == num)
					{
						goto IL_DC;
					}
				}
				agent = agent3;
				base.Mission.SetDebugAgent(num2);
			}
			IL_DC:
			if (agent != null)
			{
				MatrixFrame lastFinalRenderCameraFrame = base.Mission.Scene.LastFinalRenderCameraFrame;
				if (Input.DebugInput.IsKeyDown(InputKey.MiddleMouseButton))
				{
					float num3;
					base.Mission.Scene.RayCastForClosestEntityOrTerrain(lastFinalRenderCameraFrame.origin, lastFinalRenderCameraFrame.origin + -lastFinalRenderCameraFrame.rotation.u * 100f, out num3, 0.01f, BodyFlags.CommonCollisionExcludeFlags);
				}
				float num4;
				if (Input.DebugInput.IsKeyReleased(InputKey.MiddleMouseButton) && base.Mission.Scene.RayCastForClosestEntityOrTerrain(lastFinalRenderCameraFrame.origin, lastFinalRenderCameraFrame.origin + -lastFinalRenderCameraFrame.rotation.u * 100f, out num4, 0.01f, BodyFlags.CommonCollisionExcludeFlags))
				{
					Vec3 vec = lastFinalRenderCameraFrame.origin + -lastFinalRenderCameraFrame.rotation.u * num4;
					if (Input.DebugInput.IsHotKeyReleased("DebugAgentTeleportMissionControllerHotkeyTeleportMainAgent"))
					{
						agent.TeleportToPosition(vec);
					}
					else
					{
						Vec2 vec2 = -lastFinalRenderCameraFrame.rotation.u.AsVec2;
						WorldPosition worldPosition = new WorldPosition(base.Mission.Scene, UIntPtr.Zero, vec, false);
						agent.SetScriptedPositionAndDirection(ref worldPosition, vec2.RotationInRadians, false, Agent.AIScriptedFrameFlags.NoAttack);
					}
				}
				if (Input.DebugInput.IsHotKeyPressed("DebugAgentTeleportMissionControllerHotkeyDisableScriptedMovement"))
				{
					agent.DisableScriptedMovement();
				}
			}
		}
	}
}
