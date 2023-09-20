using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001B5 RID: 437
	public static class MBGlobals
	{
		// Token: 0x06001966 RID: 6502 RVA: 0x0005B5EF File Offset: 0x000597EF
		public static void InitializeReferences()
		{
			if (!MBGlobals._initialized)
			{
				MBGlobals._actionSets = new Dictionary<string, MBActionSet>();
				MBGlobals._initialized = true;
			}
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x0005B608 File Offset: 0x00059808
		public static MBActionSet GetActionSetWithSuffix(Monster monster, bool isFemale, string suffix)
		{
			return MBGlobals.GetActionSet(ActionSetCode.GenerateActionSetNameWithSuffix(monster, isFemale, suffix));
		}

		// Token: 0x06001968 RID: 6504 RVA: 0x0005B618 File Offset: 0x00059818
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

		// Token: 0x06001969 RID: 6505 RVA: 0x0005B662 File Offset: 0x00059862
		public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
		{
			return ((MemberExpression)memberExpression.Body).Member.Name;
		}

		// Token: 0x0600196A RID: 6506 RVA: 0x0005B679 File Offset: 0x00059879
		public static string GetMethodName<T>(Expression<Func<T>> memberExpression)
		{
			return ((MethodCallExpression)memberExpression.Body).Method.Name;
		}

		// Token: 0x040007C2 RID: 1986
		public const float Gravity = 9.806f;

		// Token: 0x040007C3 RID: 1987
		public static readonly Vec3 GravityVec3 = new Vec3(0f, 0f, -9.806f, -1f);

		// Token: 0x040007C4 RID: 1988
		private static bool _initialized;

		// Token: 0x040007C5 RID: 1989
		private static Dictionary<string, MBActionSet> _actionSets;
	}
}
