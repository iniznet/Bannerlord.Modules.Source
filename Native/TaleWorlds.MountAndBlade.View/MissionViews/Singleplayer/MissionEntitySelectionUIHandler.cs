using System;
using System.Diagnostics;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	public class MissionEntitySelectionUIHandler : MissionView
	{
		public MissionEntitySelectionUIHandler(Action<GameEntity> onSelect = null, Action<GameEntity> onHover = null)
		{
			this.onSelect = onSelect;
			this.onHover = onHover;
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			GameEntity value = new Lazy<GameEntity>(new Func<GameEntity>(this.GetCollidedEntity)).Value;
			Action<GameEntity> action = this.onHover;
			if (action != null)
			{
				action(value);
			}
			if (base.Input.IsKeyReleased(224))
			{
				Action<GameEntity> action2 = this.onSelect;
				if (action2 == null)
				{
					return;
				}
				action2(value);
			}
		}

		private GameEntity GetCollidedEntity()
		{
			Vec2 mousePositionRanged = base.Input.GetMousePositionRanged();
			Vec3 vec;
			Vec3 vec2;
			base.MissionScreen.ScreenPointToWorldRay(mousePositionRanged, out vec, out vec2);
			TWSharedMutexUpgradeableReadLock twsharedMutexUpgradeableReadLock;
			twsharedMutexUpgradeableReadLock..ctor(Scene.PhysicsAndRayCastLock);
			GameEntity gameEntity;
			try
			{
				float num;
				GameEntity parent;
				base.Mission.Scene.RayCastForClosestEntityOrTerrainMT(vec, vec2, ref num, ref parent, 0.3f, 79617);
				while (parent != null && parent.Parent != null)
				{
					parent = parent.Parent;
				}
				gameEntity = parent;
			}
			finally
			{
				twsharedMutexUpgradeableReadLock.Dispose();
			}
			return gameEntity;
		}

		public override void OnRemoveBehavior()
		{
			this.onSelect = null;
			this.onHover = null;
			base.OnRemoveBehavior();
		}

		[Conditional("DEBUG")]
		public void TickDebug()
		{
			GameEntity collidedEntity = this.GetCollidedEntity();
			if (!(collidedEntity == null))
			{
				string name = collidedEntity.Name;
			}
		}

		private Action<GameEntity> onSelect;

		private Action<GameEntity> onHover;
	}
}
