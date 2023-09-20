using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000369 RID: 873
	public class WallSegment : SynchedMissionObject, IPointDefendable, ICastleKeyPosition
	{
		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x06002F94 RID: 12180 RVA: 0x000C321B File Offset: 0x000C141B
		// (set) Token: 0x06002F95 RID: 12181 RVA: 0x000C3223 File Offset: 0x000C1423
		public TacticalPosition MiddlePosition { get; private set; }

		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x06002F96 RID: 12182 RVA: 0x000C322C File Offset: 0x000C142C
		// (set) Token: 0x06002F97 RID: 12183 RVA: 0x000C3234 File Offset: 0x000C1434
		public TacticalPosition WaitPosition { get; private set; }

		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x06002F98 RID: 12184 RVA: 0x000C323D File Offset: 0x000C143D
		// (set) Token: 0x06002F99 RID: 12185 RVA: 0x000C3245 File Offset: 0x000C1445
		public TacticalPosition AttackerWaitPosition { get; private set; }

		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x06002F9A RID: 12186 RVA: 0x000C324E File Offset: 0x000C144E
		// (set) Token: 0x06002F9B RID: 12187 RVA: 0x000C3256 File Offset: 0x000C1456
		public IPrimarySiegeWeapon AttackerSiegeWeapon { get; set; }

		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x06002F9C RID: 12188 RVA: 0x000C325F File Offset: 0x000C145F
		// (set) Token: 0x06002F9D RID: 12189 RVA: 0x000C3267 File Offset: 0x000C1467
		public IEnumerable<DefencePoint> DefencePoints { get; protected set; }

		// Token: 0x17000892 RID: 2194
		// (get) Token: 0x06002F9E RID: 12190 RVA: 0x000C3270 File Offset: 0x000C1470
		// (set) Token: 0x06002F9F RID: 12191 RVA: 0x000C3278 File Offset: 0x000C1478
		public bool IsBreachedWall { get; private set; }

		// Token: 0x17000893 RID: 2195
		// (get) Token: 0x06002FA0 RID: 12192 RVA: 0x000C3281 File Offset: 0x000C1481
		// (set) Token: 0x06002FA1 RID: 12193 RVA: 0x000C3289 File Offset: 0x000C1489
		public WorldFrame MiddleFrame { get; private set; }

		// Token: 0x17000894 RID: 2196
		// (get) Token: 0x06002FA2 RID: 12194 RVA: 0x000C3292 File Offset: 0x000C1492
		// (set) Token: 0x06002FA3 RID: 12195 RVA: 0x000C329A File Offset: 0x000C149A
		public WorldFrame DefenseWaitFrame { get; private set; }

		// Token: 0x17000895 RID: 2197
		// (get) Token: 0x06002FA4 RID: 12196 RVA: 0x000C32A3 File Offset: 0x000C14A3
		// (set) Token: 0x06002FA5 RID: 12197 RVA: 0x000C32AB File Offset: 0x000C14AB
		public WorldFrame AttackerWaitFrame { get; private set; } = WorldFrame.Invalid;

		// Token: 0x17000896 RID: 2198
		// (get) Token: 0x06002FA6 RID: 12198 RVA: 0x000C32B4 File Offset: 0x000C14B4
		// (set) Token: 0x06002FA7 RID: 12199 RVA: 0x000C32BC File Offset: 0x000C14BC
		public FormationAI.BehaviorSide DefenseSide { get; private set; }

		// Token: 0x06002FA8 RID: 12200 RVA: 0x000C32C5 File Offset: 0x000C14C5
		public Vec3 GetPosition()
		{
			return base.GameEntity.GlobalPosition;
		}

		// Token: 0x06002FA9 RID: 12201 RVA: 0x000C32D4 File Offset: 0x000C14D4
		public WallSegment()
		{
			this.AttackerSiegeWeapon = null;
		}

		// Token: 0x06002FAA RID: 12202 RVA: 0x000C3338 File Offset: 0x000C1538
		protected internal override void OnInit()
		{
			base.OnInit();
			string sideTag = this.SideTag;
			if (!(sideTag == "left"))
			{
				if (!(sideTag == "middle"))
				{
					if (!(sideTag == "right"))
					{
						this.DefenseSide = FormationAI.BehaviorSide.BehaviorSideNotSet;
					}
					else
					{
						this.DefenseSide = FormationAI.BehaviorSide.Right;
					}
				}
				else
				{
					this.DefenseSide = FormationAI.BehaviorSide.Middle;
				}
			}
			else
			{
				this.DefenseSide = FormationAI.BehaviorSide.Left;
			}
			GameEntity gameEntity = base.GameEntity.GetChildren().FirstOrDefault((GameEntity ce) => ce.HasTag("solid_child"));
			List<GameEntity> list = new List<GameEntity>();
			List<GameEntity> list2 = new List<GameEntity>();
			if (gameEntity != null)
			{
				list = gameEntity.CollectChildrenEntitiesWithTag("middle_pos");
				list2 = gameEntity.CollectChildrenEntitiesWithTag("wait_pos");
			}
			else
			{
				list = base.GameEntity.CollectChildrenEntitiesWithTag("middle_pos");
				list2 = base.GameEntity.CollectChildrenEntitiesWithTag("wait_pos");
			}
			MatrixFrame matrixFrame;
			if (list.Count > 0)
			{
				GameEntity gameEntity2 = list.FirstOrDefault<GameEntity>();
				this.MiddlePosition = gameEntity2.GetFirstScriptOfType<TacticalPosition>();
				matrixFrame = gameEntity2.GetGlobalFrame();
			}
			else
			{
				matrixFrame = base.GameEntity.GetGlobalFrame();
			}
			this.MiddleFrame = new WorldFrame(matrixFrame.rotation, matrixFrame.origin.ToWorldPosition());
			if (list2.Count > 0)
			{
				GameEntity gameEntity3 = list2.FirstOrDefault<GameEntity>();
				this.WaitPosition = gameEntity3.GetFirstScriptOfType<TacticalPosition>();
				matrixFrame = gameEntity3.GetGlobalFrame();
				this.DefenseWaitFrame = new WorldFrame(matrixFrame.rotation, matrixFrame.origin.ToWorldPosition());
				return;
			}
			this.DefenseWaitFrame = this.MiddleFrame;
		}

		// Token: 0x06002FAB RID: 12203 RVA: 0x000C34C1 File Offset: 0x000C16C1
		protected internal override bool MovesEntity()
		{
			return false;
		}

		// Token: 0x06002FAC RID: 12204 RVA: 0x000C34C4 File Offset: 0x000C16C4
		public void OnChooseUsedWallSegment(bool isBroken)
		{
			GameEntity gameEntity = base.GameEntity.GetChildren().FirstOrDefault((GameEntity ce) => ce.HasTag("solid_child"));
			GameEntity gameEntity2 = base.GameEntity.GetChildren().FirstOrDefault((GameEntity ce) => ce.HasTag("broken_child"));
			Scene scene = base.GameEntity.Scene;
			if (isBroken)
			{
				gameEntity.GetFirstScriptOfType<WallSegment>().SetDisabledSynched();
				gameEntity2.GetFirstScriptOfType<WallSegment>().SetVisibleSynched(true, false);
				if (!GameNetwork.IsClientOrReplay)
				{
					if (this._properGroundOutsideNavmeshID > 0 && this._underDebrisOutsideNavmeshID > 0)
					{
						scene.SeparateFacesWithId(this._properGroundOutsideNavmeshID, this._underDebrisOutsideNavmeshID);
					}
					if (this._properGroundInsideNavmeshID > 0 && this._underDebrisInsideNavmeshID > 0)
					{
						scene.SeparateFacesWithId(this._properGroundInsideNavmeshID, this._underDebrisInsideNavmeshID);
					}
					if (this._underDebrisOutsideNavmeshID > 0)
					{
						scene.SetAbilityOfFacesWithId(this._underDebrisOutsideNavmeshID, false);
					}
					if (this._underDebrisInsideNavmeshID > 0)
					{
						scene.SetAbilityOfFacesWithId(this._underDebrisInsideNavmeshID, false);
					}
					if (this._underDebrisGenericNavmeshID > 0)
					{
						scene.SetAbilityOfFacesWithId(this._underDebrisGenericNavmeshID, false);
					}
					if (this._overDebrisOutsideNavmeshID > 0)
					{
						scene.SetAbilityOfFacesWithId(this._overDebrisOutsideNavmeshID, true);
						if (this._properGroundOutsideNavmeshID > 0)
						{
							scene.MergeFacesWithId(this._overDebrisOutsideNavmeshID, this._properGroundOutsideNavmeshID, 0);
						}
					}
					if (this._overDebrisInsideNavmeshID > 0)
					{
						scene.SetAbilityOfFacesWithId(this._overDebrisInsideNavmeshID, true);
						if (this._properGroundInsideNavmeshID > 0)
						{
							scene.MergeFacesWithId(this._overDebrisInsideNavmeshID, this._properGroundInsideNavmeshID, 1);
						}
					}
					if (this._overDebrisGenericNavmeshID > 0)
					{
						scene.SetAbilityOfFacesWithId(this._overDebrisGenericNavmeshID, true);
					}
					if (this._onSolidWallGenericNavmeshID > 0)
					{
						scene.SetAbilityOfFacesWithId(this._onSolidWallGenericNavmeshID, false);
					}
					foreach (StrategicArea strategicArea in from c in gameEntity.GetChildren()
						where c.HasScriptOfType<StrategicArea>()
						select c.GetFirstScriptOfType<StrategicArea>())
					{
						strategicArea.OnParentGameEntityVisibilityChanged(false);
					}
					foreach (StrategicArea strategicArea2 in from c in gameEntity2.GetChildren()
						where c.HasScriptOfType<StrategicArea>()
						select c.GetFirstScriptOfType<StrategicArea>())
					{
						strategicArea2.OnParentGameEntityVisibilityChanged(true);
					}
				}
				this.IsBreachedWall = true;
				List<GameEntity> list = gameEntity2.CollectChildrenEntitiesWithTag("middle_pos");
				if (list.Count > 0)
				{
					GameEntity gameEntity3 = list.FirstOrDefault<GameEntity>();
					this.MiddlePosition = gameEntity3.GetFirstScriptOfType<TacticalPosition>();
					MatrixFrame globalFrame = gameEntity3.GetGlobalFrame();
					this.MiddleFrame = new WorldFrame(globalFrame.rotation, globalFrame.origin.ToWorldPosition());
				}
				else
				{
					MBDebug.ShowWarning("Broken child of wall does not have middle position");
					MatrixFrame globalFrame2 = gameEntity2.GetGlobalFrame();
					this.MiddleFrame = new WorldFrame(globalFrame2.rotation, new WorldPosition(scene, UIntPtr.Zero, globalFrame2.origin, false));
				}
				List<GameEntity> list2 = gameEntity2.CollectChildrenEntitiesWithTag("wait_pos");
				if (list2.Count > 0)
				{
					GameEntity gameEntity4 = list2.FirstOrDefault<GameEntity>();
					this.WaitPosition = gameEntity4.GetFirstScriptOfType<TacticalPosition>();
					MatrixFrame globalFrame3 = gameEntity4.GetGlobalFrame();
					this.DefenseWaitFrame = new WorldFrame(globalFrame3.rotation, globalFrame3.origin.ToWorldPosition());
				}
				else
				{
					this.DefenseWaitFrame = this.MiddleFrame;
				}
				WallSegment firstScriptOfType = gameEntity.GetFirstScriptOfType<WallSegment>();
				if (firstScriptOfType != null)
				{
					firstScriptOfType.SetDisabledAndMakeInvisible(true);
				}
				GameEntity gameEntity5 = gameEntity2.CollectChildrenEntitiesWithTag("attacker_wait_pos").FirstOrDefault<GameEntity>();
				if (gameEntity5 != null)
				{
					MatrixFrame globalFrame4 = gameEntity5.GetGlobalFrame();
					this.AttackerWaitFrame = new WorldFrame(globalFrame4.rotation, globalFrame4.origin.ToWorldPosition());
					this.AttackerWaitPosition = gameEntity5.GetFirstScriptOfType<TacticalPosition>();
					return;
				}
			}
			else if (!GameNetwork.IsClientOrReplay)
			{
				gameEntity.GetFirstScriptOfType<WallSegment>().SetVisibleSynched(true, false);
				gameEntity2.GetFirstScriptOfType<WallSegment>().SetDisabledSynched();
				if (this._overDebrisOutsideNavmeshID > 0)
				{
					scene.SetAbilityOfFacesWithId(this._overDebrisOutsideNavmeshID, false);
				}
				if (this._overDebrisInsideNavmeshID > 0)
				{
					scene.SetAbilityOfFacesWithId(this._overDebrisInsideNavmeshID, false);
				}
				if (this._overDebrisGenericNavmeshID > 0)
				{
					scene.SetAbilityOfFacesWithId(this._overDebrisGenericNavmeshID, false);
				}
				foreach (StrategicArea strategicArea3 in from c in gameEntity.GetChildren()
					where c.HasScriptOfType<StrategicArea>()
					select c.GetFirstScriptOfType<StrategicArea>())
				{
					strategicArea3.OnParentGameEntityVisibilityChanged(true);
				}
				foreach (StrategicArea strategicArea4 in from c in gameEntity2.GetChildren()
					where c.HasScriptOfType<StrategicArea>()
					select c.GetFirstScriptOfType<StrategicArea>())
				{
					strategicArea4.OnParentGameEntityVisibilityChanged(false);
				}
			}
		}

		// Token: 0x06002FAD RID: 12205 RVA: 0x000C3A5C File Offset: 0x000C1C5C
		protected internal override void OnEditorValidate()
		{
			base.OnEditorValidate();
		}

		// Token: 0x06002FAE RID: 12206 RVA: 0x000C3A64 File Offset: 0x000C1C64
		protected internal override bool OnCheckForProblems()
		{
			bool flag = base.OnCheckForProblems();
			if (!base.Scene.IsMultiplayerScene() && this.SideTag == "left")
			{
				List<GameEntity> list = new List<GameEntity>();
				base.Scene.GetEntities(ref list);
				int num = 0;
				foreach (GameEntity gameEntity in list)
				{
					if (base.GameEntity.GetUpgradeLevelOfEntity() == gameEntity.GetUpgradeLevelOfEntity() && gameEntity.GetFirstScriptOfType<SiegeLadderSpawner>() != null)
					{
						num++;
					}
				}
				if (num != 4)
				{
					MBEditor.AddEntityWarning(base.GameEntity, "The siege ladder count in the scene is not 4, for upgrade level " + base.GameEntity.GetUpgradeLevelOfEntity().ToString() + ". Current siege ladder count: " + num.ToString());
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x040013AF RID: 5039
		private const string WaitPositionTag = "wait_pos";

		// Token: 0x040013B0 RID: 5040
		private const string MiddlePositionTag = "middle_pos";

		// Token: 0x040013B1 RID: 5041
		private const string AttackerWaitPositionTag = "attacker_wait_pos";

		// Token: 0x040013B2 RID: 5042
		private const string SolidChildTag = "solid_child";

		// Token: 0x040013B3 RID: 5043
		private const string BrokenChildTag = "broken_child";

		// Token: 0x040013B4 RID: 5044
		[EditableScriptComponentVariable(true)]
		private int _properGroundOutsideNavmeshID = -1;

		// Token: 0x040013B5 RID: 5045
		[EditableScriptComponentVariable(true)]
		private int _properGroundInsideNavmeshID = -1;

		// Token: 0x040013B6 RID: 5046
		[EditableScriptComponentVariable(true)]
		private int _underDebrisOutsideNavmeshID = -1;

		// Token: 0x040013B7 RID: 5047
		[EditableScriptComponentVariable(true)]
		private int _underDebrisInsideNavmeshID = -1;

		// Token: 0x040013B8 RID: 5048
		[EditableScriptComponentVariable(true)]
		private int _overDebrisOutsideNavmeshID = -1;

		// Token: 0x040013B9 RID: 5049
		[EditableScriptComponentVariable(true)]
		private int _overDebrisInsideNavmeshID = -1;

		// Token: 0x040013BA RID: 5050
		[EditableScriptComponentVariable(true)]
		private int _underDebrisGenericNavmeshID = -1;

		// Token: 0x040013BB RID: 5051
		[EditableScriptComponentVariable(true)]
		private int _overDebrisGenericNavmeshID = -1;

		// Token: 0x040013BC RID: 5052
		[EditableScriptComponentVariable(true)]
		private int _onSolidWallGenericNavmeshID = -1;

		// Token: 0x040013C2 RID: 5058
		public string SideTag;
	}
}
