using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	public class KingdomTruceItemVM : KingdomDiplomacyItemVM
	{
		public KingdomTruceItemVM(IFaction faction1, IFaction faction2, Action<KingdomDiplomacyItemVM> onSelection, Action<KingdomTruceItemVM> onAction)
			: base(faction1, faction2)
		{
			this._onAction = onAction;
			this._onSelection = onSelection;
			this.UpdateDiplomacyProperties();
		}

		protected override void OnSelect()
		{
			this.UpdateDiplomacyProperties();
			this._onSelection(this);
		}

		protected override void UpdateDiplomacyProperties()
		{
			base.UpdateDiplomacyProperties();
			base.Stats.Add(new KingdomWarComparableStatVM((int)this.Faction1.TotalStrength, (int)this.Faction2.TotalStrength, GameTexts.FindText("str_total_strength", null), this._faction1Color, this._faction2Color, 10000, null, null));
			base.Stats.Add(new KingdomWarComparableStatVM(this._faction1Towns.Count, this._faction2Towns.Count, GameTexts.FindText("str_towns", null), this._faction1Color, this._faction2Color, 25, new BasicTooltipViewModel(() => CampaignUIHelper.GetTruceOwnedSettlementsTooltip(this._faction1Towns, this.Faction1.Name, true)), new BasicTooltipViewModel(() => CampaignUIHelper.GetTruceOwnedSettlementsTooltip(this._faction2Towns, this.Faction2.Name, true))));
			base.Stats.Add(new KingdomWarComparableStatVM(this._faction1Castles.Count, this._faction2Castles.Count, GameTexts.FindText("str_castles", null), this._faction1Color, this._faction2Color, 25, new BasicTooltipViewModel(() => CampaignUIHelper.GetTruceOwnedSettlementsTooltip(this._faction1Castles, this.Faction1.Name, false)), new BasicTooltipViewModel(() => CampaignUIHelper.GetTruceOwnedSettlementsTooltip(this._faction2Castles, this.Faction2.Name, false))));
			StanceLink stanceWith = this._playerKingdom.GetStanceWith(this.Faction2);
			this.TributePaid = stanceWith.GetDailyTributePaid(this._playerKingdom);
			if (stanceWith.IsNeutral && this.TributePaid != 0)
			{
				base.Stats.Add(new KingdomWarComparableStatVM(stanceWith.GetTotalTributePaid(this.Faction2), stanceWith.GetTotalTributePaid(this.Faction1), GameTexts.FindText("str_comparison_tribute_received", null), this._faction1Color, this._faction2Color, 10000, null, null));
			}
		}

		protected override void ExecuteAction()
		{
			this._onAction(this);
		}

		public int TributePaid
		{
			get
			{
				return this._tributePaid;
			}
			set
			{
				if (value != this._tributePaid)
				{
					this._tributePaid = value;
					base.OnPropertyChangedWithValue(value, "TributePaid");
				}
			}
		}

		private readonly Action<KingdomTruceItemVM> _onAction;

		private readonly Action<KingdomDiplomacyItemVM> _onSelection;

		private int _tributePaid;
	}
}
