﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	public class CampaignInformationManager
	{
		private void MapNoticeRemoved(InformationData obj)
		{
			int num = -1;
			for (int i = 0; i < this._mapNotices.Count; i++)
			{
				if (obj == this._mapNotices[i])
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				this._mapNotices.RemoveAt(num);
			}
		}

		public CampaignInformationManager()
		{
			this._mapNotices = new List<InformationData>();
		}

		internal void NewLogEntryAdded(LogEntry log)
		{
			IChatNotification chatNotification;
			if (this._isSessionLaunched && (chatNotification = log as IChatNotification) != null && chatNotification.IsVisibleNotification)
			{
				InformationManager.DisplayMessage(new InformationMessage
				{
					Information = chatNotification.GetNotificationText().ToString(),
					Color = Color.FromUint(Campaign.Current.Models.DiplomacyModel.GetNotificationColor(chatNotification.NotificationType))
				});
			}
		}

		private void AddInformationData(InformationData informationData)
		{
			List<InformationData> mapNotices = this._mapNotices;
			if (mapNotices != null)
			{
				mapNotices.Add(informationData);
			}
			MBInformationManager.AddNotice(informationData);
		}

		internal void RegisterEvents()
		{
			this._isSessionLaunched = true;
			MBInformationManager.OnRemoveMapNotice += this.MapNoticeRemoved;
		}

		internal void DeRegisterEvents()
		{
			this._isSessionLaunched = false;
			MBInformationManager.OnRemoveMapNotice -= this.MapNoticeRemoved;
		}

		public void OnGameLoaded()
		{
			this._mapNotices.RemoveAll((InformationData t) => t == null || !t.IsValid());
			foreach (InformationData informationData in this._mapNotices)
			{
				MBInformationManager.AddNotice(informationData);
			}
		}

		public void NewMapNoticeAdded(InformationData informationData)
		{
			this.AddInformationData(informationData);
		}

		public bool InformationDataExists<T>(Func<T, bool> predicate) where T : InformationData
		{
			using (List<InformationData>.Enumerator enumerator = this._mapNotices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T t;
					if ((t = enumerator.Current as T) != null && (predicate == null || predicate(t)))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static void AutoGeneratedStaticCollectObjectsCampaignInformationManager(object o, List<object> collectedObjects)
		{
			((CampaignInformationManager)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._mapNotices);
		}

		internal static object AutoGeneratedGetMemberValue_mapNotices(object o)
		{
			return ((CampaignInformationManager)o)._mapNotices;
		}

		[SaveableField(10)]
		private List<InformationData> _mapNotices;

		[CachedData]
		private bool _isSessionLaunched;

		public enum NoticeType
		{
			None,
			WarAnnouncement,
			PeaceAnnouncement,
			ChangeSettlementOwner,
			FortificationIsCaptured,
			HeroChangedFaction,
			BarterAnnouncement
		}
	}
}
