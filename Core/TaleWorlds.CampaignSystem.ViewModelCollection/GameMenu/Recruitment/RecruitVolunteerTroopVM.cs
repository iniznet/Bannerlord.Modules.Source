using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Recruitment
{
	// Token: 0x0200009C RID: 156
	public class RecruitVolunteerTroopVM : ViewModel
	{
		// Token: 0x06000F61 RID: 3937 RVA: 0x0003C144 File Offset: 0x0003A344
		public RecruitVolunteerTroopVM(RecruitVolunteerVM owner, CharacterObject character, int index, Action<RecruitVolunteerTroopVM> onClick, Action<RecruitVolunteerTroopVM> onRemoveFromCart)
		{
			if (character != null)
			{
				this.NameText = character.Name.ToString();
				this._character = character;
				GameTexts.SetVariable("LEVEL", character.Level);
				this.Level = GameTexts.FindText("str_level_with_value", null).ToString();
				this.Character = character;
				this.Wage = this.Character.TroopWage;
				this.Cost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(this.Character, Hero.MainHero, false);
				this.IsTroopEmpty = false;
				CharacterCode characterCode = CampaignUIHelper.GetCharacterCode(character, false);
				this.ImageIdentifier = new ImageIdentifierVM(characterCode);
				this.TierIconData = CampaignUIHelper.GetCharacterTierData(character, false);
				this.TypeIconData = CampaignUIHelper.GetCharacterTypeData(character, false);
			}
			else
			{
				this.IsTroopEmpty = true;
			}
			this.Owner = owner;
			if (this.Owner != null)
			{
				this._currentRelation = Hero.MainHero.GetRelation(this.Owner.OwnerHero);
			}
			this._maximumIndexCanBeRecruit = Campaign.Current.Models.VolunteerModel.MaximumIndexHeroCanRecruitFromHero(Hero.MainHero, this.Owner.OwnerHero, -101);
			for (int i = -100; i < 100; i++)
			{
				if (index < Campaign.Current.Models.VolunteerModel.MaximumIndexHeroCanRecruitFromHero(Hero.MainHero, this.Owner.OwnerHero, i))
				{
					this._requiredRelation = i;
					break;
				}
			}
			this._onClick = onClick;
			this.Index = index;
			this._onRemoveFromCart = onRemoveFromCart;
			this.RefreshValues();
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x0003C2CC File Offset: 0x0003A4CC
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._character != null)
			{
				this.NameText = this._character.Name.ToString();
				GameTexts.SetVariable("LEVEL", this._character.Level);
				this.Level = GameTexts.FindText("str_level_with_value", null).ToString();
			}
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x0003C328 File Offset: 0x0003A528
		public void ExecuteRecruit()
		{
			if (this.CanBeRecruited)
			{
				this._onClick(this);
				return;
			}
			if (this.IsInCart)
			{
				this._onRemoveFromCart(this);
			}
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x0003C353 File Offset: 0x0003A553
		public void ExecuteOpenEncyclopedia()
		{
			if (this.Character != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Character.EncyclopediaLink);
			}
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x0003C377 File Offset: 0x0003A577
		public void ExecuteRemoveFromCart()
		{
			if (this.IsInCart)
			{
				this._onRemoveFromCart(this);
			}
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x0003C390 File Offset: 0x0003A590
		public virtual void ExecuteBeginHint()
		{
			if (this._character != null)
			{
				if (this.PlayerHasEnoughRelation)
				{
					InformationManager.ShowTooltip(typeof(CharacterObject), new object[] { this._character });
					return;
				}
				List<TooltipProperty> list = new List<TooltipProperty>();
				string text = "";
				list.Add(new TooltipProperty(text, this._character.Name.ToString(), 1, false, TooltipProperty.TooltipPropertyFlags.None));
				list.Add(new TooltipProperty(text, text, -1, false, TooltipProperty.TooltipPropertyFlags.None));
				GameTexts.SetVariable("LEVEL", this._character.Level);
				GameTexts.SetVariable("newline", "\n");
				list.Add(new TooltipProperty(text, GameTexts.FindText("str_level_with_value", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				GameTexts.SetVariable("REL1", this._currentRelation);
				GameTexts.SetVariable("REL2", this._requiredRelation);
				list.Add(new TooltipProperty(text, GameTexts.FindText("str_recruit_volunteers_not_enough_relation", null).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { list });
				return;
			}
			else
			{
				if (this.PlayerHasEnoughRelation)
				{
					MBInformationManager.ShowHint(GameTexts.FindText("str_recruit_volunteers_new_troop", null).ToString());
					return;
				}
				GameTexts.SetVariable("newline", "\n");
				GameTexts.SetVariable("REL1", this._currentRelation);
				GameTexts.SetVariable("REL2", this._requiredRelation);
				GameTexts.SetVariable("STR1", GameTexts.FindText("str_recruit_volunteers_new_troop", null));
				GameTexts.SetVariable("STR2", GameTexts.FindText("str_recruit_volunteers_not_enough_relation", null));
				MBInformationManager.ShowHint(GameTexts.FindText("str_string_newline_string", null).ToString());
				return;
			}
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x0003C532 File Offset: 0x0003A732
		public virtual void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x0003C539 File Offset: 0x0003A739
		public void ExecuteFocus()
		{
			if (!this.IsTroopEmpty)
			{
				Action<RecruitVolunteerTroopVM> onFocused = RecruitVolunteerTroopVM.OnFocused;
				if (onFocused == null)
				{
					return;
				}
				onFocused(this);
			}
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x0003C553 File Offset: 0x0003A753
		public void ExecuteUnfocus()
		{
			Action<RecruitVolunteerTroopVM> onFocused = RecruitVolunteerTroopVM.OnFocused;
			if (onFocused == null)
			{
				return;
			}
			onFocused(null);
		}

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06000F6A RID: 3946 RVA: 0x0003C565 File Offset: 0x0003A765
		// (set) Token: 0x06000F6B RID: 3947 RVA: 0x0003C56D File Offset: 0x0003A76D
		[DataSourceProperty]
		public string Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (value != this._level)
				{
					this._level = value;
					base.OnPropertyChangedWithValue<string>(value, "Level");
				}
			}
		}

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06000F6C RID: 3948 RVA: 0x0003C590 File Offset: 0x0003A790
		// (set) Token: 0x06000F6D RID: 3949 RVA: 0x0003C598 File Offset: 0x0003A798
		[DataSourceProperty]
		public bool CanBeRecruited
		{
			get
			{
				return this._canBeRecruited;
			}
			set
			{
				if (value != this._canBeRecruited)
				{
					this._canBeRecruited = value;
					base.OnPropertyChangedWithValue(value, "CanBeRecruited");
				}
			}
		}

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06000F6E RID: 3950 RVA: 0x0003C5B6 File Offset: 0x0003A7B6
		// (set) Token: 0x06000F6F RID: 3951 RVA: 0x0003C5BE File Offset: 0x0003A7BE
		[DataSourceProperty]
		public bool IsHiglightEnabled
		{
			get
			{
				return this._isHiglightEnabled;
			}
			set
			{
				if (value != this._isHiglightEnabled)
				{
					this._isHiglightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsHiglightEnabled");
				}
			}
		}

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06000F70 RID: 3952 RVA: 0x0003C5DC File Offset: 0x0003A7DC
		// (set) Token: 0x06000F71 RID: 3953 RVA: 0x0003C5E4 File Offset: 0x0003A7E4
		[DataSourceProperty]
		public int Wage
		{
			get
			{
				return this._wage;
			}
			set
			{
				if (value != this._wage)
				{
					this._wage = value;
					base.OnPropertyChangedWithValue(value, "Wage");
				}
			}
		}

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x06000F72 RID: 3954 RVA: 0x0003C602 File Offset: 0x0003A802
		// (set) Token: 0x06000F73 RID: 3955 RVA: 0x0003C60A File Offset: 0x0003A80A
		[DataSourceProperty]
		public int Cost
		{
			get
			{
				return this._cost;
			}
			set
			{
				if (value != this._cost)
				{
					this._cost = value;
					base.OnPropertyChangedWithValue(value, "Cost");
				}
			}
		}

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x06000F74 RID: 3956 RVA: 0x0003C628 File Offset: 0x0003A828
		// (set) Token: 0x06000F75 RID: 3957 RVA: 0x0003C630 File Offset: 0x0003A830
		[DataSourceProperty]
		public bool IsInCart
		{
			get
			{
				return this._isInCart;
			}
			set
			{
				if (value != this._isInCart)
				{
					this._isInCart = value;
					base.OnPropertyChangedWithValue(value, "IsInCart");
				}
			}
		}

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x06000F76 RID: 3958 RVA: 0x0003C64E File Offset: 0x0003A84E
		// (set) Token: 0x06000F77 RID: 3959 RVA: 0x0003C656 File Offset: 0x0003A856
		[DataSourceProperty]
		public bool IsTroopEmpty
		{
			get
			{
				return this._isTroopEmpty;
			}
			set
			{
				if (value != this._isTroopEmpty)
				{
					this._isTroopEmpty = value;
					base.OnPropertyChangedWithValue(value, "IsTroopEmpty");
				}
			}
		}

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06000F78 RID: 3960 RVA: 0x0003C674 File Offset: 0x0003A874
		// (set) Token: 0x06000F79 RID: 3961 RVA: 0x0003C67C File Offset: 0x0003A87C
		[DataSourceProperty]
		public bool PlayerHasEnoughRelation
		{
			get
			{
				return this._playerHasEnoughRelation;
			}
			set
			{
				if (value != this._playerHasEnoughRelation)
				{
					this._playerHasEnoughRelation = value;
					base.OnPropertyChangedWithValue(value, "PlayerHasEnoughRelation");
				}
			}
		}

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06000F7A RID: 3962 RVA: 0x0003C69A File Offset: 0x0003A89A
		// (set) Token: 0x06000F7B RID: 3963 RVA: 0x0003C6A2 File Offset: 0x0003A8A2
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

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06000F7C RID: 3964 RVA: 0x0003C6C0 File Offset: 0x0003A8C0
		// (set) Token: 0x06000F7D RID: 3965 RVA: 0x0003C6C8 File Offset: 0x0003A8C8
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

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06000F7E RID: 3966 RVA: 0x0003C6EB File Offset: 0x0003A8EB
		// (set) Token: 0x06000F7F RID: 3967 RVA: 0x0003C6F3 File Offset: 0x0003A8F3
		[DataSourceProperty]
		public StringItemWithHintVM TierIconData
		{
			get
			{
				return this._tierIconData;
			}
			set
			{
				if (value != this._tierIconData)
				{
					this._tierIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TierIconData");
				}
			}
		}

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06000F80 RID: 3968 RVA: 0x0003C711 File Offset: 0x0003A911
		// (set) Token: 0x06000F81 RID: 3969 RVA: 0x0003C719 File Offset: 0x0003A919
		[DataSourceProperty]
		public StringItemWithHintVM TypeIconData
		{
			get
			{
				return this._typeIconData;
			}
			set
			{
				if (value != this._typeIconData)
				{
					this._typeIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TypeIconData");
				}
			}
		}

		// Token: 0x0400071D RID: 1821
		public static Action<RecruitVolunteerTroopVM> OnFocused;

		// Token: 0x0400071E RID: 1822
		private readonly Action<RecruitVolunteerTroopVM> _onClick;

		// Token: 0x0400071F RID: 1823
		private readonly Action<RecruitVolunteerTroopVM> _onRemoveFromCart;

		// Token: 0x04000720 RID: 1824
		private CharacterObject _character;

		// Token: 0x04000721 RID: 1825
		public CharacterObject Character;

		// Token: 0x04000722 RID: 1826
		public int Index;

		// Token: 0x04000723 RID: 1827
		private int _maximumIndexCanBeRecruit;

		// Token: 0x04000724 RID: 1828
		private int _requiredRelation;

		// Token: 0x04000725 RID: 1829
		public RecruitVolunteerVM Owner;

		// Token: 0x04000726 RID: 1830
		private ImageIdentifierVM _imageIdentifier;

		// Token: 0x04000727 RID: 1831
		private string _nameText;

		// Token: 0x04000728 RID: 1832
		private string _level;

		// Token: 0x04000729 RID: 1833
		private bool _canBeRecruited;

		// Token: 0x0400072A RID: 1834
		private bool _isInCart;

		// Token: 0x0400072B RID: 1835
		private int _wage;

		// Token: 0x0400072C RID: 1836
		private int _cost;

		// Token: 0x0400072D RID: 1837
		private bool _isTroopEmpty;

		// Token: 0x0400072E RID: 1838
		private bool _playerHasEnoughRelation;

		// Token: 0x0400072F RID: 1839
		private int _currentRelation;

		// Token: 0x04000730 RID: 1840
		private bool _isHiglightEnabled;

		// Token: 0x04000731 RID: 1841
		private StringItemWithHintVM _tierIconData;

		// Token: 0x04000732 RID: 1842
		private StringItemWithHintVM _typeIconData;
	}
}
