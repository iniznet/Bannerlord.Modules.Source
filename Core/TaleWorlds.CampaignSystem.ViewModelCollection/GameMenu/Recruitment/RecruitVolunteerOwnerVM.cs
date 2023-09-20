using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Recruitment
{
	// Token: 0x0200009B RID: 155
	public class RecruitVolunteerOwnerVM : HeroVM
	{
		// Token: 0x06000F58 RID: 3928 RVA: 0x0003BFDB File Offset: 0x0003A1DB
		public RecruitVolunteerOwnerVM(Hero hero, int relation)
			: base(hero, hero != null && hero.IsNotable)
		{
			this._hero = hero;
			this.RelationToPlayer = relation;
			this.RefreshValues();
		}

		// Token: 0x06000F59 RID: 3929 RVA: 0x0003C004 File Offset: 0x0003A204
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._hero != null)
			{
				if (this._hero.IsPreacher)
				{
					this.TitleText = GameTexts.FindText("str_preacher", null).ToString();
					return;
				}
				if (this._hero.IsGangLeader)
				{
					this.TitleText = GameTexts.FindText("str_gang_leader", null).ToString();
					return;
				}
				if (this._hero.IsMerchant)
				{
					this.TitleText = GameTexts.FindText("str_merchant", null).ToString();
					return;
				}
				if (this._hero.IsRuralNotable)
				{
					this.TitleText = GameTexts.FindText("str_rural_notable", null).ToString();
				}
			}
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x0003C0B1 File Offset: 0x0003A2B1
		public void ExecuteOpenEncyclopedia()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this._hero.EncyclopediaLink);
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x0003C0CD File Offset: 0x0003A2CD
		public void ExecuteFocus()
		{
			Action<RecruitVolunteerOwnerVM> onFocused = RecruitVolunteerOwnerVM.OnFocused;
			if (onFocused == null)
			{
				return;
			}
			onFocused(this);
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x0003C0DF File Offset: 0x0003A2DF
		public void ExecuteUnfocus()
		{
			Action<RecruitVolunteerOwnerVM> onFocused = RecruitVolunteerOwnerVM.OnFocused;
			if (onFocused == null)
			{
				return;
			}
			onFocused(null);
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06000F5D RID: 3933 RVA: 0x0003C0F1 File Offset: 0x0003A2F1
		// (set) Token: 0x06000F5E RID: 3934 RVA: 0x0003C0F9 File Offset: 0x0003A2F9
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06000F5F RID: 3935 RVA: 0x0003C11C File Offset: 0x0003A31C
		// (set) Token: 0x06000F60 RID: 3936 RVA: 0x0003C124 File Offset: 0x0003A324
		[DataSourceProperty]
		public int RelationToPlayer
		{
			get
			{
				return this._relationToPlayer;
			}
			set
			{
				if (value != this._relationToPlayer)
				{
					this._relationToPlayer = value;
					base.OnPropertyChangedWithValue(value, "RelationToPlayer");
				}
			}
		}

		// Token: 0x04000719 RID: 1817
		public static Action<RecruitVolunteerOwnerVM> OnFocused;

		// Token: 0x0400071A RID: 1818
		private Hero _hero;

		// Token: 0x0400071B RID: 1819
		private string _titleText;

		// Token: 0x0400071C RID: 1820
		private int _relationToPlayer;
	}
}
