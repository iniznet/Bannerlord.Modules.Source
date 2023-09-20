using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame.HostGameOptions
{
	// Token: 0x02000079 RID: 121
	public class InputHostGameOptionDataVM : GenericHostGameOptionDataVM
	{
		// Token: 0x06000AD9 RID: 2777 RVA: 0x00026B05 File Offset: 0x00024D05
		public InputHostGameOptionDataVM(MultiplayerOptions.OptionType optionType, int preferredIndex)
			: base(OptionsVM.OptionsDataType.InputOption, optionType, preferredIndex)
		{
			this.RefreshData();
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x00026B18 File Offset: 0x00024D18
		public override void RefreshData()
		{
			string strValue = base.OptionType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			this.Text = strValue;
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06000ADB RID: 2779 RVA: 0x00026B39 File Offset: 0x00024D39
		// (set) Token: 0x06000ADC RID: 2780 RVA: 0x00026B41 File Offset: 0x00024D41
		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
					base.OptionType.SetValue(value, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
			}
		}

		// Token: 0x04000542 RID: 1346
		private string _text;
	}
}
