using System;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	// Token: 0x020000C6 RID: 198
	public class EncyclopediaFamilyMemberVM : HeroVM
	{
		// Token: 0x0600130F RID: 4879 RVA: 0x00049752 File Offset: 0x00047952
		public EncyclopediaFamilyMemberVM(Hero hero, Hero baseHero)
			: base(hero, false)
		{
			this._baseHero = baseHero;
			this.RefreshValues();
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x00049769 File Offset: 0x00047969
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._baseHero != null)
			{
				this.Role = ConversationHelper.GetHeroRelationToHeroTextShort(base.Hero, this._baseHero, true);
			}
		}

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x06001311 RID: 4881 RVA: 0x00049791 File Offset: 0x00047991
		// (set) Token: 0x06001312 RID: 4882 RVA: 0x00049799 File Offset: 0x00047999
		[DataSourceProperty]
		public string Role
		{
			get
			{
				return this._role;
			}
			set
			{
				if (value != this._role)
				{
					this._role = value;
					base.OnPropertyChangedWithValue<string>(value, "Role");
				}
			}
		}

		// Token: 0x040008D5 RID: 2261
		private readonly Hero _baseHero;

		// Token: 0x040008D6 RID: 2262
		private string _role;
	}
}
