using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions.ItemTypes
{
	public class KingSelectionDecisionItemVM : DecisionItemBaseVM
	{
		public IFaction TargetFaction
		{
			get
			{
				return (this._decision as KingSelectionKingdomDecision).Kingdom;
			}
		}

		public KingSelectionDecisionItemVM(KingSelectionKingdomDecision decision, Action onDecisionOver)
			: base(decision, onDecisionOver)
		{
			this._kingSelectionDecision = decision;
			base.DecisionType = 6;
		}

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

		private void ExecuteLocationLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
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

		private readonly KingSelectionKingdomDecision _kingSelectionDecision;

		private string _nameText;

		private string _factionName;

		private ImageIdentifierVM _factionBanner;

		private string _settlementsText;

		private string _settlementsListText;

		private string _castlesText;

		private string _castlesListText;

		private int _totalStrength;

		private string _totalStrengthText;

		private string _activePoliciesText;

		private string _activePoliciesListText;
	}
}
