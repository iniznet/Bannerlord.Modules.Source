using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.MapSiege
{
	// Token: 0x02000031 RID: 49
	public class MapSiegeProductionMachineVM : ViewModel
	{
		// Token: 0x17000131 RID: 305
		// (get) Token: 0x060003C5 RID: 965 RVA: 0x00011964 File Offset: 0x0000FB64
		public SiegeEngineType Engine { get; }

		// Token: 0x060003C6 RID: 966 RVA: 0x0001196C File Offset: 0x0000FB6C
		public MapSiegeProductionMachineVM(SiegeEngineType engineType, int number, Action<MapSiegeProductionMachineVM> onSelection)
		{
			this._onSelection = onSelection;
			this.Engine = engineType;
			this.NumberOfMachines = number;
			this.MachineID = engineType.StringId;
			this.IsReserveOption = false;
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0001199C File Offset: 0x0000FB9C
		public MapSiegeProductionMachineVM(Action<MapSiegeProductionMachineVM> onSelection, bool isCancel)
		{
			this._onSelection = onSelection;
			this.Engine = null;
			this.NumberOfMachines = 0;
			this.MachineID = "reserve";
			this.IsReserveOption = true;
			this._isCancel = isCancel;
			this.RefreshValues();
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x000119D8 File Offset: 0x0000FBD8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ActionText = (this._isCancel ? GameTexts.FindText("str_cancel", null).ToString() : GameTexts.FindText("str_siege_move_to_reserve", null).ToString());
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x00011A10 File Offset: 0x0000FC10
		public void OnSelection()
		{
			this._onSelection(this);
		}

		// Token: 0x060003CA RID: 970 RVA: 0x00011A1E File Offset: 0x0000FC1E
		public void ExecuteShowTooltip()
		{
			if (this.Engine != null)
			{
				InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { SandBoxUIHelper.GetSiegeEngineTooltip(this.Engine) });
			}
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00011A4B File Offset: 0x0000FC4B
		public void ExecuteHideTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x060003CC RID: 972 RVA: 0x00011A52 File Offset: 0x0000FC52
		// (set) Token: 0x060003CD RID: 973 RVA: 0x00011A5A File Offset: 0x0000FC5A
		[DataSourceProperty]
		public int MachineType
		{
			get
			{
				return this._machineType;
			}
			set
			{
				if (value != this._machineType)
				{
					this._machineType = value;
					base.OnPropertyChangedWithValue(value, "MachineType");
				}
			}
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060003CE RID: 974 RVA: 0x00011A78 File Offset: 0x0000FC78
		// (set) Token: 0x060003CF RID: 975 RVA: 0x00011A80 File Offset: 0x0000FC80
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

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060003D0 RID: 976 RVA: 0x00011AA3 File Offset: 0x0000FCA3
		// (set) Token: 0x060003D1 RID: 977 RVA: 0x00011AAB File Offset: 0x0000FCAB
		[DataSourceProperty]
		public int NumberOfMachines
		{
			get
			{
				return this._numberOfMachines;
			}
			set
			{
				if (value != this._numberOfMachines)
				{
					this._numberOfMachines = value;
					base.OnPropertyChangedWithValue(value, "NumberOfMachines");
				}
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060003D2 RID: 978 RVA: 0x00011AC9 File Offset: 0x0000FCC9
		// (set) Token: 0x060003D3 RID: 979 RVA: 0x00011AD1 File Offset: 0x0000FCD1
		[DataSourceProperty]
		public string ActionText
		{
			get
			{
				return this._actionText;
			}
			set
			{
				if (value != this._actionText)
				{
					this._actionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionText");
				}
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060003D4 RID: 980 RVA: 0x00011AF4 File Offset: 0x0000FCF4
		// (set) Token: 0x060003D5 RID: 981 RVA: 0x00011AFC File Offset: 0x0000FCFC
		[DataSourceProperty]
		public bool IsReserveOption
		{
			get
			{
				return this._isReserveOption;
			}
			set
			{
				if (value != this._isReserveOption)
				{
					this._isReserveOption = value;
					base.OnPropertyChangedWithValue(value, "IsReserveOption");
				}
			}
		}

		// Token: 0x040001FA RID: 506
		private Action<MapSiegeProductionMachineVM> _onSelection;

		// Token: 0x040001FC RID: 508
		private bool _isCancel;

		// Token: 0x040001FD RID: 509
		private int _machineType;

		// Token: 0x040001FE RID: 510
		private int _numberOfMachines;

		// Token: 0x040001FF RID: 511
		private string _machineID;

		// Token: 0x04000200 RID: 512
		private bool _isReserveOption;

		// Token: 0x04000201 RID: 513
		private string _actionText;
	}
}
