using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	public class DeploymentSiegeMachineVM : ViewModel
	{
		public DeploymentPoint DeploymentPoint { get; }

		public DeploymentSiegeMachineVM(DeploymentPoint selectedDeploymentPoint, SiegeWeapon siegeMachine, Camera deploymentCamera, Action<DeploymentSiegeMachineVM> onSelectSiegeMachine, Action<DeploymentPoint> onHoverSiegeMachine, bool isSelected)
		{
			this._deploymentCamera = deploymentCamera;
			this.DeploymentPoint = selectedDeploymentPoint;
			this._onSelect = onSelectSiegeMachine;
			this._onHover = onHoverSiegeMachine;
			this.SiegeWeapon = siegeMachine;
			this.IsSelected = isSelected;
			if (siegeMachine != null)
			{
				this.MachineType = siegeMachine.GetType();
				this.Machine = OrderSiegeMachineVM.GetSiegeType(this.MachineType, siegeMachine.Side);
				this.MachineClass = siegeMachine.GetSiegeEngineType().StringId;
			}
			else
			{
				this.MachineType = null;
				this.MachineClass = "Empty";
			}
			this.Type = (int)selectedDeploymentPoint.GetDeploymentPointType();
			this._worldPos = selectedDeploymentPoint.GameEntity.GlobalPosition;
			this.IsPlayerGeneral = Mission.Current.PlayerTeam.IsPlayerGeneral;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.BreachedText = new TextObject("{=D0TbQm4r}BREACHED", null).ToString();
		}

		public void Update()
		{
			this.CalculatePosition();
			this.RefreshPosition();
		}

		public void CalculatePosition()
		{
			this._latestX = 0f;
			this._latestY = 0f;
			MatrixFrame identity = MatrixFrame.Identity;
			this._deploymentCamera.GetViewProjMatrix(ref identity);
			Vec3 worldPos = this._worldPos;
			worldPos.z += 8f;
			worldPos.w = 1f;
			Vec3 vec = worldPos * identity;
			this.IsInFront = vec.w > 0f;
			vec.x /= vec.w;
			vec.y /= vec.w;
			vec.z /= vec.w;
			vec.w /= vec.w;
			vec *= 0.5f;
			vec.x += 0.5f;
			vec.y += 0.5f;
			vec.y = 1f - vec.y;
			int num = (int)Screen.RealScreenResolutionWidth;
			int num2 = (int)Screen.RealScreenResolutionHeight;
			this._latestX = vec.x * (float)num;
			this._latestY = vec.y * (float)num2;
		}

		public void RefreshPosition()
		{
			this.IsInside = this.IsInsideWindow();
			this.Position = new Vec2(this._latestX, this._latestY);
		}

		private bool IsInsideWindow()
		{
			return this._latestX <= Screen.RealScreenResolutionWidth && this._latestY <= Screen.RealScreenResolutionHeight && this._latestX + 200f >= 0f && this._latestY + 100f >= 0f;
		}

		public void ExecuteAction()
		{
			Action<DeploymentSiegeMachineVM> onSelect = this._onSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(this);
		}

		public void ExecuteFocusBegin()
		{
			Action<DeploymentPoint> onHover = this._onHover;
			if (onHover == null)
			{
				return;
			}
			onHover(this.DeploymentPoint);
		}

		public void ExecuteFocusEnd()
		{
			Action<DeploymentPoint> onHover = this._onHover;
			if (onHover == null)
			{
				return;
			}
			onHover(null);
		}

		public void RefreshWithDeployedWeapon()
		{
			SiegeWeapon siegeWeapon = this.DeploymentPoint.DeployedWeapon as SiegeWeapon;
			this.SiegeWeapon = siegeWeapon;
			if (siegeWeapon != null)
			{
				this.MachineType = siegeWeapon.GetType();
				this.MachineClass = siegeWeapon.GetSiegeEngineType().StringId;
				return;
			}
			this.MachineType = null;
			this.MachineClass = "none";
		}

		[DataSourceProperty]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					base.OnPropertyChangedWithValue(value, "Type");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPlayerGeneral
		{
			get
			{
				return this._isPlayerGeneral;
			}
			set
			{
				if (value != this._isPlayerGeneral)
				{
					this._isPlayerGeneral = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerGeneral");
				}
			}
		}

		[DataSourceProperty]
		public string MachineClass
		{
			get
			{
				return this._machineClass;
			}
			set
			{
				if (value != this._machineClass)
				{
					this._machineClass = value;
					base.OnPropertyChangedWithValue<string>(value, "MachineClass");
				}
			}
		}

		[DataSourceProperty]
		public string BreachedText
		{
			get
			{
				return this._breachedText;
			}
			set
			{
				if (value != this._breachedText)
				{
					this._breachedText = value;
					base.OnPropertyChangedWithValue<string>(value, "BreachedText");
				}
			}
		}

		[DataSourceProperty]
		public int RemainingCount
		{
			get
			{
				return this._remainingCount;
			}
			set
			{
				if (value != this._remainingCount)
				{
					this._remainingCount = value;
					base.OnPropertyChangedWithValue(value, "RemainingCount");
				}
			}
		}

		public bool IsInside
		{
			get
			{
				return this._isInside;
			}
			set
			{
				if (value != this._isInside)
				{
					this._isInside = value;
					base.OnPropertyChangedWithValue(value, "IsInside");
				}
			}
		}

		public bool IsInFront
		{
			get
			{
				return this._isInFront;
			}
			set
			{
				if (value != this._isInFront)
				{
					this._isInFront = value;
					base.OnPropertyChangedWithValue(value, "IsInFront");
				}
			}
		}

		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (this._position != value)
				{
					this._position = value;
					base.OnPropertyChangedWithValue(value, "Position");
				}
			}
		}

		public Type MachineType;

		public SiegeEngineType Machine;

		public SiegeWeapon SiegeWeapon;

		private readonly Camera _deploymentCamera;

		private Vec3 _worldPos;

		private float _latestX;

		private float _latestY;

		private readonly Action<DeploymentSiegeMachineVM> _onSelect;

		private readonly Action<DeploymentPoint> _onHover;

		private string _machineClass = "";

		private int _remainingCount = -1;

		private bool _isSelected;

		private bool _isPlayerGeneral;

		private int _type;

		private bool _isInside;

		private bool _isInFront;

		private string _breachedText;

		private Vec2 _position;
	}
}
