using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace SandBox.ViewModelCollection.Nameplate.NameplateNotifications.SettlementNotificationTypes
{
	// Token: 0x0200001F RID: 31
	public class ItemSoldNotificationItemVM : SettlementNotificationItemBaseVM
	{
		// Token: 0x170000DF RID: 223
		// (get) Token: 0x060002B6 RID: 694 RVA: 0x0000D6AC File Offset: 0x0000B8AC
		public ItemRosterElement Item { get; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060002B7 RID: 695 RVA: 0x0000D6B4 File Offset: 0x0000B8B4
		public PartyBase ReceiverParty { get; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x0000D6BC File Offset: 0x0000B8BC
		public PartyBase PayerParty { get; }

		// Token: 0x060002B9 RID: 697 RVA: 0x0000D6C4 File Offset: 0x0000B8C4
		public ItemSoldNotificationItemVM(Action<SettlementNotificationItemBaseVM> onRemove, PartyBase receiverParty, PartyBase payerParty, ItemRosterElement item, int number, int createdTick)
			: base(onRemove, createdTick)
		{
			this.Item = item;
			this.ReceiverParty = receiverParty;
			this.PayerParty = payerParty;
			this._number = number;
			this._heroParty = (receiverParty.IsSettlement ? payerParty : receiverParty);
			base.Text = SandBoxUIHelper.GetItemSoldNotificationText(this.Item, this._number, this._number < 0);
			base.CharacterName = ((this._heroParty.LeaderHero != null) ? this._heroParty.LeaderHero.Name.ToString() : this._heroParty.Name.ToString());
			CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(this._heroParty);
			base.CharacterVisual = new ImageIdentifierVM(SandBoxUIHelper.GetCharacterCode(visualPartyLeader, false));
			base.RelationType = 0;
			base.CreatedTick = createdTick;
			if (this._heroParty.LeaderHero != null)
			{
				base.RelationType = (this._heroParty.LeaderHero.Clan.IsAtWarWith(Hero.MainHero.Clan) ? (-1) : 1);
			}
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000D7C9 File Offset: 0x0000B9C9
		public void AddNewTransaction(int amount)
		{
			this._number += amount;
			if (this._number == 0)
			{
				base.ExecuteRemove();
				return;
			}
			base.Text = SandBoxUIHelper.GetItemSoldNotificationText(this.Item, this._number, this._number < 0);
		}

		// Token: 0x0400015E RID: 350
		private int _number;

		// Token: 0x0400015F RID: 351
		private PartyBase _heroParty;
	}
}
