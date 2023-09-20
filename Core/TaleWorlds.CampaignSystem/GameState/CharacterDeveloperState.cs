using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x0200032F RID: 815
	public class CharacterDeveloperState : GameState
	{
		// Token: 0x17000AF2 RID: 2802
		// (get) Token: 0x06002E10 RID: 11792 RVA: 0x000BFC94 File Offset: 0x000BDE94
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000AF3 RID: 2803
		// (get) Token: 0x06002E11 RID: 11793 RVA: 0x000BFC97 File Offset: 0x000BDE97
		// (set) Token: 0x06002E12 RID: 11794 RVA: 0x000BFC9F File Offset: 0x000BDE9F
		public Hero InitialSelectedHero { get; private set; }

		// Token: 0x06002E13 RID: 11795 RVA: 0x000BFCA8 File Offset: 0x000BDEA8
		public CharacterDeveloperState()
		{
		}

		// Token: 0x06002E14 RID: 11796 RVA: 0x000BFCB0 File Offset: 0x000BDEB0
		public CharacterDeveloperState(Hero initialSelectedHero)
		{
			this.InitialSelectedHero = initialSelectedHero;
		}

		// Token: 0x17000AF4 RID: 2804
		// (get) Token: 0x06002E15 RID: 11797 RVA: 0x000BFCBF File Offset: 0x000BDEBF
		// (set) Token: 0x06002E16 RID: 11798 RVA: 0x000BFCC7 File Offset: 0x000BDEC7
		public ICharacterDeveloperStateHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		// Token: 0x04000DDF RID: 3551
		private ICharacterDeveloperStateHandler _handler;
	}
}
