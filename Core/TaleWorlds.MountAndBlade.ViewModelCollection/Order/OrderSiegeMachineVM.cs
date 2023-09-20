using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	// Token: 0x02000025 RID: 37
	public class OrderSiegeMachineVM : OrderSubjectVM
	{
		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060002BE RID: 702 RVA: 0x0000D0E3 File Offset: 0x0000B2E3
		// (set) Token: 0x060002BF RID: 703 RVA: 0x0000D0EB File Offset: 0x0000B2EB
		public DeploymentPoint DeploymentPoint { get; private set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x0000D0F4 File Offset: 0x0000B2F4
		public SiegeWeapon SiegeWeapon
		{
			get
			{
				if (this.DeploymentPoint != null)
				{
					return this.DeploymentPoint.DeployedWeapon as SiegeWeapon;
				}
				return null;
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x0000D110 File Offset: 0x0000B310
		public bool IsPrimarySiegeMachine
		{
			get
			{
				return this.SiegeWeapon is IPrimarySiegeWeapon;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x0000D120 File Offset: 0x0000B320
		// (set) Token: 0x060002C3 RID: 707 RVA: 0x0000D128 File Offset: 0x0000B328
		[DataSourceProperty]
		public string MachineClass
		{
			get
			{
				return this._machineClass;
			}
			set
			{
				this._machineClass = value;
				base.OnPropertyChangedWithValue<string>(value, "MachineClass");
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x0000D13D File Offset: 0x0000B33D
		// (set) Token: 0x060002C5 RID: 709 RVA: 0x0000D145 File Offset: 0x0000B345
		[DataSourceProperty]
		public double CurrentHP
		{
			get
			{
				return this._currentHP;
			}
			set
			{
				if (value != this._currentHP)
				{
					this._currentHP = value;
					base.OnPropertyChangedWithValue(value, "CurrentHP");
				}
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x0000D163 File Offset: 0x0000B363
		// (set) Token: 0x060002C7 RID: 711 RVA: 0x0000D16B File Offset: 0x0000B36B
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

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x0000D189 File Offset: 0x0000B389
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x0000D191 File Offset: 0x0000B391
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

		// Token: 0x060002CA RID: 714 RVA: 0x0000D1B4 File Offset: 0x0000B3B4
		private void ExecuteAction()
		{
			if (this.SiegeWeapon != null)
			{
				this.SetSelected(this);
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000D1CA File Offset: 0x0000B3CA
		public OrderSiegeMachineVM(DeploymentPoint deploymentPoint, Action<OrderSiegeMachineVM> setSelected, int keyIndex)
		{
			this.DeploymentPoint = deploymentPoint;
			this.SetSelected = setSelected;
			base.ShortcutText = keyIndex.ToString();
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000D1F8 File Offset: 0x0000B3F8
		public void RefreshSiegeWeapon()
		{
			if (this.SiegeWeapon == null)
			{
				this.MachineType = null;
				this.MachineClass = "none";
				this.CurrentHP = 1.0;
				base.IsSelectable = false;
				base.IsSelected = false;
				return;
			}
			base.IsSelectable = !this.SiegeWeapon.IsDestroyed && !this.SiegeWeapon.IsDeactivated;
			this.MachineType = this.SiegeWeapon.GetType();
			this.MachineClass = this.SiegeWeapon.GetSiegeEngineType().StringId;
			if (this.SiegeWeapon.DestructionComponent != null)
			{
				this.CurrentHP = (double)(this.SiegeWeapon.DestructionComponent.HitPoint / this.SiegeWeapon.DestructionComponent.MaxHitPoint);
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0000D2C0 File Offset: 0x0000B4C0
		public static SiegeEngineType GetSiegeType(Type t, BattleSideEnum side)
		{
			if (t == typeof(SiegeLadder))
			{
				return DefaultSiegeEngineTypes.Ladder;
			}
			if (t == typeof(Ballista))
			{
				return DefaultSiegeEngineTypes.Ballista;
			}
			if (t == typeof(FireBallista))
			{
				return DefaultSiegeEngineTypes.FireBallista;
			}
			if (t == typeof(BatteringRam))
			{
				return DefaultSiegeEngineTypes.Ram;
			}
			if (t == typeof(SiegeTower))
			{
				return DefaultSiegeEngineTypes.SiegeTower;
			}
			if (t == typeof(Mangonel))
			{
				if (side != BattleSideEnum.Attacker)
				{
					return DefaultSiegeEngineTypes.Catapult;
				}
				return DefaultSiegeEngineTypes.Onager;
			}
			else if (t == typeof(FireMangonel))
			{
				if (side != BattleSideEnum.Attacker)
				{
					return DefaultSiegeEngineTypes.FireCatapult;
				}
				return DefaultSiegeEngineTypes.FireOnager;
			}
			else
			{
				if (t == typeof(Trebuchet))
				{
					return DefaultSiegeEngineTypes.Trebuchet;
				}
				if (t == typeof(FireTrebuchet))
				{
					return DefaultSiegeEngineTypes.FireTrebuchet;
				}
				Debug.FailedAssert("Invalid siege weapon", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Order\\OrderSiegeMachineVM.cs", "GetSiegeType", 163);
				return DefaultSiegeEngineTypes.Ladder;
			}
		}

		// Token: 0x04000158 RID: 344
		public Type MachineType;

		// Token: 0x04000159 RID: 345
		public Action<OrderSiegeMachineVM> SetSelected;

		// Token: 0x0400015A RID: 346
		private string _machineClass = "";

		// Token: 0x0400015B RID: 347
		private double _currentHP;

		// Token: 0x0400015C RID: 348
		private bool _isInside;

		// Token: 0x0400015D RID: 349
		private Vec2 _position;
	}
}
