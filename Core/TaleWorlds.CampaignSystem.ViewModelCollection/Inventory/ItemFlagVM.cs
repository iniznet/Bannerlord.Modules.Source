using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	// Token: 0x02000079 RID: 121
	public class ItemFlagVM : ViewModel
	{
		// Token: 0x06000AF0 RID: 2800 RVA: 0x0002AD1A File Offset: 0x00028F1A
		public ItemFlagVM(string iconName, TextObject hint)
		{
			this.Icon = this.GetIconPath(iconName);
			this.Hint = new HintViewModel(hint, null);
		}

		// Token: 0x06000AF1 RID: 2801 RVA: 0x0002AD3C File Offset: 0x00028F3C
		private string GetIconPath(string iconName)
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "GetIconPath");
			mbstringBuilder.Append<string>("<img src=\"SPGeneral\\");
			mbstringBuilder.Append<string>(iconName);
			mbstringBuilder.Append<string>("\"/>");
			return mbstringBuilder.ToStringAndRelease();
		}

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06000AF2 RID: 2802 RVA: 0x0002AD89 File Offset: 0x00028F89
		// (set) Token: 0x06000AF3 RID: 2803 RVA: 0x0002AD91 File Offset: 0x00028F91
		[DataSourceProperty]
		public string Icon
		{
			get
			{
				return this._icon;
			}
			set
			{
				if (value != this._icon)
				{
					this._icon = value;
					base.OnPropertyChangedWithValue<string>(value, "Icon");
				}
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06000AF4 RID: 2804 RVA: 0x0002ADB4 File Offset: 0x00028FB4
		// (set) Token: 0x06000AF5 RID: 2805 RVA: 0x0002ADBC File Offset: 0x00028FBC
		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x040004F3 RID: 1267
		private string _icon;

		// Token: 0x040004F4 RID: 1268
		private HintViewModel _hint;
	}
}
