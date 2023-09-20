using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame.HostGameOptions
{
	// Token: 0x0200007B RID: 123
	public class NumericHostGameOptionDataVM : GenericHostGameOptionDataVM
	{
		// Token: 0x06000AE3 RID: 2787 RVA: 0x00026E2A File Offset: 0x0002502A
		public NumericHostGameOptionDataVM(MultiplayerOptions.OptionType optionType, int preferredIndex)
			: base(OptionsVM.OptionsDataType.NumericOption, optionType, preferredIndex)
		{
			this.RefreshData();
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x00026E3C File Offset: 0x0002503C
		public override void RefreshData()
		{
			MultiplayerOptionsProperty optionProperty = base.OptionType.GetOptionProperty();
			this.Min = optionProperty.BoundsMin;
			this.Max = optionProperty.BoundsMax;
			this.Value = base.OptionType.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
		}

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06000AE5 RID: 2789 RVA: 0x00026E7F File Offset: 0x0002507F
		// (set) Token: 0x06000AE6 RID: 2790 RVA: 0x00026E87 File Offset: 0x00025087
		[DataSourceProperty]
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (value != this._value)
				{
					this._value = value;
					base.OnPropertyChangedWithValue(value, "Value");
					base.OnPropertyChanged("ValueAsString");
					base.OptionType.SetValue(value, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
			}
		}

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06000AE7 RID: 2791 RVA: 0x00026EBD File Offset: 0x000250BD
		[DataSourceProperty]
		public string ValueAsString
		{
			get
			{
				return this._value.ToString();
			}
		}

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06000AE8 RID: 2792 RVA: 0x00026ECA File Offset: 0x000250CA
		// (set) Token: 0x06000AE9 RID: 2793 RVA: 0x00026ED2 File Offset: 0x000250D2
		[DataSourceProperty]
		public int Min
		{
			get
			{
				return this._min;
			}
			set
			{
				if (value != this._min)
				{
					this._min = value;
					base.OnPropertyChangedWithValue(value, "Min");
				}
			}
		}

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06000AEA RID: 2794 RVA: 0x00026EF0 File Offset: 0x000250F0
		// (set) Token: 0x06000AEB RID: 2795 RVA: 0x00026EF8 File Offset: 0x000250F8
		[DataSourceProperty]
		public int Max
		{
			get
			{
				return this._max;
			}
			set
			{
				if (value != this._max)
				{
					this._max = value;
					base.OnPropertyChangedWithValue(value, "Max");
				}
			}
		}

		// Token: 0x04000545 RID: 1349
		private int _value;

		// Token: 0x04000546 RID: 1350
		private int _min;

		// Token: 0x04000547 RID: 1351
		private int _max;
	}
}
