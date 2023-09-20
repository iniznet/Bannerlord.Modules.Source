using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x0200001A RID: 26
	public static class ViewCreatorManager
	{
		// Token: 0x060000D0 RID: 208 RVA: 0x00007200 File Offset: 0x00005400
		static ViewCreatorManager()
		{
			Assembly[] viewAssemblies = ViewCreatorManager.GetViewAssemblies();
			Assembly assembly = typeof(ViewCreatorModule).Assembly;
			ViewCreatorManager.CheckAssemblyScreens(assembly);
			Assembly[] array = viewAssemblies;
			for (int i = 0; i < array.Length; i++)
			{
				ViewCreatorManager.CheckAssemblyScreens(array[i]);
			}
			ViewCreatorManager.CollectDefaults(assembly);
			array = viewAssemblies;
			for (int i = 0; i < array.Length; i++)
			{
				ViewCreatorManager.CollectDefaults(array[i]);
			}
			array = viewAssemblies;
			for (int i = 0; i < array.Length; i++)
			{
				ViewCreatorManager.CheckOverridenViews(array[i]);
			}
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00007298 File Offset: 0x00005498
		private static void CheckAssemblyScreens(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(ViewCreatorModule), false);
				if (customAttributes != null && customAttributes.Length == 1 && customAttributes[0] is ViewCreatorModule)
				{
					foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
					{
						ViewMethod viewMethod = methodInfo.GetCustomAttributes(typeof(ViewMethod), false)[0] as ViewMethod;
						if (viewMethod != null)
						{
							ViewCreatorManager._viewCreators.Add(viewMethod.Name, methodInfo);
						}
					}
				}
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00007340 File Offset: 0x00005540
		private static Assembly[] GetViewAssemblies()
		{
			List<Assembly> list = new List<Assembly>();
			Assembly assembly = typeof(ViewCreatorModule).Assembly;
			foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyName[] referencedAssemblies = assembly2.GetReferencedAssemblies();
				for (int j = 0; j < referencedAssemblies.Length; j++)
				{
					if (referencedAssemblies[j].ToString() == assembly.GetName().ToString())
					{
						list.Add(assembly2);
						break;
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x000073CC File Offset: 0x000055CC
		internal static IEnumerable<MissionBehavior> CreateDefaultMissionBehaviors(Mission mission)
		{
			List<MissionBehavior> list = new List<MissionBehavior>();
			foreach (Type type in ViewCreatorManager._defaultTypes)
			{
				MissionBehavior missionBehavior = Activator.CreateInstance(type) as MissionBehavior;
				list.Add(missionBehavior);
			}
			return list;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00007430 File Offset: 0x00005630
		internal static IEnumerable<MissionBehavior> CollectMissionBehaviors(string missionName, Mission mission, IEnumerable<MissionBehavior> behaviors)
		{
			List<MissionBehavior> list = new List<MissionBehavior>();
			if (ViewCreatorManager._viewCreators.ContainsKey(missionName))
			{
				MissionBehavior[] array = ViewCreatorManager._viewCreators[missionName].Invoke(null, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { mission }, null) as MissionBehavior[];
				list.AddRange(array);
			}
			return behaviors.Concat(list);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00007484 File Offset: 0x00005684
		public static ScreenBase CreateScreenView<T>() where T : ScreenBase, new()
		{
			if (ViewCreatorManager._actualViewTypes.ContainsKey(typeof(T)))
			{
				return Activator.CreateInstance(ViewCreatorManager._actualViewTypes[typeof(T)]) as ScreenBase;
			}
			return new T();
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000074D0 File Offset: 0x000056D0
		public static ScreenBase CreateScreenView<T>(params object[] parameters) where T : ScreenBase
		{
			Type type = typeof(T);
			if (ViewCreatorManager._actualViewTypes.ContainsKey(typeof(T)))
			{
				type = ViewCreatorManager._actualViewTypes[typeof(T)];
			}
			return Activator.CreateInstance(type, parameters) as ScreenBase;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00007520 File Offset: 0x00005720
		public static MissionView CreateMissionView<T>(bool isNetwork = false, Mission mission = null, params object[] parameters) where T : MissionView, new()
		{
			if (ViewCreatorManager._actualViewTypes.ContainsKey(typeof(T)))
			{
				return Activator.CreateInstance(ViewCreatorManager._actualViewTypes[typeof(T)], parameters) as MissionView;
			}
			return new T();
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00007570 File Offset: 0x00005770
		public static MissionView CreateMissionViewWithArgs<T>(params object[] parameters) where T : MissionView
		{
			Type type = typeof(T);
			if (ViewCreatorManager._actualViewTypes.ContainsKey(typeof(T)))
			{
				type = ViewCreatorManager._actualViewTypes[typeof(T)];
			}
			return Activator.CreateInstance(type, parameters) as MissionView;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000075C0 File Offset: 0x000057C0
		private static void CheckOverridenViews(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(MissionView).IsAssignableFrom(type) || typeof(ScreenBase).IsAssignableFrom(type))
				{
					object[] customAttributes = type.GetCustomAttributes(typeof(OverrideView), false);
					if (customAttributes != null && customAttributes.Length == 1)
					{
						OverrideView overrideView = customAttributes[0] as OverrideView;
						if (overrideView != null)
						{
							if (!ViewCreatorManager._actualViewTypes.ContainsKey(overrideView.BaseType))
							{
								ViewCreatorManager._actualViewTypes.Add(overrideView.BaseType, type);
							}
							else
							{
								ViewCreatorManager._actualViewTypes[overrideView.BaseType] = type;
							}
							if (ViewCreatorManager._defaultTypes.Contains(overrideView.BaseType))
							{
								for (int j = 0; j < ViewCreatorManager._defaultTypes.Count; j++)
								{
									if (ViewCreatorManager._defaultTypes[j] == overrideView.BaseType)
									{
										ViewCreatorManager._defaultTypes[j] = type;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000076D4 File Offset: 0x000058D4
		private static void CollectDefaults(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(MissionBehavior).IsAssignableFrom(type) && type.GetCustomAttributes(typeof(DefaultView), false).Length == 1)
				{
					ViewCreatorManager._defaultTypes.Add(type);
				}
			}
		}

		// Token: 0x04000042 RID: 66
		private static Dictionary<string, MethodInfo> _viewCreators = new Dictionary<string, MethodInfo>();

		// Token: 0x04000043 RID: 67
		private static Dictionary<Type, Type> _actualViewTypes = new Dictionary<Type, Type>();

		// Token: 0x04000044 RID: 68
		private static List<Type> _defaultTypes = new List<Type>();
	}
}
