using System;
using System.Collections.Generic;
using System.Reflection;
using SandBox.View.Map;
using SandBox.View.Menu;
using SandBox.View.Missions;
using SandBox.View.Missions.Tournaments;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.ScreenSystem;

namespace SandBox.View
{
	// Token: 0x0200000A RID: 10
	public static class SandBoxViewCreator
	{
		// Token: 0x06000031 RID: 49 RVA: 0x00003A50 File Offset: 0x00001C50
		static SandBoxViewCreator()
		{
			Assembly[] viewAssemblies = SandBoxViewCreator.GetViewAssemblies();
			Assembly assembly = typeof(ViewCreatorModule).Assembly;
			SandBoxViewCreator.CheckAssemblyScreens(assembly);
			Assembly[] array = viewAssemblies;
			for (int i = 0; i < array.Length; i++)
			{
				SandBoxViewCreator.CheckAssemblyScreens(array[i]);
			}
			SandBoxViewCreator.CollectDefaults(assembly);
			array = viewAssemblies;
			for (int i = 0; i < array.Length; i++)
			{
				SandBoxViewCreator.CollectDefaults(array[i]);
			}
			array = viewAssemblies;
			for (int i = 0; i < array.Length; i++)
			{
				SandBoxViewCreator.CheckOverridenViews(array[i]);
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003AE8 File Offset: 0x00001CE8
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
							SandBoxViewCreator._viewCreators.Add(viewMethod.Name, methodInfo);
						}
					}
				}
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003B90 File Offset: 0x00001D90
		private static void CheckOverridenViews(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(MapView).IsAssignableFrom(type) || typeof(MenuView).IsAssignableFrom(type) || typeof(MissionView).IsAssignableFrom(type) || typeof(ScreenBase).IsAssignableFrom(type))
				{
					object[] customAttributes = type.GetCustomAttributes(typeof(OverrideView), false);
					if (customAttributes != null && customAttributes.Length == 1)
					{
						OverrideView overrideView = customAttributes[0] as OverrideView;
						if (overrideView != null)
						{
							if (!SandBoxViewCreator._actualViewTypes.ContainsKey(overrideView.BaseType))
							{
								SandBoxViewCreator._actualViewTypes.Add(overrideView.BaseType, type);
							}
							else
							{
								SandBoxViewCreator._actualViewTypes[overrideView.BaseType] = type;
							}
							if (SandBoxViewCreator._defaultTypes.Contains(overrideView.BaseType))
							{
								for (int j = 0; j < SandBoxViewCreator._defaultTypes.Count; j++)
								{
									if (SandBoxViewCreator._defaultTypes[j] == overrideView.BaseType)
									{
										SandBoxViewCreator._defaultTypes[j] = type;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003CC8 File Offset: 0x00001EC8
		private static void CollectDefaults(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(MissionBehavior).IsAssignableFrom(type))
				{
					object[] customAttributes = type.GetCustomAttributes(typeof(DefaultView), false);
					if (customAttributes != null && customAttributes.Length == 1)
					{
						SandBoxViewCreator._defaultTypes.Add(type);
					}
				}
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003D28 File Offset: 0x00001F28
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

		// Token: 0x06000036 RID: 54 RVA: 0x00003DB1 File Offset: 0x00001FB1
		public static ScreenBase CreateSaveLoadScreen(bool isSaving)
		{
			return ViewCreatorManager.CreateScreenView<SaveLoadScreen>(new object[] { isSaving });
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003DC7 File Offset: 0x00001FC7
		public static MissionView CreateMissionCraftingView()
		{
			return null;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003DCA File Offset: 0x00001FCA
		public static MissionView CreateMissionNameMarkerUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionNameMarkerUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003DDB File Offset: 0x00001FDB
		public static MissionView CreateMissionConversationView(Mission mission)
		{
			return ViewCreatorManager.CreateMissionView<MissionConversationView>(true, mission, Array.Empty<object>());
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003DE9 File Offset: 0x00001FE9
		public static MissionView CreateMissionBarterView()
		{
			return ViewCreatorManager.CreateMissionView<BarterView>(false, null, Array.Empty<object>());
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003DF7 File Offset: 0x00001FF7
		public static MissionView CreateMissionTournamentView()
		{
			return ViewCreatorManager.CreateMissionView<MissionTournamentView>(false, null, Array.Empty<object>());
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003E08 File Offset: 0x00002008
		public static MapView CreateMapView<T>(params object[] parameters) where T : MapView
		{
			Type type = typeof(T);
			if (SandBoxViewCreator._actualViewTypes.ContainsKey(typeof(T)))
			{
				type = SandBoxViewCreator._actualViewTypes[typeof(T)];
			}
			return Activator.CreateInstance(type, parameters) as MapView;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003E58 File Offset: 0x00002058
		public static MenuView CreateMenuView<T>(params object[] parameters) where T : MenuView
		{
			Type type = typeof(T);
			if (SandBoxViewCreator._actualViewTypes.ContainsKey(typeof(T)))
			{
				type = SandBoxViewCreator._actualViewTypes[typeof(T)];
			}
			return Activator.CreateInstance(type, parameters) as MenuView;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003EA7 File Offset: 0x000020A7
		public static MissionView CreateBoardGameView()
		{
			return ViewCreatorManager.CreateMissionView<BoardGameView>(false, null, Array.Empty<object>());
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003EB5 File Offset: 0x000020B5
		public static MissionView CreateMissionAmbushDeploymentView()
		{
			return ViewCreatorManager.CreateMissionView<MissionAmbushDeploymentView>(false, null, Array.Empty<object>());
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003EC3 File Offset: 0x000020C3
		public static MissionView CreateMissionArenaPracticeFightView()
		{
			return ViewCreatorManager.CreateMissionView<MissionArenaPracticeFightView>(false, null, Array.Empty<object>());
		}

		// Token: 0x04000012 RID: 18
		private static Dictionary<string, MethodInfo> _viewCreators = new Dictionary<string, MethodInfo>();

		// Token: 0x04000013 RID: 19
		private static Dictionary<Type, Type> _actualViewTypes = new Dictionary<Type, Type>();

		// Token: 0x04000014 RID: 20
		private static List<Type> _defaultTypes = new List<Type>();
	}
}
