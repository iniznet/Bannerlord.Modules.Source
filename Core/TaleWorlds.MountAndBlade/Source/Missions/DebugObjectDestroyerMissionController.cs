using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	public class DebugObjectDestroyerMissionController : MissionLogic
	{
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			Vec3 lastFinalRenderCameraPosition = base.Mission.Scene.LastFinalRenderCameraPosition;
			Vec3 vec = -base.Mission.Scene.LastFinalRenderCameraFrame.rotation.u;
			float num;
			GameEntity gameEntity;
			bool flag = Mission.Current.Scene.RayCastForClosestEntityOrTerrain(lastFinalRenderCameraPosition, lastFinalRenderCameraPosition + vec * 100f, out num, out gameEntity, 0.01f, BodyFlags.OnlyCollideWithRaycast);
			if (Input.DebugInput.IsShiftDown() && Agent.Main != null && gameEntity != null && !gameEntity.HasScriptOfType<DestructableComponent>())
			{
				foreach (MissionObject missionObject in Mission.Current.ActiveMissionObjects.Where((MissionObject x) => x is DestructableComponent))
				{
					DestructableComponent destructableComponent = (DestructableComponent)missionObject;
					if ((destructableComponent.GameEntity.GlobalPosition - Agent.Main.Position).Length < 5f)
					{
						gameEntity = destructableComponent.GameEntity;
					}
				}
			}
			GameEntity gameEntity2 = null;
			if (flag && (Input.DebugInput.IsKeyDown(InputKey.MiddleMouseButton) || Input.DebugInput.IsKeyReleased(InputKey.MiddleMouseButton)))
			{
				Vec3 vec2 = lastFinalRenderCameraPosition + vec * num;
				if (gameEntity == null)
				{
					return;
				}
				bool flag2 = Input.DebugInput.IsKeyReleased(InputKey.MiddleMouseButton);
				int num2 = 0;
				if (flag2)
				{
					if (Input.DebugInput.IsAltDown())
					{
						num2 = (int)Game.Current.ObjectManager.GetObject<ItemObject>("boulder").Id.InternalValue;
					}
					else if (Input.DebugInput.IsControlDown())
					{
						num2 = (int)Game.Current.ObjectManager.GetObject<ItemObject>("pot").Id.InternalValue;
					}
					else
					{
						num2 = (int)Game.Current.ObjectManager.GetObject<ItemObject>("ballista_projectile").Id.InternalValue;
					}
				}
				GameEntity gameEntity3 = gameEntity;
				DestructableComponent destructableComponent2 = null;
				while (destructableComponent2 == null && gameEntity3 != null)
				{
					destructableComponent2 = gameEntity3.GetFirstScriptOfType<DestructableComponent>();
					gameEntity3 = gameEntity3.Parent;
				}
				if (destructableComponent2 != null && !destructableComponent2.IsDestroyed)
				{
					if (flag2)
					{
						if (Agent.Main != null)
						{
							DestructableComponent destructableComponent3 = destructableComponent2;
							Agent main = Agent.Main;
							int num3 = 400;
							Vec3 vec3 = vec2 - vec * 0.1f;
							Vec3 vec4 = vec;
							MissionWeapon missionWeapon = new MissionWeapon(ItemObject.GetItemFromWeaponKind(num2), null, null);
							destructableComponent3.TriggerOnHit(main, num3, vec3, vec4, missionWeapon, null);
						}
					}
					else
					{
						gameEntity2 = destructableComponent2.GameEntity;
					}
				}
			}
			if (gameEntity2 != this._contouredEntity && this._contouredEntity != null)
			{
				this._contouredEntity.SetContourColor(null, true);
			}
			this._contouredEntity = gameEntity2;
			if (this._contouredEntity != null)
			{
				this._contouredEntity.SetContourColor(new uint?(4294967040U), true);
			}
		}

		private GameEntity _contouredEntity;
	}
}
