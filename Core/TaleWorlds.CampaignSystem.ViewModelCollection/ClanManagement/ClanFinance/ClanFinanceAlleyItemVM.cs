using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanFinance
{
	// Token: 0x0200010D RID: 269
	public class ClanFinanceAlleyItemVM : ClanFinanceIncomeItemBaseVM
	{
		// Token: 0x060019A7 RID: 6567 RVA: 0x0005CB24 File Offset: 0x0005AD24
		public ClanFinanceAlleyItemVM(Alley alley, Action<ClanCardSelectionInfo> openCardSelectionPopup, Action<ClanFinanceAlleyItemVM> onSelection, Action onRefresh)
			: base(null, onRefresh)
		{
			this.Alley = alley;
			this._alleyModel = Campaign.Current.Models.AlleyModel;
			this._alleyBehavior = Campaign.Current.GetCampaignBehavior<IAlleyCampaignBehavior>();
			this._onSelection = new Action<ClanFinanceIncomeItemBaseVM>(this.tempOnSelection);
			this._onSelectionT = onSelection;
			this._openCardSelectionPopup = openCardSelectionPopup;
			this.ManageAlleyHint = new HintViewModel();
			this._alleyOwner = this._alleyBehavior.GetAssignedClanMemberOfAlley(this.Alley);
			if (this._alleyOwner == null)
			{
				this._alleyOwner = this.Alley.Owner;
			}
			this.OwnerVisual = new ImageIdentifierVM(CharacterCode.CreateFrom(this._alleyOwner.CharacterObject));
			Settlement settlement = this.Alley.Settlement;
			base.ImageName = ((((settlement != null) ? settlement.SettlementComponent : null) != null) ? this.Alley.Settlement.SettlementComponent.WaitMeshName : "");
			this.RefreshValues();
		}

		// Token: 0x060019A8 RID: 6568 RVA: 0x0005CC20 File Offset: 0x0005AE20
		public override void RefreshValues()
		{
			base.RefreshValues();
			base.Name = this.Alley.Name.ToString();
			base.Location = this.Alley.Settlement.Name.ToString();
			base.Income = this._alleyModel.GetDailyIncomeOfAlley(this.Alley);
			this.IncomeText = GameTexts.FindText("str_plus_with_number", null).SetTextVariable("NUMBER", base.Income).ToString();
			this.ManageAlleyHint.HintText = new TextObject("{=dQBArrqh}Manage Alley", null);
			this.PopulateStatsList();
		}

		// Token: 0x060019A9 RID: 6569 RVA: 0x0005CCBD File Offset: 0x0005AEBD
		private void tempOnSelection(ClanFinanceIncomeItemBaseVM item)
		{
			this._onSelectionT(this);
		}

		// Token: 0x060019AA RID: 6570 RVA: 0x0005CCCC File Offset: 0x0005AECC
		protected override void PopulateStatsList()
		{
			base.PopulateStatsList();
			base.ItemProperties.Clear();
			string text = GameTexts.FindText("str_plus_with_number", null).SetTextVariable("NUMBER", this._alleyModel.GetDailyCrimeRatingOfAlley).ToString();
			string text2 = new TextObject("{=LuC5ZZMu}{CRIMINAL_RATING} ({INCREASE}){CRIME_ICON}", null).SetTextVariable("CRIMINAL_RATING", this.Alley.Settlement.MapFaction.MainHeroCrimeRating).SetTextVariable("INCREASE", text).SetTextVariable("CRIME_ICON", "{=!}<img src=\"SPGeneral\\MapOverlay\\Settlement\\icon_crime\" extend=\"16\">")
				.ToString();
			this.IncomeTextWithVisual = new TextObject("{=ePmSvu1s}{AMOUNT}{GOLD_ICON}", null).SetTextVariable("AMOUNT", base.Income).ToString();
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=FkhJz0po}Location", null).ToString(), this.Alley.Settlement.Name.ToString(), null));
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=5k4dxUEJ}Troops", null).ToString(), this._alleyBehavior.GetPlayerOwnedAlleyTroopCount(this.Alley).ToString(), null));
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=QPoA6vvx}Income", null).ToString(), this.IncomeTextWithVisual, null));
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=r0WIRUHo}Criminal Rating", null).ToString(), text2, null));
			string statusText = this.GetStatusText();
			if (!string.IsNullOrEmpty(statusText))
			{
				base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=DXczLzml}Status", null).ToString(), statusText, null));
			}
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x0005CE6C File Offset: 0x0005B06C
		private string GetStatusText()
		{
			string text = string.Empty;
			List<ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail>> clanMembersAndAvailabilityDetailsForLeadingAnAlley = this._alleyModel.GetClanMembersAndAvailabilityDetailsForLeadingAnAlley(this.Alley);
			Hero assignedClanMemberOfAlley = this._alleyBehavior.GetAssignedClanMemberOfAlley(this.Alley);
			if (this._alleyBehavior.GetIsAlleyUnderAttack(this.Alley))
			{
				TextObject textObject = new TextObject("{=q1DVNQS7}Under Attack! ({RESPONSE_TIME} {?RESPONSE_TIME>1}days{?}day{\\?} left.)", null);
				textObject.SetTextVariable("RESPONSE_TIME", this._alleyBehavior.GetResponseTimeLeftForAttackInDays(this.Alley));
				text = textObject.ToString();
			}
			else if (assignedClanMemberOfAlley.IsDead)
			{
				text = new TextObject("{=KjuxDQfn}Alley leader is dead.", null).ToString();
			}
			else if (assignedClanMemberOfAlley.IsTraveling)
			{
				TextObject textObject2 = new TextObject("{=SFB2uYHa}Alley leader is traveling to the alley. ({LEFT_TIME} {?LEFT_TIME>1}hours{?}hour{\\?} left.)", null);
				textObject2.SetTextVariable("LEFT_TIME", MathF.Ceiling(TeleportationHelper.GetHoursLeftForTeleportingHeroToReachItsDestination(assignedClanMemberOfAlley)));
				text = textObject2.ToString();
			}
			else
			{
				for (int i = 0; i < clanMembersAndAvailabilityDetailsForLeadingAnAlley.Count; i++)
				{
					if (clanMembersAndAvailabilityDetailsForLeadingAnAlley[i].Item1 == Hero.MainHero && clanMembersAndAvailabilityDetailsForLeadingAnAlley[i].Item2 != DefaultAlleyModel.AlleyMemberAvailabilityDetail.Available)
					{
						text = new TextObject("{=NHZ1jNIF}Below Requirements", null).ToString();
						break;
					}
				}
			}
			return text;
		}

		// Token: 0x060019AC RID: 6572 RVA: 0x0005CF7C File Offset: 0x0005B17C
		private ClanCardSelectionItemPropertyInfo GetSkillProperty(Hero hero, SkillObject skill)
		{
			TextObject textObject = ClanCardSelectionItemPropertyInfo.CreateLabeledValueText(skill.Name, new TextObject("{=!}" + hero.GetSkillValue(skill), null));
			return new ClanCardSelectionItemPropertyInfo(TextObject.Empty, textObject);
		}

		// Token: 0x060019AD RID: 6573 RVA: 0x0005CFBC File Offset: 0x0005B1BC
		private IEnumerable<ClanCardSelectionItemPropertyInfo> GetHeroProperties(Hero hero, Alley alley, DefaultAlleyModel.AlleyMemberAvailabilityDetail detail)
		{
			if (detail == DefaultAlleyModel.AlleyMemberAvailabilityDetail.AvailableWithDelay)
			{
				string partyDistanceByTimeText = CampaignUIHelper.GetPartyDistanceByTimeText(Campaign.Current.Models.DelayedTeleportationModel.GetTeleportationDelayAsHours(hero, alley.Settlement.Party).ResultNumber, Campaign.Current.Models.DelayedTeleportationModel.DefaultTeleportationSpeed);
				yield return new ClanCardSelectionItemPropertyInfo(new TextObject("{=!}" + partyDistanceByTimeText, null));
			}
			yield return new ClanCardSelectionItemPropertyInfo(new TextObject("{=bz7Glmsm}Skills", null), TextObject.Empty);
			yield return this.GetSkillProperty(hero, DefaultSkills.Tactics);
			yield return this.GetSkillProperty(hero, DefaultSkills.Leadership);
			yield return this.GetSkillProperty(hero, DefaultSkills.Steward);
			yield return this.GetSkillProperty(hero, DefaultSkills.Roguery);
			yield break;
		}

		// Token: 0x060019AE RID: 6574 RVA: 0x0005CFE1 File Offset: 0x0005B1E1
		private IEnumerable<ClanCardSelectionItemInfo> GetAvailableMembers()
		{
			yield return new ClanCardSelectionItemInfo(new TextObject("{=W3hmFcfv}Abandon Alley", null), false, TextObject.Empty, TextObject.Empty);
			List<ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail>> availabilityDetails = this._alleyModel.GetClanMembersAndAvailabilityDetailsForLeadingAnAlley(this.Alley);
			using (List<Hero>.Enumerator enumerator = Clan.PlayerClan.Heroes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Hero member = enumerator.Current;
					ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail> valueTuple = availabilityDetails.FirstOrDefault((ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail> x) => x.Item1 == member);
					if (valueTuple.Item1 != null)
					{
						CharacterCode characterCode = CharacterCode.CreateFrom(member.CharacterObject);
						bool flag = valueTuple.Item2 != DefaultAlleyModel.AlleyMemberAvailabilityDetail.Available && valueTuple.Item2 != DefaultAlleyModel.AlleyMemberAvailabilityDetail.AvailableWithDelay;
						yield return new ClanCardSelectionItemInfo(member, member.Name, new ImageIdentifier(characterCode), CardSelectionItemSpriteType.None, null, null, this.GetHeroProperties(member, this.Alley, valueTuple.Item2), flag, this._alleyModel.GetDisabledReasonTextForHero(member, this.Alley, valueTuple.Item2), null);
					}
				}
			}
			List<Hero>.Enumerator enumerator = default(List<Hero>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x0005CFF4 File Offset: 0x0005B1F4
		private void OnMemberSelection(List<object> members, Action closePopup)
		{
			if (members.Count > 0)
			{
				Hero hero = members[0] as Hero;
				if (hero != null)
				{
					this._alleyBehavior.ChangeAlleyMember(this.Alley, hero);
					Action onRefresh = this._onRefresh;
					if (onRefresh != null)
					{
						onRefresh();
					}
					Action closePopup2 = closePopup;
					if (closePopup2 == null)
					{
						return;
					}
					closePopup2();
					return;
				}
				else
				{
					InformationManager.ShowInquiry(new InquiryData(new TextObject("{=W3hmFcfv}Abandon Alley", null).ToString(), new TextObject("{=pBVbKYwo}You will lose the ownership of the alley and the troops in it. Are you sure?", null).ToString(), true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), delegate
					{
						this._alleyBehavior.AbandonAlleyFromClanMenu(this.Alley);
						Action onRefresh2 = this._onRefresh;
						if (onRefresh2 != null)
						{
							onRefresh2();
						}
						Action closePopup3 = closePopup;
						if (closePopup3 == null)
						{
							return;
						}
						closePopup3();
					}, null, "", 0f, null, null, null), false, false);
				}
			}
		}

		// Token: 0x060019B0 RID: 6576 RVA: 0x0005D0D0 File Offset: 0x0005B2D0
		public void ExecuteManageAlley()
		{
			ClanCardSelectionInfo clanCardSelectionInfo = new ClanCardSelectionInfo(new TextObject("{=dQBArrqh}Manage Alley", null), this.GetAvailableMembers(), new Action<List<object>, Action>(this.OnMemberSelection), false);
			this._openCardSelectionPopup(clanCardSelectionInfo);
		}

		// Token: 0x060019B1 RID: 6577 RVA: 0x0005D10E File Offset: 0x0005B30E
		public void ExecuteBeginHeroHint()
		{
			InformationManager.ShowTooltip(typeof(Hero), new object[] { this._alleyOwner, true });
		}

		// Token: 0x060019B2 RID: 6578 RVA: 0x0005D137 File Offset: 0x0005B337
		public void ExecuteEndHeroHint()
		{
			InformationManager.HideTooltip();
		}

		// Token: 0x170008CD RID: 2253
		// (get) Token: 0x060019B3 RID: 6579 RVA: 0x0005D13E File Offset: 0x0005B33E
		// (set) Token: 0x060019B4 RID: 6580 RVA: 0x0005D146 File Offset: 0x0005B346
		[DataSourceProperty]
		public HintViewModel ManageAlleyHint
		{
			get
			{
				return this._manageAlleyHint;
			}
			set
			{
				if (value != this._manageAlleyHint)
				{
					this._manageAlleyHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ManageAlleyHint");
				}
			}
		}

		// Token: 0x170008CE RID: 2254
		// (get) Token: 0x060019B5 RID: 6581 RVA: 0x0005D164 File Offset: 0x0005B364
		// (set) Token: 0x060019B6 RID: 6582 RVA: 0x0005D16C File Offset: 0x0005B36C
		[DataSourceProperty]
		public ImageIdentifierVM OwnerVisual
		{
			get
			{
				return this._ownerVisual;
			}
			set
			{
				if (value != this._ownerVisual)
				{
					this._ownerVisual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "OwnerVisual");
				}
			}
		}

		// Token: 0x170008CF RID: 2255
		// (get) Token: 0x060019B7 RID: 6583 RVA: 0x0005D18A File Offset: 0x0005B38A
		// (set) Token: 0x060019B8 RID: 6584 RVA: 0x0005D192 File Offset: 0x0005B392
		[DataSourceProperty]
		public string IncomeText
		{
			get
			{
				return this._incomeText;
			}
			set
			{
				if (value != this._incomeText)
				{
					this._incomeText = value;
					base.OnPropertyChangedWithValue<string>(value, "IncomeText");
				}
			}
		}

		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x060019B9 RID: 6585 RVA: 0x0005D1B5 File Offset: 0x0005B3B5
		// (set) Token: 0x060019BA RID: 6586 RVA: 0x0005D1BD File Offset: 0x0005B3BD
		[DataSourceProperty]
		public string IncomeTextWithVisual
		{
			get
			{
				return this._incomeTextWithVisual;
			}
			set
			{
				if (value != this._incomeTextWithVisual)
				{
					this._incomeTextWithVisual = value;
					base.OnPropertyChangedWithValue<string>(value, "IncomeTextWithVisual");
				}
			}
		}

		// Token: 0x04000C2E RID: 3118
		public readonly Alley Alley;

		// Token: 0x04000C2F RID: 3119
		private readonly Hero _alleyOwner;

		// Token: 0x04000C30 RID: 3120
		private readonly IAlleyCampaignBehavior _alleyBehavior;

		// Token: 0x04000C31 RID: 3121
		private readonly AlleyModel _alleyModel;

		// Token: 0x04000C32 RID: 3122
		private readonly Action<ClanFinanceAlleyItemVM> _onSelectionT;

		// Token: 0x04000C33 RID: 3123
		private readonly Action<ClanCardSelectionInfo> _openCardSelectionPopup;

		// Token: 0x04000C34 RID: 3124
		private HintViewModel _manageAlleyHint;

		// Token: 0x04000C35 RID: 3125
		private ImageIdentifierVM _ownerVisual;

		// Token: 0x04000C36 RID: 3126
		private string _incomeText;

		// Token: 0x04000C37 RID: 3127
		private string _incomeTextWithVisual;
	}
}
