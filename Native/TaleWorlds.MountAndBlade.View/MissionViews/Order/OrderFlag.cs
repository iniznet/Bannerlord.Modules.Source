using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.View.Screens;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Order
{
	// Token: 0x02000080 RID: 128
	public class OrderFlag
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060004AE RID: 1198 RVA: 0x00023E59 File Offset: 0x00022059
		// (set) Token: 0x060004AF RID: 1199 RVA: 0x00023E64 File Offset: 0x00022064
		private GameEntity Current
		{
			get
			{
				return this._current;
			}
			set
			{
				if (this._current != value)
				{
					this._current = value;
					this._flag.SetVisibilityExcludeParents(false);
					this._gear.SetVisibilityExcludeParents(false);
					this._arrow.SetVisibilityExcludeParents(false);
					this._width.SetVisibilityExcludeParents(false);
					this._attack.SetVisibilityExcludeParents(false);
					this._flagUnavailable.SetVisibilityExcludeParents(false);
					this._current.SetVisibilityExcludeParents(true);
					if (this._current == this._arrow || this._current == this._flagUnavailable)
					{
						this._flag.SetVisibilityExcludeParents(true);
					}
				}
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060004B0 RID: 1200 RVA: 0x00023F0F File Offset: 0x0002210F
		// (set) Token: 0x060004B1 RID: 1201 RVA: 0x00023F17 File Offset: 0x00022117
		public IOrderable FocusedOrderableObject { get; private set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x00023F20 File Offset: 0x00022120
		// (set) Token: 0x060004B3 RID: 1203 RVA: 0x00023F28 File Offset: 0x00022128
		public int LatestUpdateFrameNo { get; private set; }

		// Token: 0x060004B4 RID: 1204 RVA: 0x00023F34 File Offset: 0x00022134
		public OrderFlag(Mission mission, MissionScreen missionScreen)
		{
			this._mission = mission;
			this._missionScreen = missionScreen;
			this._entity = GameEntity.CreateEmpty(this._mission.Scene, true);
			this._flag = GameEntity.CreateEmpty(this._mission.Scene, true);
			this._gear = GameEntity.CreateEmpty(this._mission.Scene, true);
			this._arrow = GameEntity.CreateEmpty(this._mission.Scene, true);
			this._width = GameEntity.CreateEmpty(this._mission.Scene, true);
			this._attack = GameEntity.CreateEmpty(this._mission.Scene, true);
			this._flagUnavailable = GameEntity.CreateEmpty(this._mission.Scene, true);
			this._widthLeft = GameEntity.CreateEmpty(this._mission.Scene, true);
			this._widthRight = GameEntity.CreateEmpty(this._mission.Scene, true);
			this._entity.EntityFlags |= 4194304;
			this._flag.EntityFlags |= 4194304;
			this._gear.EntityFlags |= 4194304;
			this._arrow.EntityFlags |= 4194304;
			this._width.EntityFlags |= 4194304;
			this._attack.EntityFlags |= 4194304;
			this._flagUnavailable.EntityFlags |= 4194304;
			this._widthLeft.EntityFlags |= 4194304;
			this._widthRight.EntityFlags |= 4194304;
			this._flag.AddComponent(MetaMesh.GetCopy("order_flag_a", true, false));
			MatrixFrame frame = this._flag.GetFrame();
			frame.Scale(new Vec3(10f, 10f, 10f, -1f));
			this._flag.SetFrame(ref frame);
			this._gear.AddComponent(MetaMesh.GetCopy("order_gear", true, false));
			MatrixFrame frame2 = this._gear.GetFrame();
			frame2.Scale(new Vec3(10f, 10f, 10f, -1f));
			this._gear.SetFrame(ref frame2);
			this._arrow.AddComponent(MetaMesh.GetCopy("order_arrow_a", true, false));
			this._widthLeft.AddComponent(MetaMesh.GetCopy("order_arrow_a", true, false));
			this._widthRight.AddComponent(MetaMesh.GetCopy("order_arrow_a", true, false));
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			matrixFrame.rotation.RotateAboutUp(-1.5707964f);
			this._widthLeft.SetFrame(ref matrixFrame);
			matrixFrame = MatrixFrame.Identity;
			matrixFrame.rotation.RotateAboutUp(1.5707964f);
			this._widthRight.SetFrame(ref matrixFrame);
			this._width.AddChild(this._widthLeft, false);
			this._width.AddChild(this._widthRight, false);
			MetaMesh copy = MetaMesh.GetCopy("destroy_icon", true, false);
			copy.RecomputeBoundingBox(true);
			MatrixFrame frame3 = copy.Frame;
			frame3.Scale(new Vec3(0.15f, 0.15f, 0.15f, -1f));
			frame3.Elevate(10f);
			copy.Frame = frame3;
			this._attack.AddMultiMesh(copy, true);
			this._flagUnavailable.AddComponent(MetaMesh.GetCopy("order_unavailable", true, false));
			this._entity.AddChild(this._flag, false);
			this._entity.AddChild(this._gear, false);
			this._entity.AddChild(this._arrow, false);
			this._entity.AddChild(this._width, false);
			this._entity.AddChild(this._attack, false);
			this._entity.AddChild(this._flagUnavailable, false);
			this._flag.SetVisibilityExcludeParents(false);
			this._gear.SetVisibilityExcludeParents(false);
			this._arrow.SetVisibilityExcludeParents(false);
			this._width.SetVisibilityExcludeParents(false);
			this._attack.SetVisibilityExcludeParents(false);
			this._flagUnavailable.SetVisibilityExcludeParents(false);
			this.Current = this._flag;
			BoundingBox boundingBox = this._arrow.GetMetaMesh(0).GetBoundingBox();
			this._arrowLength = boundingBox.max.y - boundingBox.min.y;
			bool flag;
			this.UpdateFrame(out flag, false);
			this._orderablesWithInteractionArea = this._mission.MissionObjects.OfType<IOrderableWithInteractionArea>();
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x000243D0 File Offset: 0x000225D0
		public void Tick(float dt)
		{
			this.FocusedOrderableObject = null;
			GameEntity collidedEntity = this.GetCollidedEntity();
			if (collidedEntity != null)
			{
				BattleSideEnum side = Mission.Current.PlayerTeam.Side;
				IOrderable orderable = (IOrderable)collidedEntity.GetScriptComponents().First(delegate(ScriptComponentBehavior sc)
				{
					IOrderable orderable2;
					return (orderable2 = sc as IOrderable) != null && orderable2.GetOrder(side) > 0;
				});
				if (orderable.GetOrder(side) != null)
				{
					this.FocusedOrderableObject = orderable;
				}
			}
			bool flag;
			this.UpdateFrame(out flag, collidedEntity != null);
			this.LatestUpdateFrameNo = Utilities.EngineFrameNo;
			if (!this.IsVisible)
			{
				return;
			}
			if (this.FocusedOrderableObject == null)
			{
				this.FocusedOrderableObject = this._orderablesWithInteractionArea.FirstOrDefault((IOrderableWithInteractionArea o) => ((ScriptComponentBehavior)o).GameEntity.IsVisibleIncludeParents() && o.IsPointInsideInteractionArea(this.Position));
				ScriptComponentBehavior scriptComponentBehavior;
				if ((scriptComponentBehavior = this.FocusedOrderableObject as ScriptComponentBehavior) != null && scriptComponentBehavior.GameEntity.Scene == null)
				{
					this.FocusedOrderableObject = null;
				}
			}
			this.UpdateCurrentMesh(flag);
			if (this.Current == this._flag || this.Current == this._flagUnavailable)
			{
				MatrixFrame frame = this._flag.GetFrame();
				float num = MathF.Sin(MBCommon.GetApplicationTime() * 2f) + 1f;
				num *= 0.25f;
				frame.origin.z = num;
				this._flag.SetFrame(ref frame);
				this._flagUnavailable.SetFrame(ref frame);
			}
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x00024538 File Offset: 0x00022738
		private void UpdateCurrentMesh(bool isOnValidGround)
		{
			if (this.FocusedOrderableObject != null && isOnValidGround)
			{
				BattleSideEnum side = Mission.Current.PlayerTeam.Side;
				if (this.FocusedOrderableObject.GetOrder(side) == 43)
				{
					this.Current = this._attack;
					return;
				}
				OrderType order = this.FocusedOrderableObject.GetOrder(side);
				if (order == 42 || order == 8)
				{
					this.Current = this._gear;
					return;
				}
			}
			if (this._isArrowVisible)
			{
				this.Current = this._arrow;
				return;
			}
			if (this._isWidthVisible)
			{
				this.Current = this._width;
				return;
			}
			this.Current = (isOnValidGround ? this._flag : this._flagUnavailable);
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x000245E3 File Offset: 0x000227E3
		public void SetArrowVisibility(bool isVisible, Vec2 arrowDirection)
		{
			this._isArrowVisible = isVisible;
			this._arrowDirection = arrowDirection;
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x000245F4 File Offset: 0x000227F4
		private void UpdateFrame(out bool isOnValidGround, bool checkForTargetEntity)
		{
			Vec3 vec;
			Vec3 vec2;
			if (this._missionScreen.GetProjectedMousePositionOnGround(out vec, out vec2, 67108864, true))
			{
				WorldPosition worldPosition;
				worldPosition..ctor(Mission.Current.Scene, UIntPtr.Zero, vec, false);
				isOnValidGround = ((!this.IsVisible || checkForTargetEntity) ? Mission.Current.IsOrderPositionAvailable(ref worldPosition, Mission.Current.PlayerTeam) : OrderFlag.IsPositionOnValidGround(worldPosition));
			}
			else
			{
				isOnValidGround = false;
				vec..ctor(0f, 0f, -100000f, -1f);
			}
			this.Position = vec;
			Vec3 vec3;
			this._missionScreen.ScreenPointToWorldRay(Vec2.One * 0.5f, out vec3, out vec2);
			float num;
			if (this._missionScreen.LastFollowedAgent != null)
			{
				vec2 = vec3 - this.Position;
				num = vec2.AsVec2.RotationInRadians;
			}
			else
			{
				num = this._missionScreen.CombatCamera.Frame.rotation.f.RotationZ;
			}
			float num2 = num;
			MatrixFrame frame = this._entity.GetFrame();
			frame.rotation = Mat3.Identity;
			frame.rotation.RotateAboutUp(num2);
			this._entity.SetFrame(ref frame);
			if (this._isArrowVisible)
			{
				num2 = this._arrowDirection.RotationInRadians;
				Mat3 identity = Mat3.Identity;
				identity.RotateAboutUp(num2);
				MatrixFrame identity2 = MatrixFrame.Identity;
				identity2.rotation = frame.rotation.TransformToLocal(identity);
				identity2.Advance(-this._arrowLength);
				this._arrow.SetFrame(ref identity2);
			}
			if (this._isWidthVisible)
			{
				this._widthLeft.SetLocalPosition(Vec3.Side * (this._customWidth * 0.5f - 0f));
				this._widthRight.SetLocalPosition(Vec3.Side * (this._customWidth * -0.5f + 0f));
				this._widthLeft.SetLocalPosition(Vec3.Side * (this._customWidth * 0.5f - this._arrowLength));
				this._widthRight.SetLocalPosition(Vec3.Side * (this._customWidth * -0.5f + this._arrowLength));
			}
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x0002482A File Offset: 0x00022A2A
		public static bool IsPositionOnValidGround(WorldPosition worldPosition)
		{
			return Mission.Current.IsFormationUnitPositionAvailable(ref worldPosition, Mission.Current.PlayerTeam);
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x00024842 File Offset: 0x00022A42
		public static bool IsOrderPositionValid(WorldPosition orderPosition)
		{
			return Mission.Current.IsOrderPositionAvailable(ref orderPosition, Mission.Current.PlayerTeam);
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060004BB RID: 1211 RVA: 0x0002485A File Offset: 0x00022A5A
		// (set) Token: 0x060004BC RID: 1212 RVA: 0x00024868 File Offset: 0x00022A68
		public Vec3 Position
		{
			get
			{
				return this._entity.GlobalPosition;
			}
			private set
			{
				MatrixFrame frame = this._entity.GetFrame();
				frame.origin = value;
				this._entity.SetFrame(ref frame);
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060004BD RID: 1213 RVA: 0x00024896 File Offset: 0x00022A96
		public MatrixFrame Frame
		{
			get
			{
				return this._entity.GetGlobalFrame();
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060004BE RID: 1214 RVA: 0x000248A3 File Offset: 0x00022AA3
		// (set) Token: 0x060004BF RID: 1215 RVA: 0x000248B0 File Offset: 0x00022AB0
		public bool IsVisible
		{
			get
			{
				return this._entity.IsVisibleIncludeParents();
			}
			set
			{
				this._entity.SetVisibilityExcludeParents(value);
				if (!value)
				{
					this.FocusedOrderableObject = null;
				}
			}
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x000248C8 File Offset: 0x00022AC8
		private GameEntity GetCollidedEntity()
		{
			Vec2 vec = ((Mission.Current.GetMissionBehavior<BattleDeploymentHandler>() != null) ? Input.MousePositionRanged : new Vec2(0.5f, 0.5f));
			Vec3 vec2;
			Vec3 vec3;
			this._missionScreen.ScreenPointToWorldRay(vec, out vec2, out vec3);
			float num;
			GameEntity parent;
			this._mission.Scene.RayCastForClosestEntityOrTerrain(vec2, vec3, ref num, ref parent, 0.3f, 67188481);
			while (parent != null)
			{
				if (parent.GetScriptComponents().Any(delegate(ScriptComponentBehavior sc)
				{
					IOrderable orderable;
					return (orderable = sc as IOrderable) != null && orderable.GetOrder(Mission.Current.PlayerTeam.Side) > 0;
				}))
				{
					break;
				}
				parent = parent.Parent;
			}
			return parent;
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x00024968 File Offset: 0x00022B68
		public void SetWidthVisibility(bool isVisible, float width)
		{
			this._isWidthVisible = isVisible;
			this._customWidth = width;
		}

		// Token: 0x040002CE RID: 718
		private readonly GameEntity _entity;

		// Token: 0x040002CF RID: 719
		private readonly GameEntity _flag;

		// Token: 0x040002D0 RID: 720
		private readonly GameEntity _gear;

		// Token: 0x040002D1 RID: 721
		private readonly GameEntity _arrow;

		// Token: 0x040002D2 RID: 722
		private readonly GameEntity _width;

		// Token: 0x040002D3 RID: 723
		private readonly GameEntity _attack;

		// Token: 0x040002D4 RID: 724
		private readonly GameEntity _flagUnavailable;

		// Token: 0x040002D5 RID: 725
		private readonly GameEntity _widthLeft;

		// Token: 0x040002D6 RID: 726
		private readonly GameEntity _widthRight;

		// Token: 0x040002D7 RID: 727
		public bool IsTroop = true;

		// Token: 0x040002D8 RID: 728
		private bool _isWidthVisible;

		// Token: 0x040002D9 RID: 729
		private float _customWidth;

		// Token: 0x040002DA RID: 730
		private GameEntity _current;

		// Token: 0x040002DC RID: 732
		private readonly IEnumerable<IOrderableWithInteractionArea> _orderablesWithInteractionArea;

		// Token: 0x040002DD RID: 733
		private readonly Mission _mission;

		// Token: 0x040002DE RID: 734
		private readonly MissionScreen _missionScreen;

		// Token: 0x040002DF RID: 735
		private readonly float _arrowLength;

		// Token: 0x040002E0 RID: 736
		private bool _isArrowVisible;

		// Token: 0x040002E1 RID: 737
		private Vec2 _arrowDirection;
	}
}
