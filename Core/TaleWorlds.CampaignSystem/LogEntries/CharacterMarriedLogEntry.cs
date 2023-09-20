﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	// Token: 0x020002D5 RID: 725
	public class CharacterMarriedLogEntry : LogEntry, IEncyclopediaLog, IChatNotification
	{
		// Token: 0x06002A26 RID: 10790 RVA: 0x000B392B File Offset: 0x000B1B2B
		internal static void AutoGeneratedStaticCollectObjectsCharacterMarriedLogEntry(object o, List<object> collectedObjects)
		{
			((CharacterMarriedLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06002A27 RID: 10791 RVA: 0x000B3939 File Offset: 0x000B1B39
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.MarriedHero);
			collectedObjects.Add(this.MarriedTo);
		}

		// Token: 0x06002A28 RID: 10792 RVA: 0x000B395A File Offset: 0x000B1B5A
		internal static object AutoGeneratedGetMemberValueMarriedHero(object o)
		{
			return ((CharacterMarriedLogEntry)o).MarriedHero;
		}

		// Token: 0x06002A29 RID: 10793 RVA: 0x000B3967 File Offset: 0x000B1B67
		internal static object AutoGeneratedGetMemberValueMarriedTo(object o)
		{
			return ((CharacterMarriedLogEntry)o).MarriedTo;
		}

		// Token: 0x17000A59 RID: 2649
		// (get) Token: 0x06002A2A RID: 10794 RVA: 0x000B3974 File Offset: 0x000B1B74
		public override CampaignTime KeepInHistoryTime
		{
			get
			{
				return CampaignTime.Weeks(240f);
			}
		}

		// Token: 0x17000A5A RID: 2650
		// (get) Token: 0x06002A2B RID: 10795 RVA: 0x000B3980 File Offset: 0x000B1B80
		public override ChatNotificationType NotificationType
		{
			get
			{
				return base.DiplomaticNotification(this.MarriedHero.Clan, this.MarriedTo.Clan);
			}
		}

		// Token: 0x17000A5B RID: 2651
		// (get) Token: 0x06002A2C RID: 10796 RVA: 0x000B399E File Offset: 0x000B1B9E
		public bool IsVisibleNotification
		{
			get
			{
				return this.MarriedHero.CharacterObject.IsHero && this.MarriedTo.CharacterObject.IsHero;
			}
		}

		// Token: 0x06002A2D RID: 10797 RVA: 0x000B39C4 File Offset: 0x000B1BC4
		public CharacterMarriedLogEntry(Hero marriedHero, Hero marriedTo)
		{
			this.MarriedHero = marriedHero;
			this.MarriedTo = marriedTo;
		}

		// Token: 0x06002A2E RID: 10798 RVA: 0x000B39DA File Offset: 0x000B1BDA
		public override string ToString()
		{
			return this.GetEncyclopediaText().ToString();
		}

		// Token: 0x06002A2F RID: 10799 RVA: 0x000B39E8 File Offset: 0x000B1BE8
		public TextObject GetNotificationText()
		{
			TextObject textObject = GameTexts.FindText("str_hero_married_hero", null);
			StringHelpers.SetCharacterProperties("MARRIED_TO", this.MarriedTo.CharacterObject, textObject, false);
			StringHelpers.SetCharacterProperties("MARRIED_HERO", this.MarriedHero.CharacterObject, textObject, false);
			return textObject;
		}

		// Token: 0x06002A30 RID: 10800 RVA: 0x000B3A32 File Offset: 0x000B1C32
		public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
		{
			return obj == this.MarriedHero || obj == this.MarriedTo;
		}

		// Token: 0x06002A31 RID: 10801 RVA: 0x000B3A52 File Offset: 0x000B1C52
		public TextObject GetEncyclopediaText()
		{
			return this.GetNotificationText();
		}

		// Token: 0x04000CBE RID: 3262
		[SaveableField(130)]
		public readonly Hero MarriedHero;

		// Token: 0x04000CBF RID: 3263
		[SaveableField(131)]
		public readonly Hero MarriedTo;
	}
}
