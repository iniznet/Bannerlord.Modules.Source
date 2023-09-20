using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200033F RID: 831
	public class Mover : ScriptComponentBehavior
	{
		// Token: 0x06002C6E RID: 11374 RVA: 0x000AC6E8 File Offset: 0x000AA8E8
		protected internal override void OnEditorTick(float dt)
		{
			if (!base.GameEntity.EntityFlags.HasAnyFlag(EntityFlags.IsHelper))
			{
				if (this._moverGhost == null && this._pathname != "")
				{
					this.CreateOrUpdateMoverGhost();
				}
				if (this._tracker != null && this._tracker.IsValid)
				{
					if (this._moveGhost)
					{
						this._tracker.Advance(this._speed * dt);
						if (this._tracker.TotalDistanceTraveled >= this._tracker.GetPathLength())
						{
							this._tracker.Reset();
						}
					}
					else
					{
						this._tracker.Advance(0f);
					}
					MatrixFrame currentFrame = this._tracker.CurrentFrame;
					this._moverGhost.SetFrame(ref currentFrame);
				}
			}
		}

		// Token: 0x06002C6F RID: 11375 RVA: 0x000AC7B4 File Offset: 0x000AA9B4
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			if (variableName == "_pathname")
			{
				this.CreateOrUpdateMoverGhost();
				return;
			}
			if (variableName == "_moveGhost")
			{
				if (!this._moveGhost)
				{
					this._moverGhost.SetVisibilityExcludeParents(false);
					return;
				}
				this._moverGhost.SetVisibilityExcludeParents(true);
			}
		}

		// Token: 0x06002C70 RID: 11376 RVA: 0x000AC804 File Offset: 0x000AAA04
		private void CreateOrUpdateMoverGhost()
		{
			Path pathWithName = base.GameEntity.Scene.GetPathWithName(this._pathname);
			if (pathWithName != null)
			{
				this._tracker = new PathTracker(pathWithName, Vec3.One);
				this._tracker.Reset();
				base.GameEntity.SetLocalPosition(this._tracker.CurrentFrame.origin);
				if (this._moverGhost == null)
				{
					this._moverGhost = GameEntity.CopyFrom(base.GameEntity.Scene, base.GameEntity);
					this._moverGhost.EntityFlags |= EntityFlags.IsHelper | EntityFlags.DontSaveToScene | EntityFlags.DoNotTick;
					this._moverGhost.SetAlpha(0.2f);
					return;
				}
				this._moverGhost.SetLocalPosition(this._tracker.CurrentFrame.origin);
			}
		}

		// Token: 0x06002C71 RID: 11377 RVA: 0x000AC8D8 File Offset: 0x000AAAD8
		protected internal override void OnInit()
		{
			base.OnInit();
			Path pathWithName = base.GameEntity.Scene.GetPathWithName(this._pathname);
			if (pathWithName != null)
			{
				this._tracker = new PathTracker(pathWithName, Vec3.One);
				this._tracker.Reset();
				base.GameEntity.SetLocalPosition(this._tracker.CurrentFrame.origin);
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06002C72 RID: 11378 RVA: 0x000AC94E File Offset: 0x000AAB4E
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002C73 RID: 11379 RVA: 0x000AC96C File Offset: 0x000AAB6C
		protected internal override void OnTick(float dt)
		{
			if (Mission.Current.Mode == MissionMode.Battle && this._tracker != null && this._tracker.IsValid && this._tracker.TotalDistanceTraveled < this._tracker.GetPathLength())
			{
				this._tracker.Advance(this._speed * dt);
				MatrixFrame currentFrame = this._tracker.CurrentFrame;
				base.GameEntity.SetFrame(ref currentFrame);
			}
		}

		// Token: 0x040010F4 RID: 4340
		[EditorVisibleScriptComponentVariable(true)]
		private string _pathname = "";

		// Token: 0x040010F5 RID: 4341
		[EditorVisibleScriptComponentVariable(true)]
		private float _speed;

		// Token: 0x040010F6 RID: 4342
		[EditorVisibleScriptComponentVariable(true)]
		private bool _moveGhost;

		// Token: 0x040010F7 RID: 4343
		private GameEntity _moverGhost;

		// Token: 0x040010F8 RID: 4344
		private PathTracker _tracker;
	}
}
