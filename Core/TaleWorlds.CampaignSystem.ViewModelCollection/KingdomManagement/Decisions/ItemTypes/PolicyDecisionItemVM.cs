using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions.ItemTypes
{
	// Token: 0x0200006D RID: 109
	public class PolicyDecisionItemVM : DecisionItemBaseVM
	{
		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06000970 RID: 2416 RVA: 0x00026ED8 File Offset: 0x000250D8
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

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06000971 RID: 2417 RVA: 0x00026F03 File Offset: 0x00025103
		public PolicyObject Policy
		{
			get
			{
				return this.PolicyDecision.Policy;
			}
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x00026F10 File Offset: 0x00025110
		public PolicyDecisionItemVM(KingdomPolicyDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			base.DecisionType = 3;
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x00026F24 File Offset: 0x00025124
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

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06000974 RID: 2420 RVA: 0x00026FBE File Offset: 0x000251BE
		// (set) Token: 0x06000975 RID: 2421 RVA: 0x00026FC6 File Offset: 0x000251C6
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

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06000976 RID: 2422 RVA: 0x00026FE9 File Offset: 0x000251E9
		// (set) Token: 0x06000977 RID: 2423 RVA: 0x00026FF1 File Offset: 0x000251F1
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

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06000978 RID: 2424 RVA: 0x00027014 File Offset: 0x00025214
		// (set) Token: 0x06000979 RID: 2425 RVA: 0x0002701C File Offset: 0x0002521C
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

		// Token: 0x0400043F RID: 1087
		private KingdomPolicyDecision _policyDecision;

		// Token: 0x04000440 RID: 1088
		private MBBindingList<StringItemWithHintVM> _policyEffectList;

		// Token: 0x04000441 RID: 1089
		private string _nameText;

		// Token: 0x04000442 RID: 1090
		private string _policyDescriptionText;
	}
}
