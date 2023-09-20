using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200036D RID: 877
	public class TacticalRegion : MissionObject
	{
		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x06002FEE RID: 12270 RVA: 0x000C4DBF File Offset: 0x000C2FBF
		// (set) Token: 0x06002FEF RID: 12271 RVA: 0x000C4DC7 File Offset: 0x000C2FC7
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

		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x06002FF0 RID: 12272 RVA: 0x000C4DD0 File Offset: 0x000C2FD0
		// (set) Token: 0x06002FF1 RID: 12273 RVA: 0x000C4DD8 File Offset: 0x000C2FD8
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

		// Token: 0x06002FF2 RID: 12274 RVA: 0x000C4DE1 File Offset: 0x000C2FE1
		protected internal override void OnInit()
		{
			base.OnInit();
			this._position = new WorldPosition(base.GameEntity.GetScenePointer(), base.GameEntity.GlobalPosition);
		}

		// Token: 0x06002FF3 RID: 12275 RVA: 0x000C4E0C File Offset: 0x000C300C
		public override void AfterMissionStart()
		{
			base.AfterMissionStart();
			this._linkedTacticalPositions = new List<TacticalPosition>();
			this._linkedTacticalPositions = (from c in base.GameEntity.GetChildren()
				where c.HasScriptOfType<TacticalPosition>()
				select c.GetFirstScriptOfType<TacticalPosition>()).ToList<TacticalPosition>();
		}

		// Token: 0x06002FF4 RID: 12276 RVA: 0x000C4E88 File Offset: 0x000C3088
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._position = new WorldPosition(base.GameEntity.GetScenePointer(), UIntPtr.Zero, base.GameEntity.GlobalPosition, false);
		}

		// Token: 0x06002FF5 RID: 12277 RVA: 0x000C4EB7 File Offset: 0x000C30B7
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this._position.SetVec3(UIntPtr.Zero, base.GameEntity.GlobalPosition, false);
			if (this.IsEditorDebugRingVisible)
			{
				MBEditor.HelpersEnabled();
			}
		}

		// Token: 0x040013E1 RID: 5089
		public bool IsEditorDebugRingVisible;

		// Token: 0x040013E2 RID: 5090
		private WorldPosition _position;

		// Token: 0x040013E3 RID: 5091
		public float radius = 1f;

		// Token: 0x040013E4 RID: 5092
		private List<TacticalPosition> _linkedTacticalPositions;

		// Token: 0x040013E5 RID: 5093
		public TacticalRegion.TacticalRegionTypeEnum tacticalRegionType;

		// Token: 0x02000680 RID: 1664
		public enum TacticalRegionTypeEnum
		{
			// Token: 0x0400212B RID: 8491
			Forest,
			// Token: 0x0400212C RID: 8492
			DifficultTerrain,
			// Token: 0x0400212D RID: 8493
			Opening
		}
	}
}
