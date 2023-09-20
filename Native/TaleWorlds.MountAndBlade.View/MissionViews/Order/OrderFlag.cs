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
	public class OrderFlag
	{
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

		public IOrderable FocusedOrderableObject { get; private set; }

		public int LatestUpdateFrameNo { get; private set; }

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

		public void SetArrowVisibility(bool isVisible, Vec2 arrowDirection)
		{
			this._isArrowVisible = isVisible;
			this._arrowDirection = arrowDirection;
		}

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

		public static bool IsPositionOnValidGround(WorldPosition worldPosition)
		{
			return Mission.Current.IsFormationUnitPositionAvailable(ref worldPosition, Mission.Current.PlayerTeam);
		}

		public static bool IsOrderPositionValid(WorldPosition orderPosition)
		{
			return Mission.Current.IsOrderPositionAvailable(ref orderPosition, Mission.Current.PlayerTeam);
		}

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

		public MatrixFrame Frame
		{
			get
			{
				return this._entity.GetGlobalFrame();
			}
		}

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

		public void SetWidthVisibility(bool isVisible, float width)
		{
			this._isWidthVisible = isVisible;
			this._customWidth = width;
		}

		private readonly GameEntity _entity;

		private readonly GameEntity _flag;

		private readonly GameEntity _gear;

		private readonly GameEntity _arrow;

		private readonly GameEntity _width;

		private readonly GameEntity _attack;

		private readonly GameEntity _flagUnavailable;

		private readonly GameEntity _widthLeft;

		private readonly GameEntity _widthRight;

		public bool IsTroop = true;

		private bool _isWidthVisible;

		private float _customWidth;

		private GameEntity _current;

		private readonly IEnumerable<IOrderableWithInteractionArea> _orderablesWithInteractionArea;

		private readonly Mission _mission;

		private readonly MissionScreen _missionScreen;

		private readonly float _arrowLength;

		private bool _isArrowVisible;

		private Vec2 _arrowDirection;
	}
}
