using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	public class KingdomDiplomacyFactionItemVM : ViewModel
	{
		public KingdomDiplomacyFactionItemVM(IFaction faction)
		{
			this.Hint = new HintViewModel(faction.Name, null);
			this.Visual = new ImageIdentifierVM(BannerCode.CreateFrom(faction.Banner), true);
		}

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

		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
				}
			}
		}

		private HintViewModel _hint;

		private ImageIdentifierVM _visual;
	}
}
