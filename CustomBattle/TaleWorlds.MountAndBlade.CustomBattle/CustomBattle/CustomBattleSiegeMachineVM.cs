using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
	public class CustomBattleSiegeMachineVM : ViewModel
	{
		public SiegeEngineType SiegeEngineType { get; private set; }

		public CustomBattleSiegeMachineVM(SiegeEngineType machineType, Action<CustomBattleSiegeMachineVM> onSelection, Action<CustomBattleSiegeMachineVM> onResetSelection)
		{
			this._onSelection = onSelection;
			this._onResetSelection = onResetSelection;
			this.SetMachineType(machineType);
		}

		public void SetMachineType(SiegeEngineType machine)
		{
			this.SiegeEngineType = machine;
			this.Name = ((machine != null) ? machine.StringId : "");
			this.IsRanged = machine != null && machine.IsRanged;
			this.MachineID = ((machine != null) ? machine.StringId : "");
		}

		private void OnSelection()
		{
			this._onSelection(this);
		}

		private void OnResetSelection()
		{
			this._onResetSelection(this);
		}

		[DataSourceProperty]
		public bool IsRanged
		{
			get
			{
				return this._isRanged;
			}
			set
			{
				if (value != this._isRanged)
				{
					this._isRanged = value;
					base.OnPropertyChangedWithValue(value, "IsRanged");
				}
			}
		}

		[DataSourceProperty]
		public string MachineID
		{
			get
			{
				return this._machineID;
			}
			set
			{
				if (value != this._machineID)
				{
					this._machineID = value;
					base.OnPropertyChangedWithValue<string>(value, "MachineID");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		private Action<CustomBattleSiegeMachineVM> _onSelection;

		private Action<CustomBattleSiegeMachineVM> _onResetSelection;

		private string _name;

		private bool _isRanged;

		private string _machineID;
	}
}
