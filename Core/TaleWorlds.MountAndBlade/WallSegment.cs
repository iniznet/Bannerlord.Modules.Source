using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	public class WallSegment : SynchedMissionObject, IPointDefendable, ICastleKeyPosition
	{
		public TacticalPosition MiddlePosition { get; private set; }

		public TacticalPosition WaitPosition { get; private set; }

		public TacticalPosition AttackerWaitPosition { get; private set; }

		public IPrimarySiegeWeapon AttackerSiegeWeapon { get; set; }

		public IEnumerable<DefencePoint> DefencePoints { get; protected set; }

		public bool IsBreachedWall { get; private set; }

		public WorldFrame MiddleFrame { get; private set; }

		public WorldFrame DefenseWaitFrame { get; private set; }

		public WorldFrame AttackerWaitFrame { get; private set; } = WorldFrame.Invalid;

		public FormationAI.BehaviorSide DefenseSide { get; private set; }

		public Vec3 GetPosition()
		{
			return base.GameEntity.GlobalPosition;
		}

		public WallSegment()
		{
			this.AttackerSiegeWeapon = null;
		}

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

		protected internal override bool MovesEntity()
		{
			return false;
		}

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

		protected internal override void OnEditorValidate()
		{
			base.OnEditorValidate();
		}

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

		private const string WaitPositionTag = "wait_pos";

		private const string MiddlePositionTag = "middle_pos";

		private const string AttackerWaitPositionTag = "attacker_wait_pos";

		private const string SolidChildTag = "solid_child";

		private const string BrokenChildTag = "broken_child";

		[EditableScriptComponentVariable(true)]
		private int _properGroundOutsideNavmeshID = -1;

		[EditableScriptComponentVariable(true)]
		private int _properGroundInsideNavmeshID = -1;

		[EditableScriptComponentVariable(true)]
		private int _underDebrisOutsideNavmeshID = -1;

		[EditableScriptComponentVariable(true)]
		private int _underDebrisInsideNavmeshID = -1;

		[EditableScriptComponentVariable(true)]
		private int _overDebrisOutsideNavmeshID = -1;

		[EditableScriptComponentVariable(true)]
		private int _overDebrisInsideNavmeshID = -1;

		[EditableScriptComponentVariable(true)]
		private int _underDebrisGenericNavmeshID = -1;

		[EditableScriptComponentVariable(true)]
		private int _overDebrisGenericNavmeshID = -1;

		[EditableScriptComponentVariable(true)]
		private int _onSolidWallGenericNavmeshID = -1;

		public string SideTag;
	}
}
