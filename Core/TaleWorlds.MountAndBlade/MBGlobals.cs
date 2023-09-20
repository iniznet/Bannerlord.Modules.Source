using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public static class MBGlobals
	{
		public static void InitializeReferences()
		{
			if (!MBGlobals._initialized)
			{
				MBGlobals._actionSets = new Dictionary<string, MBActionSet>();
				MBGlobals._initialized = true;
			}
		}

		public static MBActionSet GetActionSetWithSuffix(Monster monster, bool isFemale, string suffix)
		{
			return MBGlobals.GetActionSet(ActionSetCode.GenerateActionSetNameWithSuffix(monster, isFemale, suffix));
		}

		public static MBActionSet GetActionSet(string actionSetCode)
		{
			MBActionSet actionSet;
			if (!MBGlobals._actionSets.TryGetValue(actionSetCode, out actionSet))
			{
				actionSet = MBActionSet.GetActionSet(actionSetCode);
				if (!actionSet.IsValid)
				{
					throw new MBNotFoundException("Unable to find action set : " + actionSetCode);
				}
				MBGlobals._actionSets[actionSetCode] = actionSet;
			}
			return actionSet;
		}

		public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
		{
			return ((MemberExpression)memberExpression.Body).Member.Name;
		}

		public static string GetMethodName<T>(Expression<Func<T>> memberExpression)
		{
			return ((MethodCallExpression)memberExpression.Body).Method.Name;
		}

		public const float Gravity = 9.806f;

		public static readonly Vec3 GravityVec3 = new Vec3(0f, 0f, -9.806f, -1f);

		private static bool _initialized;

		private static Dictionary<string, MBActionSet> _actionSets;
	}
}
