﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	public class KingdomDestroyedLogEntry : LogEntry, IEncyclopediaLog, IChatNotification
	{
		internal static void AutoGeneratedStaticCollectObjectsKingdomDestroyedLogEntry(object o, List<object> collectedObjects)
		{
			((KingdomDestroyedLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._kingdom);
		}

		internal static object AutoGeneratedGetMemberValue_kingdom(object o)
		{
			return ((KingdomDestroyedLogEntry)o)._kingdom;
		}

		public bool IsVisibleNotification
		{
			get
			{
				return true;
			}
		}

		public KingdomDestroyedLogEntry(Kingdom kingdom)
		{
			this._kingdom = kingdom;
		}

		public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
		{
			return obj == this._kingdom;
		}

		public TextObject GetEncyclopediaText()
		{
			TextObject textObject = GameTexts.FindText("str_kingdom_destroyed", null);
			textObject.SetTextVariable("KINGDOM", this._kingdom.EncyclopediaLinkWithName);
			return textObject;
		}

		public TextObject GetNotificationText()
		{
			return this.GetEncyclopediaText();
		}

		[SaveableField(10)]
		private Kingdom _kingdom;
	}
}
