using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public class ClanRoleMemberItemVM : ViewModel
	{
		public SkillEffect.PerkRole Role { get; private set; }

		public SkillObject RelevantSkill { get; private set; }

		public int RelevantSkillValue { get; private set; }

		public ClanRoleMemberItemVM(MobileParty party, SkillEffect.PerkRole role, ClanPartyMemberItemVM member, Action onRoleAssigned)
		{
			this.Role = role;
			this.Member = member;
			this._party = party;
			this._onRoleAssigned = onRoleAssigned;
			this.RelevantSkill = ClanRoleMemberItemVM.GetRelevantSkillForRole(role);
			ClanPartyMemberItemVM member2 = this.Member;
			int? num;
			if (member2 == null)
			{
				num = null;
			}
			else
			{
				Hero heroObject = member2.HeroObject;
				num = ((heroObject != null) ? new int?(heroObject.GetSkillValue(this.RelevantSkill)) : null);
			}
			this.RelevantSkillValue = num ?? (-1);
			this._skillEffects = SkillEffect.All.Where((SkillEffect x) => x.PrimaryRole != SkillEffect.PerkRole.Personal || x.SecondaryRole != SkillEffect.PerkRole.Personal);
			this._perks = PerkObject.All.Where((PerkObject x) => this.Member.HeroObject.GetPerkValue(x));
			this.IsRemoveAssigneeOption = this.Member == null;
			this.Hint = new HintViewModel(this.IsRemoveAssigneeOption ? new TextObject("{=bfWlTVjs}Remove assignee", null) : this.GetRoleHint(this.Role), null);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
		}

		public void ExecuteAssignHeroToRole()
		{
			if (this.Member == null)
			{
				switch (this.Role)
				{
				case SkillEffect.PerkRole.Surgeon:
					this._party.SetPartySurgeon(null);
					break;
				case SkillEffect.PerkRole.Engineer:
					this._party.SetPartyEngineer(null);
					break;
				case SkillEffect.PerkRole.Scout:
					this._party.SetPartyScout(null);
					break;
				case SkillEffect.PerkRole.Quartermaster:
					this._party.SetPartyQuartermaster(null);
					break;
				}
			}
			else
			{
				this.OnSetMemberAsRole(this.Role);
			}
			Action onRoleAssigned = this._onRoleAssigned;
			if (onRoleAssigned == null)
			{
				return;
			}
			onRoleAssigned();
		}

		private void OnSetMemberAsRole(SkillEffect.PerkRole role)
		{
			if (role != SkillEffect.PerkRole.None)
			{
				if (this._party.GetHeroPerkRole(this.Member.HeroObject) != role)
				{
					this._party.RemoveHeroPerkRole(this.Member.HeroObject);
					if (role == SkillEffect.PerkRole.Engineer)
					{
						this._party.SetPartyEngineer(this.Member.HeroObject);
					}
					else if (role == SkillEffect.PerkRole.Quartermaster)
					{
						this._party.SetPartyQuartermaster(this.Member.HeroObject);
					}
					else if (role == SkillEffect.PerkRole.Scout)
					{
						this._party.SetPartyScout(this.Member.HeroObject);
					}
					else if (role == SkillEffect.PerkRole.Surgeon)
					{
						this._party.SetPartySurgeon(this.Member.HeroObject);
					}
					Game game = Game.Current;
					if (game != null)
					{
						game.EventManager.TriggerEvent<ClanRoleAssignedThroughClanScreenEvent>(new ClanRoleAssignedThroughClanScreenEvent(role, this.Member.HeroObject));
					}
				}
			}
			else if (role == SkillEffect.PerkRole.None)
			{
				this._party.RemoveHeroPerkRole(this.Member.HeroObject);
			}
			Action onRoleAssigned = this._onRoleAssigned;
			if (onRoleAssigned == null)
			{
				return;
			}
			onRoleAssigned();
		}

		private TextObject GetRoleHint(SkillEffect.PerkRole role)
		{
			string text = "";
			if (this.RelevantSkillValue <= 0)
			{
				GameTexts.SetVariable("LEFT", this.RelevantSkill.Name.ToString());
				GameTexts.SetVariable("RIGHT", this.RelevantSkillValue.ToString());
				GameTexts.SetVariable("STR2", GameTexts.FindText("str_LEFT_colon_RIGHT", null).ToString());
				GameTexts.SetVariable("STR1", this.Member.Name.ToString());
				text = GameTexts.FindText("str_string_newline_string", null).ToString();
			}
			else if (!ClanRoleMemberItemVM.DoesHeroHaveEnoughSkillForRole(this.Member.HeroObject, role, this._party))
			{
				GameTexts.SetVariable("SKILL_NAME", this.RelevantSkill.Name.ToString());
				GameTexts.SetVariable("MIN_SKILL_AMOUNT", 0);
				text = GameTexts.FindText("str_character_role_disabled_tooltip", null).ToString();
			}
			else
			{
				if (!role.Equals(SkillEffect.PerkRole.None))
				{
					IEnumerable<SkillEffect> enumerable = this._skillEffects.Where((SkillEffect x) => x.PrimaryRole == role || x.SecondaryRole == role);
					IEnumerable<PerkObject> enumerable2 = this._perks.Where((PerkObject x) => x.PrimaryRole == role || x.SecondaryRole == role);
					GameTexts.SetVariable("LEFT", this.RelevantSkill.Name.ToString());
					GameTexts.SetVariable("RIGHT", this.RelevantSkillValue.ToString());
					GameTexts.SetVariable("STR2", GameTexts.FindText("str_LEFT_colon_RIGHT", null).ToString());
					GameTexts.SetVariable("STR1", this.Member.Name.ToString());
					text = GameTexts.FindText("str_string_newline_string", null).ToString();
					int num = 0;
					TextObject textObject = GameTexts.FindText("str_LEFT_colon_RIGHT", null).CopyTextObject();
					textObject.SetTextVariable("LEFT", new TextObject("{=Avy8Gua1}Perks", null));
					textObject.SetTextVariable("RIGHT", new TextObject("{=Gp2vmZGZ}{PERKS}", null));
					foreach (PerkObject perkObject in enumerable2)
					{
						if (num == 0)
						{
							GameTexts.SetVariable("PERKS", perkObject.Name.ToString());
						}
						else
						{
							GameTexts.SetVariable("RIGHT", perkObject.Name.ToString());
							GameTexts.SetVariable("LEFT", new TextObject("{=Gp2vmZGZ}{PERKS}", null).ToString());
							GameTexts.SetVariable("PERKS", GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString());
						}
						num++;
					}
					GameTexts.SetVariable("newline", "\n \n");
					if (num > 0)
					{
						GameTexts.SetVariable("STR1", text);
						GameTexts.SetVariable("STR2", textObject.ToString());
						text = GameTexts.FindText("str_string_newline_string", null).ToString();
					}
					GameTexts.SetVariable("LEFT", new TextObject("{=DKJIp6xG}Effects", null).ToString());
					string text2 = GameTexts.FindText("str_LEFT_colon", null).ToString();
					GameTexts.SetVariable("STR1", text);
					GameTexts.SetVariable("STR2", text2);
					text = GameTexts.FindText("str_string_newline_string", null).ToString();
					GameTexts.SetVariable("newline", "\n");
					using (IEnumerator<SkillEffect> enumerator2 = enumerable.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							SkillEffect skillEffect = enumerator2.Current;
							GameTexts.SetVariable("STR1", text);
							GameTexts.SetVariable("STR2", SkillHelper.GetEffectDescriptionForSkillLevel(skillEffect, this.RelevantSkillValue));
							text = GameTexts.FindText("str_string_newline_string", null).ToString();
						}
						goto IL_390;
					}
				}
				text = null;
			}
			IL_390:
			if (!string.IsNullOrEmpty(text))
			{
				return new TextObject("{=!}" + text, null);
			}
			return TextObject.Empty;
		}

		public string GetEffectsList(SkillEffect.PerkRole role)
		{
			string text = "";
			IEnumerable<SkillEffect> enumerable = this._skillEffects.Where((SkillEffect x) => x.PrimaryRole == role || x.SecondaryRole == role);
			int num = 0;
			if (this.RelevantSkillValue > 0)
			{
				foreach (SkillEffect skillEffect in enumerable)
				{
					if (num == 0)
					{
						text = SkillHelper.GetEffectDescriptionForSkillLevel(skillEffect, this.RelevantSkillValue);
					}
					else
					{
						GameTexts.SetVariable("STR1", text);
						GameTexts.SetVariable("STR2", SkillHelper.GetEffectDescriptionForSkillLevel(skillEffect, this.RelevantSkillValue));
						text = GameTexts.FindText("str_string_newline_string", null).ToString();
					}
					num++;
				}
			}
			return text;
		}

		private static SkillObject GetRelevantSkillForRole(SkillEffect.PerkRole role)
		{
			if (role == SkillEffect.PerkRole.Engineer)
			{
				return DefaultSkills.Engineering;
			}
			if (role == SkillEffect.PerkRole.Quartermaster)
			{
				return DefaultSkills.Steward;
			}
			if (role == SkillEffect.PerkRole.Scout)
			{
				return DefaultSkills.Scouting;
			}
			if (role == SkillEffect.PerkRole.Surgeon)
			{
				return DefaultSkills.Medicine;
			}
			Debug.FailedAssert(string.Format("Undefined clan role relevant skill {0}", role), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\ClanManagement\\ClanRoleMemberItemVM.cs", "GetRelevantSkillForRole", 246);
			return null;
		}

		public static bool IsHeroAssignableForRole(Hero hero, SkillEffect.PerkRole role, MobileParty party)
		{
			return ClanRoleMemberItemVM.DoesHeroHaveEnoughSkillForRole(hero, role, party) && hero.CanBeGovernorOrHavePartyRole();
		}

		private static bool DoesHeroHaveEnoughSkillForRole(Hero hero, SkillEffect.PerkRole role, MobileParty party)
		{
			if (party.GetHeroPerkRole(hero) == role)
			{
				return true;
			}
			if (role == SkillEffect.PerkRole.Engineer)
			{
				return MobilePartyHelper.IsHeroAssignableForEngineerInParty(hero, party);
			}
			if (role == SkillEffect.PerkRole.Quartermaster)
			{
				return MobilePartyHelper.IsHeroAssignableForQuartermasterInParty(hero, party);
			}
			if (role == SkillEffect.PerkRole.Scout)
			{
				return MobilePartyHelper.IsHeroAssignableForScoutInParty(hero, party);
			}
			if (role == SkillEffect.PerkRole.Surgeon)
			{
				return MobilePartyHelper.IsHeroAssignableForSurgeonInParty(hero, party);
			}
			if (role == SkillEffect.PerkRole.None)
			{
				return true;
			}
			Debug.FailedAssert(string.Format("Undefined clan role is asked if assignable {0}", role), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\ClanManagement\\ClanRoleMemberItemVM.cs", "DoesHeroHaveEnoughSkillForRole", 284);
			return false;
		}

		[DataSourceProperty]
		public ClanPartyMemberItemVM Member
		{
			get
			{
				return this._member;
			}
			set
			{
				if (value != this._member)
				{
					this._member = value;
					base.OnPropertyChangedWithValue<ClanPartyMemberItemVM>(value, "Member");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRemoveAssigneeOption
		{
			get
			{
				return this._isRemoveAssigneeOption;
			}
			set
			{
				if (value != this._isRemoveAssigneeOption)
				{
					this._isRemoveAssigneeOption = value;
					base.OnPropertyChangedWithValue(value, "IsRemoveAssigneeOption");
				}
			}
		}

		private Action _onRoleAssigned;

		private MobileParty _party;

		private readonly IEnumerable<SkillEffect> _skillEffects;

		private readonly IEnumerable<PerkObject> _perks;

		private ClanPartyMemberItemVM _member;

		private HintViewModel _hint;

		private bool _isRemoveAssigneeOption;
	}
}
