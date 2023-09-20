using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Policies
{
	public class KingdomPolicyItemVM : KingdomItemVM
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			Func<PolicyObject, bool> getIsPolicyActive = this._getIsPolicyActive;
			this.PolicyAcceptanceText = ((getIsPolicyActive != null && getIsPolicyActive(this.Policy)) ? GameTexts.FindText("str_policy_support_for_abolishing", null).ToString() : GameTexts.FindText("str_policy_support_for_enacting", null).ToString());
		}

		private void DeterminePolicyLikelihood()
		{
			float likelihoodForSponsor = new KingdomElection(new KingdomPolicyDecision(Clan.PlayerClan, this._policy, false)).GetLikelihoodForSponsor(Clan.PlayerClan);
			this.PolicyLikelihood = MathF.Round(likelihoodForSponsor * 100f);
			GameTexts.SetVariable("NUMBER", this.PolicyLikelihood);
			this.PolicyLikelihoodText = GameTexts.FindText("str_NUMBER_percent", null).ToString();
		}

		protected override void OnSelect()
		{
			base.OnSelect();
			this._onSelect(this);
		}

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

		private readonly Action<KingdomPolicyItemVM> _onSelect;

		private readonly Func<PolicyObject, bool> _getIsPolicyActive;

		private string _name;

		private string _explanation;

		private string _policyAcceptanceText;

		private PolicyObject _policy;

		private int _policyLikelihood;

		private string _policyLikelihoodText;

		private HintViewModel _likelihoodHint;

		private MBBindingList<StringItemWithHintVM> _policyEffectList;
	}
}
