using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class Mover : ScriptComponentBehavior
	{
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

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			if (Mission.Current.Mode == MissionMode.Battle && this._tracker != null && this._tracker.IsValid && this._tracker.TotalDistanceTraveled < this._tracker.GetPathLength())
			{
				this._tracker.Advance(this._speed * dt);
				MatrixFrame currentFrame = this._tracker.CurrentFrame;
				base.GameEntity.SetFrame(ref currentFrame);
			}
		}

		[EditorVisibleScriptComponentVariable(true)]
		private string _pathname = "";

		[EditorVisibleScriptComponentVariable(true)]
		private float _speed;

		[EditorVisibleScriptComponentVariable(true)]
		private bool _moveGhost;

		private GameEntity _moverGhost;

		private PathTracker _tracker;
	}
}
