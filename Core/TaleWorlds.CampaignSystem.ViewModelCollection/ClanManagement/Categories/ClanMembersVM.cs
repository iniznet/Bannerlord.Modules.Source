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
	public class ClanMembersVM : ViewModel
	{
		public ClanMembersVM(Action onRefresh, Action<Hero> showHeroOnMap)
		{
			this._onRefresh = onRefresh;
			this._faction = Hero.MainHero.Clan;
			this._showHeroOnMap = showHeroOnMap;
			this._teleportationBehavior = Campaign.Current.GetCampaignBehavior<ITeleportationCampaignBehavior>();
			this.Family = new MBBindingList<ClanLordItemVM>();
			this.Companions = new MBBindingList<ClanLordItemVM>();
			MBBindingList<MBBindingList<ClanLordItemVM>> mbbindingList = new MBBindingList<MBBindingList<ClanLordItemVM>> { this.Family, this.Companions };
			this.SortController = new ClanMembersSortControllerVM(mbbindingList);
			this.RefreshMembersList();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
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
			this.SortController.RefreshValues();
		}

		public void RefreshMembersList()
		{
			this.Family.Clear();
			this.Companions.Clear();
			this.SortController.ResetAllStates();
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
			GameTexts.SetVariable("RANK", GameTexts.FindText("str_family_group", null));
			GameTexts.SetVariable("NUMBER", this.Family.Count);
			this.FamilyText = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).ToString();
			GameTexts.SetVariable("STR1", GameTexts.FindText("str_companions_group", null));
			GameTexts.SetVariable("LEFT", this._faction.Companions.Count);
			GameTexts.SetVariable("RIGHT", this._faction.CompanionLimit);
			GameTexts.SetVariable("STR2", GameTexts.FindText("str_LEFT_over_RIGHT_in_paranthesis", null));
			this.CompanionsText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			this.OnMemberSelection(this.GetDefaultMember());
		}

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

		private void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

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

		[DataSourceProperty]
		public ClanMembersSortControllerVM SortController
		{
			get
			{
				return this._sortController;
			}
			set
			{
				if (value != this._sortController)
				{
					this._sortController = value;
					base.OnPropertyChangedWithValue<ClanMembersSortControllerVM>(value, "SortController");
				}
			}
		}

		private readonly Clan _faction;

		private readonly Action _onRefresh;

		private readonly Action<Hero> _showHeroOnMap;

		private readonly ITeleportationCampaignBehavior _teleportationBehavior;

		private bool _isSelected;

		private MBBindingList<ClanLordItemVM> _companions;

		private MBBindingList<ClanLordItemVM> _family;

		private ClanLordItemVM _currentSelectedMember;

		private string _familyText;

		private string _traitsText;

		private string _companionsText;

		private string _skillsText;

		private string _nameText;

		private string _locationText;

		private bool _isAnyValidMemberSelected;

		private ClanMembersSortControllerVM _sortController;
	}
}
