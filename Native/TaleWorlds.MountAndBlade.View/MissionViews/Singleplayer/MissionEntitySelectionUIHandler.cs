using System;
using System.Diagnostics;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	// Token: 0x0200006B RID: 107
	public class MissionEntitySelectionUIHandler : MissionView
	{
		// Token: 0x06000459 RID: 1113 RVA: 0x0002275A File Offset: 0x0002095A
		public MissionEntitySelectionUIHandler(Action<GameEntity> onSelect = null, Action<GameEntity> onHover = null)
		{
			this.onSelect = onSelect;
			this.onHover = onHover;
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x00022770 File Offset: 0x00020970
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

		// Token: 0x0600045B RID: 1115 RVA: 0x000227D0 File Offset: 0x000209D0
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

		// Token: 0x0600045C RID: 1116 RVA: 0x00022870 File Offset: 0x00020A70
		public override void OnRemoveBehavior()
		{
			this.onSelect = null;
			this.onHover = null;
			base.OnRemoveBehavior();
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x00022888 File Offset: 0x00020A88
		[Conditional("DEBUG")]
		public void TickDebug()
		{
			GameEntity collidedEntity = this.GetCollidedEntity();
			if (!(collidedEntity == null))
			{
				string name = collidedEntity.Name;
			}
		}

		// Token: 0x040002B9 RID: 697
		private Action<GameEntity> onSelect;

		// Token: 0x040002BA RID: 698
		private Action<GameEntity> onHover;
	}
}
