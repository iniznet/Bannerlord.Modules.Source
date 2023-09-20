using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using psai.net;

namespace psai.Editor
{
	[Serializable]
	public class Segment : PsaiMusicEntity, ICloneable
	{
		public int Id { get; set; }

		public bool IsAutomaticBridgeSegment { get; set; }

		public float Intensity
		{
			get
			{
				return this._intensity;
			}
			set
			{
				if (value >= 0f && value <= 1f)
				{
					this._intensity = value;
				}
			}
		}

		public bool IsUsableAtStart { get; set; }

		public bool IsUsableInMiddle { get; set; }

		public bool IsUsableAtEnd { get; set; }

		public AudioData AudioData { get; set; }

		public bool CalculatePostAndPrebeatLengthBasedOnBeats
		{
			get
			{
				return this.AudioData.CalculatePostAndPrebeatLengthBasedOnBeats;
			}
			set
			{
				this.AudioData.CalculatePostAndPrebeatLengthBasedOnBeats = value;
			}
		}

		public int PreBeatLengthInSamples
		{
			get
			{
				return this.AudioData.PreBeatLengthInSamples;
			}
			set
			{
				this.AudioData.PreBeatLengthInSamples = value;
			}
		}

		public int PostBeatLengthInSamples
		{
			get
			{
				return this.AudioData.PostBeatLengthInSamples;
			}
			set
			{
				this.AudioData.PostBeatLengthInSamples = value;
			}
		}

		public float PreBeats
		{
			get
			{
				return this.AudioData.PreBeats;
			}
			set
			{
				this.AudioData.PreBeats = value;
			}
		}

		public float PostBeats
		{
			get
			{
				return this.AudioData.PostBeats;
			}
			set
			{
				this.AudioData.PostBeats = value;
			}
		}

		public float Bpm
		{
			get
			{
				return this.AudioData.Bpm;
			}
			set
			{
				this.AudioData.Bpm = value;
			}
		}

		public int SampleRate
		{
			get
			{
				return this.AudioData.SampleRate;
			}
			set
			{
				this.AudioData.SampleRate = value;
			}
		}

		public int TotalLengthInSamples
		{
			get
			{
				return this.AudioData.TotalLengthInSamples;
			}
		}

		public int BitsPerSample
		{
			get
			{
				return this.AudioData.BitsPerSample;
			}
			set
			{
				this.AudioData.BitsPerSample = value;
			}
		}

		public int ThemeId { get; set; }

		public List<int> Serialization_ManuallyBlockedSegmentIds { get; set; }

		public List<int> Serialization_ManuallyLinkedSegmentIds { get; set; }

		public CompatibilityType DefaultCompatibiltyAsFollower { get; set; }

		public override List<PsaiMusicEntity> GetChildren()
		{
			return null;
		}

		[XmlIgnore]
		public Group Group { get; set; }

		[XmlIgnore]
		public HashSet<Segment> ManuallyLinkedSnippets
		{
			get
			{
				return this._manuallyLinkedSnippets;
			}
			set
			{
				this._manuallyLinkedSnippets = value;
			}
		}

		[XmlIgnore]
		public HashSet<Segment> ManuallyBlockedSnippets
		{
			get
			{
				return this._manuallyBlockedSnippets;
			}
			set
			{
				this._manuallyBlockedSnippets = value;
			}
		}

		[XmlIgnore]
		public Dictionary<int, float> CompatibleSnippetsIds
		{
			get
			{
				return this._compatibleSnippetsIds;
			}
		}

		public override string GetClassString()
		{
			return "Segment";
		}

		public Segment()
		{
			this.init();
		}

		public Segment(int id, string name, int snippetTypes, float intensity)
		{
			this.init();
			this.Id = id;
			this.SetStartMiddleEndPropertiesFromBitfield(snippetTypes);
			base.Name = name;
			this.Intensity = intensity;
		}

		public Segment(int id, AudioData audioData)
		{
			this.init();
			this.AudioData = audioData;
			this.Id = id;
			base.Name = Path.GetFileNameWithoutExtension(Path.GetFileName(audioData.FilePathRelativeToProjectDir));
		}

