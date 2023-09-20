using System;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	// Token: 0x02000061 RID: 97
	public class KingdomWarLogItemVM : ViewModel
	{
		// Token: 0x06000862 RID: 2146 RVA: 0x000238CC File Offset: 0x00021ACC
		public KingdomWarLogItemVM(IEncyclopediaLog log, IFaction effectorFaction)
		{
			this._log = log;
			this.Banner = new ImageIdentifierVM(BannerCode.CreateFrom(effectorFaction.Banner), true);
			this.RefreshValues();
		}

		// Token: 0x06000863 RID: 2147 RVA: 0x000238F8 File Offset: 0x00021AF8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.WarLogTimeText = this._log.GameTime.ToString();
			this.WarLogText = this._log.GetEncyclopediaText().ToString();
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x00023940 File Offset: 0x00021B40
		private void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06000865 RID: 2149 RVA: 0x00023952 File Offset: 0x00021B52
		// (set) Token: 0x06000866 RID: 2150 RVA: 0x0002395A File Offset: 0x00021B5A
		[DataSourceProperty]
		public string WarLogTimeText
		{
			get
			{
				return this._warLogTimeText;
			}
			set
			{
				if (value != this._warLogTimeText)
				{
					this._warLogTimeText = value;
					base.OnPropertyChangedWithValue<string>(value, "WarLogTimeText");
				}
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06000867 RID: 2151 RVA: 0x0002397D File Offset: 0x00021B7D
		// (set) Token: 0x06000868 RID: 2152 RVA: 0x00023985 File Offset: 0x00021B85
		[DataSourceProperty]
		public string WarLogText
		{
			get
			{
				return this._warLogText;
			}
			set
			{
				if (value != this._warLogText)
				{
					this._warLogText = value;
					base.OnPropertyChangedWithValue<string>(value, "WarLogText");
				}
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06000869 RID: 2153 RVA: 0x000239A8 File Offset: 0x00021BA8
		// (set) Token: 0x0600086A RID: 2154 RVA: 0x000239B0 File Offset: 0x00021BB0
		[DataSourceProperty]
		public ImageIdentifierVM Banner
		{
			get
			{
				return this._banner;
			}
			set
			{
				if (value != this._banner)
				{
					this._banner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner");
				}
			}
		}

		// Token: 0x040003C4 RID: 964
		private readonly IEncyclopediaLog _log;

		// Token: 0x040003C5 RID: 965
		private string _warLogText;

		// Token: 0x040003C6 RID: 966
		private string _warLogTimeText;

		// Token: 0x040003C7 RID: 967
		private ImageIdentifierVM _banner;
	}
}
