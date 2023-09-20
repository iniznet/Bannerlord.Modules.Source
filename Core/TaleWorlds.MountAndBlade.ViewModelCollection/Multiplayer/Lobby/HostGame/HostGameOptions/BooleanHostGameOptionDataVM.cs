using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame.HostGameOptions
{
	// Token: 0x02000077 RID: 119
	public class BooleanHostGameOptionDataVM : GenericHostGameOptionDataVM
	{
		// Token: 0x06000AC8 RID: 2760 RVA: 0x00026990 File Offset: 0x00024B90
		public BooleanHostGameOptionDataVM(MultiplayerOptions.OptionType optionType, int preferredIndex)
			: base(OptionsVM.OptionsDataType.BooleanOption, optionType, preferredIndex)
		{
			this.RefreshData();
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x000269A1 File Offset: 0x00024BA1
		public override void RefreshData()
		{
			this.IsSelected = base.OptionType.GetBoolValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06000ACA RID: 2762 RVA: 0x000269B5 File Offset: 0x00024BB5
		// (set) Token: 0x06000ACB RID: 2763 RVA: 0x000269BD File Offset: 0x00024BBD
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
					base.OptionType.SetValue(value, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
			}
		}

		// Token: 0x0400053B RID: 1339
		private bool _isSelected;
	}
}
