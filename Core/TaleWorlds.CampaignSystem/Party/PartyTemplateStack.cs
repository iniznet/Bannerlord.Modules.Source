﻿using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Party
{
	public struct PartyTemplateStack
	{
		public static void AutoGeneratedStaticCollectObjectsPartyTemplateStack(object o, List<object> collectedObjects)
		{
			((PartyTemplateStack)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this.Character);
		}

		internal static object AutoGeneratedGetMemberValueCharacter(object o)
		{
			return ((PartyTemplateStack)o).Character;
		}

		internal static object AutoGeneratedGetMemberValueMinValue(object o)
		{
			return ((PartyTemplateStack)o).MinValue;
		}

		internal static object AutoGeneratedGetMemberValueMaxValue(object o)
		{
			return ((PartyTemplateStack)o).MaxValue;
		}

		public PartyTemplateStack(CharacterObject character, int minValue, int maxValue)
		{
			this.Character = character;
			this.MinValue = minValue;
			this.MaxValue = maxValue;
		}

		[SaveableField(0)]
		public CharacterObject Character;

		[SaveableField(1)]
		public int MinValue;

		[SaveableField(2)]
		public int MaxValue;
	}
}
