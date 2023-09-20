using System;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	public class KingdomWarLogItemVM : ViewModel
	{
		public KingdomWarLogItemVM(IEncyclopediaLog log, IFaction effectorFaction)
		{
			this._log = log;
			this.Banner = new ImageIdentifierVM(BannerCode.CreateFrom(effectorFaction.Banner), true);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.WarLogTimeText = this._log.GameTime.ToString();
			this.WarLogText = this._log.GetEncyclopediaText().ToString();
		}

		private void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

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

		private readonly IEncyclopediaLog _log;

		private string _warLogText;

		private string _warLogTimeText;

		private ImageIdentifierVM _banner;
	}
}
