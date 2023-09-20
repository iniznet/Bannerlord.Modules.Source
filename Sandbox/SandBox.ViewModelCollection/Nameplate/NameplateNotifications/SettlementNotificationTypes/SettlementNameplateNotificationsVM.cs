using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate.NameplateNotifications.SettlementNotificationTypes
{
	// Token: 0x02000021 RID: 33
	public class SettlementNameplateNotificationsVM : ViewModel
	{
		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x060002BF RID: 703 RVA: 0x0000D900 File Offset: 0x0000BB00
		// (set) Token: 0x060002C0 RID: 704 RVA: 0x0000D908 File Offset: 0x0000BB08
		public bool IsEventsRegistered { get; private set; }

		// Token: 0x060002C1 RID: 705 RVA: 0x0000D911 File Offset: 0x0000BB11
		public SettlementNameplateNotificationsVM(Settlement settlement)
		{
			this._settlement = settlement;
			this.Notifications = new MBBindingList<SettlementNotificationItemBaseVM>();
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0000D92B File Offset: 0x0000BB2B
		public void Tick()
		{
			this._tickSinceEnabled++;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000D93C File Offset: 0x0000BB3C
		private void OnTroopRecruited(Hero recruiterHero, Settlement settlement, Hero troopSource, CharacterObject troop, int amount)
		{
			if (amount > 0 && settlement == this._settlement && this._settlement.IsInspected && recruiterHero != null && (recruiterHero.CurrentSettlement == this._settlement || (recruiterHero.PartyBelongedTo != null && recruiterHero.PartyBelongedTo.LastVisitedSettlement == this._settlement)))
			{
				TroopRecruitmentNotificationItemVM updatableNotificationByPredicate = this.GetUpdatableNotificationByPredicate<TroopRecruitmentNotificationItemVM>((TroopRecruitmentNotificationItemVM n) => n.RecruiterHero == recruiterHero);
				if (updatableNotificationByPredicate != null)
				{
					updatableNotificationByPredicate.AddNewAction(amount);
					return;
				}
				this.Notifications.Add(new TroopRecruitmentNotificationItemVM(new Action<SettlementNotificationItemBaseVM>(this.RemoveItem), recruiterHero, amount, this._tickSinceEnabled));
			}
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0000DA08 File Offset: 0x0000BC08
		private void OnCaravanTransactionCompleted(MobileParty caravanParty, Town town, List<ValueTuple<EquipmentElement, int>> items)
		{
			if (this._settlement != town.Owner.Settlement)
			{
				return;
			}
			CaravanTransactionNotificationItemVM updatableNotificationByPredicate = this.GetUpdatableNotificationByPredicate<CaravanTransactionNotificationItemVM>((CaravanTransactionNotificationItemVM n) => n.CaravanParty == caravanParty);
			if (updatableNotificationByPredicate != null)
			{
				updatableNotificationByPredicate.AddNewItems(items);
				return;
			}
			this.Notifications.Add(new CaravanTransactionNotificationItemVM(new Action<SettlementNotificationItemBaseVM>(this.RemoveItem), caravanParty, items, this._tickSinceEnabled));
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000DA80 File Offset: 0x0000BC80
		private void OnPrisonerSold(MobileParty party, TroopRoster prisoners, Settlement currentSettlement)
		{
			if (this._settlement.IsInspected && prisoners.Count > 0 && party.LeaderHero != null && currentSettlement == this._settlement)
			{
				PrisonerSoldNotificationItemVM updatableNotificationByPredicate = this.GetUpdatableNotificationByPredicate<PrisonerSoldNotificationItemVM>((PrisonerSoldNotificationItemVM n) => n.Party == party);
				if (updatableNotificationByPredicate != null)
				{
					updatableNotificationByPredicate.AddNewPrisoners(prisoners);
					return;
				}
				this.Notifications.Add(new PrisonerSoldNotificationItemVM(new Action<SettlementNotificationItemBaseVM>(this.RemoveItem), party, prisoners, this._tickSinceEnabled));
			}
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000DB10 File Offset: 0x0000BD10
		private void OnTroopGivenToSettlement(Hero giverHero, Settlement givenSettlement, TroopRoster givenTroops)
		{
			if (this._settlement.IsInspected && givenTroops.TotalManCount > 0 && giverHero != null && givenSettlement == this._settlement)
			{
				TroopGivenToSettlementNotificationItemVM updatableNotificationByPredicate = this.GetUpdatableNotificationByPredicate<TroopGivenToSettlementNotificationItemVM>((TroopGivenToSettlementNotificationItemVM n) => n.GiverHero == giverHero);
				if (updatableNotificationByPredicate != null)
				{
					updatableNotificationByPredicate.AddNewAction(givenTroops);
					return;
				}
				this.Notifications.Add(new TroopGivenToSettlementNotificationItemVM(new Action<SettlementNotificationItemBaseVM>(this.RemoveItem), giverHero, givenTroops, this._tickSinceEnabled));
			}
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000DB98 File Offset: 0x0000BD98
		private void OnItemSold(PartyBase receiverParty, PartyBase payerParty, ItemRosterElement item, int number, Settlement currentSettlement)
		{
			if (this._settlement.IsInspected && number > 0 && currentSettlement == this._settlement)
			{
				int num = (receiverParty.IsSettlement ? (-1) : 1);
				ItemSoldNotificationItemVM updatableNotificationByPredicate = this.GetUpdatableNotificationByPredicate<ItemSoldNotificationItemVM>((ItemSoldNotificationItemVM n) => n.Item.EquipmentElement.Item == item.EquipmentElement.Item && (n.PayerParty == receiverParty || n.PayerParty == payerParty));
				if (updatableNotificationByPredicate != null)
				{
					updatableNotificationByPredicate.AddNewTransaction(number * num);
					return;
				}
				this.Notifications.Add(new ItemSoldNotificationItemVM(new Action<SettlementNotificationItemBaseVM>(this.RemoveItem), receiverParty, payerParty, item, number * num, this._tickSinceEnabled));
			}
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0000DC47 File Offset: 0x0000BE47
		private void OnIssueUpdated(IssueBase issue, IssueBase.IssueUpdateDetails updateType, Hero relatedHero)
		{
			if (updateType == 7 && relatedHero != null && relatedHero.CurrentSettlement == this._settlement)
			{
				this.Notifications.Add(new IssueSolvedByLordNotificationItemVM(new Action<SettlementNotificationItemBaseVM>(this.RemoveItem), relatedHero, this._tickSinceEnabled));
			}
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0000DC81 File Offset: 0x0000BE81
		private void RemoveItem(SettlementNotificationItemBaseVM item)
		{
			this.Notifications.Remove(item);
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0000DC90 File Offset: 0x0000BE90
		public void RegisterEvents()
		{
			if (!this.IsEventsRegistered)
			{
				CampaignEvents.OnTroopRecruitedEvent.AddNonSerializedListener(this, new Action<Hero, Settlement, Hero, CharacterObject, int>(this.OnTroopRecruited));
				CampaignEvents.OnPrisonerSoldEvent.AddNonSerializedListener(this, new Action<MobileParty, TroopRoster, Settlement>(this.OnPrisonerSold));
				CampaignEvents.OnCaravanTransactionCompletedEvent.AddNonSerializedListener(this, new Action<MobileParty, Town, List<ValueTuple<EquipmentElement, int>>>(this.OnCaravanTransactionCompleted));
				CampaignEvents.OnTroopGivenToSettlementEvent.AddNonSerializedListener(this, new Action<Hero, Settlement, TroopRoster>(this.OnTroopGivenToSettlement));
				CampaignEvents.OnItemSoldEvent.AddNonSerializedListener(this, new Action<PartyBase, PartyBase, ItemRosterElement, int, Settlement>(this.OnItemSold));
				CampaignEvents.OnIssueUpdatedEvent.AddNonSerializedListener(this, new Action<IssueBase, IssueBase.IssueUpdateDetails, Hero>(this.OnIssueUpdated));
				this._tickSinceEnabled = 0;
				this.IsEventsRegistered = true;
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000DD40 File Offset: 0x0000BF40
		public void UnloadEvents()
		{
			if (this.IsEventsRegistered)
			{
				CampaignEvents.OnTroopRecruitedEvent.ClearListeners(this);
				CampaignEvents.OnItemSoldEvent.ClearListeners(this);
				CampaignEvents.OnPrisonerSoldEvent.ClearListeners(this);
				CampaignEvents.OnCaravanTransactionCompletedEvent.ClearListeners(this);
				CampaignEvents.OnTroopGivenToSettlementEvent.ClearListeners(this);
				CampaignEvents.OnIssueUpdatedEvent.ClearListeners(this);
				this._tickSinceEnabled = 0;
				this.IsEventsRegistered = false;
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000DDA8 File Offset: 0x0000BFA8
		public bool IsValidItemForNotification(ItemRosterElement item)
		{
			switch (item.EquipmentElement.Item.Type)
			{
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 21:
			case 22:
			case 23:
				return true;
			}
			return false;
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0000DE3C File Offset: 0x0000C03C
		private T GetUpdatableNotificationByPredicate<T>(Func<T, bool> predicate) where T : SettlementNotificationItemBaseVM
		{
			for (int i = 0; i < this.Notifications.Count; i++)
			{
				SettlementNotificationItemBaseVM settlementNotificationItemBaseVM = this.Notifications[i];
				T t;
				if (this._tickSinceEnabled - settlementNotificationItemBaseVM.CreatedTick < 10 && (t = settlementNotificationItemBaseVM as T) != null && predicate(t))
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x060002CE RID: 718 RVA: 0x0000DEA7 File Offset: 0x0000C0A7
		// (set) Token: 0x060002CF RID: 719 RVA: 0x0000DEAF File Offset: 0x0000C0AF
		public MBBindingList<SettlementNotificationItemBaseVM> Notifications
		{
			get
			{
				return this._notifications;
			}
			set
			{
				if (value != this._notifications)
				{
					this._notifications = value;
					base.OnPropertyChangedWithValue<MBBindingList<SettlementNotificationItemBaseVM>>(value, "Notifications");
				}
			}
		}

		// Token: 0x04000162 RID: 354
		private readonly Settlement _settlement;

		// Token: 0x04000164 RID: 356
		private int _tickSinceEnabled;

		// Token: 0x04000165 RID: 357
		private const int _maxTickDeltaToCongregate = 10;

		// Token: 0x04000166 RID: 358
		private MBBindingList<SettlementNotificationItemBaseVM> _notifications;
	}
}
