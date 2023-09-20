using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using psai.net;

namespace psai.Editor
{
	[Serializable]
	public class Theme : PsaiMusicEntity, ICloneable
	{
		public static bool ConvertPlaycountVsRandomWeightingToBooleanPlaycountPreferred(float weightingPlaycountVsRandom)
		{
			return weightingPlaycountVsRandom >= Theme.PLAYCOUNT_VS_RANDOM_WEIGHTING_IF_PLAYCOUNT_PREFERRED;
		}

		public override string GetClassString()
		{
			if (this.ThemeTypeInt == 6)
			{
				return "Highlight Layer";
			}
			return "Theme";
		}

		public override List<PsaiMusicEntity> GetChildren()
		{
			List<PsaiMusicEntity> list = new List<PsaiMusicEntity>();
			for (int i = 0; i < this.Groups.Count; i++)
			{
				list.Add(this._groups[i]);
			}
			return list;
		}

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

		public string Description { get; set; }

		public int ThemeTypeInt { get; set; }

		public List<int> Serialization_ManuallyBlockedThemeIds { get; set; }

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

		public int MusicPhaseSecondsAfterRest { get; set; }

		public int MusicPhaseSecondsGeneral { get; set; }

		public int RestSecondsMin { get; set; }

		public int RestSecondsMax { get; set; }

		public int FadeoutMs { get; set; }

		public int Priority { get; set; }

		public float WeightingSwitchGroups { get; set; }

		public float WeightingIntensityVsVariance { get; set; }

		public float WeightingLowPlaycountVsRandom { get; set; }

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

		public Theme()
		{
			this.Initialize();
		}

		public Theme(int id)
		{
			this.Initialize();
			this.Id = id;
		}

		public Theme(int id, string name)
		{
			this.Initialize();
			this.Id = id;
			base.Name = name;
		}

		public override PsaiMusicEntity GetParent()
		{
			return null;
		}

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

		public override string ToString()
		{
			return "Theme '" + base.Name + "'";
		}

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

		public void DeleteGroup(Group group)
		{
			if (group != this._groups[0])
			{
				this._groups.Remove(group);
			}
		}

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

		public override CompatibilitySetting GetCompatibilitySetting(PsaiMusicEntity targetEntity)
		{
			if (targetEntity is Theme && this.ManuallyBlockedTargetThemes.Contains((Theme)targetEntity))
			{
				return CompatibilitySetting.blocked;
			}
			return CompatibilitySetting.neutral;
		}

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

		public override int GetIndexPositionWithinParentEntity(PsaiProject parentProject)
		{
			return parentProject.Themes.IndexOf(this);
		}

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

		public override PsaiMusicEntity ShallowCopy()
		{
			return (Theme)base.MemberwiseClone();
		}

		internal static float PLAYCOUNT_VS_RANDOM_WEIGHTING_IF_PLAYCOUNT_PREFERRED = 0.8f;

		private static readonly string DEFAULT_NAME = "new_theme";

		private static readonly int DEFAULT_PRIORITY = 1;

		private static readonly int DEFAULT_REST_SECONDS_MIN = 30;

		private static readonly int DEFAULT_REST_SECONDS_MAX = 60;

		private static readonly int DEFAULT_FADEOUT_MS = 20;

		private static readonly int DEFAULT_THEME_DURATION_SECONDS = 60;

		private static readonly float DEFAULT_INTENSITY_AFTER_REST = 0.5f;

		private static readonly int DEFAULT_THEME_DURATION_SECONDS_AFTER_REST = 40;

		private static readonly float DEFAULT_WEIGHTING_COMPATIBILITY = 0.5f;

		private static readonly float DEFAULT_WEIGHTING_INTENSITY = 0.5f;

		private static readonly float DEFAULT_WEIGHTING_LOW_PLAYCOUNT_VS_RANDOM = 0f;

		private static readonly int DEFAULT_THEMETYPEINT = 1;

		private List<Group> _groups;

		private HashSet<Theme> _manuallyBlockedThemes = new HashSet<Theme>();

		private float _intensityAfterRest;

		private int _id;
	}
}
