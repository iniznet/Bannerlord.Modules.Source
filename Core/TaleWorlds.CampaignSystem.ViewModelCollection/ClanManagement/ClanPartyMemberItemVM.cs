using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x02000106 RID: 262
	public class ClanPartyMemberItemVM : ViewModel
	{
		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x06001915 RID: 6421 RVA: 0x0005AC94 File Offset: 0x00058E94
		// (set) Token: 0x06001916 RID: 6422 RVA: 0x0005AC9C File Offset: 0x00058E9C
		public Hero HeroObject { get; private set; }

		// Token: 0x06001917 RID: 6423 RVA: 0x0005ACA8 File Offset: 0x00058EA8
		public ClanPartyMemberItemVM(Hero hero, MobileParty party)
		{
			this.HeroObject = hero;
			this.IsLeader = hero == party.LeaderHero;
			CharacterCode characterCode = CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false);
			this.Visual = new ImageIdentifierVM(characterCode);
			this.HeroModel = new HeroViewModel(CharacterViewModel.StanceTypes.None);
			this.HeroModel.FillFrom(this.HeroObject, -1, false, false);
			this.RefreshValues();
		}

		// Token: 0x06001918 RID: 6424 RVA: 0x0005AD10 File Offset: 0x00058F10
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.HeroObject.Name.ToString();
			this.UpdateProperties();
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x0005AD34 File Offset: 0x00058F34
		private void ExecuteLocationLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x0005AD46 File Offset: 0x00058F46
		public void UpdateProperties()
		{
			this.HeroModel = new HeroViewModel(CharacterViewModel.StanceTypes.None);
			this.HeroModel.FillFrom(this.HeroObject, -1, false, false);
			this.Banner_9 = new ImageIdentifierVM(BannerCode.CreateFrom(this.HeroObject.ClanBanner), true);
		}

		// Token: 0x0600191B RID: 6427 RVA: 0x0005AD84 File Offset: 0x00058F84
		public void ExecuteLink()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this.HeroObject.EncyclopediaLink);
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x0005ADA0 File Offset: 0x00058FA0
		public virtual void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(Hero), new object[] { this.HeroObject, true });
		}

		// Token: 0x0600191D RID: 6429 RVA: 0x0005ADC9 File Offset: 0x00058FC9
		public virtual void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x0600191E RID: 6430 RVA: 0x0005ADD0 File Offset: 0x00058FD0
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.HeroModel.OnFinalize();
		}

		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x0600191F RID: 6431 RVA: 0x0005ADE3 File Offset: 0x00058FE3
		// (set) Token: 0x06001920 RID: 6432 RVA: 0x0005ADEB File Offset: 0x00058FEB
		[DataSourceProperty]
		public HeroViewModel HeroModel
		{
			get
			{
				return this._heroModel;
			}
			set
			{
				if (value != this._heroModel)
				{
					this._heroModel = value;
					base.OnPropertyChangedWithValue<HeroViewModel>(value, "HeroModel");
				}
			}
		}

		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x06001921 RID: 6433 RVA: 0x0005AE09 File Offset: 0x00059009
		// (set) Token: 0x06001922 RID: 6434 RVA: 0x0005AE11 File Offset: 0x00059011
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

		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x06001923 RID: 6435 RVA: 0x0005AE2F File Offset: 0x0005902F
		// (set) Token: 0x06001924 RID: 6436 RVA: 0x0005AE37 File Offset: 0x00059037
		[DataSourceProperty]
		public ImageIdentifierVM Banner_9
		{
			get
			{
				return this._banner_9;
			}
			set
			{
				if (value != this._banner_9)
				{
					this._banner_9 = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner_9");
				}
			}
		}

		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x06001925 RID: 6437 RVA: 0x0005AE55 File Offset: 0x00059055
		// (set) Token: 0x06001926 RID: 6438 RVA: 0x0005AE5D File Offset: 0x0005905D
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x06001927 RID: 6439 RVA: 0x0005AE80 File Offset: 0x00059080
		// (set) Token: 0x06001928 RID: 6440 RVA: 0x0005AE88 File Offset: 0x00059088
		[DataSourceProperty]
		public bool IsLeader
		{
			get
			{
				return this._isLeader;
			}
			set
			{
				if (value != this._isLeader)
				{
					this._isLeader = value;
					base.OnPropertyChangedWithValue(value, "IsLeader");
				}
			}
		}

		// Token: 0x04000BEE RID: 3054
		private ImageIdentifierVM _visual;

		// Token: 0x04000BEF RID: 3055
		private ImageIdentifierVM _banner_9;

		// Token: 0x04000BF0 RID: 3056
		private string _name;

		// Token: 0x04000BF1 RID: 3057
		private bool _isLeader;

		// Token: 0x04000BF2 RID: 3058
		private HeroViewModel _heroModel;
	}
}
