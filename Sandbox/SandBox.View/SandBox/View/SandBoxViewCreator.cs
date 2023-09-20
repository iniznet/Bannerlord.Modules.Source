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
	public static class SandBoxViewCreator
	{
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

		public static ScreenBase CreateSaveLoadScreen(bool isSaving)
		{
			return ViewCreatorManager.CreateScreenView<SaveLoadScreen>(new object[] { isSaving });
		}

		public static MissionView CreateMissionCraftingView()
		{
			return null;
		}

		public static MissionView CreateMissionNameMarkerUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionNameMarkerUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionConversationView(Mission mission)
		{
			return ViewCreatorManager.CreateMissionView<MissionConversationView>(true, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionBarterView()
		{
			return ViewCreatorManager.CreateMissionView<BarterView>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionTournamentView()
		{
			return ViewCreatorManager.CreateMissionView<MissionTournamentView>(false, null, Array.Empty<object>());
		}

		public static MapView CreateMapView<T>(params object[] parameters) where T : MapView
		{
			Type type = typeof(T);
			if (SandBoxViewCreator._actualViewTypes.ContainsKey(typeof(T)))
			{
				type = SandBoxViewCreator._actualViewTypes[typeof(T)];
			}
			return Activator.CreateInstance(type, parameters) as MapView;
		}

		public static MenuView CreateMenuView<T>(params object[] parameters) where T : MenuView
		{
			Type type = typeof(T);
			if (SandBoxViewCreator._actualViewTypes.ContainsKey(typeof(T)))
			{
				type = SandBoxViewCreator._actualViewTypes[typeof(T)];
			}
			return Activator.CreateInstance(type, parameters) as MenuView;
		}

		public static MissionView CreateBoardGameView()
		{
			return ViewCreatorManager.CreateMissionView<BoardGameView>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionAmbushDeploymentView()
		{
			return ViewCreatorManager.CreateMissionView<MissionAmbushDeploymentView>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionArenaPracticeFightView()
		{
			return ViewCreatorManager.CreateMissionView<MissionArenaPracticeFightView>(false, null, Array.Empty<object>());
		}

		private static Dictionary<string, MethodInfo> _viewCreators = new Dictionary<string, MethodInfo>();

		private static Dictionary<Type, Type> _actualViewTypes = new Dictionary<Type, Type>();

		private static List<Type> _defaultTypes = new List<Type>();
	}
}
