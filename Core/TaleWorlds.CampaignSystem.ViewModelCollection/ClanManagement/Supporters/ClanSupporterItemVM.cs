using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Supporters
{
	// Token: 0x0200010C RID: 268
	public class ClanSupporterItemVM : ViewModel
	{
		// Token: 0x060019A2 RID: 6562 RVA: 0x0005CAB1 File Offset: 0x0005ACB1
		public ClanSupporterItemVM(Hero hero)
		{
			this.Hero = new HeroVM(hero, false);
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x0005CAC6 File Offset: 0x0005ACC6
		public void ExecuteOpenTooltip()
		{
			InformationManager.ShowTooltip(typeof(Hero), new object[]
			{
				this.Hero.Hero,
				false
			});
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x0005CAF4 File Offset: 0x0005ACF4
		public void ExecuteCloseTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x060019A5 RID: 6565 RVA: 0x0005CAFB File Offset: 0x0005ACFB
		// (set) Token: 0x060019A6 RID: 6566 RVA: 0x0005CB03 File Offset: 0x0005AD03
		[DataSourceProperty]
		public HeroVM Hero
		{
			get
			{
				return this._hero;
			}
			set
			{
				if (value != this._hero)
				{
					this._hero = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Hero");
				}
			}
		}

		// Token: 0x04000C2D RID: 3117
		private HeroVM _hero;
	}
}
