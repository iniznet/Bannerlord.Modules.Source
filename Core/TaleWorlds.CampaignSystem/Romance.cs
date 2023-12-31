﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	public class Romance
	{
		internal static void AutoGeneratedStaticCollectObjectsRomance(object o, List<object> collectedObjects)
		{
			((Romance)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._romanticStateList);
		}

		internal static object AutoGeneratedGetMemberValue_romanticStateList(object o)
		{
			return ((Romance)o)._romanticStateList;
		}

		public static List<Romance.RomanticState> RomanticStateList
		{
			get
			{
				return Campaign.Current.Romance._romanticStateList;
			}
		}

		public Romance()
		{
			this._romanticStateList = new List<Romance.RomanticState>();
		}

		public static Hero GetCourtedHeroInOtherClan(Hero person1, Hero person2)
		{
			foreach (Hero hero in person2.Clan.Lords.Where((Hero x) => x != person2))
			{
				if (Romance.GetRomanticLevel(person1, hero) >= Romance.RomanceLevelEnum.MatchMadeByFamily)
				{
					return person2;
				}
			}
			return null;
		}

		public static Romance.RomanceLevelEnum GetRomanticLevel(Hero person1, Hero person2)
		{
			Romance.RomanticState romanticState = Romance.GetRomanticState(person1, person2);
			if (romanticState == null)
			{
				return Romance.RomanceLevelEnum.Untested;
			}
			return romanticState.Level;
		}

		public static Romance.RomanticState GetRomanticState(Hero person1, Hero person2)
		{
			if (Romance.RomanticStateList == null)
			{
				return null;
			}
			foreach (Romance.RomanticState romanticState in Romance.RomanticStateList)
			{
				if (romanticState != null && ((romanticState.Person1 == person1 && romanticState.Person2 == person2) || (romanticState.Person1 == person2 && romanticState.Person2 == person1)))
				{
					return romanticState;
				}
			}
			return null;
		}

		internal static void EndAllCourtships(Hero forHero)
		{
			foreach (Romance.RomanticState romanticState in Romance.RomanticStateList)
			{
				if (romanticState.Person1 == forHero || romanticState.Person2 == forHero)
				{
					romanticState.Level = Romance.RomanceLevelEnum.Ended;
				}
			}
		}

		internal static void SetRomanticState(Hero person1, Hero person2, Romance.RomanceLevelEnum romanceLevelEnum)
		{
			Romance.RomanticState romanticState = Romance.GetRomanticState(person1, person2);
			if (romanticState != null)
			{
				romanticState.Level = romanceLevelEnum;
				return;
			}
			Romance.RomanticState romanticState2 = new Romance.RomanticState();
			romanticState2.Person1 = person1;
			romanticState2.Person2 = person2;
			romanticState2.Level = romanceLevelEnum;
			romanticState2.ProgressToNextLevel = 0;
			Romance.RomanticStateList.Add(romanticState2);
		}

		[SaveableField(7)]
		private readonly List<Romance.RomanticState> _romanticStateList;

		public class RomanticState
		{
			internal static void AutoGeneratedStaticCollectObjectsRomanticState(object o, List<object> collectedObjects)
			{
				((Romance.RomanticState)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.Person1);
				collectedObjects.Add(this.Person2);
			}

			internal static object AutoGeneratedGetMemberValuePerson1(object o)
			{
				return ((Romance.RomanticState)o).Person1;
			}

			internal static object AutoGeneratedGetMemberValuePerson2(object o)
			{
				return ((Romance.RomanticState)o).Person2;
			}

			internal static object AutoGeneratedGetMemberValueLevel(object o)
			{
				return ((Romance.RomanticState)o).Level;
			}

			internal static object AutoGeneratedGetMemberValueProgressToNextLevel(object o)
			{
				return ((Romance.RomanticState)o).ProgressToNextLevel;
			}

			internal static object AutoGeneratedGetMemberValueLastVisit(object o)
			{
				return ((Romance.RomanticState)o).LastVisit;
			}

			internal static object AutoGeneratedGetMemberValueScoreFromPersuasion(object o)
			{
				return ((Romance.RomanticState)o).ScoreFromPersuasion;
			}

			public Hero Partner(Hero hero)
			{
				if (this.Person1 == hero)
				{
					return this.Person2;
				}
				if (this.Person2 == hero)
				{
					return this.Person1;
				}
				return null;
			}

			[SaveableField(0)]
			public Hero Person1;

			[SaveableField(1)]
			public Hero Person2;

			[SaveableField(2)]
			public Romance.RomanceLevelEnum Level;

			[SaveableField(3)]
			public int ProgressToNextLevel;

			[SaveableField(5)]
			public float LastVisit;

			[SaveableField(6)]
			public float ScoreFromPersuasion;
		}

		public enum RomanceLevelEnum
		{
			Ended = -2,
			Rejection,
			Untested,
			FailedInCompatibility,
			FailedInPracticalities,
			MatchMadeByFamily,
			CourtshipStarted,
			CoupleDecidedThatTheyAreCompatible,
			CoupleAgreedOnMarriage,
			Marriage
		}
	}
}
