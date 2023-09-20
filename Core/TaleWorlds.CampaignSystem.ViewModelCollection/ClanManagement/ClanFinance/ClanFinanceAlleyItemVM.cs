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
	public class ClanFinanceAlleyItemVM : ClanFinanceIncomeItemBaseVM
	{
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

		private void tempOnSelection(ClanFinanceIncomeItemBaseVM item)
		{
			this._onSelectionT(this);
		}

		protected override void PopulateStatsList()
		{
			base.PopulateStatsList();
			base.ItemProperties.Clear();
			string text = GameTexts.FindText("str_plus_with_number", null).SetTextVariable("NUMBER", this._alleyModel.GetDailyCrimeRatingOfAlley).ToString();
			string text2 = new TextObject("{=LuC5ZZMu}{CRIMINAL_RATING} ({INCREASE}){CRIME_ICON}", null).SetTextVariable("CRIMINAL_RATING", this.Alley.Settlement.MapFaction.MainHeroCrimeRating).SetTextVariable("INCREASE", text).SetTextVariable("CRIME_ICON", "{=!}<img src=\"SPGeneral\\MapOverlay\\Settlement\\icon_crime\" extend=\"16\">")
				.ToString();
			this.IncomeTextWithVisual = new TextObject("{=ePmSvu1s}{AMOUNT}{GOLD_ICON}", null).SetTextVariable("AMOUNT", base.Income).ToString();
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=FkhJz0po}Location", null).ToString(), this.Alley.Settlement.Name.ToString(), false, null));
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=5k4dxUEJ}Troops", null).ToString(), this._alleyBehavior.GetPlayerOwnedAlleyTroopCount(this.Alley).ToString(), false, null));
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=QPoA6vvx}Income", null).ToString(), this.IncomeTextWithVisual, false, null));
			base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=r0WIRUHo}Criminal Rating", null).ToString(), text2, false, null));
			string statusText = this.GetStatusText();
			if (!string.IsNullOrEmpty(statusText))
			{
				base.ItemProperties.Add(new SelectableItemPropertyVM(new TextObject("{=DXczLzml}Status", null).ToString(), statusText, false, null));
			}
		}

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

		private ClanCardSelectionItemPropertyInfo GetSkillProperty(Hero hero, SkillObject skill)
		{
			TextObject textObject = ClanCardSelectionItemPropertyInfo.CreateLabeledValueText(skill.Name, new TextObject("{=!}" + hero.GetSkillValue(skill), null));
			return new ClanCardSelectionItemPropertyInfo(TextObject.Empty, textObject);
		}

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

		public void ExecuteManageAlley()
		{
			ClanCardSelectionInfo clanCardSelectionInfo = new ClanCardSelectionInfo(new TextObject("{=dQBArrqh}Manage Alley", null), this.GetAvailableMembers(), new Action<List<object>, Action>(this.OnMemberSelection), false);
			this._openCardSelectionPopup(clanCardSelectionInfo);
		}

		public void ExecuteBeginHeroHint()
		{
			InformationManager.ShowTooltip(typeof(Hero), new object[] { this._alleyOwner, true });
		}

		public void ExecuteEndHeroHint()
		{
			InformationManager.HideTooltip();
		}

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

		public readonly Alley Alley;

		private readonly Hero _alleyOwner;

		private readonly IAlleyCampaignBehavior _alleyBehavior;

		private readonly AlleyModel _alleyModel;

		private readonly Action<ClanFinanceAlleyItemVM> _onSelectionT;

		private readonly Action<ClanCardSelectionInfo> _openCardSelectionPopup;

		private HintViewModel _manageAlleyHint;

		private ImageIdentifierVM _ownerVisual;

		private string _incomeText;

		private string _incomeTextWithVisual;
	}
}
