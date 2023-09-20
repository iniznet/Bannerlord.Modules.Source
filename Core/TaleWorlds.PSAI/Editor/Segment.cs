using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using psai.net;

namespace psai.Editor
{
	// Token: 0x0200000A RID: 10
	[Serializable]
	public class Segment : PsaiMusicEntity, ICloneable
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600009D RID: 157 RVA: 0x000048EF File Offset: 0x00002AEF
		// (set) Token: 0x0600009E RID: 158 RVA: 0x000048F7 File Offset: 0x00002AF7
		public int Id { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00004900 File Offset: 0x00002B00
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x00004908 File Offset: 0x00002B08
		public bool IsAutomaticBridgeSegment { get; set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00004911 File Offset: 0x00002B11
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x00004919 File Offset: 0x00002B19
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

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00004932 File Offset: 0x00002B32
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x0000493A File Offset: 0x00002B3A
		public bool IsUsableAtStart { get; set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00004943 File Offset: 0x00002B43
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x0000494B File Offset: 0x00002B4B
		public bool IsUsableInMiddle { get; set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00004954 File Offset: 0x00002B54
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x0000495C File Offset: 0x00002B5C
		public bool IsUsableAtEnd { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00004965 File Offset: 0x00002B65
		// (set) Token: 0x060000AA RID: 170 RVA: 0x0000496D File Offset: 0x00002B6D
		public AudioData AudioData { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000AB RID: 171 RVA: 0x00004976 File Offset: 0x00002B76
		// (set) Token: 0x060000AC RID: 172 RVA: 0x00004983 File Offset: 0x00002B83
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

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00004991 File Offset: 0x00002B91
		// (set) Token: 0x060000AE RID: 174 RVA: 0x0000499E File Offset: 0x00002B9E
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

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000AF RID: 175 RVA: 0x000049AC File Offset: 0x00002BAC
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x000049B9 File Offset: 0x00002BB9
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

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x000049C7 File Offset: 0x00002BC7
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x000049D4 File Offset: 0x00002BD4
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

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x000049E2 File Offset: 0x00002BE2
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x000049EF File Offset: 0x00002BEF
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

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x000049FD File Offset: 0x00002BFD
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x00004A0A File Offset: 0x00002C0A
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

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x00004A18 File Offset: 0x00002C18
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x00004A25 File Offset: 0x00002C25
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

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00004A33 File Offset: 0x00002C33
		public int TotalLengthInSamples
		{
			get
			{
				return this.AudioData.TotalLengthInSamples;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00004A40 File Offset: 0x00002C40
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00004A4D File Offset: 0x00002C4D
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

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00004A5B File Offset: 0x00002C5B
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00004A63 File Offset: 0x00002C63
		public int ThemeId { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00004A6C File Offset: 0x00002C6C
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00004A74 File Offset: 0x00002C74
		public List<int> Serialization_ManuallyBlockedSegmentIds { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00004A7D File Offset: 0x00002C7D
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x00004A85 File Offset: 0x00002C85
		public List<int> Serialization_ManuallyLinkedSegmentIds { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x00004A8E File Offset: 0x00002C8E
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x00004A96 File Offset: 0x00002C96
		public CompatibilityType DefaultCompatibiltyAsFollower { get; set; }

		// Token: 0x060000C4 RID: 196 RVA: 0x00004A9F File Offset: 0x00002C9F
		public override List<PsaiMusicEntity> GetChildren()
		{
			return null;
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00004AA2 File Offset: 0x00002CA2
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x00004AAA File Offset: 0x00002CAA
		[XmlIgnore]
		public Group Group { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00004ABC File Offset: 0x00002CBC
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x00004AB3 File Offset: 0x00002CB3
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

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00004ACD File Offset: 0x00002CCD
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x00004AC4 File Offset: 0x00002CC4
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

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00004AD5 File Offset: 0x00002CD5
		[XmlIgnore]
		public Dictionary<int, float> CompatibleSnippetsIds
		{
			get
			{
				return this._compatibleSnippetsIds;
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00004ADD File Offset: 0x00002CDD
		public override string GetClassString()
		{
			return "Segment";
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00004AE4 File Offset: 0x00002CE4
		public Segment()
		{
			this.init();
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00004B14 File Offset: 0x00002D14
		public Segment(int id, string name, int snippetTypes, float intensity)
		{
			this.init();
			this.Id = id;
			this.SetStartMiddleEndPropertiesFromBitfield(snippetTypes);
			base.Name = name;
			this.Intensity = intensity;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00004B6C File Offset: 0x00002D6C
		public Segment(int id, AudioData audioData)
		{
			this.init();
			this.AudioData = audioData;
			this.Id = id;
			base.Name = Path.GetFileNameWithoutExtension(Path.GetFileName(audioData.FilePathRelativeToProjectDir));
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00004BCA File Offset: 0x00002DCA
		private void init()
		{
			this.Id = 1;
			base.Name = "new segment";
			this.DefaultCompatibiltyAsFollower = CompatibilityType.allowed_implicitly;
			this.SetStartMiddleEndPropertiesFromBitfield(3);
			this.Intensity = 0.5f;
			this.AudioData = new AudioData();
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00004C04 File Offset: 0x00002E04
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

		// Token: 0x060000D2 RID: 210 RVA: 0x00004CE0 File Offset: 0x00002EE0
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Segment '");
			stringBuilder.Append(base.Name);
			stringBuilder.Append("'");
			return stringBuilder.ToString();
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00004D11 File Offset: 0x00002F11
		public bool AddCompatibleSnippet(Segment snippet, float compatibility)
		{
			if (snippet != null && compatibility >= 0f && compatibility <= 1f)
			{
				this._compatibleSnippetsIds[snippet.Id] = compatibility;
				return true;
			}
			return false;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00004D3C File Offset: 0x00002F3C
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

		// Token: 0x060000D5 RID: 213 RVA: 0x00004DE0 File Offset: 0x00002FE0
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

		// Token: 0x060000D6 RID: 214 RVA: 0x00004E88 File Offset: 0x00003088
		public void SetStartMiddleEndPropertiesFromBitfield(int bitfield)
		{
			this.IsUsableAtStart = Segment.ReadOutSegmentSuitabilityFlag(bitfield, SegmentSuitability.start);
			this.IsUsableInMiddle = Segment.ReadOutSegmentSuitabilityFlag(bitfield, SegmentSuitability.middle);
			this.IsUsableAtEnd = Segment.ReadOutSegmentSuitabilityFlag(bitfield, SegmentSuitability.end);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00004EB4 File Offset: 0x000030B4
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

		// Token: 0x060000D8 RID: 216 RVA: 0x00004F10 File Offset: 0x00003110
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

		// Token: 0x060000D9 RID: 217 RVA: 0x00004FE8 File Offset: 0x000031E8
		public bool HasOnlyStartSuitability()
		{
			return this.IsUsableAtStart && !this.IsUsableInMiddle && !this.IsUsableAtEnd;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00005005 File Offset: 0x00003205
		public bool HasOnlyMiddleSuitability()
		{
			return !this.IsUsableAtStart && this.IsUsableInMiddle && !this.IsUsableAtEnd;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00005022 File Offset: 0x00003222
		public bool HasOnlyEndSuitability()
		{
			return !this.IsUsableAtStart && !this.IsUsableInMiddle && this.IsUsableAtEnd;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000503C File Offset: 0x0000323C
		public static bool ReadOutSegmentSuitabilityFlag(int bitfield, SegmentSuitability suitability)
		{
			return (bitfield & (int)suitability) > 0;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00005044 File Offset: 0x00003244
		public static void SetSegmentSuitabilityFlag(ref int bitfield, SegmentSuitability snippetType)
		{
			bitfield |= (int)snippetType;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000505C File Offset: 0x0000325C
		public static void ClearSegmentSuitabilityFlag(ref int bitfield, SegmentSuitability snippetType)
		{
			bitfield &= (int)(~(int)snippetType);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00005074 File Offset: 0x00003274
		public bool IsBridgeSnippetToAnyGroup(PsaiProject project)
		{
			List<Group> list = null;
			return this.IsAutomaticBridgeSegment || project.CheckIfSnippetIsManualBridgeSnippetToAnyGroup(this, false, out list);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00005098 File Offset: 0x00003298
		public bool IsManualBridgeSnippetForAnyGroup(PsaiProject project)
		{
			List<Group> list = null;
			return project.CheckIfSnippetIsManualBridgeSnippetToAnyGroup(this, false, out list);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000050B1 File Offset: 0x000032B1
		public bool IsManualBridgeSegmentForSourceGroup(Group sourceGroup)
		{
			return sourceGroup.ManualBridgeSnippetsOfTargetGroups.Contains(this);
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000050BF File Offset: 0x000032BF
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

		// Token: 0x060000E3 RID: 227 RVA: 0x000050F4 File Offset: 0x000032F4
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

		// Token: 0x060000E4 RID: 228 RVA: 0x0000524F File Offset: 0x0000344F
		public override PsaiMusicEntity GetParent()
		{
			return this.Group;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00005257 File Offset: 0x00003457
		public override int GetIndexPositionWithinParentEntity(PsaiProject parentProject)
		{
			return this.Group.Segments.IndexOf(this);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000526A File Offset: 0x0000346A
		public static Segment GetExampleSnippet1()
		{
			return new Segment
			{
				Name = "snippet1",
				Intensity = 0.5f
			};
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00005287 File Offset: 0x00003487
		public static Segment GetExampleSnippet2()
		{
			return new Segment
			{
				Name = "snippet2",
				Intensity = 0.6f
			};
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x000052A4 File Offset: 0x000034A4
		public static Segment GetExampleSnippet3()
		{
			return new Segment
			{
				Name = "snippet3",
				Intensity = 0.7f
			};
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x000052C1 File Offset: 0x000034C1
		public static Segment GetExampleSnippet4()
		{
			return new Segment
			{
				Name = "snippet4",
				Intensity = 0.7f
			};
		}

		// Token: 0x04000044 RID: 68
		private const int DEFAULT_SNIPPET_TYPES = 3;

		// Token: 0x04000045 RID: 69
		private const float DEFAULT_INTENSITY = 0.5f;

		// Token: 0x04000046 RID: 70
		private const float COMPATIBILITY_PERCENTAGE_SAME_GROUP = 1f;

		// Token: 0x04000047 RID: 71
		private const float COMPATIBILITY_PERCENTAGE_OTHER_GROUP = 0.5f;

		// Token: 0x04000048 RID: 72
		private float _intensity;

		// Token: 0x04000049 RID: 73
		[NonSerialized]
		private Dictionary<int, float> _compatibleSnippetsIds = new Dictionary<int, float>();

		// Token: 0x0400004A RID: 74
		[NonSerialized]
		private HashSet<Segment> _manuallyLinkedSnippets = new HashSet<Segment>();

		// Token: 0x0400004B RID: 75
		[NonSerialized]
		private HashSet<Segment> _manuallyBlockedSnippets = new HashSet<Segment>();
	}
}
