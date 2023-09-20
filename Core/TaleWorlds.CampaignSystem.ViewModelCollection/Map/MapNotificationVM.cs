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
	public class MapNotificationVM : ViewModel
	{
		public event Action<MapNotificationItemBaseVM> ReceiveNewNotification;

		public MapNotificationVM(INavigationHandler navigationHandler, Action<Vec2> fastMoveCameraToPosition)
		{
			this._navigationHandler = navigationHandler;
			this._fastMoveCameraToPosition = fastMoveCameraToPosition;
			MBInformationManager.OnAddMapNotice += this.AddMapNotification;
			this.NotificationItems = new MBBindingList<MapNotificationItemBaseVM>();
			this.PopulateTypeDictionary();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NotificationItems.ApplyActionOnAllItems(delegate(MapNotificationItemBaseVM x)
			{
				x.RefreshValues();
			});
		}

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

		public void RegisterMapNotificationType(Type data, Type item)
		{
			this._itemConstructors[data] = item;
		}

		public override void OnFinalize()
		{
			MBInformationManager.OnAddMapNotice -= this.AddMapNotification;
		}

		public void OnFrameTick(float dt)
		{
			for (int i = 0; i < this.NotificationItems.Count; i++)
			{
				this.NotificationItems[i].ManualRefreshRelevantStatus();
			}
		}

		public void OnMenuModeTick(float dt)
		{
			for (int i = 0; i < this.NotificationItems.Count; i++)
			{
				this.NotificationItems[i].ManualRefreshRelevantStatus();
			}
		}

		private void RemoveNotificationItem(MapNotificationItemBaseVM item)
		{
			item.OnFinalize();
			this.NotificationItems.Remove(item);
			MBInformationManager.MapNoticeRemoved(item.Data);
		}

		private void OnNotificationItemFocus(MapNotificationItemBaseVM item)
		{
			this.FocusedNotificationItem = item;
		}

		private void GoToSettlement(Settlement settlement)
		{
			this._fastMoveCameraToPosition(settlement.Position2D);
		}

		private void GoToPosOnMap(Vec2 posOnMap)
		{
			this._fastMoveCameraToPosition(posOnMap);
		}

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

		public void RemoveAllNotifications()
		{
			foreach (MapNotificationItemBaseVM mapNotificationItemBaseVM in this.NotificationItems.ToList<MapNotificationItemBaseVM>())
			{
				this.RemoveNotificationItem(mapNotificationItemBaseVM);
			}
		}

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

		public void SetRemoveInputKey(HotKey hotKey)
		{
			this.RemoveInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

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

		private INavigationHandler _navigationHandler;

		private Action<Vec2> _fastMoveCameraToPosition;

		private Dictionary<Type, Type> _itemConstructors = new Dictionary<Type, Type>();

		private InputKeyItemVM _removeInputKey;

		private MapNotificationItemBaseVM _focusedNotificationItem;

		private MBBindingList<MapNotificationItemBaseVM> _notificationItems;
	}
}
