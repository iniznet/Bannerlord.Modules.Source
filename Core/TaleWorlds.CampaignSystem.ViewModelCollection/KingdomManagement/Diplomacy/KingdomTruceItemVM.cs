using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	// Token: 0x0200005E RID: 94
	public class KingdomTruceItemVM : KingdomDiplomacyItemVM
	{
		// Token: 0x06000829 RID: 2089 RVA: 0x00022D2E File Offset: 0x00020F2E
		public KingdomTruceItemVM(IFaction faction1, IFaction faction2, Action<KingdomDiplomacyItemVM> onSelection, Action<KingdomTruceItemVM> onAction)
			: base(faction1, faction2)
		{
			this._onAction = onAction;
			this._onSelection = onSelection;
			this.UpdateDiplomacyProperties();
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x00022D4D File Offset: 0x00020F4D
		protected override void OnSelect()
		{
			this.UpdateDiplomacyProperties();
			this._onSelection(this);
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x00022D64 File Offset: 0x00020F64
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

		// Token: 0x0600082C RID: 2092 RVA: 0x00022EF9 File Offset: 0x000210F9
		protected override void ExecuteAction()
		{
			this._onAction(this);
		}

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x0600082D RID: 2093 RVA: 0x00022F07 File Offset: 0x00021107
		// (set) Token: 0x0600082E RID: 2094 RVA: 0x00022F0F File Offset: 0x0002110F
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

		// Token: 0x040003A4 RID: 932
		private readonly Action<KingdomTruceItemVM> _onAction;

		// Token: 0x040003A5 RID: 933
		private readonly Action<KingdomDiplomacyItemVM> _onSelection;

		// Token: 0x040003A6 RID: 934
		private int _tributePaid;
	}
}