		private void init()
		{
			this.Id = 1;
			base.Name = "new segment";
			this.DefaultCompatibiltyAsFollower = CompatibilityType.allowed_implicitly;
			this.SetStartMiddleEndPropertiesFromBitfield(3);
			this.Intensity = 0.5f;
			this.AudioData = new AudioData();
		}

		public override object Clone()
		{
			Segment segment = (Segment)base.MemberwiseClone();
			segment.AudioData = (AudioData)this.AudioData.Clone();
			segment.ManuallyBlockedSnippets = new HashSet<Segment>();
			segment.ManuallyLinkedSnippets = new HashSet<Segment>();
			foreach (Segment segment2 in this._manuallyBlockedSnippets)
			{
				segment._manuallyBlockedSnippets.Add(segment2);
			}
			foreach (Segment segment3 in this._manuallyLinkedSnippets)
			{
				segment._manuallyLinkedSnippets.Add(segment3);
			}
			return segment;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Segment '");
			stringBuilder.Append(base.Name);
			stringBuilder.Append("'");
			return stringBuilder.ToString();
		}

		public bool AddCompatibleSnippet(Segment snippet, float compatibility)
		{
			if (snippet != null && compatibility >= 0f && compatibility <= 1f)
			{
				this._compatibleSnippetsIds[snippet.Id] = compatibility;
				return true;
			}
			return false;
		}

		public override bool PropertyDifferencesAffectCompatibilities(PsaiMusicEntity otherEntity)
		{
			if (otherEntity is Segment)
			{
				Segment segment = otherEntity as Segment;
				if (!this.IsUsableAtStart.Equals(segment.IsUsableAtStart))
				{
					return true;
				}
				if (!this.IsUsableInMiddle.Equals(segment.IsUsableInMiddle))
				{
					return true;
				}
				if (!this.IsUsableAtEnd.Equals(segment.IsUsableAtEnd))
				{
					return true;
				}
				if (!this.IsAutomaticBridgeSegment.Equals(segment.IsAutomaticBridgeSegment))
				{
					return true;
				}
				if (!this.DefaultCompatibiltyAsFollower.Equals(segment.DefaultCompatibiltyAsFollower))
				{
					return true;
				}
			}
			return false;
		}

		public void BuildCompatibleSegmentsSet(PsaiProject project)
		{
			HashSet<Segment> segmentsOfAllThemes = project.GetSegmentsOfAllThemes();
			this.CompatibleSnippetsIds.Clear();
			foreach (Segment segment in segmentsOfAllThemes)
			{
				CompatibilityReason compatibilityReason = CompatibilityReason.not_set;
				CompatibilityType compatibilityType = this.GetCompatibilityType(segment, out compatibilityReason);
				bool flag = compatibilityType == CompatibilityType.allowed_implicitly || compatibilityType == CompatibilityType.allowed_manually;
				if (flag)
				{
					float num;
					if (segment.Group == this.Group)
					{
						num = 1f;
					}
					else
					{
						num = 0.5f;
					}
					this.AddCompatibleSnippet(segment, num);
				}
			}
		}

		public void SetStartMiddleEndPropertiesFromBitfield(int bitfield)
		{
			this.IsUsableAtStart = Segment.ReadOutSegmentSuitabilityFlag(bitfield, SegmentSuitability.start);
			this.IsUsableInMiddle = Segment.ReadOutSegmentSuitabilityFlag(bitfield, SegmentSuitability.middle);
			this.IsUsableAtEnd = Segment.ReadOutSegmentSuitabilityFlag(bitfield, SegmentSuitability.end);
		}

		public int CreateSegmentSuitabilityBitfield(PsaiProject parentProject)
		{
			int num = 0;
			if (this.IsAutomaticBridgeSegment || this.IsBridgeSnippetToAnyGroup(parentProject))
			{
				Segment.SetSegmentSuitabilityFlag(ref num, SegmentSuitability.bridge);
			}
			if (this.IsUsableAtStart)
			{
				Segment.SetSegmentSuitabilityFlag(ref num, SegmentSuitability.start);
			}
			if (this.IsUsableInMiddle)
			{
				Segment.SetSegmentSuitabilityFlag(ref num, SegmentSuitability.middle);
			}
			if (this.IsUsableAtEnd)
			{
				Segment.SetSegmentSuitabilityFlag(ref num, SegmentSuitability.end);
			}
			return num;
		}

