﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Party.PartyComponents
{
	// Token: 0x020002BB RID: 699
	public class CustomPartyComponent : PartyComponent
	{
		// Token: 0x060027F5 RID: 10229 RVA: 0x000A9F0B File Offset: 0x000A810B
		internal static void AutoGeneratedStaticCollectObjectsCustomPartyComponent(object o, List<object> collectedObjects)
		{
			((CustomPartyComponent)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x060027F6 RID: 10230 RVA: 0x000A9F19 File Offset: 0x000A8119
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._name);
			collectedObjects.Add(this._homeSettlement);
			collectedObjects.Add(this._owner);
		}

		// Token: 0x060027F7 RID: 10231 RVA: 0x000A9F46 File Offset: 0x000A8146
		internal static object AutoGeneratedGetMemberValueAvoidHostileActions(object o)
		{
			return ((CustomPartyComponent)o).AvoidHostileActions;
		}

		// Token: 0x060027F8 RID: 10232 RVA: 0x000A9F58 File Offset: 0x000A8158
		internal static object AutoGeneratedGetMemberValue_name(object o)
		{
			return ((CustomPartyComponent)o)._name;
		}

		// Token: 0x060027F9 RID: 10233 RVA: 0x000A9F65 File Offset: 0x000A8165
		internal static object AutoGeneratedGetMemberValue_homeSettlement(object o)
		{
			return ((CustomPartyComponent)o)._homeSettlement;
		}

		// Token: 0x060027FA RID: 10234 RVA: 0x000A9F72 File Offset: 0x000A8172
		internal static object AutoGeneratedGetMemberValue_owner(object o)
		{
			return ((CustomPartyComponent)o)._owner;
		}

		// Token: 0x060027FB RID: 10235 RVA: 0x000A9F7F File Offset: 0x000A817F
		internal static object AutoGeneratedGetMemberValue_customPartyBaseSpeed(object o)
		{
			return ((CustomPartyComponent)o)._customPartyBaseSpeed;
		}

		// Token: 0x060027FC RID: 10236 RVA: 0x000A9F91 File Offset: 0x000A8191
		internal static object AutoGeneratedGetMemberValue_partyMountStringId(object o)
		{
			return ((CustomPartyComponent)o)._partyMountStringId;
		}

		// Token: 0x060027FD RID: 10237 RVA: 0x000A9F9E File Offset: 0x000A819E
		internal static object AutoGeneratedGetMemberValue_partyHarnessStringId(object o)
		{
			return ((CustomPartyComponent)o)._partyHarnessStringId;
		}

		// Token: 0x170009F3 RID: 2547
		// (get) Token: 0x060027FE RID: 10238 RVA: 0x000A9FAB File Offset: 0x000A81AB
		// (set) Token: 0x060027FF RID: 10239 RVA: 0x000A9FB3 File Offset: 0x000A81B3
		public float CustomPartyBaseSpeed
		{
			get
			{
				return this._customPartyBaseSpeed;
			}
			set
			{
				this._customPartyBaseSpeed = value;
			}
		}

		// Token: 0x170009F4 RID: 2548
		// (get) Token: 0x06002800 RID: 10240 RVA: 0x000A9FBC File Offset: 0x000A81BC
		public float BaseSpeed
		{
			get
			{
				return this._customPartyBaseSpeed;
			}
		}

		// Token: 0x170009F5 RID: 2549
		// (get) Token: 0x06002801 RID: 10241 RVA: 0x000A9FC4 File Offset: 0x000A81C4
		public override Hero PartyOwner
		{
			get
			{
				return this._owner;
			}
		}

		// Token: 0x170009F6 RID: 2550
		// (get) Token: 0x06002802 RID: 10242 RVA: 0x000A9FCC File Offset: 0x000A81CC
		public override TextObject Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x170009F7 RID: 2551
		// (get) Token: 0x06002803 RID: 10243 RVA: 0x000A9FD4 File Offset: 0x000A81D4
		public override Settlement HomeSettlement
		{
			get
			{
				return this._homeSettlement;
			}
		}

		// Token: 0x06002804 RID: 10244 RVA: 0x000A9FDC File Offset: 0x000A81DC
		public static MobileParty CreateQuestParty(Vec2 position, float spawnRadius, Settlement homeSettlement, TextObject name, Clan clan, PartyTemplateObject partyTemplate, Hero owner, int troopLimit = 0, string partyMountStringId = "", string partyHarnessStringId = "", float customPartyBaseSpeed = 0f, bool avoidHostileActions = false)
		{
			return MobileParty.CreateParty("quest_party_template_1", new CustomPartyComponent(), delegate(MobileParty mobileParty)
			{
				(mobileParty.PartyComponent as CustomPartyComponent).InitializeQuestPartyProperties(mobileParty, position, spawnRadius, homeSettlement, name, partyTemplate, owner, troopLimit, partyMountStringId, partyHarnessStringId, customPartyBaseSpeed, avoidHostileActions);
				mobileParty.ActualClan = clan;
			});
		}

		// Token: 0x06002805 RID: 10245 RVA: 0x000AA068 File Offset: 0x000A8268
		public static MobileParty CreateQuestParty(Vec2 position, float spawnRadius, Settlement homeSettlement, TextObject name, Clan clan, TroopRoster troopRoster, TroopRoster prisonerRoster, Hero owner, string partyMountStringId = "", string partyHarnessStringId = "", float customPartyBaseSpeed = 0f, bool avoidHostileActions = false)
		{
			return MobileParty.CreateParty("quest_party_template_1", new CustomPartyComponent(), delegate(MobileParty mobileParty)
			{
				(mobileParty.PartyComponent as CustomPartyComponent).InitializeQuestPartyProperties(mobileParty, position, spawnRadius, homeSettlement, name, troopRoster, prisonerRoster, owner, partyMountStringId, partyHarnessStringId, customPartyBaseSpeed, avoidHostileActions);
				mobileParty.ActualClan = clan;
			});
		}

		// Token: 0x06002806 RID: 10246 RVA: 0x000AA0F4 File Offset: 0x000A82F4
		private void InitializeQuestPartyProperties(MobileParty mobileParty, Vec2 position, float spawnRadius, Settlement homeSettlement, TextObject name, PartyTemplateObject partyTemplate, Hero owner, int troopLimit, string partyMountStringId, string partyHarnessStringId, float customPartyBaseSpeed, bool avoidHostileActions)
		{
			this._name = name;
			this._homeSettlement = homeSettlement;
			this._owner = owner;
			mobileParty.InitializeMobilePartyAroundPosition(partyTemplate, position, spawnRadius, 0f, troopLimit);
			mobileParty.Party.Visuals.SetMapIconAsDirty();
			this._partyMountStringId = partyMountStringId;
			this._partyHarnessStringId = partyHarnessStringId;
			this.SetBaseSpeed(customPartyBaseSpeed);
			this.AvoidHostileActions = avoidHostileActions;
		}

		// Token: 0x06002807 RID: 10247 RVA: 0x000AA15C File Offset: 0x000A835C
		private void InitializeQuestPartyProperties(MobileParty mobileParty, Vec2 position, float spawnRadius, Settlement homeSettlement, TextObject name, TroopRoster troopRoster, TroopRoster prisonerRoster, Hero owner, string partyMountStringId, string partyHarnessStringId, float customPartyBaseSpeed, bool avoidHostileActions)
		{
			this._name = name;
			this._homeSettlement = homeSettlement;
			this._owner = owner;
			mobileParty.InitializeMobilePartyAroundPosition(troopRoster, prisonerRoster, position, spawnRadius, 0f);
			mobileParty.Party.Visuals.SetMapIconAsDirty();
			this._partyMountStringId = partyMountStringId;
			this._partyHarnessStringId = partyHarnessStringId;
			this.SetBaseSpeed(customPartyBaseSpeed);
			this.AvoidHostileActions = avoidHostileActions;
		}

		// Token: 0x06002808 RID: 10248 RVA: 0x000AA1C2 File Offset: 0x000A83C2
		public void SetBaseSpeed(float speed)
		{
			this._customPartyBaseSpeed = speed;
			MobileParty mobileParty = base.MobileParty;
			if (mobileParty == null)
			{
				return;
			}
			mobileParty.UpdateVersionNo();
		}

		// Token: 0x06002809 RID: 10249 RVA: 0x000AA1DB File Offset: 0x000A83DB
		public override void GetMountAndHarnessVisualIdsForPartyIcon(PartyBase party, out string mountStringId, out string harnessStringId)
		{
			mountStringId = this._partyMountStringId;
			harnessStringId = this._partyHarnessStringId;
		}

		// Token: 0x04000C3A RID: 3130
		private const string StringId = "quest_party_template_1";

		// Token: 0x04000C3B RID: 3131
		[SaveableField(10)]
		private TextObject _name;

		// Token: 0x04000C3C RID: 3132
		[SaveableField(20)]
		private Settlement _homeSettlement;

		// Token: 0x04000C3D RID: 3133
		[SaveableField(30)]
		private Hero _owner;

		// Token: 0x04000C3E RID: 3134
		[SaveableField(40)]
		private float _customPartyBaseSpeed;

		// Token: 0x04000C3F RID: 3135
		[SaveableField(50)]
		private string _partyMountStringId;

		// Token: 0x04000C40 RID: 3136
		[SaveableField(60)]
		private string _partyHarnessStringId;

		// Token: 0x04000C41 RID: 3137
		[SaveableField(70)]
		public bool AvoidHostileActions;
	}
}
