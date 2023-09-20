using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x020000F8 RID: 248
	public class ClanCardSelectionPopupItemPropertyVM : ViewModel
	{
		// Token: 0x06001756 RID: 5974 RVA: 0x000564CF File Offset: 0x000546CF
		public ClanCardSelectionPopupItemPropertyVM(in ClanCardSelectionItemPropertyInfo info)
		{
			this._titleText = info.Title;
			this._valueText = info.Value;
		}

		// Token: 0x06001757 RID: 5975 RVA: 0x000564F0 File Offset: 0x000546F0
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject titleText = this._titleText;
			this.Title = ((titleText != null) ? titleText.ToString() : null) ?? string.Empty;
			TextObject valueText = this._valueText;
			this.Value = ((valueText != null) ? valueText.ToString() : null) ?? string.Empty;
		}

		// Token: 0x170007E7 RID: 2023
		// (get) Token: 0x06001758 RID: 5976 RVA: 0x00056545 File Offset: 0x00054745
		// (set) Token: 0x06001759 RID: 5977 RVA: 0x0005654D File Offset: 0x0005474D
		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		// Token: 0x170007E8 RID: 2024
		// (get) Token: 0x0600175A RID: 5978 RVA: 0x00056570 File Offset: 0x00054770
		// (set) Token: 0x0600175B RID: 5979 RVA: 0x00056578 File Offset: 0x00054778
		[DataSourceProperty]
		public string Value
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
					base.OnPropertyChangedWithValue<string>(value, "Value");
				}
			}
		}

		// Token: 0x04000AF1 RID: 2801
		private readonly TextObject _titleText;

		// Token: 0x04000AF2 RID: 2802
		private readonly TextObject _valueText;

		// Token: 0x04000AF3 RID: 2803
		private string _title;

		// Token: 0x04000AF4 RID: 2804
		private string _value;
	}
}