		public Segment CreatePsaiDotNetVersion(PsaiProject parentProject)
		{
			Segment segment = new Segment();
			segment.audioData = this.AudioData.CreatePsaiDotNetVersion();
			segment.Id = this.Id;
			segment.Intensity = this.Intensity;
			segment.SnippetTypeBitfield = this.CreateSegmentSuitabilityBitfield(parentProject);
			segment.ThemeId = this.ThemeId;
			segment.Name = base.Name;
			segment.Followers.Capacity = this.CompatibleSnippetsIds.Count;
			foreach (KeyValuePair<int, float> keyValuePair in this.CompatibleSnippetsIds)
			{
				segment.Followers.Add(new Follower(keyValuePair.Key, keyValuePair.Value));
			}
			return segment;
		}

		public bool HasOnlyStartSuitability()
		{
			return this.IsUsableAtStart && !this.IsUsableInMiddle && !this.IsUsableAtEnd;
		}

		public bool HasOnlyMiddleSuitability()
		{
			return !this.IsUsableAtStart && this.IsUsableInMiddle && !this.IsUsableAtEnd;
		}

		public bool HasOnlyEndSuitability()
		{
			return !this.IsUsableAtStart && !this.IsUsableInMiddle && this.IsUsableAtEnd;
		}

		public static bool ReadOutSegmentSuitabilityFlag(int bitfield, SegmentSuitability suitability)
		{
			return (bitfield & (int)suitability) > 0;
		}

		public static void SetSegmentSuitabilityFlag(ref int bitfield, SegmentSuitability snippetType)
		{
			bitfield |= (int)snippetType;
		}

		public static void ClearSegmentSuitabilityFlag(ref int bitfield, SegmentSuitability snippetType)
		{
			bitfield &= (int)(~(int)snippetType);
		}

		public bool IsBridgeSnippetToAnyGroup(PsaiProject project)
		{
			List<Group> list = null;
			return this.IsAutomaticBridgeSegment || project.CheckIfSnippetIsManualBridgeSnippetToAnyGroup(this, false, out list);
		}

		public bool IsManualBridgeSnippetForAnyGroup(PsaiProject project)
		{
			List<Group> list = null;
			return project.CheckIfSnippetIsManualBridgeSnippetToAnyGroup(this, false, out list);
		}

		public bool IsManualBridgeSegmentForSourceGroup(Group sourceGroup)
		{
			return sourceGroup.ManualBridgeSnippetsOfTargetGroups.Contains(this);
		}

		public override CompatibilitySetting GetCompatibilitySetting(PsaiMusicEntity targetEntity)
		{
			if (targetEntity is Segment)
			{
				if (this.ManuallyBlockedSnippets.Contains((Segment)targetEntity))
				{
					return CompatibilitySetting.blocked;
				}
				if (this.ManuallyLinkedSnippets.Contains((Segment)targetEntity))
				{
					return CompatibilitySetting.allowed;
				}
			}
			return CompatibilitySetting.neutral;
		}

