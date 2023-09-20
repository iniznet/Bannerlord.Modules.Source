using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories
{
	// Token: 0x02000114 RID: 276
	public class ClanMembersVM : ViewModel
	{
		// Token: 0x06001A4D RID: 6733 RVA: 0x0005F1F8 File Offset: 0x0005D3F8
		public ClanMembersVM(Action onRefresh, Action<Hero> showHeroOnMap)
		{
			this._onRefresh = onRefresh;
			this._faction = Hero.MainHero.Clan;
			this._showHeroOnMap = showHeroOnMap;
			this._teleportationBehavior = Campaign.Current.GetCampaignBehavior<ITeleportationCampaignBehavior>();
			this.Family = new MBBindingList<ClanLordItemVM>();
			this.Companions = new MBBindingList<ClanLordItemVM>();
			this.RefreshMembersList();
			this.RefreshValues();
		}

		// Token: 0x06001A4E RID: 6734 RVA: 0x0005F25C File Offset: 0x0005D45C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.FamilyText = GameTexts.FindText("str_family_group", null).ToString();
			this.TraitsText = GameTexts.FindText("str_traits_group", null).ToString();
			this.SkillsText = GameTexts.FindText("str_skills", null).ToString();
			this.NameText = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.LocationText = GameTexts.FindText("str_tooltip_label_location", null).ToString();
			this.Family.ApplyActionOnAllItems(delegate(ClanLordItemVM x)
			{
				x.RefreshValues();
			});
			this.Companions.ApplyActionOnAllItems(delegate(ClanLordItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06001A4F RID: 6735 RVA: 0x0005F334 File Offset: 0x0005D534
		public void RefreshMembersList()
		{
			this.Family.Clear();
			this.Companions.Clear();
			List<Hero> list = new List<Hero>();
			foreach (Hero hero in this._faction.Lords)
			{
				if (hero.IsAlive && !hero.IsDisabled)
				{
					if (hero == Hero.MainHero)
					{
						list.Insert(0, hero);
					}
					else
					{
						list.Add(hero);
					}
				}
			}
			IEnumerable<Hero> enumerable = this._faction.Companions.Where((Hero m) => m.IsPlayerCompanion);
			foreach (Hero hero2 in list)
			{
				this.Family.Add(new ClanLordItemVM(hero2, this._teleportationBehavior, this._showHeroOnMap, new Action<ClanLordItemVM>(this.OnMemberSelection), new Action(this.OnRequestRecall), new Action(this.OnTalkWithMember)));
			}
			foreach (Hero hero3 in enumerable)
			{
				this.Companions.Add(new ClanLordItemVM(hero3, this._teleportationBehavior, this._showHeroOnMap, new Action<ClanLordItemVM>(this.OnMemberSelection), new Action(this.OnRequestRecall), new Action(this.OnTalkWithMember)));
			}
			GameTexts.SetVariable("COMPANION_COUNT", this._faction.Companions.Count);
			GameTexts.SetVariable("COMPANION_LIMIT", this._faction.CompanionLimit);
			this.CompanionsText = GameTexts.FindText("str_companions_group", null).ToString();
			this.OnMemberSelection(this.GetDefaultMember());
		}

		// Token: 0x06001A50 RID: 6736 RVA: 0x0005F53C File Offset: 0x0005D73C
		private ClanLordItemVM GetDefaultMember()
		{
			if (this.Family.Count > 0)
			{
				return this.Family[0];
			}
			if (this.Companions.Count <= 0)
			{
				return null;
			}
			return this.Companions[0];
		}

		// Token: 0x06001A51 RID: 6737 RVA: 0x0005F578 File Offset: 0x0005D778
		public void SelectMember(Hero hero)
		{
			bool flag = false;
			foreach (ClanLordItemVM clanLordItemVM in this.Family)
			{
				if (clanLordItemVM.GetHero() == hero)
				{
					this.OnMemberSelection(clanLordItemVM);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				foreach (ClanLordItemVM clanLordItemVM2 in this.Companions)
				{
					if (clanLordItemVM2.GetHero() == hero)
					{
						this.OnMemberSelection(clanLordItemVM2);
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				foreach (ClanLordItemVM clanLordItemVM3 in this.Family)
				{
					if (clanLordItemVM3.GetHero() == Hero.MainHero)
					{
						this.OnMemberSelection(clanLordItemVM3);
						flag = true;
						break;
					}
				}
			}
		}

		// Token: 0x06001A52 RID: 6738 RVA: 0x0005F674 File Offset: 0x0005D874
		private void OnMemberSelection(ClanLordItemVM member)
		{
			if (this.CurrentSelectedMember != null)
			{
				this.CurrentSelectedMember.IsSelected = false;
			}
			this.CurrentSelectedMember = member;
			if (member != null)
			{
				member.IsSelected = true;
			}
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x0005F69C File Offset: 0x0005D89C
		private void OnRequestRecall()
		{
			ClanLordItemVM currentSelectedMember = this.CurrentSelectedMember;
			Hero hero = ((currentSelectedMember != null) ? currentSelectedMember.GetHero() : null);
			if (hero != null)
			{
				int num = (int)Math.Ceiling((double)Campaign.Current.Models.DelayedTeleportationModel.GetTeleportationDelayAsHours(hero, PartyBase.MainParty).ResultNumber);
				MBTextManager.SetTextVariable("TRAVEL_DURATION", CampaignUIHelper.GetHoursAndDaysTextFromHourValue(num).ToString(), false);
				MBTextManager.SetTextVariable("HERO_NAME", hero.Name.ToString(), false);
				object obj = GameTexts.FindText("str_recall_member", null);
				TextObject textObject = GameTexts.FindText("str_recall_clan_member_inquiry", null);
				InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.OnConfirmRecall), null, "", 0f, null, null, null), false, false);
			}
		}

		// Token: 0x06001A54 RID: 6740 RVA: 0x0005F782 File Offset: 0x0005D982
		private void OnConfirmRecall()
		{
			TeleportHeroAction.ApplyDelayedTeleportToParty(this.CurrentSelectedMember.GetHero(), MobileParty.MainParty);
			Action onRefresh = this._onRefresh;
			if (onRefresh == null)
			{
				return;
			}
			onRefresh();
		}

		// Token: 0x06001A55 RID: 6741 RVA: 0x0005F7A9 File Offset: 0x0005D9A9
		private void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x06001A56 RID: 6742 RVA: 0x0005F7BC File Offset: 0x0005D9BC
		private void OnTalkWithMember()
		{
			ClanLordItemVM currentSelectedMember = this.CurrentSelectedMember;
			bool flag;
			if (currentSelectedMember == null)
			{
				flag = null != null;
			}
			else
			{
				Hero hero = currentSelectedMember.GetHero();
				flag = ((hero != null) ? hero.CharacterObject : null) != null;
			}
			if (flag)
			{
				CharacterObject characterObject = this.CurrentSelectedMember.GetHero().CharacterObject;
				LocationComplex locationComplex = LocationComplex.Current;
				Location location = ((locationComplex != null) ? locationComplex.GetLocationOfCharacter(LocationComplex.Current.GetFirstLocationCharacterOfCharacter(characterObject)) : null);
				if (location == null)
				{
					CampaignMission.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false, false, false), new ConversationCharacterData(characterObject, PartyBase.MainParty, false, false, false, false, false, false), "", "");
					return;
				}
				PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(location, null, characterObject, null);
			}
		}

		// Token: 0x06001A57 RID: 6743 RVA: 0x0005F860 File Offset: 0x0005DA60
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Family.ApplyActionOnAllItems(delegate(ClanLordItemVM f)
			{
				f.OnFinalize();
			});
			this.Companions.ApplyActionOnAllItems(delegate(ClanLordItemVM f)
			{
				f.OnFinalize();
			});
		}

		// Token: 0x17000901 RID: 2305
		// (get) Token: 0x06001A58 RID: 6744 RVA: 0x0005F8C7 File Offset: 0x0005DAC7
		// (set) Token: 0x06001A59 RID: 6745 RVA: 0x0005F8CF File Offset: 0x0005DACF
		[DataSourceProperty]
		public bool IsAnyValidMemberSelected
		{
			get
			{
				return this._isAnyValidMemberSelected;
			}
			set
			{
				if (value != this._isAnyValidMemberSelected)
				{
					this._isAnyValidMemberSelected = value;
					base.OnPropertyChangedWithValue(value, "IsAnyValidMemberSelected");
				}
			}
		}

		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x06001A5A RID: 6746 RVA: 0x0005F8ED File Offset: 0x0005DAED
		// (set) Token: 0x06001A5B RID: 6747 RVA: 0x0005F8F5 File Offset: 0x0005DAF5
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

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x06001A5C RID: 6748 RVA: 0x0005F913 File Offset: 0x0005DB13
		// (set) Token: 0x06001A5D RID: 6749 RVA: 0x0005F91B File Offset: 0x0005DB1B
		[DataSourceProperty]
		public string FamilyText
		{
			get
			{
				return this._familyText;
			}
			set
			{
				if (value != this._familyText)
				{
					this._familyText = value;
					base.OnPropertyChangedWithValue<string>(value, "FamilyText");
				}
			}
		}

		// Token: 0x17000904 RID: 2308
		// (get) Token: 0x06001A5E RID: 6750 RVA: 0x0005F93E File Offset: 0x0005DB3E
		// (set) Token: 0x06001A5F RID: 6751 RVA: 0x0005F946 File Offset: 0x0005DB46
		[DataSourceProperty]
		public string TraitsText
		{
			get
			{
				return this._traitsText;
			}
			set
			{
				if (value != this._traitsText)
				{
					this._traitsText = value;
					base.OnPropertyChangedWithValue<string>(value, "TraitsText");
				}
			}
		}

		// Token: 0x17000905 RID: 2309
		// (get) Token: 0x06001A60 RID: 6752 RVA: 0x0005F969 File Offset: 0x0005DB69
		// (set) Token: 0x06001A61 RID: 6753 RVA: 0x0005F971 File Offset: 0x0005DB71
		[DataSourceProperty]
		public string SkillsText
		{
			get
			{
				return this._skillsText;
			}
			set
			{
				if (value != this._skillsText)
				{
					this._skillsText = value;
					base.OnPropertyChangedWithValue<string>(value, "SkillsText");
				}
			}
		}

		// Token: 0x17000906 RID: 2310
		// (get) Token: 0x06001A62 RID: 6754 RVA: 0x0005F994 File Offset: 0x0005DB94
		// (set) Token: 0x06001A63 RID: 6755 RVA: 0x0005F99C File Offset: 0x0005DB9C
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

		// Token: 0x17000907 RID: 2311
		// (get) Token: 0x06001A64 RID: 6756 RVA: 0x0005F9BF File Offset: 0x0005DBBF
		// (set) Token: 0x06001A65 RID: 6757 RVA: 0x0005F9C7 File Offset: 0x0005DBC7
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

		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x06001A66 RID: 6758 RVA: 0x0005F9EA File Offset: 0x0005DBEA
		// (set) Token: 0x06001A67 RID: 6759 RVA: 0x0005F9F2 File Offset: 0x0005DBF2
		[DataSourceProperty]
		public string CompanionsText
		{
			get
			{
				return this._companionsText;
			}
			set
			{
				if (value != this._companionsText)
				{
					this._companionsText = value;
					base.OnPropertyChangedWithValue<string>(value, "CompanionsText");
				}
			}
		}

		// Token: 0x17000909 RID: 2313
		// (get) Token: 0x06001A68 RID: 6760 RVA: 0x0005FA15 File Offset: 0x0005DC15
		// (set) Token: 0x06001A69 RID: 6761 RVA: 0x0005FA1D File Offset: 0x0005DC1D
		[DataSourceProperty]
		public MBBindingList<ClanLordItemVM> Companions
		{
			get
			{
				return this._companions;
			}
			set
			{
				if (value != this._companions)
				{
					this._companions = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanLordItemVM>>(value, "Companions");
				}
			}
		}

		// Token: 0x1700090A RID: 2314
		// (get) Token: 0x06001A6A RID: 6762 RVA: 0x0005FA3B File Offset: 0x0005DC3B
		// (set) Token: 0x06001A6B RID: 6763 RVA: 0x0005FA43 File Offset: 0x0005DC43
		[DataSourceProperty]
		public MBBindingList<ClanLordItemVM> Family
		{
			get
			{
				return this._family;
			}
			set
			{
				if (value != this._family)
				{
					this._family = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanLordItemVM>>(value, "Family");
				}
			}
		}

		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x06001A6C RID: 6764 RVA: 0x0005FA61 File Offset: 0x0005DC61
		// (set) Token: 0x06001A6D RID: 6765 RVA: 0x0005FA69 File Offset: 0x0005DC69
		[DataSourceProperty]
		public ClanLordItemVM CurrentSelectedMember
		{
			get
			{
				return this._currentSelectedMember;
			}
			set
			{
				if (value != this._currentSelectedMember)
				{
					this._currentSelectedMember = value;
					base.OnPropertyChangedWithValue<ClanLordItemVM>(value, "CurrentSelectedMember");
					this.IsAnyValidMemberSelected = value != null;
				}
			}
		}

		// Token: 0x04000C74 RID: 3188
		private readonly Clan _faction;

		// Token: 0x04000C75 RID: 3189
		private readonly Action _onRefresh;

		// Token: 0x04000C76 RID: 3190
		private readonly Action<Hero> _showHeroOnMap;

		// Token: 0x04000C77 RID: 3191
		private readonly ITeleportationCampaignBehavior _teleportationBehavior;

		// Token: 0x04000C78 RID: 3192
		private bool _isSelected;

		// Token: 0x04000C79 RID: 3193
		private MBBindingList<ClanLordItemVM> _companions;

		// Token: 0x04000C7A RID: 3194
		private MBBindingList<ClanLordItemVM> _family;

		// Token: 0x04000C7B RID: 3195
		private ClanLordItemVM _currentSelectedMember;

		// Token: 0x04000C7C RID: 3196
		private string _familyText;

		// Token: 0x04000C7D RID: 3197
		private string _traitsText;

		// Token: 0x04000C7E RID: 3198
		private string _companionsText;

		// Token: 0x04000C7F RID: 3199
		private string _skillsText;

		// Token: 0x04000C80 RID: 3200
		private string _nameText;

		// Token: 0x04000C81 RID: 3201
		private string _locationText;

		// Token: 0x04000C82 RID: 3202
		private bool _isAnyValidMemberSelected;
	}
}
