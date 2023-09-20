using System;
using System.Collections.Generic;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	public class HeroRelationComparer : IComparer<HeroVM>
	{
		public HeroRelationComparer(Hero pageHero, bool isAscending)
		{
			this._pageHero = pageHero;
			this._isAscending = isAscending;
		}

		int IComparer<HeroVM>.Compare(HeroVM x, HeroVM y)
		{
			int heroRelation = CharacterRelationManager.GetHeroRelation(this._pageHero, x.Hero);
			int heroRelation2 = CharacterRelationManager.GetHeroRelation(this._pageHero, y.Hero);
			return heroRelation.CompareTo(heroRelation2) * (this._isAscending ? 1 : (-1));
		}

		private readonly Hero _pageHero;

		private readonly bool _isAscending;
	}
}