		public override CompatibilityType GetCompatibilityType(PsaiMusicEntity targetEntity, out CompatibilityReason reason)
		{
			reason = CompatibilityReason.not_set;
			if (!(targetEntity is Segment))
			{
				return CompatibilityType.undefined;
			}
			Segment segment = (Segment)targetEntity;
			Group group = this.Group;
			Group group2 = segment.Group;
			if (group.GetCompatibilityType(group2, out reason) == CompatibilityType.logically_impossible)
			{
				return CompatibilityType.logically_impossible;
			}
			if (this.HasOnlyEndSuitability() && segment.HasOnlyEndSuitability())
			{
				reason = CompatibilityReason.target_segment_and_source_segment_are_both_only_usable_at_end;
				return CompatibilityType.logically_impossible;
			}
			if (group != group2 && segment.HasOnlyEndSuitability() && !segment.IsAutomaticBridgeSegment && !segment.IsManualBridgeSegmentForSourceGroup(group))
			{
				reason = CompatibilityReason.target_segment_is_of_a_different_group_and_is_only_usable_at_end;
				return CompatibilityType.logically_impossible;
			}
			if (group == group2 && !segment.IsUsableInMiddle && !segment.IsUsableAtEnd && (segment.IsAutomaticBridgeSegment || segment.IsManualBridgeSegmentForSourceGroup(group)))
			{
				reason = CompatibilityReason.target_segment_is_a_pure_bridge_segment_within_the_same_group;
				return CompatibilityType.blocked_implicitly;
			}
			if (this.ManuallyLinkedSnippets.Contains(segment))
			{
				reason = CompatibilityReason.manual_setting_within_same_hierarchy;
				return CompatibilityType.allowed_manually;
			}
			if (this.ManuallyBlockedSnippets.Contains(segment))
			{
				reason = CompatibilityReason.manual_setting_within_same_hierarchy;
				return CompatibilityType.blocked_manually;
			}
			if (group != null && group != group2 && (group2.ContainsAtLeastOneAutomaticBridgeSegment() || group2.ContainsAtLeastOneManualBridgeSegmentForSourceGroup(group)))
			{
				if (segment.IsManualBridgeSegmentForSourceGroup(group))
				{
					reason = CompatibilityReason.target_segment_is_a_manual_bridge_segment_for_the_source_group;
					return CompatibilityType.allowed_manually;
				}
				if (segment.IsAutomaticBridgeSegment)
				{
					reason = CompatibilityReason.target_segment_is_an_automatic_bridge_segment;
					return CompatibilityType.allowed_implicitly;
				}
				reason = CompatibilityReason.target_group_contains_at_least_one_bridge_segment;
				return CompatibilityType.blocked_implicitly;
			}
			else
			{
				if (this.HasOnlyEndSuitability())
				{
					reason = CompatibilityReason.anything_may_be_played_after_a_pure_end_segment;
					return CompatibilityType.allowed_implicitly;
				}
				if (segment.DefaultCompatibiltyAsFollower != CompatibilityType.allowed_implicitly)
				{
					reason = CompatibilityReason.default_compatibility_of_the_target_segment_as_a_follower;
					return segment.DefaultCompatibiltyAsFollower;
				}
				switch (this.Group.GetCompatibilityType(segment.Group, out reason))
				{
				case CompatibilityType.allowed_manually:
					reason = CompatibilityReason.manual_setting_of_parent_entity;
					return CompatibilityType.allowed_implicitly;
				case CompatibilityType.blocked_implicitly:
					return CompatibilityType.blocked_implicitly;
				case CompatibilityType.blocked_manually:
					reason = CompatibilityReason.manual_setting_of_parent_entity;
					return CompatibilityType.blocked_implicitly;
				default:
					reason = CompatibilityReason.inherited_from_parent_hierarchy;
					return CompatibilityType.allowed_implicitly;
				}
			}
		}

		public override PsaiMusicEntity GetParent()
		{
			return this.Group;
		}

		public override int GetIndexPositionWithinParentEntity(PsaiProject parentProject)
		{
			return this.Group.Segments.IndexOf(this);
		}

		public static Segment GetExampleSnippet1()
		{
			return new Segment
			{
				Name = "snippet1",
				Intensity = 0.5f
			};
		}

		public static Segment GetExampleSnippet2()
		{
			return new Segment
			{
				Name = "snippet2",
				Intensity = 0.6f
			};
		}

		public static Segment GetExampleSnippet3()
		{
			return new Segment
			{
				Name = "snippet3",
				Intensity = 0.7f
			};
		}

		public static Segment GetExampleSnippet4()
		{
			return new Segment
			{
				Name = "snippet4",
				Intensity = 0.7f
			};
		}

		private const int DEFAULT_SNIPPET_TYPES = 3;

		private const float DEFAULT_INTENSITY = 0.5f;

		private const float COMPATIBILITY_PERCENTAGE_SAME_GROUP = 1f;

		private const float COMPATIBILITY_PERCENTAGE_OTHER_GROUP = 0.5f;

		private float _intensity;

		[NonSerialized]
		private Dictionary<int, float> _compatibleSnippetsIds = new Dictionary<int, float>();

		[NonSerialized]
		private HashSet<Segment> _manuallyLinkedSnippets = new HashSet<Segment>();

		[NonSerialized]
		private HashSet<Segment> _manuallyBlockedSnippets = new HashSet<Segment>();
	}
}
