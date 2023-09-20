using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions.ItemTypes
{
	// Token: 0x0200006B RID: 107
	public class KingSelectionDecisionItemVM : DecisionItemBaseVM
	{
		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06000942 RID: 2370 RVA: 0x00026555 File Offset: 0x00024755
		public IFaction TargetFaction
		{
			get
			{
				return (this._decision as KingSelectionKingdomDecision).Kingdom;
			}
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x00026567 File Offset: 0x00024767
		public KingSelectionDecisionItemVM(KingSelectionKingdomDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			this._kingSelectionDecision = decision;
			base.DecisionType = 6;
		}

		// Token: 0x06000944 RID: 2372 RVA: 0x00026580 File Offset: 0x00024780
		protected override void InitValues()
		{
			base.InitValues();
			TextObject textObject = GameTexts.FindText("str_kingdom_decision_king_selection", null);
			textObject.SetTextVariable("FACTION", this.TargetFaction.Name);
			this.NameText = textObject.ToString();
			this.FactionBanner = new ImageIdentifierVM(BannerCode.CreateFrom(this.TargetFaction.Banner), true);
			this.FactionName = this.TargetFaction.Culture.Name.ToString();
			bool flag = true;
			bool flag2 = true;
			int num = 0;
			int num2 = 0;
			foreach (Settlement settlement in this.TargetFaction.Settlements)
			{
				if (settlement.IsTown)
				{
					if (flag)
					{
						this.SettlementsListText = settlement.EncyclopediaLinkWithName.ToString();
						flag = false;
					}
					else
					{
						GameTexts.SetVariable("LEFT", this.SettlementsListText);
						GameTexts.SetVariable("RIGHT", settlement.EncyclopediaLinkWithName);
						this.SettlementsListText = GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString();
					}
					num++;
				}
				else if (settlement.IsCastle)
				{
					if (flag2)
					{
						this.CastlesListText = settlement.EncyclopediaLinkWithName.ToString();
						flag2 = false;
					}
					else
					{
						GameTexts.SetVariable("LEFT", this.CastlesListText);
						GameTexts.SetVariable("RIGHT", settlement.EncyclopediaLinkWithName);
						this.CastlesListText = GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString();
					}
					num2++;
				}
			}
			TextObject textObject2 = GameTexts.FindText("str_settlements", null);
			TextObject textObject3 = GameTexts.FindText("str_STR_in_parentheses", null);
			textObject3.SetTextVariable("STR", num);
			TextObject textObject4 = GameTexts.FindText("str_LEFT_RIGHT", null);
			textObject4.SetTextVariable("LEFT", textObject2);
			textObject4.SetTextVariable("RIGHT", textObject3);
			this.SettlementsText = textObject4.ToString();
			TextObject textObject5 = GameTexts.FindText("str_castles", null);
			TextObject textObject6 = GameTexts.FindText("str_STR_in_parentheses", null);
			textObject6.SetTextVariable("STR", num2);
			TextObject textObject7 = GameTexts.FindText("str_LEFT_RIGHT", null);
			textObject7.SetTextVariable("LEFT", textObject5);
			textObject7.SetTextVariable("RIGHT", textObject6);
			this.CastlesText = textObject7.ToString();
			this.TotalStrengthText = GameTexts.FindText("str_total_strength", null).ToString();
			this.TotalStrength = (int)this.TargetFaction.TotalStrength;
			this.ActivePoliciesText = GameTexts.FindText("str_active_policies", null).ToString();
			Kingdom kingdom = this.TargetFaction as Kingdom;
			foreach (PolicyObject policyObject in kingdom.ActivePolicies)
			{
				if (policyObject == kingdom.ActivePolicies[0])
				{
					this.ActivePoliciesListText = policyObject.Name.ToString();
				}
				else
				{
					GameTexts.SetVariable("LEFT", this.ActivePoliciesListText);
					GameTexts.SetVariable("RIGHT", policyObject.Name.ToString());
					this.ActivePoliciesListText = GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString();
				}
			}
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x000268B8 File Offset: 0x00024AB8
		private void ExecuteLocationLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06000946 RID: 2374 RVA: 0x000268CA File Offset: 0x00024ACA
		// (set) Token: 0x06000947 RID: 2375 RVA: 0x000268D2 File Offset: 0x00024AD2
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

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06000948 RID: 2376 RVA: 0x000268F5 File Offset: 0x00024AF5
		// (set) Token: 0x06000949 RID: 2377 RVA: 0x000268FD File Offset: 0x00024AFD
		[DataSourceProperty]
		public string FactionName
		{
			get
			{
				return this._factionName;
			}
			set
			{
				if (value != this._factionName)
				{
					this._factionName = value;
					base.OnPropertyChangedWithValue<string>(value, "FactionName");
				}
			}
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x0600094A RID: 2378 RVA: 0x00026920 File Offset: 0x00024B20
		// (set) Token: 0x0600094B RID: 2379 RVA: 0x00026928 File Offset: 0x00024B28
		[DataSourceProperty]
		public ImageIdentifierVM FactionBanner
		{
			get
			{
				return this._factionBanner;
			}
			set
			{
				if (value != this._factionBanner)
				{
					this._factionBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "FactionBanner");
				}
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x0600094C RID: 2380 RVA: 0x00026946 File Offset: 0x00024B46
		// (set) Token: 0x0600094D RID: 2381 RVA: 0x0002694E File Offset: 0x00024B4E
		[DataSourceProperty]
		public string SettlementsText
		{
			get
			{
				return this._settlementsText;
			}
			set
			{
				if (value != this._settlementsText)
				{
					this._settlementsText = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementsText");
				}
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x0600094E RID: 2382 RVA: 0x00026971 File Offset: 0x00024B71
		// (set) Token: 0x0600094F RID: 2383 RVA: 0x00026979 File Offset: 0x00024B79
		[DataSourceProperty]
		public string SettlementsListText
		{
			get
			{
				return this._settlementsListText;
			}
			set
			{
				if (value != this._settlementsListText)
				{
					this._settlementsListText = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementsListText");
				}
			}
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06000950 RID: 2384 RVA: 0x0002699C File Offset: 0x00024B9C
		// (set) Token: 0x06000951 RID: 2385 RVA: 0x000269A4 File Offset: 0x00024BA4
		[DataSourceProperty]
		public string CastlesText
		{
			get
			{
				return this._castlesText;
			}
			set
			{
				if (value != this._castlesText)
				{
					this._castlesText = value;
					base.OnPropertyChangedWithValue<string>(value, "CastlesText");
				}
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06000952 RID: 2386 RVA: 0x000269C7 File Offset: 0x00024BC7
		// (set) Token: 0x06000953 RID: 2387 RVA: 0x000269CF File Offset: 0x00024BCF
		[DataSourceProperty]
		public string CastlesListText
		{
			get
			{
				return this._castlesListText;
			}
			set
			{
				if (value != this._castlesListText)
				{
					this._castlesListText = value;
					base.OnPropertyChangedWithValue<string>(value, "CastlesListText");
				}
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06000954 RID: 2388 RVA: 0x000269F2 File Offset: 0x00024BF2
		// (set) Token: 0x06000955 RID: 2389 RVA: 0x000269FA File Offset: 0x00024BFA
		[DataSourceProperty]
		public string TotalStrengthText
		{
			get
			{
				return this._totalStrengthText;
			}
			set
			{
				if (value != this._totalStrengthText)
				{
					this._totalStrengthText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalStrengthText");
				}
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06000956 RID: 2390 RVA: 0x00026A1D File Offset: 0x00024C1D
		// (set) Token: 0x06000957 RID: 2391 RVA: 0x00026A25 File Offset: 0x00024C25
		[DataSourceProperty]
		public int TotalStrength
		{
			get
			{
				return this._totalStrength;
			}
			set
			{
				if (value != this._totalStrength)
				{
					this._totalStrength = value;
					base.OnPropertyChangedWithValue(value, "TotalStrength");
				}
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06000958 RID: 2392 RVA: 0x00026A43 File Offset: 0x00024C43
		// (set) Token: 0x06000959 RID: 2393 RVA: 0x00026A4B File Offset: 0x00024C4B
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

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x0600095A RID: 2394 RVA: 0x00026A6E File Offset: 0x00024C6E
		// (set) Token: 0x0600095B RID: 2395 RVA: 0x00026A76 File Offset: 0x00024C76
		[DataSourceProperty]
		public string ActivePoliciesListText
		{
			get
			{
				return this._activePoliciesListText;
			}
			set
			{
				if (value != this._activePoliciesListText)
				{
					this._activePoliciesListText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActivePoliciesListText");
				}
			}
		}

		// Token: 0x0400042A RID: 1066
		private readonly KingSelectionKingdomDecision _kingSelectionDecision;

		// Token: 0x0400042B RID: 1067
		private string _nameText;

		// Token: 0x0400042C RID: 1068
		private string _factionName;

		// Token: 0x0400042D RID: 1069
		private ImageIdentifierVM _factionBanner;

		// Token: 0x0400042E RID: 1070
		private string _settlementsText;

		// Token: 0x0400042F RID: 1071
		private string _settlementsListText;

		// Token: 0x04000430 RID: 1072
		private string _castlesText;

		// Token: 0x04000431 RID: 1073
		private string _castlesListText;

		// Token: 0x04000432 RID: 1074
		private int _totalStrength;

		// Token: 0x04000433 RID: 1075
		private string _totalStrengthText;

		// Token: 0x04000434 RID: 1076
		private string _activePoliciesText;

		// Token: 0x04000435 RID: 1077
		private string _activePoliciesListText;
	}
}
