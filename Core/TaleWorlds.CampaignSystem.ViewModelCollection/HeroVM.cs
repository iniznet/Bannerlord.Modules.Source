using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public class HeroVM : ViewModel
	{
		public Hero Hero { get; }

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

		public void ExecuteLink()
		{
			if (this.Hero != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Hero.EncyclopediaLink);
			}
		}

		public virtual void ExecuteBeginHint()
		{
			if (this.Hero != null)
			{
				InformationManager.ShowTooltip(typeof(Hero), new object[] { this.Hero, false });
			}
		}

		public virtual void ExecuteEndHint()
		{
			if (this.Hero != null)
			{
				MBInformationManager.HideInformations();
			}
		}

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

		private ImageIdentifierVM _imageIdentifier;

		private ImageIdentifierVM _clanBanner;

		private ImageIdentifierVM _clanBanner_9;

		private string _nameText;

		private int _relation = -102;

		private bool _isDead = true;

		private bool _isChild;
	}
}
