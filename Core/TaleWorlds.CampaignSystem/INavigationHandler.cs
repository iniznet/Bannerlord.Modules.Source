using System;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200008D RID: 141
	public interface INavigationHandler
	{
		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x060010B4 RID: 4276
		bool PartyEnabled { get; }

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x060010B5 RID: 4277
		bool InventoryEnabled { get; }

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x060010B6 RID: 4278
		bool QuestsEnabled { get; }

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x060010B7 RID: 4279
		bool CharacterDeveloperEnabled { get; }

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x060010B8 RID: 4280
		NavigationPermissionItem ClanPermission { get; }

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x060010B9 RID: 4281
		NavigationPermissionItem KingdomPermission { get; }

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x060010BA RID: 4282
		bool EscapeMenuEnabled { get; }

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x060010BB RID: 4283
		bool PartyActive { get; }

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x060010BC RID: 4284
		bool InventoryActive { get; }

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x060010BD RID: 4285
		bool QuestsActive { get; }

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x060010BE RID: 4286
		bool CharacterDeveloperActive { get; }

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x060010BF RID: 4287
		bool ClanActive { get; }

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x060010C0 RID: 4288
		bool KingdomActive { get; }

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x060010C1 RID: 4289
		bool EscapeMenuActive { get; }

		// Token: 0x060010C2 RID: 4290
		void OpenQuests();

		// Token: 0x060010C3 RID: 4291
		void OpenQuests(QuestBase quest);

		// Token: 0x060010C4 RID: 4292
		void OpenQuests(IssueBase issue);

		// Token: 0x060010C5 RID: 4293
		void OpenQuests(JournalLogEntry log);

		// Token: 0x060010C6 RID: 4294
		void OpenInventory();

		// Token: 0x060010C7 RID: 4295
		void OpenParty();

		// Token: 0x060010C8 RID: 4296
		void OpenCharacterDeveloper();

		// Token: 0x060010C9 RID: 4297
		void OpenCharacterDeveloper(Hero hero);

		// Token: 0x060010CA RID: 4298
		void OpenKingdom();

		// Token: 0x060010CB RID: 4299
		void OpenKingdom(Army army);

		// Token: 0x060010CC RID: 4300
		void OpenKingdom(Settlement settlement);

		// Token: 0x060010CD RID: 4301
		void OpenKingdom(Clan clan);

		// Token: 0x060010CE RID: 4302
		void OpenKingdom(PolicyObject policy);

		// Token: 0x060010CF RID: 4303
		void OpenKingdom(IFaction faction);

		// Token: 0x060010D0 RID: 4304
		void OpenClan();

		// Token: 0x060010D1 RID: 4305
		void OpenClan(Hero hero);

		// Token: 0x060010D2 RID: 4306
		void OpenClan(PartyBase party);

		// Token: 0x060010D3 RID: 4307
		void OpenClan(Settlement settlement);

		// Token: 0x060010D4 RID: 4308
		void OpenClan(Workshop workshop);

		// Token: 0x060010D5 RID: 4309
		void OpenClan(Alley alley);

		// Token: 0x060010D6 RID: 4310
		void OpenEscapeMenu();
	}
}
