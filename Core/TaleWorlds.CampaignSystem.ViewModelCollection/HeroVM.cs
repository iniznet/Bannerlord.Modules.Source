using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x02000018 RID: 24
	public class HeroVM : ViewModel
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600015E RID: 350 RVA: 0x0000A975 File Offset: 0x00008B75
		public Hero Hero { get; }

		// Token: 0x0600015F RID: 351 RVA: 0x0000A980 File Offset: 0x00008B80
		public HeroVM(Hero hero, bool useCivilian = false)
		{
			if (hero != null)
			{
				CharacterCode characterCode = CampaignUIHelper.GetCharacterCode(hero.CharacterObject, useCivilian);
				this.ImageIdentifier = new ImageIdentifierVM(characterCode);
				this.ClanBanner = new ImageIdentifierVM(hero.ClanBanner);
				this.ClanBanner_9 = new ImageIdentifierVM(BannerCode.CreateFrom(hero.ClanBanner), true);
				this.Relation = HeroVM.GetRelation(hero);
				this.IsDead = !hero.IsAlive;
				TextObject textObject;
				this.IsChild = !CampaignUIHelper.IsHeroInformationHidden(hero, out textObject) && FaceGen.GetMaturityTypeWithAge(hero.Age) <= BodyMeshMaturityType.Child;
			}
			else
			{
				this.ImageIdentifier = new ImageIdentifierVM(ImageIdentifierType.Null);
				this.ClanBanner = new ImageIdentifierVM(ImageIdentifierType.Null);
				this.ClanBanner_9 = new ImageIdentifierVM(ImageIdentifierType.Null);
				this.Relation = 0;
			}
			this.Hero = hero;
			this.RefreshValues();
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000AA62 File Offset: 0x00008C62
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.Hero != null)
			{
				this.NameText = this.Hero.Name.ToString();
				return;
			}
			this.NameText = "";
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000AA94 File Offset: 0x00008C94
		public void ExecuteLink()
		{
			if (this.Hero != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Hero.EncyclopediaLink);
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000AAB8 File Offset: 0x00008CB8
		public virtual void ExecuteBeginHint()
		{
			if (this.Hero != null)
			{
				InformationManager.ShowTooltip(typeof(Hero), new object[] { this.Hero, false });
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000AAE9 File Offset: 0x00008CE9
		public virtual void ExecuteEndHint()
		{
			if (this.Hero != null)
			{
				MBInformationManager.HideInformations();
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000164 RID: 356 RVA: 0x0000AAF8 File Offset: 0x00008CF8
		// (set) Token: 0x06000165 RID: 357 RVA: 0x0000AB00 File Offset: 0x00008D00
		[DataSourceProperty]
		public bool IsDead
		{
			get
			{
				return this._isDead;
			}
			set
			{
				if (value != this._isDead)
				{
					this._isDead = value;
					base.OnPropertyChangedWithValue(value, "IsDead");
				}
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000166 RID: 358 RVA: 0x0000AB1E File Offset: 0x00008D1E
		// (set) Token: 0x06000167 RID: 359 RVA: 0x0000AB26 File Offset: 0x00008D26
		[DataSourceProperty]
		public bool IsChild
		{
			get
			{
				return this._isChild;
			}
			set
			{
				if (value != this._isChild)
				{
					this._isChild = value;
					base.OnPropertyChangedWithValue(value, "IsChild");
				}
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000168 RID: 360 RVA: 0x0000AB44 File Offset: 0x00008D44
		// (set) Token: 0x06000169 RID: 361 RVA: 0x0000AB4C File Offset: 0x00008D4C
		[DataSourceProperty]
		public int Relation
		{
			get
			{
				return this._relation;
			}
			set
			{
				if (value != this._relation)
				{
					this._relation = value;
					base.OnPropertyChangedWithValue(value, "Relation");
				}
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600016A RID: 362 RVA: 0x0000AB6A File Offset: 0x00008D6A
		// (set) Token: 0x0600016B RID: 363 RVA: 0x0000AB72 File Offset: 0x00008D72
		[DataSourceProperty]
		public ImageIdentifierVM ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (value != this._imageIdentifier)
				{
					this._imageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ImageIdentifier");
				}
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600016C RID: 364 RVA: 0x0000AB90 File Offset: 0x00008D90
		// (set) Token: 0x0600016D RID: 365 RVA: 0x0000AB98 File Offset: 0x00008D98
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600016E RID: 366 RVA: 0x0000ABBB File Offset: 0x00008DBB
		// (set) Token: 0x0600016F RID: 367 RVA: 0x0000ABC3 File Offset: 0x00008DC3
		[DataSourceProperty]
		public ImageIdentifierVM ClanBanner
		{
			get
			{
				return this._clanBanner;
			}
			set
			{
				if (value != this._clanBanner)
				{
					this._clanBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ClanBanner");
				}
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000170 RID: 368 RVA: 0x0000ABE1 File Offset: 0x00008DE1
		// (set) Token: 0x06000171 RID: 369 RVA: 0x0000ABE9 File Offset: 0x00008DE9
		[DataSourceProperty]
		public ImageIdentifierVM ClanBanner_9
		{
			get
			{
				return this._clanBanner_9;
			}
			set
			{
				if (value != this._clanBanner_9)
				{
					this._clanBanner_9 = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ClanBanner_9");
				}
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000AC07 File Offset: 0x00008E07
		public static int GetRelation(Hero hero)
		{
			if (hero == null)
			{
				return -101;
			}
			if (hero == Hero.MainHero)
			{
				return 101;
			}
			if (ViewModel.UIDebugMode)
			{
				return MBRandom.RandomInt(-100, 100);
			}
			return Hero.MainHero.GetRelation(hero);
		}

		// Token: 0x040000A9 RID: 169
		private ImageIdentifierVM _imageIdentifier;

		// Token: 0x040000AA RID: 170
		private ImageIdentifierVM _clanBanner;

		// Token: 0x040000AB RID: 171
		private ImageIdentifierVM _clanBanner_9;

		// Token: 0x040000AC RID: 172
		private string _nameText;

		// Token: 0x040000AD RID: 173
		private int _relation = -102;

		// Token: 0x040000AE RID: 174
		private bool _isDead = true;

		// Token: 0x040000AF RID: 175
		private bool _isChild;
	}
}
