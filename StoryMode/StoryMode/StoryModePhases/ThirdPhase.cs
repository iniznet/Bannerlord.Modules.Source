﻿using System;
using System.Collections.Generic;
using StoryMode.GameComponents.CampaignBehaviors;
using TaleWorlds.ActivitySystem;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace StoryMode.StoryModePhases
{
	public class ThirdPhase
	{
		internal static void AutoGeneratedStaticCollectObjectsThirdPhase(object o, List<object> collectedObjects)
		{
			((ThirdPhase)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._oppositionKingdoms);
			collectedObjects.Add(this._allyKingdoms);
		}

		internal static object AutoGeneratedGetMemberValueIsCompleted(object o)
		{
			return ((ThirdPhase)o).IsCompleted;
		}

		internal static object AutoGeneratedGetMemberValue_oppositionKingdoms(object o)
		{
			return ((ThirdPhase)o)._oppositionKingdoms;
		}

		internal static object AutoGeneratedGetMemberValue_allyKingdoms(object o)
		{
			return ((ThirdPhase)o)._allyKingdoms;
		}

		[SaveableProperty(3)]
		public bool IsCompleted { get; private set; }

		public MBReadOnlyList<Kingdom> OppositionKingdoms
		{
			get
			{
				return this._oppositionKingdoms;
			}
		}

		public MBReadOnlyList<Kingdom> AllyKingdoms
		{
			get
			{
				return this._allyKingdoms;
			}
		}

		public ThirdPhase()
		{
			this._oppositionKingdoms = new MBList<Kingdom>();
			this._allyKingdoms = new MBList<Kingdom>();
			this.IsCompleted = false;
		}

		public void AddAllyKingdom(Kingdom kingdom)
		{
			this._allyKingdoms.Add(kingdom);
		}

		public void AddOppositionKingdom(Kingdom kingdom)
		{
			this._oppositionKingdoms.Add(kingdom);
		}

		public void RemoveOppositionKingdom(Kingdom kingdom)
		{
			this._oppositionKingdoms.Remove(kingdom);
		}

		public void CompleteThirdPhase(QuestBase.QuestCompleteDetails defeatTheConspiracyQuestCompleteDetail)
		{
			this.IsCompleted = true;
			if (defeatTheConspiracyQuestCompleteDetail == 1)
			{
				ActivityManager.EndActivity("CompleteMainQuest", 2);
			}
			else if (defeatTheConspiracyQuestCompleteDetail == 4 || defeatTheConspiracyQuestCompleteDetail == 2 || defeatTheConspiracyQuestCompleteDetail == null)
			{
				ActivityManager.EndActivity("CompleteMainQuest", 0);
			}
			else if (defeatTheConspiracyQuestCompleteDetail == 3 || defeatTheConspiracyQuestCompleteDetail == 5)
			{
				ActivityManager.EndActivity("CompleteMainQuest", 1);
			}
			Campaign.Current.CampaignBehaviorManager.RemoveBehavior<ThirdPhaseCampaignBehavior>();
		}

		[SaveableField(1)]
		private readonly MBList<Kingdom> _oppositionKingdoms;

		[SaveableField(2)]
		private readonly MBList<Kingdom> _allyKingdoms;
	}
}
