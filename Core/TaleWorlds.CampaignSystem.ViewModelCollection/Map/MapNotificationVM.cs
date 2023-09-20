using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map
{
	// Token: 0x02000032 RID: 50
	public class MapNotificationVM : ViewModel
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060004E5 RID: 1253 RVA: 0x0001948C File Offset: 0x0001768C
		// (remove) Token: 0x060004E6 RID: 1254 RVA: 0x000194C4 File Offset: 0x000176C4
		public event Action<MapNotificationItemBaseVM> ReceiveNewNotification;

		// Token: 0x060004E7 RID: 1255 RVA: 0x000194FC File Offset: 0x000176FC
		public MapNotificationVM(INavigationHandler navigationHandler, Action<Vec2> fastMoveCameraToPosition)
		{
			this._navigationHandler = navigationHandler;
			this._fastMoveCameraToPosition = fastMoveCameraToPosition;
			MBInformationManager.OnAddMapNotice += this.AddMapNotification;
			this.NotificationItems = new MBBindingList<MapNotificationItemBaseVM>();
			this.PopulateTypeDictionary();
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0001954A File Offset: 0x0001774A
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NotificationItems.ApplyActionOnAllItems(delegate(MapNotificationItemBaseVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x0001957C File Offset: 0x0001777C
		private void PopulateTypeDictionary()
		{
			this._itemConstructors.Add(typeof(PeaceMapNotification), typeof(PeaceNotificationItemVM));
			this._itemConstructors.Add(typeof(SettlementRebellionMapNotification), typeof(RebellionNotificationItemVM));
			this._itemConstructors.Add(typeof(WarMapNotification), typeof(WarNotificationItemVM));
			this._itemConstructors.Add(typeof(ArmyDispersionMapNotification), typeof(ArmyDispersionItemVM));
			this._itemConstructors.Add(typeof(ChildBornMapNotification), typeof(NewBornNotificationItemVM));
			this._itemConstructors.Add(typeof(DeathMapNotification), typeof(DeathNotificationItemVM));
			this._itemConstructors.Add(typeof(MarriageMapNotification), typeof(MarriageNotificationItemVM));
			this._itemConstructors.Add(typeof(MarriageOfferMapNotification), typeof(MarriageOfferNotificationItemVM));
			this._itemConstructors.Add(typeof(MercenaryOfferMapNotification), typeof(MercenaryOfferMapNotificationItemVM));
			this._itemConstructors.Add(typeof(VassalOfferMapNotification), typeof(VassalOfferMapNotificationItemVM));
			this._itemConstructors.Add(typeof(ArmyCreationMapNotification), typeof(ArmyCreationNotificationItemVM));
			this._itemConstructors.Add(typeof(KingdomDecisionMapNotification), typeof(KingdomVoteNotificationItemVM));
			this._itemConstructors.Add(typeof(SettlementOwnerChangedMapNotification), typeof(SettlementOwnerChangedNotificationItemVM));
			this._itemConstructors.Add(typeof(SettlementUnderSiegeMapNotification), typeof(SettlementUnderSiegeMapNotificationItemVM));
			this._itemConstructors.Add(typeof(AlleyLeaderDiedMapNotification), typeof(AlleyLeaderDiedMapNotificationItemVM));
			this._itemConstructors.Add(typeof(AlleyUnderAttackMapNotification), typeof(AlleyUnderAttackMapNotificationItemVM));
			this._itemConstructors.Add(typeof(EducationMapNotification), typeof(EducationNotificationItemVM));
			this._itemConstructors.Add(typeof(TraitChangedMapNotification), typeof(TraitChangedNotificationItemVM));
			this._itemConstructors.Add(typeof(RansomOfferMapNotification), typeof(RansomNotificationItemVM));
			this._itemConstructors.Add(typeof(PeaceOfferMapNotification), typeof(PeaceOfferNotificationItemVM));
			this._itemConstructors.Add(typeof(PartyLeaderChangeNotification), typeof(PartyLeaderChangeNotificationVM));
			this._itemConstructors.Add(typeof(HeirComeOfAgeMapNotification), typeof(HeirComeOfAgeNotificationItemVM));
			this._itemConstructors.Add(typeof(KingdomDestroyedMapNotification), typeof(KingdomDestroyedNotificationItemVM));
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x00019852 File Offset: 0x00017A52
		public void RegisterMapNotificationType(Type data, Type item)
		{
			this._itemConstructors[data] = item;
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x00019861 File Offset: 0x00017A61
		public override void OnFinalize()
		{
			MBInformationManager.OnAddMapNotice -= this.AddMapNotification;
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x00019874 File Offset: 0x00017A74
		public void OnFrameTick(float dt)
		{
			for (int i = 0; i < this.NotificationItems.Count; i++)
			{
				this.NotificationItems[i].ManualRefreshRelevantStatus();
			}
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x000198A8 File Offset: 0x00017AA8
		public void OnMenuModeTick(float dt)
		{
			for (int i = 0; i < this.NotificationItems.Count; i++)
			{
				this.NotificationItems[i].ManualRefreshRelevantStatus();
			}
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x000198DC File Offset: 0x00017ADC
		private void RemoveNotificationItem(MapNotificationItemBaseVM item)
		{
			item.OnFinalize();
			this.NotificationItems.Remove(item);
			MBInformationManager.MapNoticeRemoved(item.Data);
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x000198FC File Offset: 0x00017AFC
		private void OnNotificationItemFocus(MapNotificationItemBaseVM item)
		{
			this.FocusedNotificationItem = item;
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x00019905 File Offset: 0x00017B05
		private void GoToSettlement(Settlement settlement)
		{
			this._fastMoveCameraToPosition(settlement.Position2D);
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x00019918 File Offset: 0x00017B18
		private void GoToPosOnMap(Vec2 posOnMap)
		{
			this._fastMoveCameraToPosition(posOnMap);
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x00019928 File Offset: 0x00017B28
		public void AddMapNotification(InformationData data)
		{
			MapNotificationItemBaseVM notificationFromData = this.GetNotificationFromData(data);
			if (notificationFromData != null)
			{
				this.NotificationItems.Add(notificationFromData);
				Action<MapNotificationItemBaseVM> receiveNewNotification = this.ReceiveNewNotification;
				if (receiveNewNotification == null)
				{
					return;
				}
				receiveNewNotification(notificationFromData);
			}
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x00019960 File Offset: 0x00017B60
		public void RemoveAllNotifications()
		{
			foreach (MapNotificationItemBaseVM mapNotificationItemBaseVM in this.NotificationItems.ToList<MapNotificationItemBaseVM>())
			{
				this.RemoveNotificationItem(mapNotificationItemBaseVM);
			}
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x000199B8 File Offset: 0x00017BB8
		private MapNotificationItemBaseVM GetNotificationFromData(InformationData data)
		{
			Type type = data.GetType();
			MapNotificationItemBaseVM mapNotificationItemBaseVM = null;
			if (this._itemConstructors.ContainsKey(type))
			{
				mapNotificationItemBaseVM = (MapNotificationItemBaseVM)Activator.CreateInstance(this._itemConstructors[type], new object[] { data });
				if (mapNotificationItemBaseVM != null)
				{
					mapNotificationItemBaseVM.OnRemove = new Action<MapNotificationItemBaseVM>(this.RemoveNotificationItem);
					mapNotificationItemBaseVM.OnFocus = new Action<MapNotificationItemBaseVM>(this.OnNotificationItemFocus);
					mapNotificationItemBaseVM.SetNavigationHandler(this._navigationHandler);
					mapNotificationItemBaseVM.SetFastMoveCameraToPosition(this._fastMoveCameraToPosition);
					if (this.RemoveInputKey != null)
					{
						mapNotificationItemBaseVM.RemoveInputKey = this.RemoveInputKey;
					}
				}
			}
			return mapNotificationItemBaseVM;
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x00019A51 File Offset: 0x00017C51
		public void SetRemoveInputKey(HotKey hotKey)
		{
			this.RemoveInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060004F6 RID: 1270 RVA: 0x00019A60 File Offset: 0x00017C60
		// (set) Token: 0x060004F7 RID: 1271 RVA: 0x00019A68 File Offset: 0x00017C68
		[DataSourceProperty]
		public InputKeyItemVM RemoveInputKey
		{
			get
			{
				return this._removeInputKey;
			}
			set
			{
				if (value != this._removeInputKey)
				{
					this._removeInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "RemoveInputKey");
					if (this._removeInputKey != null && this.NotificationItems != null)
					{
						for (int i = 0; i < this.NotificationItems.Count; i++)
						{
							this.NotificationItems[i].RemoveInputKey = this._removeInputKey;
						}
					}
				}
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060004F8 RID: 1272 RVA: 0x00019ACE File Offset: 0x00017CCE
		// (set) Token: 0x060004F9 RID: 1273 RVA: 0x00019AD6 File Offset: 0x00017CD6
		[DataSourceProperty]
		public MapNotificationItemBaseVM FocusedNotificationItem
		{
			get
			{
				return this._focusedNotificationItem;
			}
			set
			{
				if (value != this._focusedNotificationItem)
				{
					this._focusedNotificationItem = value;
					base.OnPropertyChangedWithValue<MapNotificationItemBaseVM>(value, "FocusedNotificationItem");
				}
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060004FA RID: 1274 RVA: 0x00019AF4 File Offset: 0x00017CF4
		// (set) Token: 0x060004FB RID: 1275 RVA: 0x00019AFC File Offset: 0x00017CFC
		[DataSourceProperty]
		public MBBindingList<MapNotificationItemBaseVM> NotificationItems
		{
			get
			{
				return this._notificationItems;
			}
			set
			{
				if (value != this._notificationItems)
				{
					this._notificationItems = value;
					base.OnPropertyChangedWithValue<MBBindingList<MapNotificationItemBaseVM>>(value, "NotificationItems");
				}
			}
		}

		// Token: 0x04000216 RID: 534
		private INavigationHandler _navigationHandler;

		// Token: 0x04000217 RID: 535
		private Action<Vec2> _fastMoveCameraToPosition;

		// Token: 0x04000218 RID: 536
		private Dictionary<Type, Type> _itemConstructors = new Dictionary<Type, Type>();

		// Token: 0x04000219 RID: 537
		private InputKeyItemVM _removeInputKey;

		// Token: 0x0400021A RID: 538
		private MapNotificationItemBaseVM _focusedNotificationItem;

		// Token: 0x0400021B RID: 539
		private MBBindingList<MapNotificationItemBaseVM> _notificationItems;
	}
}
