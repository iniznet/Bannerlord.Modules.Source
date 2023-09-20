using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x020000A0 RID: 160
	public sealed class SkillEffect : PropertyObject
	{
		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x0600116D RID: 4461 RVA: 0x0005024E File Offset: 0x0004E44E
		public static MBReadOnlyList<SkillEffect> All
		{
			get
			{
				return Campaign.Current.AllSkillEffects;
			}
		}

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x0600116E RID: 4462 RVA: 0x0005025A File Offset: 0x0004E45A
		// (set) Token: 0x0600116F RID: 4463 RVA: 0x00050262 File Offset: 0x0004E462
		public SkillEffect.PerkRole PrimaryRole { get; private set; }

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x06001170 RID: 4464 RVA: 0x0005026B File Offset: 0x0004E46B
		// (set) Token: 0x06001171 RID: 4465 RVA: 0x00050273 File Offset: 0x0004E473
		public SkillEffect.PerkRole SecondaryRole { get; private set; }

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06001172 RID: 4466 RVA: 0x0005027C File Offset: 0x0004E47C
		// (set) Token: 0x06001173 RID: 4467 RVA: 0x00050284 File Offset: 0x0004E484
		public float PrimaryBonus { get; private set; }

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06001174 RID: 4468 RVA: 0x0005028D File Offset: 0x0004E48D
		// (set) Token: 0x06001175 RID: 4469 RVA: 0x00050295 File Offset: 0x0004E495
		public float SecondaryBonus { get; private set; }

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06001176 RID: 4470 RVA: 0x0005029E File Offset: 0x0004E49E
		// (set) Token: 0x06001177 RID: 4471 RVA: 0x000502A6 File Offset: 0x0004E4A6
		public SkillEffect.EffectIncrementType IncrementType { get; private set; }

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06001178 RID: 4472 RVA: 0x000502AF File Offset: 0x0004E4AF
		// (set) Token: 0x06001179 RID: 4473 RVA: 0x000502B7 File Offset: 0x0004E4B7
		public SkillObject[] EffectedSkills { get; private set; }

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x0600117A RID: 4474 RVA: 0x000502C0 File Offset: 0x0004E4C0
		// (set) Token: 0x0600117B RID: 4475 RVA: 0x000502C8 File Offset: 0x0004E4C8
		public float PrimaryBaseValue { get; private set; }

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x0600117C RID: 4476 RVA: 0x000502D1 File Offset: 0x0004E4D1
		// (set) Token: 0x0600117D RID: 4477 RVA: 0x000502D9 File Offset: 0x0004E4D9
		public float SecondaryBaseValue { get; private set; }

		// Token: 0x0600117E RID: 4478 RVA: 0x000502E2 File Offset: 0x0004E4E2
		public SkillEffect(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x000502EC File Offset: 0x0004E4EC
		public void Initialize(TextObject description, SkillObject[] effectedSkills, SkillEffect.PerkRole primaryRole = SkillEffect.PerkRole.None, float primaryBonus = 0f, SkillEffect.PerkRole secondaryRole = SkillEffect.PerkRole.None, float secondaryBonus = 0f, SkillEffect.EffectIncrementType incrementType = SkillEffect.EffectIncrementType.AddFactor, float primaryBaseValue = 0f, float secondaryBaseValue = 0f)
		{
			base.Initialize(TextObject.Empty, description);
			this.PrimaryRole = primaryRole;
			this.SecondaryRole = secondaryRole;
			this.PrimaryBonus = primaryBonus;
			this.SecondaryBonus = secondaryBonus;
			this.IncrementType = incrementType;
			this.EffectedSkills = effectedSkills;
			this.PrimaryBaseValue = primaryBaseValue;
			this.SecondaryBaseValue = secondaryBaseValue;
			base.AfterInitialized();
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x00050349 File Offset: 0x0004E549
		public float GetPrimaryValue(int skillLevel)
		{
			return MathF.Max(0f, this.PrimaryBaseValue + this.PrimaryBonus * (float)skillLevel);
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x00050365 File Offset: 0x0004E565
		public float GetSecondaryValue(int skillLevel)
		{
			return MathF.Max(0f, this.SecondaryBaseValue + this.SecondaryBonus * (float)skillLevel);
		}

		// Token: 0x020004CD RID: 1229
		public enum EffectIncrementType
		{
			// Token: 0x040014B5 RID: 5301
			Invalid = -1,
			// Token: 0x040014B6 RID: 5302
			Add,
			// Token: 0x040014B7 RID: 5303
			AddFactor
		}

		// Token: 0x020004CE RID: 1230
		public enum PerkRole
		{
			// Token: 0x040014B9 RID: 5305
			None,
			// Token: 0x040014BA RID: 5306
			Ruler,
			// Token: 0x040014BB RID: 5307
			ClanLeader,
			// Token: 0x040014BC RID: 5308
			Governor,
			// Token: 0x040014BD RID: 5309
			ArmyCommander,
			// Token: 0x040014BE RID: 5310
			PartyLeader,
			// Token: 0x040014BF RID: 5311
			PartyOwner,
			// Token: 0x040014C0 RID: 5312
			Surgeon,
			// Token: 0x040014C1 RID: 5313
			Engineer,
			// Token: 0x040014C2 RID: 5314
			Scout,
			// Token: 0x040014C3 RID: 5315
			Quartermaster,
			// Token: 0x040014C4 RID: 5316
			PartyMember,
			// Token: 0x040014C5 RID: 5317
			Personal,
			// Token: 0x040014C6 RID: 5318
			Captain,
			// Token: 0x040014C7 RID: 5319
			NumberOfPerkRoles
		}
	}
}
