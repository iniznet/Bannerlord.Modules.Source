using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Policies
{
	// Token: 0x0200005A RID: 90
	public class KingdomPolicyItemVM : KingdomItemVM
	{
		// Token: 0x060007B0 RID: 1968 RVA: 0x00021164 File Offset: 0x0001F364
		public KingdomPolicyItemVM(PolicyObject policy, Action<KingdomPolicyItemVM> onSelect, Func<PolicyObject, bool> getIsPolicyActive)
		{
			this._onSelect = onSelect;
			this._policy = policy;
			this._getIsPolicyActive = getIsPolicyActive;
			this.Name = policy.Name.ToString();
			this.Explanation = policy.Description.ToString();
			this.LikelihoodHint = new HintViewModel();
			this.PolicyEffectList = new MBBindingList<StringItemWithHintVM>();
			foreach (string text in policy.SecondaryEffects.ToString().Split(new char[] { '\n' }))
			{
				this.PolicyEffectList.Add(new StringItemWithHintVM(text, TextObject.Empty));
			}
			this.RefreshValues();
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x00021210 File Offset: 0x0001F410
		public override void RefreshValues()
		{
			base.RefreshValues();
			Func<PolicyObject, bool> getIsPolicyActive = this._getIsPolicyActive;
			this.PolicyAcceptanceText = ((getIsPolicyActive != null && getIsPolicyActive(this.Policy)) ? GameTexts.FindText("str_policy_support_for_abolishing", null).ToString() : GameTexts.FindText("str_policy_support_for_enacting", null).ToString());
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x00021268 File Offset: 0x0001F468
		private void DeterminePolicyLikelihood()
		{
			float likelihoodForSponsor = new KingdomElection(new KingdomPolicyDecision(Clan.PlayerClan, this._policy, false)).GetLikelihoodForSponsor(Clan.PlayerClan);
			this.PolicyLikelihood = MathF.Round(likelihoodForSponsor * 100f);
			GameTexts.SetVariable("NUMBER", this.PolicyLikelihood);
			this.PolicyLikelihoodText = GameTexts.FindText("str_NUMBER_percent", null).ToString();
		}

		// Token: 0x060007B3 RID: 1971 RVA: 0x000212CE File Offset: 0x0001F4CE
		protected override void OnSelect()
		{
			base.OnSelect();
			this._onSelect(this);
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x060007B4 RID: 1972 RVA: 0x000212E2 File Offset: 0x0001F4E2
		// (set) Token: 0x060007B5 RID: 1973 RVA: 0x000212EA File Offset: 0x0001F4EA
		[DataSourceProperty]
		public string PolicyAcceptanceText
		{
			get
			{
				return this._policyAcceptanceText;
			}
			set
			{
				if (value != this._policyAcceptanceText)
				{
					this._policyAcceptanceText = value;
					base.OnPropertyChangedWithValue<string>(value, "PolicyAcceptanceText");
				}
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x060007B6 RID: 1974 RVA: 0x0002130D File Offset: 0x0001F50D
		// (set) Token: 0x060007B7 RID: 1975 RVA: 0x00021315 File Offset: 0x0001F515
		[DataSourceProperty]
		public MBBindingList<StringItemWithHintVM> PolicyEffectList
		{
			get
			{
				return this._policyEffectList;
			}
			set
			{
				if (value != this._policyEffectList)
				{
					this._policyEffectList = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringItemWithHintVM>>(value, "PolicyEffectList");
				}
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x060007B8 RID: 1976 RVA: 0x00021333 File Offset: 0x0001F533
		// (set) Token: 0x060007B9 RID: 1977 RVA: 0x0002133B File Offset: 0x0001F53B
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

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x060007BA RID: 1978 RVA: 0x0002135E File Offset: 0x0001F55E
		// (set) Token: 0x060007BB RID: 1979 RVA: 0x00021366 File Offset: 0x0001F566
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

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x060007BC RID: 1980 RVA: 0x00021384 File Offset: 0x0001F584
		// (set) Token: 0x060007BD RID: 1981 RVA: 0x0002138C File Offset: 0x0001F58C
		[DataSourceProperty]
		public PolicyObject Policy
		{
			get
			{
				return this._policy;
			}
			set
			{
				if (value != this._policy)
				{
					this._policy = value;
					base.OnPropertyChangedWithValue<PolicyObject>(value, "Policy");
				}
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x060007BE RID: 1982 RVA: 0x000213AA File Offset: 0x0001F5AA
		// (set) Token: 0x060007BF RID: 1983 RVA: 0x000213B2 File Offset: 0x0001F5B2
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

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x060007C0 RID: 1984 RVA: 0x000213D0 File Offset: 0x0001F5D0
		// (set) Token: 0x060007C1 RID: 1985 RVA: 0x000213D8 File Offset: 0x0001F5D8
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

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x060007C2 RID: 1986 RVA: 0x000213FB File Offset: 0x0001F5FB
		// (set) Token: 0x060007C3 RID: 1987 RVA: 0x00021403 File Offset: 0x0001F603
		[DataSourceProperty]
		public string Explanation
		{
			get
			{
				return this._explanation;
			}
			set
			{
				if (value != this._explanation)
				{
					this._explanation = value;
					base.OnPropertyChangedWithValue<string>(value, "Explanation");
				}
			}
		}

		// Token: 0x04000366 RID: 870
		private readonly Action<KingdomPolicyItemVM> _onSelect;

		// Token: 0x04000367 RID: 871
		private readonly Func<PolicyObject, bool> _getIsPolicyActive;

		// Token: 0x04000368 RID: 872
		private string _name;

		// Token: 0x04000369 RID: 873
		private string _explanation;

		// Token: 0x0400036A RID: 874
		private string _policyAcceptanceText;

		// Token: 0x0400036B RID: 875
		private PolicyObject _policy;

		// Token: 0x0400036C RID: 876
		private int _policyLikelihood;

		// Token: 0x0400036D RID: 877
		private string _policyLikelihoodText;

		// Token: 0x0400036E RID: 878
		private HintViewModel _likelihoodHint;

		// Token: 0x0400036F RID: 879
		private MBBindingList<StringItemWithHintVM> _policyEffectList;
	}
}
