using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace psai.Editor
{
	[Serializable]
	public class Group : PsaiMusicEntity, ICloneable
	{
		public List<Segment> Segments
		{
			get
			{
				return this.m_segments;
			}
			set
			{
				this.m_segments = value;
			}
		}

		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				this._description = value;
			}
		}

		public int Serialization_Id { get; set; }

		public List<int> Serialization_ManuallyBlockedGroupIds { get; set; }

		public List<int> Serialization_ManuallyLinkedGroupIds { get; set; }

		public List<int> Serialization_ManualBridgeSegmentIds { get; set; }

		[XmlIgnore]
		public HashSet<Segment> ManualBridgeSnippetsOfTargetGroups
		{
			get
			{
				return this._manualBridgeSnippetsOfTargetGroups;
			}
			set
			{
				this._manualBridgeSnippetsOfTargetGroups = value;
			}
		}

		[XmlIgnore]
		public HashSet<Group> ManuallyBlockedGroups
		{
			get
			{
				return this._manuallyBlockedGroups;
			}
			set
			{
				this._manuallyBlockedGroups = value;
			}
		}

		[XmlIgnore]
		public HashSet<Group> ManuallyLinkedGroups
		{
			get
			{
				return this._manuallyLinkedGroups;
			}
			set
			{
				this._manuallyLinkedGroups = value;
			}
		}

		[XmlIgnore]
		public Theme Theme { get; set; }

		public override string GetClassString()
		{
			return "Group";
		}

		public Group()
		{
			base.Name = "new_group";
		}

		public Group(Theme parentTheme, string name)
			: this()
		{
			this.Theme = parentTheme;
			base.Name = name;
		}

		public Group(Theme parentTheme)
			: this()
		{
			this.Theme = parentTheme;
		}

		~Group()
		{
		}

		public void AddSegment(Segment snippet)
		{
			this.AddSnippet_internal(snippet, -1);
		}

		public void AddSegment(Segment snippet, int index)
		{
			this.AddSnippet_internal(snippet, index);
		}

		private void AddSnippet_internal(Segment snippet, int insertIndex)
		{
			snippet.Group = this;
			if (this.Theme != null)
			{
				snippet.ThemeId = this.Theme.Id;
			}
			if (insertIndex < 0 || insertIndex >= this.m_segments.Count)
			{
				this.m_segments.Add(snippet);
				return;
			}
			this.m_segments.Insert(insertIndex, snippet);
		}

		public void RemoveSegment(Segment snippet)
		{
			this.m_segments.Remove(snippet);
		}

		public bool HasAtLeastOneBridgeSegmentToTargetGroup(Group targetGroup)
		{
			using (HashSet<Segment>.Enumerator enumerator = this._manualBridgeSnippetsOfTargetGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Group == targetGroup)
					{
						return true;
					}
				}
			}
			return targetGroup.ContainsAtLeastOneAutomaticBridgeSegment();
		}

		public bool ContainsAtLeastOneManualBridgeSegmentForSourceGroup(Group sourceGroup)
		{
			using (HashSet<Segment>.Enumerator enumerator = sourceGroup.ManualBridgeSnippetsOfTargetGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Group == this)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool ContainsAtLeastOneAutomaticBridgeSegment()
		{
			using (List<Segment>.Enumerator enumerator = this.Segments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsAutomaticBridgeSegment)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Group '",
				base.Name,
				"' (",
				this.Theme.Name,
				")"
			});
		}

		public override CompatibilitySetting GetCompatibilitySetting(PsaiMusicEntity targetEntity)
		{
			if (targetEntity is Group)
			{
				if (this.ManuallyBlockedGroups.Contains((Group)targetEntity))
				{
					return CompatibilitySetting.blocked;
				}
				if (this.ManuallyLinkedGroups.Contains((Group)targetEntity))
				{
					return CompatibilitySetting.allowed;
				}
			}
			return CompatibilitySetting.neutral;
		}

		public override CompatibilityType GetCompatibilityType(PsaiMusicEntity targetEntity, out CompatibilityReason reason)
		{
			if (!(targetEntity is Group))
			{
				reason = CompatibilityReason.not_set;
				return CompatibilityType.undefined;
			}
			Group group = (Group)targetEntity;
			PsaiMusicEntity theme = this.Theme;
			Theme theme2 = group.Theme;
			if (theme.GetCompatibilityType(theme2, out reason) == CompatibilityType.logically_impossible)
			{
				return CompatibilityType.logically_impossible;
			}
			if (this.ManuallyBlockedGroups.Contains(group))
			{
				reason = CompatibilityReason.manual_setting_within_same_hierarchy;
				return CompatibilityType.blocked_manually;
			}
			if (this.ManuallyLinkedGroups.Contains(group))
			{
				reason = CompatibilityReason.manual_setting_within_same_hierarchy;
				return CompatibilityType.allowed_manually;
			}
			CompatibilityType compatibilityType = this.Theme.GetCompatibilityType(group.Theme, out reason);
			if (compatibilityType == CompatibilityType.allowed_manually)
			{
				reason = CompatibilityReason.manual_setting_of_parent_entity;
				return CompatibilityType.allowed_implicitly;
			}
			if (compatibilityType == CompatibilityType.blocked_manually)
			{
				reason = CompatibilityReason.manual_setting_of_parent_entity;
				return CompatibilityType.blocked_implicitly;
			}
			reason = CompatibilityReason.inherited_from_parent_hierarchy;
			return compatibilityType;
		}

		public void SetAsParentGroupForAllSegments()
		{
			foreach (Segment segment in this.Segments)
			{
				segment.Group = this;
			}
		}

		public override PsaiMusicEntity GetParent()
		{
			return this.Theme;
		}

		public override List<PsaiMusicEntity> GetChildren()
		{
			List<PsaiMusicEntity> list = new List<PsaiMusicEntity>();
			list.Capacity = this.m_segments.Count;
			for (int i = 0; i < this.m_segments.Count; i++)
			{
				list.Add(this.m_segments[i]);
			}
			return list;
		}

		public override int GetIndexPositionWithinParentEntity(PsaiProject parentProject)
		{
			return this.Theme.Groups.IndexOf(this);
		}

		public override object Clone()
		{
			Group group = (Group)base.MemberwiseClone();
			group.Segments = new List<Segment>();
			group.ManualBridgeSnippetsOfTargetGroups = new HashSet<Segment>();
			group.ManuallyBlockedGroups = new HashSet<Group>();
			group.ManuallyLinkedGroups = new HashSet<Group>();
			foreach (Segment segment in this.Segments)
			{
				group.AddSegment((Segment)segment.Clone());
			}
			group.ManuallyBlockedGroups = new HashSet<Group>();
			group.ManuallyLinkedGroups = new HashSet<Group>();
			foreach (Group group2 in this.ManuallyBlockedGroups)
			{
				group.ManuallyBlockedGroups.Add(group2);
			}
			foreach (Group group3 in this.ManuallyLinkedGroups)
			{
				group.ManuallyLinkedGroups.Add(group3);
			}
			return group;
		}

		public override PsaiMusicEntity ShallowCopy()
		{
			Group group = (Group)base.MemberwiseClone();
			foreach (Segment segment in this.Segments)
			{
				segment.Group = group;
			}
			return group;
		}

		[NonSerialized]
		private List<Segment> m_segments = new List<Segment>();

		[NonSerialized]
		private HashSet<Segment> _manualBridgeSnippetsOfTargetGroups = new HashSet<Segment>();

		[NonSerialized]
		private HashSet<Group> _manuallyBlockedGroups = new HashSet<Group>();

		[NonSerialized]
		private HashSet<Group> _manuallyLinkedGroups = new HashSet<Group>();

		private string _description = "";
	}
}
