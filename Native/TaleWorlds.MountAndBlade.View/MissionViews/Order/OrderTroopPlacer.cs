using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions.Handlers;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Order
{
	// Token: 0x02000081 RID: 129
	public class OrderTroopPlacer : MissionView
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060004C3 RID: 1219 RVA: 0x0002499A File Offset: 0x00022B9A
		// (set) Token: 0x060004C4 RID: 1220 RVA: 0x000249A2 File Offset: 0x00022BA2
		public bool SuspendTroopPlacer
		{
			get
			{
				return this._suspendTroopPlacer;
			}
			set
			{
				this._suspendTroopPlacer = value;
				if (value)
				{
					this.HideOrderPositionEntities();
				}
				else
				{
					this._formationDrawingStartingPosition = null;
				}
				this.Reset();
			}
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x000249C8 File Offset: 0x00022BC8
		public override void AfterStart()
		{
			base.AfterStart();
			this._formationDrawingStartingPosition = null;
			this._formationDrawingStartingPointOfMouse = null;
			this._formationDrawingStartingTime = null;
			this._orderRotationEntities = new List<GameEntity>();
			this._orderPositionEntities = new List<GameEntity>();
			this.formationDrawTimer = new Timer(MBCommon.GetApplicationTime(), 0.033333335f, true);
			this.widthEntityLeft = GameEntity.CreateEmpty(base.Mission.Scene, true);
			this.widthEntityLeft.AddComponent(MetaMesh.GetCopy("order_arrow_a", true, false));
			this.widthEntityLeft.SetVisibilityExcludeParents(false);
			this.widthEntityRight = GameEntity.CreateEmpty(base.Mission.Scene, true);
			this.widthEntityRight.AddComponent(MetaMesh.GetCopy("order_arrow_a", true, false));
			this.widthEntityRight.SetVisibilityExcludeParents(false);
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x00024A9F File Offset: 0x00022C9F
		private void InitializeInADisgustingManner()
		{
			this.PlayerTeam = base.Mission.PlayerTeam;
			this.PlayerOrderController = this.PlayerTeam.PlayerOrderController;
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x00024AC4 File Offset: 0x00022CC4
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (!this._initialized)
			{
				MissionPeer missionPeer = (GameNetwork.IsMyPeerReady ? PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer) : null);
				if (base.Mission.PlayerTeam != null || (missionPeer != null && (missionPeer.Team == base.Mission.AttackerTeam || missionPeer.Team == base.Mission.DefenderTeam)))
				{
					this.InitializeInADisgustingManner();
					this._initialized = true;
				}
			}
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x00024B38 File Offset: 0x00022D38
		public void RestrictOrdersToDeploymentBoundaries(bool enabled)
		{
			this._restrictOrdersToDeploymentBoundaries = enabled;
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x00024B44 File Offset: 0x00022D44
		private void UpdateFormationDrawingForFacingOrder(bool giveOrder)
		{
			this.isDrawnThisFrame = true;
			Vec3 vec = base.MissionScreen.GetOrderFlagPosition();
			Vec2 asVec = vec.AsVec2;
			Vec2 orderLookAtDirection = OrderController.GetOrderLookAtDirection(this.PlayerOrderController.SelectedFormations, asVec);
			List<WorldPosition> list;
			this.PlayerOrderController.SimulateNewFacingOrder(orderLookAtDirection, ref list);
			int num = 0;
			this.HideOrderPositionEntities();
			foreach (WorldPosition worldPosition in list)
			{
				int num2 = num;
				vec = worldPosition.GetGroundVec3();
				this.AddOrderPositionEntity(num2, vec, giveOrder, -1f);
				num++;
			}
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x00024BF0 File Offset: 0x00022DF0
		private void UpdateFormationDrawingForDestination(bool giveOrder)
		{
			this.isDrawnThisFrame = true;
			List<WorldPosition> list;
			this.PlayerOrderController.SimulateDestinationFrames(ref list, 3f);
			int num = 0;
			this.HideOrderPositionEntities();
			foreach (WorldPosition worldPosition in list)
			{
				int num2 = num;
				Vec3 groundVec = worldPosition.GetGroundVec3();
				this.AddOrderPositionEntity(num2, groundVec, giveOrder, 0.7f);
				num++;
			}
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x00024C74 File Offset: 0x00022E74
		private void UpdateFormationDrawingForFormingOrder(bool giveOrder)
		{
			this.isDrawnThisFrame = true;
			MatrixFrame orderFlagFrame = base.MissionScreen.GetOrderFlagFrame();
			Vec3 origin = orderFlagFrame.origin;
			Vec2 asVec = orderFlagFrame.rotation.f.AsVec2;
			float orderFormCustomWidth = OrderController.GetOrderFormCustomWidth(this.PlayerOrderController.SelectedFormations, origin);
			List<WorldPosition> list;
			this.PlayerOrderController.SimulateNewCustomWidthOrder(orderFormCustomWidth, ref list);
			Formation formation = Extensions.MaxBy<Formation, int>(this.PlayerOrderController.SelectedFormations, (Formation f) => f.CountOfUnits);
			int num = 0;
			this.HideOrderPositionEntities();
			foreach (WorldPosition worldPosition in list)
			{
				int num2 = num;
				Vec3 vec = worldPosition.GetGroundVec3();
				this.AddOrderPositionEntity(num2, vec, giveOrder, -1f);
				num++;
			}
			float unitDiameter = formation.UnitDiameter;
			float interval = formation.Interval;
			int num3 = MathF.Max(0, (int)((orderFormCustomWidth - unitDiameter) / (interval + unitDiameter) + 1E-05f)) + 1;
			float num4 = (float)(num3 - 1) * (interval + unitDiameter);
			for (int i = 0; i < num3; i++)
			{
				Vec2 vec2;
				vec2..ctor((float)i * (interval + unitDiameter) - num4 / 2f, 0f);
				Vec2 vec3 = asVec.TransformToParentUnitF(vec2);
				WorldPosition worldPosition2;
				worldPosition2..ctor(Mission.Current.Scene, UIntPtr.Zero, origin, false);
				worldPosition2.SetVec2(worldPosition2.AsVec2 + vec3);
				int num5 = num++;
				Vec3 vec = worldPosition2.GetGroundVec3();
				this.AddOrderPositionEntity(num5, vec, false, -1f);
			}
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x00024E24 File Offset: 0x00023024
		private void UpdateFormationDrawing(bool giveOrder)
		{
			this.isDrawnThisFrame = true;
			this.HideOrderPositionEntities();
			if (this._formationDrawingStartingPosition == null)
			{
				return;
			}
			WorldPosition worldPosition = WorldPosition.Invalid;
			bool flag = false;
			if (base.MissionScreen.MouseVisible && this._formationDrawingStartingPointOfMouse != null)
			{
				Vec2 vec = this._formationDrawingStartingPointOfMouse.Value - base.Input.GetMousePositionPixel();
				if (MathF.Abs(vec.x) < 10f && MathF.Abs(vec.y) < 10f)
				{
					flag = true;
					worldPosition = this._formationDrawingStartingPosition.Value;
				}
			}
			if (base.MissionScreen.MouseVisible && this._formationDrawingStartingTime != null && base.Mission.CurrentTime - this._formationDrawingStartingTime.Value < 0.3f)
			{
				flag = true;
				worldPosition = this._formationDrawingStartingPosition.Value;
			}
			if (!flag)
			{
				Vec3 vec2;
				Vec3 vec3;
				base.MissionScreen.ScreenPointToWorldRay(this.GetScreenPoint(), out vec2, out vec3);
				float num;
				if (!base.Mission.Scene.RayCastForClosestEntityOrTerrain(vec2, vec3, ref num, 0.3f, 67188481))
				{
					return;
				}
				Vec3 vec4 = vec3 - vec2;
				vec4.Normalize();
				worldPosition..ctor(base.Mission.Scene, UIntPtr.Zero, vec2 + vec4 * num, false);
			}
			WorldPosition worldPosition2;
			if (this._mouseOverDirection == 1)
			{
				worldPosition2 = worldPosition;
				worldPosition = this._formationDrawingStartingPosition.Value;
			}
			else
			{
				worldPosition2 = this._formationDrawingStartingPosition.Value;
			}
			if (!OrderFlag.IsPositionOnValidGround(worldPosition2))
			{
				return;
			}
			Vec2 vec5;
			if (this._restrictOrdersToDeploymentBoundaries)
			{
				IMissionDeploymentPlan deploymentPlan = base.Mission.DeploymentPlan;
				BattleSideEnum side = base.Mission.PlayerTeam.Side;
				vec5 = worldPosition2.AsVec2;
				if (deploymentPlan.IsPositionInsideDeploymentBoundaries(side, ref vec5, 0))
				{
					IMissionDeploymentPlan deploymentPlan2 = base.Mission.DeploymentPlan;
					BattleSideEnum side2 = base.Mission.PlayerTeam.Side;
					Vec2 asVec = worldPosition.AsVec2;
					if (deploymentPlan2.IsPositionInsideDeploymentBoundaries(side2, ref asVec, 0))
					{
						goto IL_1DD;
					}
				}
				return;
			}
			IL_1DD:
			bool flag2 = !base.DebugInput.IsControlDown();
			this.UpdateFormationDrawingForMovementOrder(giveOrder, worldPosition2, worldPosition, flag2);
			Vec2 deltaMousePosition = this._deltaMousePosition;
			float num2 = 1f;
			vec5 = base.Input.GetMousePositionRanged() - this._lastMousePosition;
			this._deltaMousePosition = deltaMousePosition * MathF.Max(num2 - vec5.Length * 10f, 0f);
			this._lastMousePosition = base.Input.GetMousePositionRanged();
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x00025080 File Offset: 0x00023280
		private void UpdateFormationDrawingForMovementOrder(bool giveOrder, WorldPosition formationRealStartingPosition, WorldPosition formationRealEndingPosition, bool isFormationLayoutVertical)
		{
			this.isDrawnThisFrame = true;
			List<WorldPosition> list;
			this.PlayerOrderController.SimulateNewOrderWithPositionAndDirection(formationRealStartingPosition, formationRealEndingPosition, ref list, isFormationLayoutVertical);
			if (giveOrder)
			{
				if (!isFormationLayoutVertical)
				{
					this.PlayerOrderController.SetOrderWithTwoPositions(3, formationRealStartingPosition, formationRealEndingPosition);
				}
				else
				{
					this.PlayerOrderController.SetOrderWithTwoPositions(2, formationRealStartingPosition, formationRealEndingPosition);
				}
			}
			int num = 0;
			foreach (WorldPosition worldPosition in list)
			{
				int num2 = num;
				Vec3 groundVec = worldPosition.GetGroundVec3();
				this.AddOrderPositionEntity(num2, groundVec, giveOrder, -1f);
				num++;
			}
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x00025124 File Offset: 0x00023324
		private void HandleMouseDown()
		{
			if (!Extensions.IsEmpty<Formation>(this.PlayerOrderController.SelectedFormations) && this._clickedFormation == null)
			{
				switch (this.GetCursorState())
				{
				case OrderTroopPlacer.CursorState.Invisible:
				case OrderTroopPlacer.CursorState.Ground:
					break;
				case OrderTroopPlacer.CursorState.Normal:
				{
					this._formationDrawingMode = true;
					Vec3 vec;
					Vec3 vec2;
					base.MissionScreen.ScreenPointToWorldRay(this.GetScreenPoint(), out vec, out vec2);
					float num;
					if (base.Mission.Scene.RayCastForClosestEntityOrTerrain(vec, vec2, ref num, 0.3f, 67188481))
					{
						Vec3 vec3 = vec2 - vec;
						vec3.Normalize();
						this._formationDrawingStartingPosition = new WorldPosition?(new WorldPosition(base.Mission.Scene, UIntPtr.Zero, vec + vec3 * num, false));
						this._formationDrawingStartingPointOfMouse = new Vec2?(base.Input.GetMousePositionPixel());
						this._formationDrawingStartingTime = new float?(base.Mission.CurrentTime);
						return;
					}
					this._formationDrawingStartingPosition = null;
					this._formationDrawingStartingPointOfMouse = null;
					this._formationDrawingStartingTime = null;
					return;
				}
				case OrderTroopPlacer.CursorState.Enemy:
				case OrderTroopPlacer.CursorState.Friend:
					this._clickedFormation = this._mouseOverFormation;
					return;
				case OrderTroopPlacer.CursorState.Rotation:
					if (this._mouseOverFormation.CountOfUnits > 0)
					{
						this.HideNonSelectedOrderRotationEntities(this._mouseOverFormation);
						this.PlayerOrderController.ClearSelectedFormations();
						this.PlayerOrderController.SelectFormation(this._mouseOverFormation);
						this._formationDrawingMode = true;
						WorldPosition worldPosition = this._mouseOverFormation.CreateNewOrderWorldPosition(2);
						Vec2 direction = this._mouseOverFormation.Direction;
						direction.RotateCCW(-1.5707964f);
						this._formationDrawingStartingPosition = new WorldPosition?(worldPosition);
						this._formationDrawingStartingPosition.Value.SetVec2(this._formationDrawingStartingPosition.Value.AsVec2 + direction * ((this._mouseOverDirection == 1) ? 0.5f : (-0.5f)) * this._mouseOverFormation.Width);
						WorldPosition worldPosition2 = worldPosition;
						worldPosition2.SetVec2(worldPosition2.AsVec2 + direction * ((this._mouseOverDirection == 1) ? (-0.5f) : 0.5f) * this._mouseOverFormation.Width);
						Vec2 vec4 = base.MissionScreen.SceneView.WorldPointToScreenPoint(worldPosition2.GetGroundVec3());
						Vec2 screenPoint = this.GetScreenPoint();
						this._deltaMousePosition = vec4 - screenPoint;
						this._lastMousePosition = base.Input.GetMousePositionRanged();
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x000253A4 File Offset: 0x000235A4
		private void HandleMouseUp()
		{
			if (this._clickedFormation != null)
			{
				if (this._clickedFormation.CountOfUnits > 0 && this._clickedFormation.Team == this.PlayerTeam)
				{
					Formation clickedFormation = this._clickedFormation;
					this._clickedFormation = null;
					this.GetCursorState();
					this._clickedFormation = clickedFormation;
					this.HideNonSelectedOrderRotationEntities(this._clickedFormation);
					this.PlayerOrderController.ClearSelectedFormations();
					this.PlayerOrderController.SelectFormation(this._clickedFormation);
				}
				this._clickedFormation = null;
			}
			else if (this.GetCursorState() == OrderTroopPlacer.CursorState.Ground)
			{
				if (this.IsDrawingFacing || this._wasDrawingFacing)
				{
					this.UpdateFormationDrawingForFacingOrder(true);
				}
				else if (this.IsDrawingForming || this._wasDrawingForming)
				{
					this.UpdateFormationDrawingForFormingOrder(true);
				}
				else
				{
					this.UpdateFormationDrawing(true);
				}
				if (this.IsDeployment)
				{
					Action onUnitDeployed = this.OnUnitDeployed;
					if (onUnitDeployed != null)
					{
						onUnitDeployed();
					}
					SoundEvent.PlaySound2D("event:/ui/mission/deploy");
				}
			}
			this._formationDrawingMode = false;
			this._deltaMousePosition = Vec2.Zero;
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0002549F File Offset: 0x0002369F
		private Vec2 GetScreenPoint()
		{
			if (!base.MissionScreen.MouseVisible)
			{
				return new Vec2(0.5f, 0.5f) + this._deltaMousePosition;
			}
			return base.Input.GetMousePositionRanged() + this._deltaMousePosition;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x000254E0 File Offset: 0x000236E0
		private OrderTroopPlacer.CursorState GetCursorState()
		{
			OrderTroopPlacer.CursorState cursorState = OrderTroopPlacer.CursorState.Invisible;
			if (!Extensions.IsEmpty<Formation>(this.PlayerOrderController.SelectedFormations) && this._clickedFormation == null)
			{
				Vec3 vec;
				Vec3 vec2;
				base.MissionScreen.ScreenPointToWorldRay(this.GetScreenPoint(), out vec, out vec2);
				float num;
				GameEntity gameEntity;
				if (!base.Mission.Scene.RayCastForClosestEntityOrTerrain(vec, vec2, ref num, ref gameEntity, 0.3f, 67188481))
				{
					num = 1000f;
				}
				if (cursorState == OrderTroopPlacer.CursorState.Invisible && num < 1000f)
				{
					if (!this._formationDrawingMode && gameEntity == null)
					{
						for (int i = 0; i < this._orderRotationEntities.Count; i++)
						{
							GameEntity gameEntity2 = this._orderRotationEntities[i];
							if (gameEntity2.IsVisibleIncludeParents() && gameEntity == gameEntity2)
							{
								this._mouseOverFormation = this.PlayerOrderController.SelectedFormations.ElementAt(i / 2);
								this._mouseOverDirection = 1 - (i & 1);
								cursorState = OrderTroopPlacer.CursorState.Rotation;
								break;
							}
						}
					}
					if (cursorState == OrderTroopPlacer.CursorState.Invisible && base.MissionScreen.OrderFlag.FocusedOrderableObject != null)
					{
						cursorState = OrderTroopPlacer.CursorState.OrderableEntity;
					}
					if (cursorState == OrderTroopPlacer.CursorState.Invisible)
					{
						cursorState = this.IsCursorStateGroundOrNormal();
					}
				}
			}
			if (cursorState != OrderTroopPlacer.CursorState.Ground && cursorState != OrderTroopPlacer.CursorState.Rotation)
			{
				this._mouseOverDirection = 0;
			}
			return cursorState;
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x00025607 File Offset: 0x00023807
		private OrderTroopPlacer.CursorState IsCursorStateGroundOrNormal()
		{
			if (!this._formationDrawingMode)
			{
				return OrderTroopPlacer.CursorState.Normal;
			}
			return OrderTroopPlacer.CursorState.Ground;
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x00025614 File Offset: 0x00023814
		private void AddOrderPositionEntity(int entityIndex, in Vec3 groundPosition, bool fadeOut, float alpha = -1f)
		{
			while (this._orderPositionEntities.Count <= entityIndex)
			{
				GameEntity gameEntity = GameEntity.CreateEmpty(base.Mission.Scene, true);
				gameEntity.EntityFlags |= 4194304;
				MetaMesh copy = MetaMesh.GetCopy("order_flag_small", true, false);
				if (OrderTroopPlacer._meshMaterial == null)
				{
					OrderTroopPlacer._meshMaterial = copy.GetMeshAtIndex(0).GetMaterial().CreateCopy();
					OrderTroopPlacer._meshMaterial.SetAlphaBlendMode(6);
				}
				copy.SetMaterial(OrderTroopPlacer._meshMaterial);
				gameEntity.AddComponent(copy);
				gameEntity.SetVisibilityExcludeParents(false);
				this._orderPositionEntities.Add(gameEntity);
			}
			GameEntity gameEntity2 = this._orderPositionEntities[entityIndex];
			MatrixFrame matrixFrame;
			matrixFrame..ctor(Mat3.Identity, groundPosition);
			gameEntity2.SetFrame(ref matrixFrame);
			if (alpha != -1f)
			{
				gameEntity2.SetVisibilityExcludeParents(true);
				gameEntity2.SetAlpha(alpha);
				return;
			}
			if (fadeOut)
			{
				GameEntityExtensions.FadeOut(gameEntity2, 0.3f, false);
				return;
			}
			GameEntityExtensions.FadeIn(gameEntity2, true);
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x00025714 File Offset: 0x00023914
		private void HideNonSelectedOrderRotationEntities(Formation formation)
		{
			for (int i = 0; i < this._orderRotationEntities.Count; i++)
			{
				GameEntity gameEntity = this._orderRotationEntities[i];
				if (gameEntity == null && gameEntity.IsVisibleIncludeParents() && this.PlayerOrderController.SelectedFormations.ElementAt(i / 2) != formation)
				{
					gameEntity.SetVisibilityExcludeParents(false);
					gameEntity.BodyFlag |= 1;
				}
			}
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x00025780 File Offset: 0x00023980
		private void HideOrderPositionEntities()
		{
			foreach (GameEntity gameEntity in this._orderPositionEntities)
			{
				GameEntityExtensions.HideIfNotFadingOut(gameEntity);
			}
			for (int i = 0; i < this._orderRotationEntities.Count; i++)
			{
				GameEntity gameEntity2 = this._orderRotationEntities[i];
				gameEntity2.SetVisibilityExcludeParents(false);
				gameEntity2.BodyFlag |= 1;
			}
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x00025808 File Offset: 0x00023A08
		[Conditional("DEBUG")]
		private void DebugTick(float dt)
		{
			bool initialized = this._initialized;
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x00025814 File Offset: 0x00023A14
		private void Reset()
		{
			this._isMouseDown = false;
			this._formationDrawingMode = false;
			this._formationDrawingStartingPosition = null;
			this._formationDrawingStartingPointOfMouse = null;
			this._formationDrawingStartingTime = null;
			this._mouseOverFormation = null;
			this._clickedFormation = null;
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x00025864 File Offset: 0x00023A64
		public override void OnMissionScreenTick(float dt)
		{
			if (!this._initialized)
			{
				return;
			}
			base.OnMissionScreenTick(dt);
			if (this.PlayerOrderController.SelectedFormations.Count == 0)
			{
				return;
			}
			this.isDrawnThisFrame = false;
			if (this.SuspendTroopPlacer)
			{
				return;
			}
			if (base.Input.IsKeyPressed(224) || base.Input.IsKeyPressed(255))
			{
				this._isMouseDown = true;
				this.HandleMouseDown();
			}
			if ((base.Input.IsKeyReleased(224) || base.Input.IsKeyReleased(255)) && this._isMouseDown)
			{
				this._isMouseDown = false;
				this.HandleMouseUp();
			}
			else if ((base.Input.IsKeyDown(224) || base.Input.IsKeyDown(255)) && this._isMouseDown)
			{
				if (this.formationDrawTimer.Check(MBCommon.GetApplicationTime()) && !this.IsDrawingFacing && !this.IsDrawingForming && this.IsCursorStateGroundOrNormal() == OrderTroopPlacer.CursorState.Ground && this.GetCursorState() == OrderTroopPlacer.CursorState.Ground)
				{
					this.UpdateFormationDrawing(false);
				}
			}
			else if (this.IsDrawingForced)
			{
				this.Reset();
				this.HandleMouseDown();
				this.UpdateFormationDrawing(false);
			}
			else if (this.IsDrawingFacing || this._wasDrawingFacing)
			{
				if (this.IsDrawingFacing)
				{
					this.Reset();
					this.UpdateFormationDrawingForFacingOrder(false);
				}
			}
			else if (this.IsDrawingForming || this._wasDrawingForming)
			{
				if (this.IsDrawingForming)
				{
					this.Reset();
					this.UpdateFormationDrawingForFormingOrder(false);
				}
			}
			else if (this._wasDrawingForced)
			{
				this.Reset();
			}
			else
			{
				this.UpdateFormationDrawingForDestination(false);
			}
			foreach (GameEntity gameEntity in this._orderPositionEntities)
			{
				gameEntity.SetPreviousFrameInvalid();
			}
			foreach (GameEntity gameEntity2 in this._orderRotationEntities)
			{
				gameEntity2.SetPreviousFrameInvalid();
			}
			this._wasDrawingForced = this.IsDrawingForced;
			this._wasDrawingFacing = this.IsDrawingFacing;
			this._wasDrawingForming = this.IsDrawingForming;
			this.wasDrawnPreviousFrame = this.isDrawnThisFrame;
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060004D9 RID: 1241 RVA: 0x00025AC4 File Offset: 0x00023CC4
		private bool IsDeployment
		{
			get
			{
				return base.Mission.GetMissionBehavior<SiegeDeploymentHandler>() != null || base.Mission.GetMissionBehavior<BattleDeploymentHandler>() != null;
			}
		}

		// Token: 0x040002E3 RID: 739
		private bool _suspendTroopPlacer;

		// Token: 0x040002E4 RID: 740
		private bool _isMouseDown;

		// Token: 0x040002E5 RID: 741
		private List<GameEntity> _orderPositionEntities;

		// Token: 0x040002E6 RID: 742
		private List<GameEntity> _orderRotationEntities;

		// Token: 0x040002E7 RID: 743
		private bool _formationDrawingMode;

		// Token: 0x040002E8 RID: 744
		private Formation _mouseOverFormation;

		// Token: 0x040002E9 RID: 745
		private Formation _clickedFormation;

		// Token: 0x040002EA RID: 746
		private Vec2 _lastMousePosition;

		// Token: 0x040002EB RID: 747
		private Vec2 _deltaMousePosition;

		// Token: 0x040002EC RID: 748
		private int _mouseOverDirection;

		// Token: 0x040002ED RID: 749
		private WorldPosition? _formationDrawingStartingPosition;

		// Token: 0x040002EE RID: 750
		private Vec2? _formationDrawingStartingPointOfMouse;

		// Token: 0x040002EF RID: 751
		private float? _formationDrawingStartingTime;

		// Token: 0x040002F0 RID: 752
		private bool _restrictOrdersToDeploymentBoundaries;

		// Token: 0x040002F1 RID: 753
		private OrderController PlayerOrderController;

		// Token: 0x040002F2 RID: 754
		private Team PlayerTeam;

		// Token: 0x040002F3 RID: 755
		private bool _initialized;

		// Token: 0x040002F4 RID: 756
		private Timer formationDrawTimer;

		// Token: 0x040002F5 RID: 757
		public bool IsDrawingForced;

		// Token: 0x040002F6 RID: 758
		public bool IsDrawingFacing;

		// Token: 0x040002F7 RID: 759
		public bool IsDrawingForming;

		// Token: 0x040002F8 RID: 760
		private bool _wasDrawingForced;

		// Token: 0x040002F9 RID: 761
		private bool _wasDrawingFacing;

		// Token: 0x040002FA RID: 762
		private bool _wasDrawingForming;

		// Token: 0x040002FB RID: 763
		private GameEntity widthEntityLeft;

		// Token: 0x040002FC RID: 764
		private GameEntity widthEntityRight;

		// Token: 0x040002FD RID: 765
		private bool isDrawnThisFrame;

		// Token: 0x040002FE RID: 766
		private bool wasDrawnPreviousFrame;

		// Token: 0x040002FF RID: 767
		public Action OnUnitDeployed;

		// Token: 0x04000300 RID: 768
		private static Material _meshMaterial;

		// Token: 0x020000CF RID: 207
		public enum CursorState
		{
			// Token: 0x040003B0 RID: 944
			Invisible,
			// Token: 0x040003B1 RID: 945
			Normal,
			// Token: 0x040003B2 RID: 946
			Ground,
			// Token: 0x040003B3 RID: 947
			Enemy,
			// Token: 0x040003B4 RID: 948
			Friend,
			// Token: 0x040003B5 RID: 949
			Rotation,
			// Token: 0x040003B6 RID: 950
			Count,
			// Token: 0x040003B7 RID: 951
			OrderableEntity
		}
	}
}
