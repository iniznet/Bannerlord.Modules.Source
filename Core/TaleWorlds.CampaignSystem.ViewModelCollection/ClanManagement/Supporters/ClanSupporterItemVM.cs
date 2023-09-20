using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Supporters
{
	public class ClanSupporterItemVM : ViewModel
	{
		public ClanSupporterItemVM(Hero hero)
		{
			this.Hero = new HeroVM(hero, false);
		}

		public void ExecuteOpenTooltip()
		{
			InformationManager.ShowTooltip(typeof(Hero), new object[]
			{
				this.Hero.Hero,
				false
			});
		}

		public void ExecuteCloseTooltip()
		{
			MBInformationManager.HideInformations();
		}

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

		private HeroVM _hero;
	}
}
