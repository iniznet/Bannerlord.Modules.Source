using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200008B RID: 139
	[SaveableInterface(22001)]
	public interface IFaction
	{
		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06001084 RID: 4228
		TextObject Name { get; }

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06001085 RID: 4229
		string StringId { get; }

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06001086 RID: 4230
		MBGUID Id { get; }

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06001087 RID: 4231
		TextObject InformalName { get; }

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06001088 RID: 4232
		string EncyclopediaLink { get; }

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06001089 RID: 4233
		TextObject EncyclopediaLinkWithName { get; }

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x0600108A RID: 4234
		TextObject EncyclopediaText { get; }

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x0600108B RID: 4235
		CultureObject Culture { get; }

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x0600108C RID: 4236
		Vec2 InitialPosition { get; }

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x0600108D RID: 4237
		uint LabelColor { get; }

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x0600108E RID: 4238
		uint Color { get; }

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x0600108F RID: 4239
		uint Color2 { get; }

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06001090 RID: 4240
		uint AlternativeColor { get; }

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06001091 RID: 4241
		uint AlternativeColor2 { get; }

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06001092 RID: 4242
		CharacterObject BasicTroop { get; }

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06001093 RID: 4243
		Hero Leader { get; }

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06001094 RID: 4244
		Banner Banner { get; }

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06001095 RID: 4245
		MBReadOnlyList<Settlement> Settlements { get; }

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06001096 RID: 4246
		MBReadOnlyList<Town> Fiefs { get; }

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06001097 RID: 4247
		MBReadOnlyList<Hero> Lords { get; }

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06001098 RID: 4248
		MBReadOnlyList<Hero> Heroes { get; }

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06001099 RID: 4249
		MBReadOnlyList<WarPartyComponent> WarPartyComponents { get; }

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x0600109A RID: 4250
		bool IsBanditFaction { get; }

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x0600109B RID: 4251
		bool IsMinorFaction { get; }

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x0600109C RID: 4252
		bool IsKingdomFaction { get; }

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x0600109D RID: 4253
		bool IsRebelClan { get; }

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x0600109E RID: 4254
		bool IsClan { get; }

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x0600109F RID: 4255
		bool IsOutlaw { get; }

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x060010A0 RID: 4256
		bool IsMapFaction { get; }

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x060010A1 RID: 4257
		IFaction MapFaction { get; }

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x060010A2 RID: 4258
		float TotalStrength { get; }

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x060010A3 RID: 4259
		Settlement FactionMidSettlement { get; }

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x060010A4 RID: 4260
		float DistanceToClosestNonAllyFortification { get; }

		// Token: 0x060010A5 RID: 4261
		bool IsAtWarWith(IFaction other);

		// Token: 0x060010A6 RID: 4262
		StanceLink GetStanceWith(IFaction other);

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x060010A7 RID: 4263
		IEnumerable<StanceLink> Stances { get; }

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x060010A8 RID: 4264
		// (set) Token: 0x060010A9 RID: 4265
		int TributeWallet { get; set; }

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x060010AA RID: 4266
		// (set) Token: 0x060010AB RID: 4267
		float MainHeroCrimeRating { get; set; }

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x060010AC RID: 4268
		float DailyCrimeRatingChange { get; }

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x060010AD RID: 4269
		float Aggressiveness { get; }

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x060010AE RID: 4270
		bool IsEliminated { get; }

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x060010AF RID: 4271
		ExplainedNumber DailyCrimeRatingChangeExplained { get; }

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x060010B0 RID: 4272
		// (set) Token: 0x060010B1 RID: 4273
		CampaignTime NotAttackableByPlayerUntilTime { get; set; }

		// Token: 0x060010B2 RID: 4274
		void ConsiderSiegesAndMapEvents(IFaction factionToConsiderAgainst);
	}
}
