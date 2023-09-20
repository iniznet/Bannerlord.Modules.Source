using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class TacticalRegion : MissionObject
	{
		public WorldPosition Position
		{
			get
			{
				return this._position;
			}
			set
			{
				this._position = value;
			}
		}

		public List<TacticalPosition> LinkedTacticalPositions
		{
			get
			{
				return this._linkedTacticalPositions;
			}
			set
			{
				this._linkedTacticalPositions = value;
			}
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this._position = new WorldPosition(base.GameEntity.GetScenePointer(), base.GameEntity.GlobalPosition);
		}

		public override void AfterMissionStart()
		{
			base.AfterMissionStart();
			this._linkedTacticalPositions = new List<TacticalPosition>();
			this._linkedTacticalPositions = (from c in base.GameEntity.GetChildren()
				where c.HasScriptOfType<TacticalPosition>()
				select c.GetFirstScriptOfType<TacticalPosition>()).ToList<TacticalPosition>();
		}

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._position = new WorldPosition(base.GameEntity.GetScenePointer(), UIntPtr.Zero, base.GameEntity.GlobalPosition, false);
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this._position.SetVec3(UIntPtr.Zero, base.GameEntity.GlobalPosition, false);
			if (this.IsEditorDebugRingVisible)
			{
				MBEditor.HelpersEnabled();
			}
		}

		public bool IsEditorDebugRingVisible;

		private WorldPosition _position;

		public float radius = 1f;

		private List<TacticalPosition> _linkedTacticalPositions;

		public TacticalRegion.TacticalRegionTypeEnum tacticalRegionType;

		public enum TacticalRegionTypeEnum
		{
			Forest,
			DifficultTerrain,
			Opening
		}
	}
}
