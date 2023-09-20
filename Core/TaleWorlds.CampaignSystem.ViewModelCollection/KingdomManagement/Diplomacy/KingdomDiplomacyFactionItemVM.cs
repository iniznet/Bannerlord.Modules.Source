using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	// Token: 0x0200005C RID: 92
	public class KingdomDiplomacyFactionItemVM : ViewModel
	{
		// Token: 0x060007E6 RID: 2022 RVA: 0x00021C2A File Offset: 0x0001FE2A
		public KingdomDiplomacyFactionItemVM(IFaction faction)
		{
			this.Hint = new HintViewModel(faction.Name, null);
			this.Visual = new ImageIdentifierVM(BannerCode.CreateFrom(faction.Banner), true);
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x060007E7 RID: 2023 RVA: 0x00021C5B File Offset: 0x0001FE5B
		// (set) Token: 0x060007E8 RID: 2024 RVA: 0x00021C63 File Offset: 0x0001FE63
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

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x060007E9 RID: 2025 RVA: 0x00021C81 File Offset: 0x0001FE81
		// (set) Token: 0x060007EA RID: 2026 RVA: 0x00021C89 File Offset: 0x0001FE89
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

		// Token: 0x04000388 RID: 904
		private HintViewModel _hint;

		// Token: 0x04000389 RID: 905
		private ImageIdentifierVM _visual;
	}
}
