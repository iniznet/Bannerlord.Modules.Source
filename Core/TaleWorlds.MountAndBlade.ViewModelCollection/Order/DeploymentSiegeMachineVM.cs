using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	// Token: 0x02000016 RID: 22
	public class DeploymentSiegeMachineVM : ViewModel
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060001AA RID: 426 RVA: 0x00006CEF File Offset: 0x00004EEF
		public DeploymentPoint DeploymentPoint { get; }

		// Token: 0x060001AB RID: 427 RVA: 0x00006CF8 File Offset: 0x00004EF8
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

		// Token: 0x060001AC RID: 428 RVA: 0x00006DCD File Offset: 0x00004FCD
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.BreachedText = new TextObject("{=D0TbQm4r}BREACHED", null).ToString();
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00006DEB File Offset: 0x00004FEB
		public void Update()
		{
			this.CalculatePosition();
			this.RefreshPosition();
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00006DFC File Offset: 0x00004FFC
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

		// Token: 0x060001AF RID: 431 RVA: 0x00006F21 File Offset: 0x00005121
		public void RefreshPosition()
		{
			this.IsInside = this.IsInsideWindow();
			this.Position = new Vec2(this._latestX, this._latestY);
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00006F48 File Offset: 0x00005148
		private bool IsInsideWindow()
		{
			return this._latestX <= Screen.RealScreenResolutionWidth && this._latestY <= Screen.RealScreenResolutionHeight && this._latestX + 200f >= 0f && this._latestY + 100f >= 0f;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00006F9A File Offset: 0x0000519A
		public void ExecuteAction()
		{
			Action<DeploymentSiegeMachineVM> onSelect = this._onSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(this);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00006FAD File Offset: 0x000051AD
		public void ExecuteFocusBegin()
		{
			Action<DeploymentPoint> onHover = this._onHover;
			if (onHover == null)
			{
				return;
			}
			onHover(this.DeploymentPoint);
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00006FC5 File Offset: 0x000051C5
		public void ExecuteFocusEnd()
		{
			Action<DeploymentPoint> onHover = this._onHover;
			if (onHover == null)
			{
				return;
			}
			onHover(null);
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00006FD8 File Offset: 0x000051D8
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

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x00007030 File Offset: 0x00005230
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x00007038 File Offset: 0x00005238
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

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x00007056 File Offset: 0x00005256
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x0000705E File Offset: 0x0000525E
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

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x0000707C File Offset: 0x0000527C
		// (set) Token: 0x060001BA RID: 442 RVA: 0x00007084 File Offset: 0x00005284
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

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060001BB RID: 443 RVA: 0x000070A2 File Offset: 0x000052A2
		// (set) Token: 0x060001BC RID: 444 RVA: 0x000070AA File Offset: 0x000052AA
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

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001BD RID: 445 RVA: 0x000070CD File Offset: 0x000052CD
		// (set) Token: 0x060001BE RID: 446 RVA: 0x000070D5 File Offset: 0x000052D5
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

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001BF RID: 447 RVA: 0x000070F8 File Offset: 0x000052F8
		// (set) Token: 0x060001C0 RID: 448 RVA: 0x00007100 File Offset: 0x00005300
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

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001C1 RID: 449 RVA: 0x0000711E File Offset: 0x0000531E
		// (set) Token: 0x060001C2 RID: 450 RVA: 0x00007126 File Offset: 0x00005326
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

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x00007144 File Offset: 0x00005344
		// (set) Token: 0x060001C4 RID: 452 RVA: 0x0000714C File Offset: 0x0000534C
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

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001C5 RID: 453 RVA: 0x0000716A File Offset: 0x0000536A
		// (set) Token: 0x060001C6 RID: 454 RVA: 0x00007172 File Offset: 0x00005372
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

		// Token: 0x040000C3 RID: 195
		public Type MachineType;

		// Token: 0x040000C4 RID: 196
		public SiegeEngineType Machine;

		// Token: 0x040000C5 RID: 197
		public SiegeWeapon SiegeWeapon;

		// Token: 0x040000C6 RID: 198
		private readonly Camera _deploymentCamera;

		// Token: 0x040000C7 RID: 199
		private Vec3 _worldPos;

		// Token: 0x040000C8 RID: 200
		private float _latestX;

		// Token: 0x040000C9 RID: 201
		private float _latestY;

		// Token: 0x040000CA RID: 202
		private readonly Action<DeploymentSiegeMachineVM> _onSelect;

		// Token: 0x040000CB RID: 203
		private readonly Action<DeploymentPoint> _onHover;

		// Token: 0x040000CC RID: 204
		private string _machineClass = "";

		// Token: 0x040000CD RID: 205
		private int _remainingCount = -1;

		// Token: 0x040000CE RID: 206
		private bool _isSelected;

		// Token: 0x040000CF RID: 207
		private bool _isPlayerGeneral;

		// Token: 0x040000D0 RID: 208
		private int _type;

		// Token: 0x040000D1 RID: 209
		private bool _isInside;

		// Token: 0x040000D2 RID: 210
		private bool _isInFront;

		// Token: 0x040000D3 RID: 211
		private string _breachedText;

		// Token: 0x040000D4 RID: 212
		private Vec2 _position;
	}
}
