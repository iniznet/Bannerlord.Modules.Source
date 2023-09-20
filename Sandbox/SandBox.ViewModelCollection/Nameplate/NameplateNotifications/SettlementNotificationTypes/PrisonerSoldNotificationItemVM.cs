using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace SandBox.ViewModelCollection.Nameplate.NameplateNotifications.SettlementNotificationTypes
{
	// Token: 0x02000020 RID: 32
	public class PrisonerSoldNotificationItemVM : SettlementNotificationItemBaseVM
	{
		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060002BB RID: 699 RVA: 0x0000D808 File Offset: 0x0000BA08
		// (set) Token: 0x060002BC RID: 700 RVA: 0x0000D810 File Offset: 0x0000BA10
		public MobileParty Party { get; private set; }

		// Token: 0x060002BD RID: 701 RVA: 0x0000D81C File Offset: 0x0000BA1C
		public PrisonerSoldNotificationItemVM(Action<SettlementNotificationItemBaseVM> onRemove, MobileParty party, TroopRoster prisoners, int createdTick)
			: base(onRemove, createdTick)
		{
			this._prisonersAmount = prisoners.TotalManCount;
			base.Text = SandBoxUIHelper.GetPrisonersSoldNotificationText(this._prisonersAmount);
			this.Party = party;
			base.CharacterName = ((party.LeaderHero != null) ? party.LeaderHero.Name.ToString() : party.Name.ToString());
			base.CharacterVisual = new ImageIdentifierVM(SandBoxUIHelper.GetCharacterCode(PartyBaseHelper.GetVisualPartyLeader(party.Party), false));
			base.RelationType = 0;
			base.CreatedTick = createdTick;
			if (party.LeaderHero != null)
			{
				base.RelationType = (party.LeaderHero.Clan.IsAtWarWith(Hero.MainHero.Clan) ? (-1) : 1);
			}
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0000D8DA File Offset: 0x0000BADA
		public void AddNewPrisoners(TroopRoster newPrisoners)
		{
			this._prisonersAmount += newPrisoners.Count;
			base.Text = SandBoxUIHelper.GetPrisonersSoldNotificationText(this._prisonersAmount);
		}

		// Token: 0x04000161 RID: 353
		private int _prisonersAmount;
	}
}
