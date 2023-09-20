using System;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x02000102 RID: 258
	public class ClanLordItemVM : ViewModel
	{
		// Token: 0x060017CD RID: 6093 RVA: 0x0005750C File Offset: 0x0005570C
		public ClanLordItemVM(Hero hero, ITeleportationCampaignBehavior teleportationBehavior, Action<Hero> showHeroOnMap, Action<ClanLordItemVM> onCharacterSelect, Action onRecall, Action onTalk)
		{
			this._hero = hero;
			this._onCharacterSelect = onCharacterSelect;
			this._onRecall = onRecall;
			this._onTalk = onTalk;
			this._showHeroOnMap = showHeroOnMap;
			this._teleportationBehavior = teleportationBehavior;
			CharacterCode characterCode = CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false);
			this.Visual = new ImageIdentifierVM(characterCode);
			this.Skills = new MBBindingList<EncyclopediaSkillVM>();
			this.Traits = new MBBindingList<EncyclopediaTraitItemVM>();
			this.IsFamilyMember = Hero.MainHero.Clan.Lords.Contains(this._hero);
			this.Banner_9 = new ImageIdentifierVM(BannerCode.CreateFrom(hero.ClanBanner), true);
			this.RefreshValues();
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x000575FC File Offset: 0x000557FC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._hero.Name.ToString();
			StringHelpers.SetCharacterProperties("NPC", this._hero.CharacterObject, null, false);
			this.CurrentActionText = ((this._hero != Hero.MainHero) ? CampaignUIHelper.GetHeroBehaviorText(this._hero, this._teleportationBehavior) : "");
			this.LocationText = this.CurrentActionText;
			this.PregnantHint = new HintViewModel(GameTexts.FindText("str_pregnant", null), null);
			this.UpdateProperties();
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x00057691 File Offset: 0x00055891
		public void ExecuteLocationLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x000576A4 File Offset: 0x000558A4
		public void UpdateProperties()
		{
			this.RelationToMainHeroText = "";
			this.GovernorOfText = "";
			this.Skills.Clear();
			this.Traits.Clear();
			this.IsMainHero = this._hero == Hero.MainHero;
			this.IsPregnant = this._hero.IsPregnant;
			foreach (SkillObject skillObject in (from s in TaleWorlds.CampaignSystem.Extensions.Skills.All
				group s by s.CharacterAttribute.Id).SelectMany((IGrouping<MBGUID, SkillObject> s) => s).ToList<SkillObject>())
			{
				this.Skills.Add(new EncyclopediaSkillVM(skillObject, this._hero.GetSkillValue(skillObject)));
			}
			foreach (TraitObject traitObject in CampaignUIHelper.GetHeroTraits())
			{
				if (this._hero.GetTraitLevel(traitObject) != 0)
				{
					this.Traits.Add(new EncyclopediaTraitItemVM(traitObject, this._hero));
				}
			}
			this.IsChild = FaceGen.GetMaturityTypeWithAge(this._hero.Age) <= BodyMeshMaturityType.Child;
			if (this._hero != Hero.MainHero)
			{
				this.RelationToMainHeroText = CampaignUIHelper.GetHeroRelationToHeroText(this._hero, Hero.MainHero, true).ToString();
			}
			if (this._hero.GovernorOf != null)
			{
				GameTexts.SetVariable("SETTLEMENT_NAME", this._hero.GovernorOf.Owner.Settlement.EncyclopediaLinkWithName);
				this.GovernorOfText = GameTexts.FindText("str_governor_of_label", null).ToString();
			}
			this.HeroModel = new HeroViewModel(CharacterViewModel.StanceTypes.None);
			this.HeroModel.FillFrom(this._hero, -1, false, false);
			this.Banner_9 = new ImageIdentifierVM(BannerCode.CreateFrom(this._hero.ClanBanner), true);
			this.CanShowLocationOfHero = (this._hero.IsActive || (this._hero.IsPrisoner && this._hero.CurrentSettlement != null)) && this._hero.PartyBelongedTo != MobileParty.MainParty;
			this.ShowOnMapHint = new HintViewModel(this.CanShowLocationOfHero ? this._showLocationOfHeroOnMap : TextObject.Empty, null);
			TextObject empty = TextObject.Empty;
			bool flag = this._hero.PartyBelongedTo == MobileParty.MainParty;
			this.IsTalkVisible = flag && !this.IsMainHero;
			this.IsTalkEnabled = this.IsTalkVisible && CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out empty);
			bool flag2;
			bool flag3;
			IMapPoint mapPoint;
			this.IsTeleporting = this._teleportationBehavior.GetTargetOfTeleportingHero(this._hero, out flag2, out flag3, out mapPoint);
			TextObject empty2 = TextObject.Empty;
			this.IsRecallVisible = !this.IsMainHero && !flag && !this.IsTeleporting;
			this.IsRecallEnabled = this.IsRecallVisible && CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out empty2) && FactionHelper.IsMainClanMemberAvailableForRecall(this._hero, MobileParty.MainParty, out empty2);
			this.RecallHint = new HintViewModel(this.IsRecallEnabled ? this._recallHeroToMainPartyHintText : empty2, null);
			this.TalkHint = new HintViewModel(this.IsTalkEnabled ? this._talkToHeroHintText : empty, null);
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x00057A2C File Offset: 0x00055C2C
		public void ExecuteLink()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this._hero.EncyclopediaLink);
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x00057A48 File Offset: 0x00055C48
		public void OnCharacterSelect()
		{
			this._onCharacterSelect(this);
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x00057A56 File Offset: 0x00055C56
		public virtual void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(Hero), new object[] { this._hero, true });
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x00057A7F File Offset: 0x00055C7F
		public virtual void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x00057A86 File Offset: 0x00055C86
		public Hero GetHero()
		{
			return this._hero;
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x00057A90 File Offset: 0x00055C90
		public void ExecuteRename()
		{
			InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=2lFwF07j}Change Name", null).ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action<string>(this.OnNamingHeroOver), null, false, new Func<string, Tuple<bool, string>>(CampaignUIHelper.IsStringApplicableForHeroName), "", ""), false, false);
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x00057B04 File Offset: 0x00055D04
		private void OnNamingHeroOver(string suggestedName)
		{
			if (CampaignUIHelper.IsStringApplicableForHeroName(suggestedName).Item1)
			{
				TextObject textObject = GameTexts.FindText("str_generic_character_firstname", null);
				textObject.SetTextVariable("CHARACTER_FIRSTNAME", new TextObject(suggestedName, null));
				TextObject textObject2 = GameTexts.FindText("str_generic_character_name", null);
				textObject2.SetTextVariable("CHARACTER_NAME", new TextObject(suggestedName, null));
				textObject2.SetTextVariable("CHARACTER_GENDER", this._hero.IsFemale ? 1 : 0);
				textObject.SetTextVariable("CHARACTER_GENDER", this._hero.IsFemale ? 1 : 0);
				this._hero.SetName(textObject2, textObject);
				this.Name = suggestedName;
				MobileParty partyBelongedTo = this._hero.PartyBelongedTo;
				if (((partyBelongedTo != null) ? partyBelongedTo.Army : null) != null && this._hero.PartyBelongedTo.Army.LeaderParty.PartyComponent.PartyOwner == this._hero)
				{
					this._hero.PartyBelongedTo.Army.UpdateName();
					return;
				}
			}
			else
			{
				Debug.FailedAssert("Suggested name is not acceptable. This shouldn't happen", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\ClanManagement\\ClanLordItemVM.cs", "OnNamingHeroOver", 190);
			}
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x00057C1C File Offset: 0x00055E1C
		public void ExecuteShowOnMap()
		{
			if (this._hero != null && this.CanShowLocationOfHero)
			{
				this._showHeroOnMap(this._hero);
			}
		}

		// Token: 0x060017D9 RID: 6105 RVA: 0x00057C3F File Offset: 0x00055E3F
		public void ExecuteRecall()
		{
			Action onRecall = this._onRecall;
			if (onRecall == null)
			{
				return;
			}
			onRecall();
		}

		// Token: 0x060017DA RID: 6106 RVA: 0x00057C51 File Offset: 0x00055E51
		public void ExecuteTalk()
		{
			Action onTalk = this._onTalk;
			if (onTalk == null)
			{
				return;
			}
			onTalk();
		}

		// Token: 0x060017DB RID: 6107 RVA: 0x00057C63 File Offset: 0x00055E63
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.HeroModel.OnFinalize();
		}

		// Token: 0x17000816 RID: 2070
		// (get) Token: 0x060017DC RID: 6108 RVA: 0x00057C76 File Offset: 0x00055E76
		// (set) Token: 0x060017DD RID: 6109 RVA: 0x00057C7E File Offset: 0x00055E7E
		[DataSourceProperty]
		public MBBindingList<EncyclopediaSkillVM> Skills
		{
			get
			{
				return this._skills;
			}
			set
			{
				if (value != this._skills)
				{
					this._skills = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaSkillVM>>(value, "Skills");
				}
			}
		}

		// Token: 0x17000817 RID: 2071
		// (get) Token: 0x060017DE RID: 6110 RVA: 0x00057C9C File Offset: 0x00055E9C
		// (set) Token: 0x060017DF RID: 6111 RVA: 0x00057CA4 File Offset: 0x00055EA4
		[DataSourceProperty]
		public MBBindingList<EncyclopediaTraitItemVM> Traits
		{
			get
			{
				return this._traits;
			}
			set
			{
				if (value != this._traits)
				{
					this._traits = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaTraitItemVM>>(value, "Traits");
				}
			}
		}

		// Token: 0x17000818 RID: 2072
		// (get) Token: 0x060017E0 RID: 6112 RVA: 0x00057CC2 File Offset: 0x00055EC2
		// (set) Token: 0x060017E1 RID: 6113 RVA: 0x00057CCA File Offset: 0x00055ECA
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

		// Token: 0x17000819 RID: 2073
		// (get) Token: 0x060017E2 RID: 6114 RVA: 0x00057CE8 File Offset: 0x00055EE8
		// (set) Token: 0x060017E3 RID: 6115 RVA: 0x00057CF0 File Offset: 0x00055EF0
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x060017E4 RID: 6116 RVA: 0x00057D0E File Offset: 0x00055F0E
		// (set) Token: 0x060017E5 RID: 6117 RVA: 0x00057D16 File Offset: 0x00055F16
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

		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x060017E6 RID: 6118 RVA: 0x00057D34 File Offset: 0x00055F34
		// (set) Token: 0x060017E7 RID: 6119 RVA: 0x00057D3C File Offset: 0x00055F3C
		[DataSourceProperty]
		public bool IsTeleporting
		{
			get
			{
				return this._isTeleporting;
			}
			set
			{
				if (value != this._isTeleporting)
				{
					this._isTeleporting = value;
					base.OnPropertyChangedWithValue(value, "IsTeleporting");
				}
			}
		}

		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x060017E8 RID: 6120 RVA: 0x00057D5A File Offset: 0x00055F5A
		// (set) Token: 0x060017E9 RID: 6121 RVA: 0x00057D62 File Offset: 0x00055F62
		[DataSourceProperty]
		public bool IsRecallVisible
		{
			get
			{
				return this._isRecallVisible;
			}
			set
			{
				if (value != this._isRecallVisible)
				{
					this._isRecallVisible = value;
					base.OnPropertyChangedWithValue(value, "IsRecallVisible");
				}
			}
		}

		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x060017EA RID: 6122 RVA: 0x00057D80 File Offset: 0x00055F80
		// (set) Token: 0x060017EB RID: 6123 RVA: 0x00057D88 File Offset: 0x00055F88
		[DataSourceProperty]
		public bool IsRecallEnabled
		{
			get
			{
				return this._isRecallEnabled;
			}
			set
			{
				if (value != this._isRecallEnabled)
				{
					this._isRecallEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsRecallEnabled");
				}
			}
		}

		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x060017EC RID: 6124 RVA: 0x00057DA6 File Offset: 0x00055FA6
		// (set) Token: 0x060017ED RID: 6125 RVA: 0x00057DAE File Offset: 0x00055FAE
		[DataSourceProperty]
		public bool IsTalkVisible
		{
			get
			{
				return this._isTalkVisible;
			}
			set
			{
				if (value != this._isTalkVisible)
				{
					this._isTalkVisible = value;
					base.OnPropertyChangedWithValue(value, "IsTalkVisible");
				}
			}
		}

		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x060017EE RID: 6126 RVA: 0x00057DCC File Offset: 0x00055FCC
		// (set) Token: 0x060017EF RID: 6127 RVA: 0x00057DD4 File Offset: 0x00055FD4
		[DataSourceProperty]
		public bool IsTalkEnabled
		{
			get
			{
				return this._isTalkEnabled;
			}
			set
			{
				if (value != this._isTalkEnabled)
				{
					this._isTalkEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsTalkEnabled");
				}
			}
		}

		// Token: 0x17000820 RID: 2080
		// (get) Token: 0x060017F0 RID: 6128 RVA: 0x00057DF2 File Offset: 0x00055FF2
		// (set) Token: 0x060017F1 RID: 6129 RVA: 0x00057DFA File Offset: 0x00055FFA
		[DataSourceProperty]
		public bool CanShowLocationOfHero
		{
			get
			{
				return this._canShowLocationOfHero;
			}
			set
			{
				if (value != this._canShowLocationOfHero)
				{
					this._canShowLocationOfHero = value;
					base.OnPropertyChangedWithValue(value, "CanShowLocationOfHero");
				}
			}
		}

		// Token: 0x17000821 RID: 2081
		// (get) Token: 0x060017F2 RID: 6130 RVA: 0x00057E18 File Offset: 0x00056018
		// (set) Token: 0x060017F3 RID: 6131 RVA: 0x00057E20 File Offset: 0x00056020
		[DataSourceProperty]
		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (value != this._isMainHero)
				{
					this._isMainHero = value;
					base.OnPropertyChangedWithValue(value, "IsMainHero");
				}
			}
		}

		// Token: 0x17000822 RID: 2082
		// (get) Token: 0x060017F4 RID: 6132 RVA: 0x00057E3E File Offset: 0x0005603E
		// (set) Token: 0x060017F5 RID: 6133 RVA: 0x00057E46 File Offset: 0x00056046
		[DataSourceProperty]
		public bool IsFamilyMember
		{
			get
			{
				return this._isFamilyMember;
			}
			set
			{
				if (value != this._isFamilyMember)
				{
					this._isFamilyMember = value;
					base.OnPropertyChangedWithValue(value, "IsFamilyMember");
				}
			}
		}

		// Token: 0x17000823 RID: 2083
		// (get) Token: 0x060017F6 RID: 6134 RVA: 0x00057E64 File Offset: 0x00056064
		// (set) Token: 0x060017F7 RID: 6135 RVA: 0x00057E6C File Offset: 0x0005606C
		[DataSourceProperty]
		public bool IsPregnant
		{
			get
			{
				return this._isPregnant;
			}
			set
			{
				if (value != this._isPregnant)
				{
					this._isPregnant = value;
					base.OnPropertyChangedWithValue(value, "IsPregnant");
				}
			}
		}

		// Token: 0x17000824 RID: 2084
		// (get) Token: 0x060017F8 RID: 6136 RVA: 0x00057E8A File Offset: 0x0005608A
		// (set) Token: 0x060017F9 RID: 6137 RVA: 0x00057E92 File Offset: 0x00056092
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

		// Token: 0x17000825 RID: 2085
		// (get) Token: 0x060017FA RID: 6138 RVA: 0x00057EB0 File Offset: 0x000560B0
		// (set) Token: 0x060017FB RID: 6139 RVA: 0x00057EB8 File Offset: 0x000560B8
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

		// Token: 0x17000826 RID: 2086
		// (get) Token: 0x060017FC RID: 6140 RVA: 0x00057ED6 File Offset: 0x000560D6
		// (set) Token: 0x060017FD RID: 6141 RVA: 0x00057EDE File Offset: 0x000560DE
		[DataSourceProperty]
		public string LocationText
		{
			get
			{
				return this._locationText;
			}
			set
			{
				if (value != this._locationText)
				{
					this._locationText = value;
					base.OnPropertyChangedWithValue<string>(value, "LocationText");
				}
			}
		}

		// Token: 0x17000827 RID: 2087
		// (get) Token: 0x060017FE RID: 6142 RVA: 0x00057F01 File Offset: 0x00056101
		// (set) Token: 0x060017FF RID: 6143 RVA: 0x00057F09 File Offset: 0x00056109
		[DataSourceProperty]
		public string CurrentActionText
		{
			get
			{
				return this._currentActionText;
			}
			set
			{
				if (value != this._currentActionText)
				{
					this._currentActionText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentActionText");
				}
			}
		}

		// Token: 0x17000828 RID: 2088
		// (get) Token: 0x06001800 RID: 6144 RVA: 0x00057F2C File Offset: 0x0005612C
		// (set) Token: 0x06001801 RID: 6145 RVA: 0x00057F34 File Offset: 0x00056134
		[DataSourceProperty]
		public string RelationToMainHeroText
		{
			get
			{
				return this._relationToMainHeroText;
			}
			set
			{
				if (value != this._relationToMainHeroText)
				{
					this._relationToMainHeroText = value;
					base.OnPropertyChangedWithValue<string>(value, "RelationToMainHeroText");
				}
			}
		}

		// Token: 0x17000829 RID: 2089
		// (get) Token: 0x06001802 RID: 6146 RVA: 0x00057F57 File Offset: 0x00056157
		// (set) Token: 0x06001803 RID: 6147 RVA: 0x00057F5F File Offset: 0x0005615F
		[DataSourceProperty]
		public string GovernorOfText
		{
			get
			{
				return this._governorOfText;
			}
			set
			{
				if (value != this._governorOfText)
				{
					this._governorOfText = value;
					base.OnPropertyChangedWithValue<string>(value, "GovernorOfText");
				}
			}
		}

		// Token: 0x1700082A RID: 2090
		// (get) Token: 0x06001804 RID: 6148 RVA: 0x00057F82 File Offset: 0x00056182
		// (set) Token: 0x06001805 RID: 6149 RVA: 0x00057F8A File Offset: 0x0005618A
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

		// Token: 0x1700082B RID: 2091
		// (get) Token: 0x06001806 RID: 6150 RVA: 0x00057FAD File Offset: 0x000561AD
		// (set) Token: 0x06001807 RID: 6151 RVA: 0x00057FB5 File Offset: 0x000561B5
		[DataSourceProperty]
		public HintViewModel PregnantHint
		{
			get
			{
				return this._pregnantHint;
			}
			set
			{
				if (value != this._pregnantHint)
				{
					this._pregnantHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PregnantHint");
				}
			}
		}

		// Token: 0x1700082C RID: 2092
		// (get) Token: 0x06001808 RID: 6152 RVA: 0x00057FD3 File Offset: 0x000561D3
		// (set) Token: 0x06001809 RID: 6153 RVA: 0x00057FDB File Offset: 0x000561DB
		[DataSourceProperty]
		public HintViewModel ShowOnMapHint
		{
			get
			{
				return this._showOnMapHint;
			}
			set
			{
				if (value != this._showOnMapHint)
				{
					this._showOnMapHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ShowOnMapHint");
				}
			}
		}

		// Token: 0x1700082D RID: 2093
		// (get) Token: 0x0600180A RID: 6154 RVA: 0x00057FF9 File Offset: 0x000561F9
		// (set) Token: 0x0600180B RID: 6155 RVA: 0x00058001 File Offset: 0x00056201
		[DataSourceProperty]
		public HintViewModel RecallHint
		{
			get
			{
				return this._recallHint;
			}
			set
			{
				if (value != this._recallHint)
				{
					this._recallHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RecallHint");
				}
			}
		}

		// Token: 0x1700082E RID: 2094
		// (get) Token: 0x0600180C RID: 6156 RVA: 0x0005801F File Offset: 0x0005621F
		// (set) Token: 0x0600180D RID: 6157 RVA: 0x00058027 File Offset: 0x00056227
		[DataSourceProperty]
		public HintViewModel TalkHint
		{
			get
			{
				return this._talkHint;
			}
			set
			{
				if (value != this._talkHint)
				{
					this._talkHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "TalkHint");
				}
			}
		}

		// Token: 0x04000B4C RID: 2892
		private readonly Action<ClanLordItemVM> _onCharacterSelect;

		// Token: 0x04000B4D RID: 2893
		private readonly Action _onRecall;

		// Token: 0x04000B4E RID: 2894
		private readonly Action _onTalk;

		// Token: 0x04000B4F RID: 2895
		private readonly Hero _hero;

		// Token: 0x04000B50 RID: 2896
		private readonly Action<Hero> _showHeroOnMap;

		// Token: 0x04000B51 RID: 2897
		private readonly ITeleportationCampaignBehavior _teleportationBehavior;

		// Token: 0x04000B52 RID: 2898
		private readonly TextObject _prisonerOfText = new TextObject("{=a8nRxITn}Prisoner of {PARTY_NAME}", null);

		// Token: 0x04000B53 RID: 2899
		private readonly TextObject _showLocationOfHeroOnMap = new TextObject("{=aGJYQOef}Show hero's location on map.", null);

		// Token: 0x04000B54 RID: 2900
		private readonly TextObject _recallHeroToMainPartyHintText = new TextObject("{=ANV8UV5f}Recall this member to your party.", null);

		// Token: 0x04000B55 RID: 2901
		private readonly TextObject _talkToHeroHintText = new TextObject("{=j4BdjLYp}Start a conversation with this clan member.", null);

		// Token: 0x04000B56 RID: 2902
		private ImageIdentifierVM _visual;

		// Token: 0x04000B57 RID: 2903
		private ImageIdentifierVM _banner_9;

		// Token: 0x04000B58 RID: 2904
		private bool _isSelected;

		// Token: 0x04000B59 RID: 2905
		private bool _isChild;

		// Token: 0x04000B5A RID: 2906
		private bool _isMainHero;

		// Token: 0x04000B5B RID: 2907
		private bool _isFamilyMember;

		// Token: 0x04000B5C RID: 2908
		private bool _isPregnant;

		// Token: 0x04000B5D RID: 2909
		private bool _isTeleporting;

		// Token: 0x04000B5E RID: 2910
		private bool _isRecallVisible;

		// Token: 0x04000B5F RID: 2911
		private bool _isRecallEnabled;

		// Token: 0x04000B60 RID: 2912
		private bool _isTalkVisible;

		// Token: 0x04000B61 RID: 2913
		private bool _isTalkEnabled;

		// Token: 0x04000B62 RID: 2914
		private bool _canShowLocationOfHero;

		// Token: 0x04000B63 RID: 2915
		private string _name;

		// Token: 0x04000B64 RID: 2916
		private string _locationText;

		// Token: 0x04000B65 RID: 2917
		private string _relationToMainHeroText;

		// Token: 0x04000B66 RID: 2918
		private string _governorOfText;

		// Token: 0x04000B67 RID: 2919
		private string _currentActionText;

		// Token: 0x04000B68 RID: 2920
		private HeroViewModel _heroModel;

		// Token: 0x04000B69 RID: 2921
		private MBBindingList<EncyclopediaSkillVM> _skills;

		// Token: 0x04000B6A RID: 2922
		private MBBindingList<EncyclopediaTraitItemVM> _traits;

		// Token: 0x04000B6B RID: 2923
		private HintViewModel _pregnantHint;

		// Token: 0x04000B6C RID: 2924
		private HintViewModel _showOnMapHint;

		// Token: 0x04000B6D RID: 2925
		private HintViewModel _recallHint;

		// Token: 0x04000B6E RID: 2926
		private HintViewModel _talkHint;
	}
}
