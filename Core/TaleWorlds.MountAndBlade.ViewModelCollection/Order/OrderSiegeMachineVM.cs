using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	public class OrderSiegeMachineVM : OrderSubjectVM
	{
		public DeploymentPoint DeploymentPoint { get; private set; }

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

		public bool IsPrimarySiegeMachine
		{
			get
			{
				return this.SiegeWeapon is IPrimarySiegeWeapon;
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
				this._machineClass = value;
				base.OnPropertyChangedWithValue<string>(value, "MachineClass");
			}
		}

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

		private void ExecuteAction()
		{
			if (this.SiegeWeapon != null)
			{
				this.SetSelected(this);
			}
		}

		public OrderSiegeMachineVM(DeploymentPoint deploymentPoint, Action<OrderSiegeMachineVM> setSelected, int keyIndex)
		{
			this.DeploymentPoint = deploymentPoint;
			this.SetSelected = setSelected;
			base.ShortcutText = keyIndex.ToString();
		}

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

		public Type MachineType;

		public Action<OrderSiegeMachineVM> SetSelected;

		private string _machineClass = "";

		private double _currentHP;

		private bool _isInside;

		private Vec2 _position;
	}
}
