using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions.ItemTypes
{
	public class PolicyDecisionItemVM : DecisionItemBaseVM
	{
		public KingdomPolicyDecision PolicyDecision
		{
			get
			{
				KingdomPolicyDecision kingdomPolicyDecision;
				if ((kingdomPolicyDecision = this._policyDecision) == null)
				{
					kingdomPolicyDecision = (this._policyDecision = this._decision as KingdomPolicyDecision);
				}
				return kingdomPolicyDecision;
			}
		}

		public PolicyObject Policy
		{
			get
			{
				return this.PolicyDecision.Policy;
			}
		}

		public PolicyDecisionItemVM(KingdomPolicyDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			base.DecisionType = 3;
		}

		protected override void InitValues()
		{
			base.InitValues();
			base.DecisionType = 3;
			this.NameText = this.Policy.Name.ToString();
			this.PolicyDescriptionText = this.Policy.Description.ToString();
			this.PolicyEffectList = new MBBindingList<StringItemWithHintVM>();
			foreach (string text in this.Policy.SecondaryEffects.ToString().Split(new char[] { '\n' }))
			{
				this.PolicyEffectList.Add(new StringItemWithHintVM(text, TextObject.Empty));
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
		public string PolicyDescriptionText
		{
			get
			{
				return this._policyDescriptionText;
			}
			set
			{
				if (value != this._policyDescriptionText)
				{
					this._policyDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "PolicyDescriptionText");
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

		private KingdomPolicyDecision _policyDecision;

		private MBBindingList<StringItemWithHintVM> _policyEffectList;

		private string _nameText;

		private string _policyDescriptionText;
	}
}
