using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Policies
{
	// Token: 0x02000059 RID: 89
	public class KingdomPoliciesVM : KingdomCategoryVM
	{
		// Token: 0x06000777 RID: 1911 RVA: 0x00020674 File Offset: 0x0001E874
		public KingdomPoliciesVM(Action<KingdomDecision> forceDecide)
		{
			this._forceDecide = forceDecide;
			this.ActivePolicies = new MBBindingList<KingdomPolicyItemVM>();
			this.OtherPolicies = new MBBindingList<KingdomPolicyItemVM>();
			this.DoneHint = new HintViewModel();
			this._playerKingdom = Hero.MainHero.MapFaction as Kingdom;
			this.ProposalAndDisavowalCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfPolicyProposalAndDisavowal();
			base.IsAcceptableItemSelected = false;
			this.RefreshValues();
			this.ExecuteSwitchMode();
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x000206F8 File Offset: 0x0001E8F8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PoliciesText = GameTexts.FindText("str_policies", null).ToString();
			this.ActivePoliciesText = GameTexts.FindText("str_active_policies", null).ToString();
			this.OtherPoliciesText = GameTexts.FindText("str_other_policies", null).ToString();
			this.ProposeNewPolicyText = GameTexts.FindText("str_propose_new_policy", null).ToString();
			this.DisavowPolicyText = GameTexts.FindText("str_disavow_a_policy", null).ToString();
			base.NoItemSelectedText = GameTexts.FindText("str_kingdom_no_policy_selected", null).ToString();
			base.CategoryNameText = new TextObject("{=Sls0KQVn}Elections", null).ToString();
			this.RefreshPolicyList();
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x000207AC File Offset: 0x0001E9AC
		public void SelectPolicy(PolicyObject policy)
		{
			bool flag = false;
			foreach (KingdomPolicyItemVM kingdomPolicyItemVM in this.ActivePolicies)
			{
				if (kingdomPolicyItemVM.Policy == policy)
				{
					this.OnPolicySelect(kingdomPolicyItemVM);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				foreach (KingdomPolicyItemVM kingdomPolicyItemVM2 in this.OtherPolicies)
				{
					if (kingdomPolicyItemVM2.Policy == policy)
					{
						this.OnPolicySelect(kingdomPolicyItemVM2);
						flag = true;
						break;
					}
				}
			}
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x00020854 File Offset: 0x0001EA54
		private void OnPolicySelect(KingdomPolicyItemVM policy)
		{
			if (this.CurrentSelectedPolicy != policy)
			{
				if (this.CurrentSelectedPolicy != null)
				{
					this.CurrentSelectedPolicy.IsSelected = false;
				}
				this.CurrentSelectedPolicy = policy;
				if (this.CurrentSelectedPolicy != null)
				{
					this.CurrentSelectedPolicy.IsSelected = true;
					this._currentSelectedPolicyObject = policy.Policy;
					this._currentItemsUnresolvedDecision = Clan.PlayerClan.Kingdom.UnresolvedDecisions.FirstOrDefault(delegate(KingdomDecision d)
					{
						KingdomPolicyDecision kingdomPolicyDecision;
						return (kingdomPolicyDecision = d as KingdomPolicyDecision) != null && kingdomPolicyDecision.Policy == this._currentSelectedPolicyObject && !d.ShouldBeCancelled();
					});
					if (this._currentItemsUnresolvedDecision != null)
					{
						TextObject textObject;
						this.CanProposeOrDisavowPolicy = this.GetCanProposeOrDisavowPolicyWithReason(true, out textObject);
						this.DoneHint.HintText = textObject;
						this.ProposeOrDisavowText = GameTexts.FindText("str_resolve", null).ToString();
						this.ProposeActionExplanationText = GameTexts.FindText("str_resolve_explanation", null).ToString();
						this.PolicyLikelihood = KingdomPoliciesVM.CalculateLikelihood(policy.Policy);
					}
					else
					{
						float influence = Clan.PlayerClan.Influence;
						int proposalAndDisavowalCost = this.ProposalAndDisavowalCost;
						bool isUnderMercenaryService = Clan.PlayerClan.IsUnderMercenaryService;
						TextObject textObject2;
						this.CanProposeOrDisavowPolicy = this.GetCanProposeOrDisavowPolicyWithReason(false, out textObject2);
						this.DoneHint.HintText = textObject2;
						if (this.IsPolicyActive(policy.Policy))
						{
							this.ProposeActionExplanationText = GameTexts.FindText("str_policy_propose_again_action_explanation", null).SetTextVariable("SUPPORT", KingdomPoliciesVM.CalculateLikelihood(policy.Policy)).ToString();
						}
						else
						{
							this.ProposeActionExplanationText = GameTexts.FindText("str_policy_propose_action_explanation", null).SetTextVariable("SUPPORT", KingdomPoliciesVM.CalculateLikelihood(policy.Policy)).ToString();
						}
						this.ProposeOrDisavowText = ((this._playerKingdom.Clans.Count > 1) ? GameTexts.FindText("str_policy_propose", null).ToString() : GameTexts.FindText("str_policy_enact", null).ToString());
						base.NotificationCount = Clan.PlayerClan.Kingdom.UnresolvedDecisions.Count((KingdomDecision d) => !d.ShouldBeCancelled());
						this.PolicyLikelihood = KingdomPoliciesVM.CalculateLikelihood(policy.Policy);
					}
					GameTexts.SetVariable("NUMBER", this.PolicyLikelihood);
					this.PolicyLikelihoodText = GameTexts.FindText("str_NUMBER_percent", null).ToString();
				}
				base.IsAcceptableItemSelected = this.CurrentSelectedPolicy != null;
			}
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x00020A90 File Offset: 0x0001EC90
		private bool GetCanProposeOrDisavowPolicyWithReason(bool hasUnresolvedDecision, out TextObject disabledReason)
		{
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReason = textObject;
				return false;
			}
			if (Clan.PlayerClan.IsUnderMercenaryService)
			{
				disabledReason = GameTexts.FindText("str_mercenaries_cannot_propose_policies", null);
				return false;
			}
			if (!hasUnresolvedDecision && Clan.PlayerClan.Influence < (float)this.ProposalAndDisavowalCost)
			{
				disabledReason = GameTexts.FindText("str_warning_you_dont_have_enough_influence", null);
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x00020AF4 File Offset: 0x0001ECF4
		public void RefreshPolicyList()
		{
			this.ActivePolicies.Clear();
			this.OtherPolicies.Clear();
			if (this._playerKingdom != null)
			{
				foreach (PolicyObject policyObject in this._playerKingdom.ActivePolicies)
				{
					this.ActivePolicies.Add(new KingdomPolicyItemVM(policyObject, new Action<KingdomPolicyItemVM>(this.OnPolicySelect), new Func<PolicyObject, bool>(this.IsPolicyActive)));
				}
				foreach (PolicyObject policyObject2 in PolicyObject.All.Where((PolicyObject p) => !this.IsPolicyActive(p)))
				{
					this.OtherPolicies.Add(new KingdomPolicyItemVM(policyObject2, new Action<KingdomPolicyItemVM>(this.OnPolicySelect), new Func<PolicyObject, bool>(this.IsPolicyActive)));
				}
			}
			GameTexts.SetVariable("STR", this.ActivePolicies.Count);
			this.NumOfActivePoliciesText = GameTexts.FindText("str_STR_in_parentheses", null).ToString();
			GameTexts.SetVariable("STR", this.OtherPolicies.Count);
			this.NumOfOtherPoliciesText = GameTexts.FindText("str_STR_in_parentheses", null).ToString();
			this.SetDefaultSelectedPolicy();
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x00020C54 File Offset: 0x0001EE54
		private bool IsPolicyActive(PolicyObject policy)
		{
			return this._playerKingdom.ActivePolicies.Contains(policy);
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x00020C68 File Offset: 0x0001EE68
		private void SetDefaultSelectedPolicy()
		{
			KingdomPolicyItemVM kingdomPolicyItemVM = (this.IsInProposeMode ? this.OtherPolicies.FirstOrDefault<KingdomPolicyItemVM>() : this.ActivePolicies.FirstOrDefault<KingdomPolicyItemVM>());
			this.OnPolicySelect(kingdomPolicyItemVM);
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x00020CA0 File Offset: 0x0001EEA0
		private void ExecuteSwitchMode()
		{
			this.IsInProposeMode = !this.IsInProposeMode;
			this.CurrentActiveModeText = (this.IsInProposeMode ? this.OtherPoliciesText : this.ActivePoliciesText);
			this.CurrentActionText = (this.IsInProposeMode ? this.DisavowPolicyText : this.ProposeNewPolicyText);
			this.SetDefaultSelectedPolicy();
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x00020CFC File Offset: 0x0001EEFC
		private void ExecuteProposeOrDisavow()
		{
			if (this._currentItemsUnresolvedDecision != null)
			{
				this._forceDecide(this._currentItemsUnresolvedDecision);
				return;
			}
			if (this.CanProposeOrDisavowPolicy)
			{
				KingdomDecision kingdomDecision = new KingdomPolicyDecision(Clan.PlayerClan, this._currentSelectedPolicyObject, this.IsPolicyActive(this._currentSelectedPolicyObject));
				Clan.PlayerClan.Kingdom.AddDecision(kingdomDecision, false);
				this._forceDecide(kingdomDecision);
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06000781 RID: 1921 RVA: 0x00020D65 File Offset: 0x0001EF65
		// (set) Token: 0x06000782 RID: 1922 RVA: 0x00020D6D File Offset: 0x0001EF6D
		[DataSourceProperty]
		public HintViewModel DoneHint
		{
			get
			{
				return this._doneHint;
			}
			set
			{
				if (value != this._doneHint)
				{
					this._doneHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DoneHint");
				}
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000783 RID: 1923 RVA: 0x00020D8B File Offset: 0x0001EF8B
		// (set) Token: 0x06000784 RID: 1924 RVA: 0x00020D93 File Offset: 0x0001EF93
		[DataSourceProperty]
		public MBBindingList<KingdomPolicyItemVM> ActivePolicies
		{
			get
			{
				return this._activePolicies;
			}
			set
			{
				if (value != this._activePolicies)
				{
					this._activePolicies = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomPolicyItemVM>>(value, "ActivePolicies");
				}
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000785 RID: 1925 RVA: 0x00020DB1 File Offset: 0x0001EFB1
		// (set) Token: 0x06000786 RID: 1926 RVA: 0x00020DB9 File Offset: 0x0001EFB9
		[DataSourceProperty]
		public MBBindingList<KingdomPolicyItemVM> OtherPolicies
		{
			get
			{
				return this._otherPolicies;
			}
			set
			{
				if (value != this._otherPolicies)
				{
					this._otherPolicies = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomPolicyItemVM>>(value, "OtherPolicies");
				}
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000787 RID: 1927 RVA: 0x00020DD7 File Offset: 0x0001EFD7
		// (set) Token: 0x06000788 RID: 1928 RVA: 0x00020DDF File Offset: 0x0001EFDF
		[DataSourceProperty]
		public KingdomPolicyItemVM CurrentSelectedPolicy
		{
			get
			{
				return this._currentSelectedPolicy;
			}
			set
			{
				if (value != this._currentSelectedPolicy)
				{
					this._currentSelectedPolicy = value;
					base.OnPropertyChangedWithValue<KingdomPolicyItemVM>(value, "CurrentSelectedPolicy");
				}
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000789 RID: 1929 RVA: 0x00020DFD File Offset: 0x0001EFFD
		// (set) Token: 0x0600078A RID: 1930 RVA: 0x00020E05 File Offset: 0x0001F005
		[DataSourceProperty]
		public bool CanProposeOrDisavowPolicy
		{
			get
			{
				return this._canProposeOrDisavowPolicy;
			}
			set
			{
				if (value != this._canProposeOrDisavowPolicy)
				{
					this._canProposeOrDisavowPolicy = value;
					base.OnPropertyChangedWithValue(value, "CanProposeOrDisavowPolicy");
				}
			}
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x0600078B RID: 1931 RVA: 0x00020E23 File Offset: 0x0001F023
		// (set) Token: 0x0600078C RID: 1932 RVA: 0x00020E2B File Offset: 0x0001F02B
		[DataSourceProperty]
		public int ProposalAndDisavowalCost
		{
			get
			{
				return this._proposalAndDisavowalCost;
			}
			set
			{
				if (value != this._proposalAndDisavowalCost)
				{
					this._proposalAndDisavowalCost = value;
					base.OnPropertyChangedWithValue(value, "ProposalAndDisavowalCost");
				}
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x0600078D RID: 1933 RVA: 0x00020E49 File Offset: 0x0001F049
		// (set) Token: 0x0600078E RID: 1934 RVA: 0x00020E51 File Offset: 0x0001F051
		[DataSourceProperty]
		public string NumOfActivePoliciesText
		{
			get
			{
				return this._numOfActivePoliciesText;
			}
			set
			{
				if (value != this._numOfActivePoliciesText)
				{
					this._numOfActivePoliciesText = value;
					base.OnPropertyChangedWithValue<string>(value, "NumOfActivePoliciesText");
				}
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x0600078F RID: 1935 RVA: 0x00020E74 File Offset: 0x0001F074
		// (set) Token: 0x06000790 RID: 1936 RVA: 0x00020E7C File Offset: 0x0001F07C
		[DataSourceProperty]
		public string NumOfOtherPoliciesText
		{
			get
			{
				return this._numOfOtherPoliciesText;
			}
			set
			{
				if (value != this._numOfOtherPoliciesText)
				{
					this._numOfOtherPoliciesText = value;
					base.OnPropertyChangedWithValue<string>(value, "NumOfOtherPoliciesText");
				}
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000791 RID: 1937 RVA: 0x00020E9F File Offset: 0x0001F09F
		// (set) Token: 0x06000792 RID: 1938 RVA: 0x00020EA7 File Offset: 0x0001F0A7
		[DataSourceProperty]
		public bool IsInProposeMode
		{
			get
			{
				return this._isInProposeMode;
			}
			set
			{
				if (value != this._isInProposeMode)
				{
					this._isInProposeMode = value;
					base.OnPropertyChangedWithValue(value, "IsInProposeMode");
				}
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000793 RID: 1939 RVA: 0x00020EC5 File Offset: 0x0001F0C5
		// (set) Token: 0x06000794 RID: 1940 RVA: 0x00020ECD File Offset: 0x0001F0CD
		[DataSourceProperty]
		public string DisavowPolicyText
		{
			get
			{
				return this._disavowPolicyText;
			}
			set
			{
				if (value != this._disavowPolicyText)
				{
					this._disavowPolicyText = value;
					base.OnPropertyChangedWithValue<string>(value, "DisavowPolicyText");
				}
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000795 RID: 1941 RVA: 0x00020EF0 File Offset: 0x0001F0F0
		// (set) Token: 0x06000796 RID: 1942 RVA: 0x00020EF8 File Offset: 0x0001F0F8
		[DataSourceProperty]
		public string CurrentActiveModeText
		{
			get
			{
				return this._currentActiveModeText;
			}
			set
			{
				if (value != this._currentActiveModeText)
				{
					this._currentActiveModeText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentActiveModeText");
				}
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000797 RID: 1943 RVA: 0x00020F1B File Offset: 0x0001F11B
		// (set) Token: 0x06000798 RID: 1944 RVA: 0x00020F23 File Offset: 0x0001F123
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

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000799 RID: 1945 RVA: 0x00020F46 File Offset: 0x0001F146
		// (set) Token: 0x0600079A RID: 1946 RVA: 0x00020F4E File Offset: 0x0001F14E
		[DataSourceProperty]
		public string ProposeNewPolicyText
		{
			get
			{
				return this._proposeNewPolicyText;
			}
			set
			{
				if (value != this._proposeNewPolicyText)
				{
					this._proposeNewPolicyText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProposeNewPolicyText");
				}
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x0600079B RID: 1947 RVA: 0x00020F71 File Offset: 0x0001F171
		// (set) Token: 0x0600079C RID: 1948 RVA: 0x00020F79 File Offset: 0x0001F179
		[DataSourceProperty]
		public string BackText
		{
			get
			{
				return this._backText;
			}
			set
			{
				if (value != this._backText)
				{
					this._backText = value;
					base.OnPropertyChangedWithValue<string>(value, "BackText");
				}
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x0600079D RID: 1949 RVA: 0x00020F9C File Offset: 0x0001F19C
		// (set) Token: 0x0600079E RID: 1950 RVA: 0x00020FA4 File Offset: 0x0001F1A4
		[DataSourceProperty]
		public string PoliciesText
		{
			get
			{
				return this._policiesText;
			}
			set
			{
				if (value != this._policiesText)
				{
					this._policiesText = value;
					base.OnPropertyChangedWithValue<string>(value, "PoliciesText");
				}
			}
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x0600079F RID: 1951 RVA: 0x00020FC7 File Offset: 0x0001F1C7
		// (set) Token: 0x060007A0 RID: 1952 RVA: 0x00020FCF File Offset: 0x0001F1CF
		[DataSourceProperty]
		public string ActivePoliciesText
		{
			get
			{
				return this._activePoliciesText;
			}
			set
			{
				if (value != this._activePoliciesText)
				{
					this._activePoliciesText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActivePoliciesText");
				}
			}
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x060007A1 RID: 1953 RVA: 0x00020FF2 File Offset: 0x0001F1F2
		// (set) Token: 0x060007A2 RID: 1954 RVA: 0x00020FFA File Offset: 0x0001F1FA
		[DataSourceProperty]
		public string PolicyLikelihoodText
		{
			get
			{
				return this._policyLikelihoodText;
			}
			set
			{
				if (value != this._policyLikelihoodText)
				{
					this._policyLikelihoodText = value;
					base.OnPropertyChangedWithValue<string>(value, "PolicyLikelihoodText");
				}
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x060007A3 RID: 1955 RVA: 0x0002101D File Offset: 0x0001F21D
		// (set) Token: 0x060007A4 RID: 1956 RVA: 0x00021025 File Offset: 0x0001F225
		[DataSourceProperty]
		public HintViewModel LikelihoodHint
		{
			get
			{
				return this._likelihoodHint;
			}
			set
			{
				if (value != this._likelihoodHint)
				{
					this._likelihoodHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "LikelihoodHint");
				}
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x060007A5 RID: 1957 RVA: 0x00021043 File Offset: 0x0001F243
		// (set) Token: 0x060007A6 RID: 1958 RVA: 0x0002104B File Offset: 0x0001F24B
		[DataSourceProperty]
		public int PolicyLikelihood
		{
			get
			{
				return this._policyLikelihood;
			}
			set
			{
				if (value != this._policyLikelihood)
				{
					this._policyLikelihood = value;
					base.OnPropertyChangedWithValue(value, "PolicyLikelihood");
				}
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x060007A7 RID: 1959 RVA: 0x00021069 File Offset: 0x0001F269
		// (set) Token: 0x060007A8 RID: 1960 RVA: 0x00021071 File Offset: 0x0001F271
		[DataSourceProperty]
		public string OtherPoliciesText
		{
			get
			{
				return this._otherPoliciesText;
			}
			set
			{
				if (value != this._otherPoliciesText)
				{
					this._otherPoliciesText = value;
					base.OnPropertyChangedWithValue<string>(value, "OtherPoliciesText");
				}
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x060007A9 RID: 1961 RVA: 0x00021094 File Offset: 0x0001F294
		// (set) Token: 0x060007AA RID: 1962 RVA: 0x0002109C File Offset: 0x0001F29C
		[DataSourceProperty]
		public string ProposeOrDisavowText
		{
			get
			{
				return this._proposeOrDisavowText;
			}
			set
			{
				if (value != this._proposeOrDisavowText)
				{
					this._proposeOrDisavowText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProposeOrDisavowText");
				}
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x060007AB RID: 1963 RVA: 0x000210BF File Offset: 0x0001F2BF
		// (set) Token: 0x060007AC RID: 1964 RVA: 0x000210C7 File Offset: 0x0001F2C7
		[DataSourceProperty]
		public string ProposeActionExplanationText
		{
			get
			{
				return this._proposeActionExplanationText;
			}
			set
			{
				if (value != this._proposeActionExplanationText)
				{
					this._proposeActionExplanationText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProposeActionExplanationText");
				}
			}
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x000210EA File Offset: 0x0001F2EA
		private static int CalculateLikelihood(PolicyObject policy)
		{
			return MathF.Round(new KingdomElection(new KingdomPolicyDecision(Clan.PlayerClan, policy, Clan.PlayerClan.Kingdom.ActivePolicies.Contains(policy))).GetLikelihoodForSponsor(Clan.PlayerClan) * 100f);
		}

		// Token: 0x0400034C RID: 844
		private readonly Action<KingdomDecision> _forceDecide;

		// Token: 0x0400034D RID: 845
		private readonly Kingdom _playerKingdom;

		// Token: 0x0400034E RID: 846
		private PolicyObject _currentSelectedPolicyObject;

		// Token: 0x0400034F RID: 847
		private KingdomDecision _currentItemsUnresolvedDecision;

		// Token: 0x04000350 RID: 848
		private MBBindingList<KingdomPolicyItemVM> _activePolicies;

		// Token: 0x04000351 RID: 849
		private MBBindingList<KingdomPolicyItemVM> _otherPolicies;

		// Token: 0x04000352 RID: 850
		private KingdomPolicyItemVM _currentSelectedPolicy;

		// Token: 0x04000353 RID: 851
		private bool _canProposeOrDisavowPolicy;

		// Token: 0x04000354 RID: 852
		private bool _isInProposeMode = true;

		// Token: 0x04000355 RID: 853
		private string _proposeOrDisavowText;

		// Token: 0x04000356 RID: 854
		private string _proposeActionExplanationText;

		// Token: 0x04000357 RID: 855
		private string _activePoliciesText;

		// Token: 0x04000358 RID: 856
		private string _otherPoliciesText;

		// Token: 0x04000359 RID: 857
		private string _currentActiveModeText;

		// Token: 0x0400035A RID: 858
		private string _currentActionText;

		// Token: 0x0400035B RID: 859
		private string _proposeNewPolicyText;

		// Token: 0x0400035C RID: 860
		private string _disavowPolicyText;

		// Token: 0x0400035D RID: 861
		private string _policiesText;

		// Token: 0x0400035E RID: 862
		private string _backText;

		// Token: 0x0400035F RID: 863
		private int _proposalAndDisavowalCost;

		// Token: 0x04000360 RID: 864
		private string _numOfActivePoliciesText;

		// Token: 0x04000361 RID: 865
		private string _numOfOtherPoliciesText;

		// Token: 0x04000362 RID: 866
		private HintViewModel _doneHint;

		// Token: 0x04000363 RID: 867
		private string _policyLikelihoodText;

		// Token: 0x04000364 RID: 868
		private HintViewModel _likelihoodHint;

		// Token: 0x04000365 RID: 869
		private int _policyLikelihood;
	}
}
