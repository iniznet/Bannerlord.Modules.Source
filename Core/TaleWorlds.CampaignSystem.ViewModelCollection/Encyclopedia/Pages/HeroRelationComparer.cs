using System;
using System.Collections.Generic;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	// Token: 0x020000B5 RID: 181
	public class HeroRelationComparer : IComparer<HeroVM>
	{
		// Token: 0x06001201 RID: 4609 RVA: 0x00046C0F File Offset: 0x00044E0F
		public HeroRelationComparer(Hero pageHero, bool isAscending)
		{
			this._pageHero = pageHero;
			this._isAscending = isAscending;
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x00046C28 File Offset: 0x00044E28
		int IComparer<HeroVM>.Compare(HeroVM x, HeroVM y)
		{
			int heroRelation = CharacterRelationManager.GetHeroRelation(this._pageHero, x.Hero);
			int heroRelation2 = CharacterRelationManager.GetHeroRelation(this._pageHero, y.Hero);
			return heroRelation.CompareTo(heroRelation2) * (this._isAscending ? 1 : (-1));
		}

		// Token: 0x04000866 RID: 2150
		private readonly Hero _pageHero;

		// Token: 0x04000867 RID: 2151
		private readonly bool _isAscending;
	}
}
