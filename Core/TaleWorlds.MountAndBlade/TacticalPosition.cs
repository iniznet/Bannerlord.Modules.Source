using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class TacticalPosition : MissionObject
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

		public Vec2 Direction
		{
			get
			{
				return this._direction;
			}
			set
			{
				this._direction = value;
			}
		}

		public float Width
		{
			get
			{
				return this._width;
			}
		}

		public float Slope
		{
			get
			{
				return this._slope;
			}
		}

		public bool IsInsurmountable
		{
			get
			{
				return this._isInsurmountable;
			}
		}

		public bool IsOuterEdge
		{
			get
			{
				return this._isOuterEdge;
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

		public TacticalPosition.TacticalPositionTypeEnum TacticalPositionType
		{
			get
			{
				return this._tacticalPositionType;
			}
		}

		public TacticalRegion.TacticalRegionTypeEnum TacticalRegionMembership
		{
			get
			{
				return this._tacticalRegionMembership;
			}
		}

		public FormationAI.BehaviorSide TacticalPositionSide
		{
			get
			{
				return this._tacticalPositionSide;
			}
		}

		public TacticalPosition()
		{
			this._width = 1f;
			this._slope = 0f;
		}

		public TacticalPosition(WorldPosition position, Vec2 direction, float width, float slope = 0f, bool isInsurmountable = false, TacticalPosition.TacticalPositionTypeEnum tacticalPositionType = TacticalPosition.TacticalPositionTypeEnum.Regional, TacticalRegion.TacticalRegionTypeEnum tacticalRegionMembership = TacticalRegion.TacticalRegionTypeEnum.Opening)
		{
			this._position = position;
			this._direction = direction;
			this._width = width;
			this._slope = slope;
			this._isInsurmountable = isInsurmountable;
			this._tacticalPositionType = tacticalPositionType;
			this._tacticalRegionMembership = tacticalRegionMembership;
			this._tacticalPositionSide = FormationAI.BehaviorSide.BehaviorSideNotSet;
			this._isOuterEdge = false;
			this._linkedTacticalPositions = new List<TacticalPosition>();
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this._position = new WorldPosition(base.GameEntity.GetScenePointer(), base.GameEntity.GlobalPosition);
			this._direction = base.GameEntity.GetGlobalFrame().rotation.f.AsVec2.Normalized();
		}

		public override void AfterMissionStart()
		{
			base.AfterMissionStart();
			this._linkedTacticalPositions = (from c in base.GameEntity.GetChildren()
				where c.HasScriptOfType<TacticalPosition>()
				select c.GetFirstScriptOfType<TacticalPosition>()).ToList<TacticalPosition>();
		}

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.ApplyChangesFromEditor();
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.ApplyChangesFromEditor();
		}

		private void ApplyChangesFromEditor()
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			if (this._width > 0f && this._width != this._oldWidth)
			{
				globalFrame.rotation.MakeUnit();
				globalFrame.rotation.ApplyScaleLocal(new Vec3(this._width, 1f, 1f, -1f));
				base.GameEntity.SetGlobalFrame(globalFrame);
				base.GameEntity.UpdateTriadFrameForEditorForAllChildren();
				this._oldWidth = this._width;
			}
			this._direction = globalFrame.rotation.f.AsVec2.Normalized();
		}

		public void SetWidth(float width)
		{
			this._width = width;
		}

		private WorldPosition _position;

		private Vec2 _direction;

		private float _oldWidth;

		[EditableScriptComponentVariable(true)]
		private float _width;

		[EditableScriptComponentVariable(true)]
		private float _slope;

		[EditableScriptComponentVariable(true)]
		private bool _isInsurmountable;

		[EditableScriptComponentVariable(true)]
		private bool _isOuterEdge;

		private List<TacticalPosition> _linkedTacticalPositions;

		[EditableScriptComponentVariable(true)]
		private TacticalPosition.TacticalPositionTypeEnum _tacticalPositionType;

		[EditableScriptComponentVariable(true)]
		private TacticalRegion.TacticalRegionTypeEnum _tacticalRegionMembership;

		[EditableScriptComponentVariable(true)]
		private FormationAI.BehaviorSide _tacticalPositionSide = FormationAI.BehaviorSide.BehaviorSideNotSet;

		public enum TacticalPositionTypeEnum
		{
			Regional,
			HighGround,
			ChokePoint,
			Cliff,
			SpecialMissionPosition
		}
	}
}
