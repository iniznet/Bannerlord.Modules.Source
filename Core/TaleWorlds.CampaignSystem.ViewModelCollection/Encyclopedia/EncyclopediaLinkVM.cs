using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia
{
	// Token: 0x020000AD RID: 173
	public class EncyclopediaLinkVM : ViewModel
	{
		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x060010EB RID: 4331 RVA: 0x000432F0 File Offset: 0x000414F0
		// (set) Token: 0x060010EC RID: 4332 RVA: 0x000432F8 File Offset: 0x000414F8
		[DataSourceProperty]
		public string ActiveLink
		{
			get
			{
				return this._activeLink;
			}
			set
			{
				if (this._activeLink != value)
				{
					this._activeLink = value;
					base.OnPropertyChangedWithValue<string>(value, "ActiveLink");
				}
			}
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x0004331B File Offset: 0x0004151B
		public void ExecuteActiveLink()
		{
			if (!string.IsNullOrEmpty(this.ActiveLink))
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.ActiveLink);
			}
		}

		// Token: 0x040007E4 RID: 2020
		private string _activeLink;
	}
}
