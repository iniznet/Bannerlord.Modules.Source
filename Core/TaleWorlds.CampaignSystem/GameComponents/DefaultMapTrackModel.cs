using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000118 RID: 280
	public class DefaultMapTrackModel : MapTrackModel
	{
		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x060015F6 RID: 5622 RVA: 0x00068342 File Offset: 0x00066542
		public override float MaxTrackLife
		{
			get
			{
				return 28f;
			}
		}

		// Token: 0x060015F7 RID: 5623 RVA: 0x0006834C File Offset: 0x0006654C
		public override float GetMaxTrackSpottingDistanceForMainParty()
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, false, null);
			SkillHelper.AddSkillBonusForParty(DefaultSkills.Scouting, DefaultSkillEffects.TrackingRadius, MobileParty.MainParty, ref explainedNumber);
			PerkHelper.AddPerkBonusForParty(DefaultPerks.Scouting.Ranger, MobileParty.MainParty, true, ref explainedNumber);
			return explainedNumber.ResultNumber;
		}

		// Token: 0x060015F8 RID: 5624 RVA: 0x00068398 File Offset: 0x00066598
		public override bool CanPartyLeaveTrack(MobileParty mobileParty)
		{
			return mobileParty.SiegeEvent == null && mobileParty.MapEvent == null && !mobileParty.IsGarrison && !mobileParty.IsMilitia && !mobileParty.IsBanditBossParty && !mobileParty.IsMainParty && (mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty);
		}

		// Token: 0x060015F9 RID: 5625 RVA: 0x000683F0 File Offset: 0x000665F0
		public override int GetTrackLife(MobileParty mobileParty)
		{
			bool flag = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(mobileParty.CurrentNavigationFace) == TerrainType.Snow;
			int num = mobileParty.MemberRoster.TotalManCount + mobileParty.PrisonRoster.TotalManCount;
			float num2 = MathF.Min(1f, (0.5f * MBRandom.RandomFloat + 0.5f + (float)num * 0.007f) / 2f) * (flag ? 0.5f : 1f);
			if (MobileParty.MainParty.HasPerk(DefaultPerks.Scouting.Tracker, false))
			{
				num2 = MathF.Min(1f, num2 * (1f + DefaultPerks.Scouting.Tracker.PrimaryBonus));
			}
			return MathF.Round(Campaign.Current.Models.MapTrackModel.MaxTrackLife * num2);
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x000684B4 File Offset: 0x000666B4
		public override float GetTrackDetectionDifficultyForMainParty(Track track, float trackSpottingDistance)
		{
			int size = track.Size;
			float elapsedHoursUntilNow = track.CreationTime.ElapsedHoursUntilNow;
			float num = (track.Position - MobileParty.MainParty.Position2D).Length / trackSpottingDistance;
			float num2 = -75f + elapsedHoursUntilNow / this.MaxTrackLife * 100f + num * 100f + MathF.Max(0f, 100f - (float)size) * (CampaignTime.Now.IsNightTime ? 10f : 1f);
			if (MobileParty.MainParty.HasPerk(DefaultPerks.Scouting.Ranger, true))
			{
				num2 -= num2 * DefaultPerks.Scouting.Ranger.SecondaryBonus;
			}
			return num2;
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x00068564 File Offset: 0x00066764
		public override float GetSkillFromTrackDetected(Track track)
		{
			float num = 0.2f * (1f + track.CreationTime.ElapsedHoursUntilNow) * (1f + 0.02f * MathF.Max(0f, 100f - (float)track.NumberOfAllMembers));
			if (track.IsEnemy)
			{
				num *= ((track.PartyType == Track.PartyTypeEnum.Lord) ? 10f : ((track.PartyType == Track.PartyTypeEnum.Bandit) ? 4f : ((track.PartyType == Track.PartyTypeEnum.Caravan) ? 3f : 2f)));
			}
			return num;
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x000685F0 File Offset: 0x000667F0
		public override float GetSkipTrackChance(MobileParty mobileParty)
		{
			float num = 0.5f;
			float num2 = (float)(mobileParty.MemberRoster.TotalManCount + mobileParty.PrisonRoster.TotalManCount);
			return MathF.Max(num - num2 * 0.01f, 0f);
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x00068630 File Offset: 0x00066830
		public override TextObject TrackTitle(Track track)
		{
			if (track.IsPointer)
			{
				return track.PartyName;
			}
			Hero effectiveScout = MobileParty.MainParty.EffectiveScout;
			if (effectiveScout == null || effectiveScout.GetSkillValue(DefaultSkills.Scouting) <= 270)
			{
				return DefaultMapTrackModel._defaultTrackTitle;
			}
			return track.PartyName;
		}

		// Token: 0x060015FE RID: 5630 RVA: 0x0006867C File Offset: 0x0006687C
		private string UncertainifyNumber(float num, float baseNum, int skillLevel)
		{
			float num2 = baseNum * MathF.Max(0f, 1f - (float)(skillLevel / 30) * 0.1f);
			float num3 = num - num % num2;
			float num4 = num3 + num2;
			if (num2 < 0.0001f)
			{
				return num.ToString();
			}
			return num3.ToString("0.0") + "-" + num4.ToString("0.0");
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x000686E4 File Offset: 0x000668E4
		private string UncertainifyNumber(int num, int baseNum, int skillLevel)
		{
			int num2 = MathF.Round((float)baseNum * MathF.Max(0f, 1f - (float)(skillLevel / 30) * 0.1f));
			if (num2 <= 1)
			{
				return num.ToString();
			}
			int num3 = num - num % num2;
			int num4 = num3 + num2;
			if (num3 == 0)
			{
				num3 = 1;
			}
			if (num3 >= num4)
			{
				return num.ToString();
			}
			return num3.ToString() + "-" + num4.ToString();
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x00068754 File Offset: 0x00066954
		public override IEnumerable<ValueTuple<TextObject, string>> GetTrackDescription(Track track)
		{
			List<ValueTuple<TextObject, string>> list = new List<ValueTuple<TextObject, string>>();
			if (!track.IsPointer && track.IsAlive)
			{
				Hero effectiveScout = MobileParty.MainParty.EffectiveScout;
				int num = ((effectiveScout != null) ? effectiveScout.GetSkillValue(DefaultSkills.Scouting) : 0);
				if (num >= 25)
				{
					int num2 = track.NumberOfAllMembers + track.NumberOfPrisoners;
					list.Add(new ValueTuple<TextObject, string>(new TextObject("{=rmydcPP3}Party Size:", null), this.UncertainifyNumber(num2, 10, num)));
				}
				if (num >= 50)
				{
					TextObject textObject = new TextObject("{=Lak0x7Sa}{HOURS} {?HOURS==1}hour{?}hours{\\?}", null);
					int num3 = MathF.Ceiling(track.CreationTime.ElapsedHoursUntilNow);
					textObject.SetTextVariable("HOURS", num3);
					list.Add(new ValueTuple<TextObject, string>(new TextObject("{=0aU9dtvV}Time:", null), textObject.ToString()));
				}
				if (num >= 75)
				{
					list.Add(new ValueTuple<TextObject, string>(new TextObject("{=PThYJE2U}Party Speed:", null), this.UncertainifyNumber(MathF.Round(track.Speed, 2), 1f, num)));
				}
				if (num >= 100)
				{
					list.Add(new ValueTuple<TextObject, string>(new TextObject("{=ZULIWupm}Mounted Troops:", null), this.UncertainifyNumber(track.NumberOfMenWithHorse, 10, num)));
				}
				if (num >= 125 && num < 250)
				{
					list.Add(new ValueTuple<TextObject, string>(new TextObject("{=1pdBdqKn}Party Type:", null), GameTexts.FindText("str_party_type", track.PartyType.ToString()).ToString()));
				}
				if (num >= 150)
				{
					list.Add(new ValueTuple<TextObject, string>(new TextObject("{=pHrxeTdc}Prisoners:", null), this.UncertainifyNumber(track.NumberOfPrisoners, 10, num)));
				}
				if (num >= 175)
				{
					list.Add(new ValueTuple<TextObject, string>(new TextObject("{=aa1yFm6q}Pack Animals:", null), this.UncertainifyNumber(track.NumberOfPackAnimals, 10, num)));
				}
				if (num >= 200)
				{
					TextObject textObject2 = (track.IsEnemy ? GameTexts.FindText("str_yes", null) : GameTexts.FindText("str_no", null));
					list.Add(new ValueTuple<TextObject, string>(new TextObject("{=6REUNz1g}Enemy Party:", null), textObject2.ToString()));
				}
				if (num >= 225)
				{
					list.Add(new ValueTuple<TextObject, string>(new TextObject("{=dicpCcb2}Party Culture:", null), track.Culture.Name.ToString()));
				}
				if (num >= 250)
				{
					list.Add(new ValueTuple<TextObject, string>(new TextObject("{=BVIm1HPw}Party Name:", null), track.PartyName.ToString()));
				}
			}
			return list;
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x000689B4 File Offset: 0x00066BB4
		public override uint GetTrackColor(Track track)
		{
			if (track.IsPointer)
			{
				return new Vec3(1f, 1f, 1f, -1f).ToARGB;
			}
			Vec3 vec = new Vec3(0.6f, 0.95f, 0.2f, -1f);
			Vec3 vec2 = new Vec3(0.45f, 0.55f, 0.2f, -1f);
			Vec3 vec3 = new Vec3(0.15f, 0.25f, 0.4f, -1f);
			Vec3 vec4 = Vec3.Zero;
			if (track.IsEnemy)
			{
				Hero effectiveScout = MobileParty.MainParty.EffectiveScout;
				if (effectiveScout != null && effectiveScout.GetSkillValue(DefaultSkills.Scouting) > 240)
				{
					vec = new Vec3(0.99f, 0.5f, 0.1f, -1f);
					vec2 = new Vec3(0.75f, 0.4f, 0.3f, -1f);
					vec3 = new Vec3(0.5f, 0.1f, 0.4f, -1f);
				}
			}
			float num = MathF.Min(track.CreationTime.ElapsedHoursUntilNow / Campaign.Current.Models.MapTrackModel.MaxTrackLife, 1f);
			if (num < 0.35f)
			{
				num /= 0.35f;
				vec4 = num * vec2 + (1f - num) * vec;
			}
			else
			{
				num -= 0.35f;
				num /= 0.65f;
				vec4 = num * vec3 + (1f - num) * vec2;
			}
			return vec4.ToARGB;
		}

		// Token: 0x06001602 RID: 5634 RVA: 0x00068B54 File Offset: 0x00066D54
		public override float GetTrackScale(Track track)
		{
			if (track.IsPointer)
			{
				return 1f;
			}
			float num = 0.1f + 0.001f * (float)(track.NumberOfAllMembers + track.NumberOfPrisoners);
			return MathF.Min(1f, num);
		}

		// Token: 0x040007AC RID: 1964
		private const float MinimumTrackSize = 0.1f;

		// Token: 0x040007AD RID: 1965
		private const float MaximumTrackSize = 1f;

		// Token: 0x040007AE RID: 1966
		private static TextObject _defaultTrackTitle = new TextObject("{=maptrack}Track", null);
	}
}
