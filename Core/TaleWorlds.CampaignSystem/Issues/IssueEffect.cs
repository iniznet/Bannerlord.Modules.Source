﻿using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	public class IssueEffect : MBObjectBase
	{
		internal static void AutoGeneratedStaticCollectObjectsIssueEffect(object o, List<object> collectedObjects)
		{
			((IssueEffect)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		public static MBReadOnlyList<IssueEffect> All
		{
			get
			{
				return Campaign.Current.AllIssueEffects;
			}
		}

		public TextObject Name { get; private set; }

		public TextObject Description { get; private set; }

		public IssueEffect(string stringId)
			: base(stringId)
		{
		}

		public void Initialize(TextObject name, TextObject description)
		{
			this.Name = name;
			this.Description = description;
			base.AfterInitialized();
		}

		public override string ToString()
		{
			return this.Name.ToString();
		}
	}
}