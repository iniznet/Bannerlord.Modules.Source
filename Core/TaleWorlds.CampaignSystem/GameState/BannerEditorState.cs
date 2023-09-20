using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x0200032C RID: 812
	public class BannerEditorState : GameState
	{
		// Token: 0x17000AEE RID: 2798
		// (get) Token: 0x06002E05 RID: 11781 RVA: 0x000BFC38 File Offset: 0x000BDE38
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000AEF RID: 2799
		// (get) Token: 0x06002E06 RID: 11782 RVA: 0x000BFC3B File Offset: 0x000BDE3B
		// (set) Token: 0x06002E07 RID: 11783 RVA: 0x000BFC43 File Offset: 0x000BDE43
		public IBannerEditorStateHandler Handler
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

		// Token: 0x06002E08 RID: 11784 RVA: 0x000BFC4C File Offset: 0x000BDE4C
		public Clan GetClan()
		{
			return Clan.PlayerClan;
		}

		// Token: 0x06002E09 RID: 11785 RVA: 0x000BFC53 File Offset: 0x000BDE53
		public CharacterObject GetCharacter()
		{
			return CharacterObject.PlayerCharacter;
		}

		// Token: 0x04000DDB RID: 3547
		private IBannerEditorStateHandler _handler;
	}
}
