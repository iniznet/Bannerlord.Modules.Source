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
	// Token: 0x02000108 RID: 264
	public class ClanRoleMemberItemVM : ViewModel
	{
		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x06001947 RID: 6471 RVA: 0x0005B3B2 File Offset: 0x000595B2
		// (set) Token: 0x06001948 RID: 6472 RVA: 0x0005B3BA File Offset: 0x000595BA
		public SkillEffect.PerkRole Role { get; private set; }

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x06001949 RID: 6473 RVA: 0x0005B3C3 File Offset: 0x000595C3
		// (set) Token: 0x0600194A RID: 6474 RVA: 0x0005B3CB File Offset: 0x000595CB
		public SkillObject RelevantSkill { get; private set; }

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x0600194B RID: 6475 RVA: 0x0005B3D4 File Offset: 0x000595D4
		// (set) Token: 0x0600194C RID: 6476 RVA: 0x0005B3DC File Offset: 0x000595DC
		public int RelevantSkillValue { get; private set; }

		// Token: 0x0600194D RID: 6477 RVA: 0x0005B3E8 File Offset: 0x000595E8
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

		// Token: 0x0600194E RID: 6478 RVA: 0x0005B4FE File Offset: 0x000596FE
		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x0005B506 File Offset: 0x00059706
		public override void OnFinalize()
		{
			base.OnFinalize();
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x0005B510 File Offset: 0x00059710
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

		// Token: 0x06001951 RID: 6481 RVA: 0x0005B59C File Offset: 0x0005979C
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

		// Token: 0x06001952 RID: 6482 RVA: 0x0005B6A4 File Offset: 0x000598A4
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

		// Token: 0x06001953 RID: 6483 RVA: 0x0005BA7C File Offset: 0x00059C7C
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

		// Token: 0x06001954 RID: 6484 RVA: 0x0005BB44 File Offset: 0x00059D44
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

		// Token: 0x06001955 RID: 6485 RVA: 0x0005BBA0 File Offset: 0x00059DA0
		public static bool IsHeroAssignableForRole(Hero hero, SkillEffect.PerkRole role, MobileParty party)
		{
			return ClanRoleMemberItemVM.DoesHeroHaveEnoughSkillForRole(hero, role, party) && hero.CanBeGovernorOrHavePartyRole();
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x0005BBB4 File Offset: 0x00059DB4
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

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x06001957 RID: 6487 RVA: 0x0005BC29 File Offset: 0x00059E29
		// (set) Token: 0x06001958 RID: 6488 RVA: 0x0005BC31 File Offset: 0x00059E31
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

		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x06001959 RID: 6489 RVA: 0x0005BC4F File Offset: 0x00059E4F
		// (set) Token: 0x0600195A RID: 6490 RVA: 0x0005BC57 File Offset: 0x00059E57
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

		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x0600195B RID: 6491 RVA: 0x0005BC75 File Offset: 0x00059E75
		// (set) Token: 0x0600195C RID: 6492 RVA: 0x0005BC7D File Offset: 0x00059E7D
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

		// Token: 0x04000C06 RID: 3078
		private Action _onRoleAssigned;

		// Token: 0x04000C07 RID: 3079
		private MobileParty _party;

		// Token: 0x04000C08 RID: 3080
		private readonly IEnumerable<SkillEffect> _skillEffects;

		// Token: 0x04000C09 RID: 3081
		private readonly IEnumerable<PerkObject> _perks;

		// Token: 0x04000C0A RID: 3082
		private ClanPartyMemberItemVM _member;

		// Token: 0x04000C0B RID: 3083
		private HintViewModel _hint;

		// Token: 0x04000C0C RID: 3084
		private bool _isRemoveAssigneeOption;
	}
}
