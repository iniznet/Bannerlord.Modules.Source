using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200036C RID: 876
	public class TacticalPosition : MissionObject
	{
		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x06002FD9 RID: 12249 RVA: 0x000C4B24 File Offset: 0x000C2D24
		// (set) Token: 0x06002FDA RID: 12250 RVA: 0x000C4B2C File Offset: 0x000C2D2C
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

		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x06002FDB RID: 12251 RVA: 0x000C4B35 File Offset: 0x000C2D35
		// (set) Token: 0x06002FDC RID: 12252 RVA: 0x000C4B3D File Offset: 0x000C2D3D
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

		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x06002FDD RID: 12253 RVA: 0x000C4B46 File Offset: 0x000C2D46
		public float Width
		{
			get
			{
				return this._width;
			}
		}

		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x06002FDE RID: 12254 RVA: 0x000C4B4E File Offset: 0x000C2D4E
		public float Slope
		{
			get
			{
				return this._slope;
			}
		}

		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x06002FDF RID: 12255 RVA: 0x000C4B56 File Offset: 0x000C2D56
		public bool IsInsurmountable
		{
			get
			{
				return this._isInsurmountable;
			}
		}

		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x06002FE0 RID: 12256 RVA: 0x000C4B5E File Offset: 0x000C2D5E
		public bool IsOuterEdge
		{
			get
			{
				return this._isOuterEdge;
			}
		}

		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x06002FE1 RID: 12257 RVA: 0x000C4B66 File Offset: 0x000C2D66
		// (set) Token: 0x06002FE2 RID: 12258 RVA: 0x000C4B6E File Offset: 0x000C2D6E
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

		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x06002FE3 RID: 12259 RVA: 0x000C4B77 File Offset: 0x000C2D77
		public TacticalPosition.TacticalPositionTypeEnum TacticalPositionType
		{
			get
			{
				return this._tacticalPositionType;
			}
		}

		// Token: 0x170008A2 RID: 2210
		// (get) Token: 0x06002FE4 RID: 12260 RVA: 0x000C4B7F File Offset: 0x000C2D7F
		public TacticalRegion.TacticalRegionTypeEnum TacticalRegionMembership
		{
			get
			{
				return this._tacticalRegionMembership;
			}
		}

		// Token: 0x170008A3 RID: 2211
		// (get) Token: 0x06002FE5 RID: 12261 RVA: 0x000C4B87 File Offset: 0x000C2D87
		public FormationAI.BehaviorSide TacticalPositionSide
		{
			get
			{
				return this._tacticalPositionSide;
			}
		}

		// Token: 0x06002FE6 RID: 12262 RVA: 0x000C4B8F File Offset: 0x000C2D8F
		public TacticalPosition()
		{
			this._width = 1f;
			this._slope = 0f;
		}

		// Token: 0x06002FE7 RID: 12263 RVA: 0x000C4BB4 File Offset: 0x000C2DB4
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

		// Token: 0x06002FE8 RID: 12264 RVA: 0x000C4C1C File Offset: 0x000C2E1C
		protected internal override void OnInit()
		{
			base.OnInit();
			this._position = new WorldPosition(base.GameEntity.GetScenePointer(), base.GameEntity.GlobalPosition);
			this._direction = base.GameEntity.GetGlobalFrame().rotation.f.AsVec2.Normalized();
		}

		// Token: 0x06002FE9 RID: 12265 RVA: 0x000C4C7C File Offset: 0x000C2E7C
		public override void AfterMissionStart()
		{
			base.AfterMissionStart();
			this._linkedTacticalPositions = (from c in base.GameEntity.GetChildren()
				where c.HasScriptOfType<TacticalPosition>()
				select c.GetFirstScriptOfType<TacticalPosition>()).ToList<TacticalPosition>();
		}

		// Token: 0x06002FEA RID: 12266 RVA: 0x000C4CED File Offset: 0x000C2EED
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.ApplyChangesFromEditor();
		}

		// Token: 0x06002FEB RID: 12267 RVA: 0x000C4CFB File Offset: 0x000C2EFB
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.ApplyChangesFromEditor();
		}

		// Token: 0x06002FEC RID: 12268 RVA: 0x000C4D0C File Offset: 0x000C2F0C
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

		// Token: 0x06002FED RID: 12269 RVA: 0x000C4DB6 File Offset: 0x000C2FB6
		public void SetWidth(float width)
		{
			this._width = width;
		}

		// Token: 0x040013D6 RID: 5078
		private WorldPosition _position;

		// Token: 0x040013D7 RID: 5079
		private Vec2 _direction;

		// Token: 0x040013D8 RID: 5080
		private float _oldWidth;

		// Token: 0x040013D9 RID: 5081
		[EditableScriptComponentVariable(true)]
		private float _width;

		// Token: 0x040013DA RID: 5082
		[EditableScriptComponentVariable(true)]
		private float _slope;

		// Token: 0x040013DB RID: 5083
		[EditableScriptComponentVariable(true)]
		private bool _isInsurmountable;

		// Token: 0x040013DC RID: 5084
		[EditableScriptComponentVariable(true)]
		private bool _isOuterEdge;

		// Token: 0x040013DD RID: 5085
		private List<TacticalPosition> _linkedTacticalPositions;

		// Token: 0x040013DE RID: 5086
		[EditableScriptComponentVariable(true)]
		private TacticalPosition.TacticalPositionTypeEnum _tacticalPositionType;

		// Token: 0x040013DF RID: 5087
		[EditableScriptComponentVariable(true)]
		private TacticalRegion.TacticalRegionTypeEnum _tacticalRegionMembership;

		// Token: 0x040013E0 RID: 5088
		[EditableScriptComponentVariable(true)]
		private FormationAI.BehaviorSide _tacticalPositionSide = FormationAI.BehaviorSide.BehaviorSideNotSet;

		// Token: 0x0200067E RID: 1662
		public enum TacticalPositionTypeEnum
		{
			// Token: 0x04002122 RID: 8482
			Regional,
			// Token: 0x04002123 RID: 8483
			HighGround,
			// Token: 0x04002124 RID: 8484
			ChokePoint,
			// Token: 0x04002125 RID: 8485
			Cliff,
			// Token: 0x04002126 RID: 8486
			SpecialMissionPosition
		}
	}
}
