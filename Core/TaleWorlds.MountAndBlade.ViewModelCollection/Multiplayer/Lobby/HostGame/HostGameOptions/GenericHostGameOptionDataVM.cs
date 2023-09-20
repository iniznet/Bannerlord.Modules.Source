using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame.HostGameOptions
{
	// Token: 0x02000078 RID: 120
	public abstract class GenericHostGameOptionDataVM : ViewModel
	{
		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06000ACC RID: 2764 RVA: 0x000269E8 File Offset: 0x00024BE8
		public MultiplayerOptions.OptionType OptionType { get; }

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06000ACD RID: 2765 RVA: 0x000269F0 File Offset: 0x00024BF0
		public int PreferredIndex { get; }

		// Token: 0x06000ACE RID: 2766 RVA: 0x000269F8 File Offset: 0x00024BF8
		internal GenericHostGameOptionDataVM(OptionsVM.OptionsDataType type, MultiplayerOptions.OptionType optionType, int preferredIndex)
		{
			this.Category = (int)type;
			this.OptionType = optionType;
			this.PreferredIndex = preferredIndex;
			this.Index = preferredIndex;
			this.IsEnabled = true;
			this.RefreshValues();
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x00026A2C File Offset: 0x00024C2C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = GameTexts.FindText("str_multiplayer_option", this.OptionType.ToString()).ToString();
		}

		// Token: 0x06000AD0 RID: 2768
		public abstract void RefreshData();

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06000AD1 RID: 2769 RVA: 0x00026A68 File Offset: 0x00024C68
		// (set) Token: 0x06000AD2 RID: 2770 RVA: 0x00026A70 File Offset: 0x00024C70
		[DataSourceProperty]
		public int Index
		{
			get
			{
				return this._index;
			}
			set
			{
				if (value != this._index)
				{
					this._index = value;
					base.OnPropertyChangedWithValue(value, "Index");
				}
			}
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06000AD3 RID: 2771 RVA: 0x00026A8E File Offset: 0x00024C8E
		// (set) Token: 0x06000AD4 RID: 2772 RVA: 0x00026A96 File Offset: 0x00024C96
		[DataSourceProperty]
		public int Category
		{
			get
			{
				return this._category;
			}
			set
			{
				if (value != this._category)
				{
					this._category = value;
					base.OnPropertyChangedWithValue(value, "Category");
				}
			}
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x00026AB4 File Offset: 0x00024CB4
		// (set) Token: 0x06000AD6 RID: 2774 RVA: 0x00026ABC File Offset: 0x00024CBC
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

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06000AD7 RID: 2775 RVA: 0x00026ADF File Offset: 0x00024CDF
		// (set) Token: 0x06000AD8 RID: 2776 RVA: 0x00026AE7 File Offset: 0x00024CE7
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x0400053E RID: 1342
		private int _index;

		// Token: 0x0400053F RID: 1343
		private int _category;

		// Token: 0x04000540 RID: 1344
		private string _name;

		// Token: 0x04000541 RID: 1345
		private bool _isEnabled;
	}
}
