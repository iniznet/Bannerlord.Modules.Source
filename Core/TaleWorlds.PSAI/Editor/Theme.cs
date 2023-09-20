using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using psai.net;

namespace psai.Editor
{
	// Token: 0x0200000B RID: 11
	[Serializable]
	public class Theme : PsaiMusicEntity, ICloneable
	{
		// Token: 0x060000EA RID: 234 RVA: 0x000052DE File Offset: 0x000034DE
		public static bool ConvertPlaycountVsRandomWeightingToBooleanPlaycountPreferred(float weightingPlaycountVsRandom)
		{
			return weightingPlaycountVsRandom >= Theme.PLAYCOUNT_VS_RANDOM_WEIGHTING_IF_PLAYCOUNT_PREFERRED;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000052EB File Offset: 0x000034EB
		public override string GetClassString()
		{
			if (this.ThemeTypeInt == 6)
			{
				return "Highlight Layer";
			}
			return "Theme";
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00005304 File Offset: 0x00003504
		public override List<PsaiMusicEntity> GetChildren()
		{
			List<PsaiMusicEntity> list = new List<PsaiMusicEntity>();
			for (int i = 0; i < this.Groups.Count; i++)
			{
				list.Add(this._groups[i]);
			}
			return list;
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000ED RID: 237 RVA: 0x00005340 File Offset: 0x00003540
		// (set) Token: 0x060000EE RID: 238 RVA: 0x00005348 File Offset: 0x00003548
		public int Id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
				this.SetAsParentThemeForAllGroupsAndSegments();
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000EF RID: 239 RVA: 0x00005357 File Offset: 0x00003557
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x0000535F File Offset: 0x0000355F
		public string Description { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x00005368 File Offset: 0x00003568
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x00005370 File Offset: 0x00003570
		public int ThemeTypeInt { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x00005379 File Offset: 0x00003579
		// (set) Token: 0x060000F4 RID: 244 RVA: 0x00005381 File Offset: 0x00003581
		public List<int> Serialization_ManuallyBlockedThemeIds { get; set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00005393 File Offset: 0x00003593
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x0000538A File Offset: 0x0000358A
		[XmlIgnore]
		public HashSet<Theme> ManuallyBlockedTargetThemes
		{
			get
			{
				return this._manuallyBlockedThemes;
			}
			private set
			{
				this._manuallyBlockedThemes = value;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x0000539B File Offset: 0x0000359B
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x000053A3 File Offset: 0x000035A3
		public float IntensityAfterRest
		{
			get
			{
				return this._intensityAfterRest;
			}
			set
			{
				this._intensityAfterRest = value;
				if (this._intensityAfterRest <= 0f)
				{
					this._intensityAfterRest = 0.01f;
				}
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x000053C4 File Offset: 0x000035C4
		// (set) Token: 0x060000FA RID: 250 RVA: 0x000053CC File Offset: 0x000035CC
		public int MusicPhaseSecondsAfterRest { get; set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000FB RID: 251 RVA: 0x000053D5 File Offset: 0x000035D5
		// (set) Token: 0x060000FC RID: 252 RVA: 0x000053DD File Offset: 0x000035DD
		public int MusicPhaseSecondsGeneral { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000FD RID: 253 RVA: 0x000053E6 File Offset: 0x000035E6
		// (set) Token: 0x060000FE RID: 254 RVA: 0x000053EE File Offset: 0x000035EE
		public int RestSecondsMin { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000FF RID: 255 RVA: 0x000053F7 File Offset: 0x000035F7
		// (set) Token: 0x06000100 RID: 256 RVA: 0x000053FF File Offset: 0x000035FF
		public int RestSecondsMax { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00005408 File Offset: 0x00003608
		// (set) Token: 0x06000102 RID: 258 RVA: 0x00005410 File Offset: 0x00003610
		public int FadeoutMs { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00005419 File Offset: 0x00003619
		// (set) Token: 0x06000104 RID: 260 RVA: 0x00005421 File Offset: 0x00003621
		public int Priority { get; set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000105 RID: 261 RVA: 0x0000542A File Offset: 0x0000362A
		// (set) Token: 0x06000106 RID: 262 RVA: 0x00005432 File Offset: 0x00003632
		public float WeightingSwitchGroups { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000107 RID: 263 RVA: 0x0000543B File Offset: 0x0000363B
		// (set) Token: 0x06000108 RID: 264 RVA: 0x00005443 File Offset: 0x00003643
		public float WeightingIntensityVsVariance { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000109 RID: 265 RVA: 0x0000544C File Offset: 0x0000364C
		// (set) Token: 0x0600010A RID: 266 RVA: 0x00005454 File Offset: 0x00003654
		public float WeightingLowPlaycountVsRandom { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600010B RID: 267 RVA: 0x0000545D File Offset: 0x0000365D
		// (set) Token: 0x0600010C RID: 268 RVA: 0x00005465 File Offset: 0x00003665
		public List<Group> Groups
		{
			get
			{
				return this._groups;
			}
			set
			{
				this._groups = value;
			}
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000546E File Offset: 0x0000366E
		public Theme()
		{
			this.Initialize();
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00005487 File Offset: 0x00003687
		public Theme(int id)
		{
			this.Initialize();
			this.Id = id;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000054A7 File Offset: 0x000036A7
		public Theme(int id, string name)
		{
			this.Initialize();
			this.Id = id;
			base.Name = name;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000054CE File Offset: 0x000036CE
		public override PsaiMusicEntity GetParent()
		{
			return null;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000054D4 File Offset: 0x000036D4
		private void Initialize()
		{
			base.Name = Theme.DEFAULT_NAME;
			this.ThemeTypeInt = Theme.DEFAULT_THEMETYPEINT;
			this.IntensityAfterRest = Theme.DEFAULT_INTENSITY_AFTER_REST;
			this.MusicPhaseSecondsAfterRest = Theme.DEFAULT_THEME_DURATION_SECONDS;
			this.MusicPhaseSecondsGeneral = Theme.DEFAULT_THEME_DURATION_SECONDS_AFTER_REST;
			this.WeightingSwitchGroups = Theme.DEFAULT_WEIGHTING_COMPATIBILITY;
			this.WeightingIntensityVsVariance = Theme.DEFAULT_WEIGHTING_INTENSITY;
			this.WeightingLowPlaycountVsRandom = Theme.DEFAULT_WEIGHTING_LOW_PLAYCOUNT_VS_RANDOM;
			this.Priority = Theme.DEFAULT_PRIORITY;
			this.RestSecondsMin = Theme.DEFAULT_REST_SECONDS_MIN;
			this.RestSecondsMax = Theme.DEFAULT_REST_SECONDS_MAX;
			this.FadeoutMs = Theme.DEFAULT_FADEOUT_MS;
			this._groups = new List<Group>();
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00005570 File Offset: 0x00003770
		public override string ToString()
		{
			return "Theme '" + base.Name + "'";
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00005588 File Offset: 0x00003788
		public bool AddGroup(Group groupToAdd)
		{
			using (List<Group>.Enumerator enumerator = this._groups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Name.Equals(groupToAdd.Name))
					{
						return false;
					}
				}
			}
			this._groups.Add(groupToAdd);
			foreach (Segment segment in this.GetSegmentsOfAllGroups())
			{
				segment.ThemeId = this.Id;
			}
			return true;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00005640 File Offset: 0x00003840
		public void DeleteGroup(Group group)
		{
			if (group != this._groups[0])
			{
				this._groups.Remove(group);
			}
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00005660 File Offset: 0x00003860
		public HashSet<Segment> GetSegmentsOfAllGroups()
		{
			HashSet<Segment> hashSet = new HashSet<Segment>();
			foreach (Group group in this._groups)
			{
				foreach (Segment segment in group.Segments)
				{
					hashSet.Add(segment);
				}
			}
			return hashSet;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x000056F4 File Offset: 0x000038F4
		public HashSet<string> GetAudioDataRelativeFilePathsUsedByThisTheme()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Segment segment in this.GetSegmentsOfAllGroups())
			{
				if (!hashSet.Contains(segment.AudioData.FilePathRelativeToProjectDir))
				{
					hashSet.Add(segment.AudioData.FilePathRelativeToProjectDir);
				}
			}
			return hashSet;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000576C File Offset: 0x0000396C
		public override CompatibilitySetting GetCompatibilitySetting(PsaiMusicEntity targetEntity)
		{
			if (targetEntity is Theme && this.ManuallyBlockedTargetThemes.Contains((Theme)targetEntity))
			{
				return CompatibilitySetting.blocked;
			}
			return CompatibilitySetting.neutral;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000578C File Offset: 0x0000398C
		public override CompatibilityType GetCompatibilityType(PsaiMusicEntity targetEntity, out CompatibilityReason reason)
		{
			if (targetEntity is Theme)
			{
				Theme theme = targetEntity as Theme;
				ThemeInterruptionBehavior themeInterruptionBehavior = Theme.GetThemeInterruptionBehavior((ThemeType)this.ThemeTypeInt, (ThemeType)theme.ThemeTypeInt);
				if (Theme.ThemeInterruptionBehaviorRequiresEvaluationOfSegmentCompatibilities(themeInterruptionBehavior))
				{
					if (this.ManuallyBlockedTargetThemes.Contains(targetEntity as Theme))
					{
						reason = CompatibilityReason.manual_setting_within_same_hierarchy;
						return CompatibilityType.blocked_manually;
					}
					reason = CompatibilityReason.default_behavior_of_psai;
					return CompatibilityType.allowed_implicitly;
				}
				else if (themeInterruptionBehavior == ThemeInterruptionBehavior.never)
				{
					reason = CompatibilityReason.target_theme_will_never_interrupt_source;
					return CompatibilityType.logically_impossible;
				}
			}
			reason = CompatibilityReason.not_set;
			return CompatibilityType.undefined;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x000057ED File Offset: 0x000039ED
		public override int GetIndexPositionWithinParentEntity(PsaiProject parentProject)
		{
			return parentProject.Themes.IndexOf(this);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x000057FC File Offset: 0x000039FC
		public override bool PropertyDifferencesAffectCompatibilities(PsaiMusicEntity otherEntity)
		{
			if (otherEntity is Theme)
			{
				Theme theme = otherEntity as Theme;
				if (this.ThemeTypeInt != theme.ThemeTypeInt)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000582C File Offset: 0x00003A2C
		public void SetAsParentThemeForAllGroupsAndSegments()
		{
			foreach (Group group in this.Groups)
			{
				group.Theme = this;
			}
			foreach (Segment segment in this.GetSegmentsOfAllGroups())
			{
				segment.ThemeId = this.Id;
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000058C4 File Offset: 0x00003AC4
		public Theme CreatePsaiDotNetVersion()
		{
			return new Theme
			{
				id = this.Id,
				Name = base.Name,
				themeType = (ThemeType)this.ThemeTypeInt,
				intensityAfterRest = this.IntensityAfterRest,
				musicDurationGeneral = this.MusicPhaseSecondsGeneral,
				musicDurationAfterRest = this.MusicPhaseSecondsAfterRest,
				restSecondsMin = this.RestSecondsMin,
				restSecondsMax = this.RestSecondsMax,
				priority = this.Priority,
				weightings = 
				{
					switchGroups = this.WeightingSwitchGroups,
					intensityVsVariety = this.WeightingIntensityVsVariance,
					lowPlaycountVsRandom = this.WeightingLowPlaycountVsRandom
				}
			};
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00005978 File Offset: 0x00003B78
		public static Theme getTestTheme1()
		{
			Theme theme = new Theme(1, "Forest");
			theme.ThemeTypeInt = 1;
			Group group = new Group(theme, "wald_streicher");
			Group group2 = new Group(theme, "wald_choir");
			Segment segment = new Segment(101, "wald_streicher_1", 1, 0.4f);
			Segment segment2 = new Segment(102, "wald_streicher_2", 2, 0.4f);
			Segment segment3 = new Segment(103, "wald_streicher_3", 2, 0.6f);
			Segment segment4 = new Segment(104, "wald_streicher_4", 4, 0.6f);
			Segment segment5 = new Segment(105, "wald_streicher_5", 1, 1f);
			Segment segment6 = new Segment(106, "wald_streicher_6", 4, 1f);
			Segment segment7 = new Segment(111, "wald_choir_1", 1, 0.4f);
			Segment segment8 = new Segment(112, "wald_choir_2", 2, 0.4f);
			Segment segment9 = new Segment(113, "wald_choir_3", 2, 0.6f);
			Segment segment10 = new Segment(114, "wald_choir_4", 1, 0.6f);
			Segment segment11 = new Segment(115, "wald_choir_5", 4, 1f);
			Segment segment12 = new Segment(116, "wald_choir_6", 4, 1f);
			group.AddSegment(segment);
			group.AddSegment(segment2);
			group.AddSegment(segment3);
			group.AddSegment(segment4);
			group.AddSegment(segment5);
			group.AddSegment(segment6);
			group2.AddSegment(segment7);
			group2.AddSegment(segment8);
			group2.AddSegment(segment9);
			group2.AddSegment(segment10);
			group2.AddSegment(segment11);
			group2.AddSegment(segment12);
			theme.AddGroup(group);
			theme.AddGroup(group2);
			return theme;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00005B0C File Offset: 0x00003D0C
		public static Theme getTestTheme2()
		{
			Theme theme = new Theme(2, "Cave");
			theme.ThemeTypeInt = 1;
			Group group = new Group(theme, "cave horns");
			Group group2 = new Group(theme, "cave choir");
			Segment segment = new Segment(201, "cave_horns_1", 1, 0.4f);
			Segment segment2 = new Segment(202, "cave_horns_2", 2, 0.4f);
			Segment segment3 = new Segment(203, "cave_horns_3", 2, 0.6f);
			Segment segment4 = new Segment(204, "cave_horns_4", 2, 0.6f);
			Segment segment5 = new Segment(205, "cave_horns_5", 2, 1f);
			Segment segment6 = new Segment(206, "cave_horns_6", 4, 1f);
			Segment segment7 = new Segment(211, "cave_choir_1", 1, 0.4f);
			Segment segment8 = new Segment(212, "cave_choir_2", 2, 0.4f);
			Segment segment9 = new Segment(213, "cave_choir_3", 2, 0.6f);
			Segment segment10 = new Segment(214, "cave_choir_4", 2, 0.6f);
			Segment segment11 = new Segment(215, "cave_choir_5", 2, 1f);
			Segment segment12 = new Segment(216, "cave_choir_6", 4, 1f);
			group.AddSegment(segment);
			group.AddSegment(segment2);
			group.AddSegment(segment3);
			group.AddSegment(segment4);
			group.AddSegment(segment5);
			group.AddSegment(segment6);
			group2.AddSegment(segment7);
			group2.AddSegment(segment8);
			group2.AddSegment(segment9);
			group2.AddSegment(segment10);
			group2.AddSegment(segment11);
			group2.AddSegment(segment12);
			theme.AddGroup(group);
			theme.AddGroup(group2);
			return theme;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00005CC4 File Offset: 0x00003EC4
		public override object Clone()
		{
			Theme theme = (Theme)base.MemberwiseClone();
			theme.Groups = new List<Group>();
			theme._manuallyBlockedThemes = new HashSet<Theme>();
			foreach (Group group in this.Groups)
			{
				theme.AddGroup((Group)group.Clone());
			}
			foreach (Theme theme2 in this._manuallyBlockedThemes)
			{
				theme._manuallyBlockedThemes.Add(theme2);
			}
			return theme;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00005D90 File Offset: 0x00003F90
		public override PsaiMusicEntity ShallowCopy()
		{
			return (Theme)base.MemberwiseClone();
		}

		// Token: 0x04000057 RID: 87
		internal static float PLAYCOUNT_VS_RANDOM_WEIGHTING_IF_PLAYCOUNT_PREFERRED = 0.8f;

		// Token: 0x04000058 RID: 88
		private static readonly string DEFAULT_NAME = "new_theme";

		// Token: 0x04000059 RID: 89
		private static readonly int DEFAULT_PRIORITY = 1;

		// Token: 0x0400005A RID: 90
		private static readonly int DEFAULT_REST_SECONDS_MIN = 30;

		// Token: 0x0400005B RID: 91
		private static readonly int DEFAULT_REST_SECONDS_MAX = 60;

		// Token: 0x0400005C RID: 92
		private static readonly int DEFAULT_FADEOUT_MS = 20;

		// Token: 0x0400005D RID: 93
		private static readonly int DEFAULT_THEME_DURATION_SECONDS = 60;

		// Token: 0x0400005E RID: 94
		private static readonly float DEFAULT_INTENSITY_AFTER_REST = 0.5f;

		// Token: 0x0400005F RID: 95
		private static readonly int DEFAULT_THEME_DURATION_SECONDS_AFTER_REST = 40;

		// Token: 0x04000060 RID: 96
		private static readonly float DEFAULT_WEIGHTING_COMPATIBILITY = 0.5f;

		// Token: 0x04000061 RID: 97
		private static readonly float DEFAULT_WEIGHTING_INTENSITY = 0.5f;

		// Token: 0x04000062 RID: 98
		private static readonly float DEFAULT_WEIGHTING_LOW_PLAYCOUNT_VS_RANDOM = 0f;

		// Token: 0x04000063 RID: 99
		private static readonly int DEFAULT_THEMETYPEINT = 1;

		// Token: 0x04000064 RID: 100
		private List<Group> _groups;

		// Token: 0x04000065 RID: 101
		private HashSet<Theme> _manuallyBlockedThemes = new HashSet<Theme>();

		// Token: 0x04000066 RID: 102
		private float _intensityAfterRest;

		// Token: 0x04000067 RID: 103
		private int _id;
	}
}
