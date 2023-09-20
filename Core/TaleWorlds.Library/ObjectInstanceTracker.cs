using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	public class ObjectInstanceTracker
	{
		public static void RegisterTrackedInstance(string name, WeakReference instance)
		{
		}

		public static bool CheckBlacklistedTypeCounts(Dictionary<string, int> typeNameCounts, ref string outputLog)
		{
			bool flag = false;
			foreach (string text in typeNameCounts.Keys)
			{
				int num = 0;
				int num2 = typeNameCounts[text];
				List<WeakReference> list;
				if (ObjectInstanceTracker.TrackedInstances.TryGetValue(text, out list))
				{
					using (List<WeakReference>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.Target != null)
							{
								num++;
							}
						}
					}
				}
				if (num != num2)
				{
					flag = true;
					outputLog = string.Concat(new object[] { outputLog, "Type(", text, ") has ", num, " alive instance, but its should be ", num2, "\n" });
				}
			}
			return flag;
		}

		private static Dictionary<string, List<WeakReference>> TrackedInstances = new Dictionary<string, List<WeakReference>>();
	}
}
