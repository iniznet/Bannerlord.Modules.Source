using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI
{
	public class WidgetInfo
	{
		public string Name { get; private set; }

		public Type Type { get; private set; }

		public bool GotCustomUpdate { get; private set; }

		public bool GotCustomLateUpdate { get; private set; }

		public bool GotCustomParallelUpdate { get; private set; }

		public bool GotUpdateBrushes { get; private set; }

		static WidgetInfo()
		{
			WidgetInfo.Reload();
		}

		public static void Reload()
		{
			WidgetInfo._widgetInfos = new Dictionary<Type, WidgetInfo>();
			foreach (Type type in WidgetInfo.CollectWidgetTypes())
			{
				WidgetInfo._widgetInfos.Add(type, new WidgetInfo(type));
			}
			TextureWidget.RecollectProviderTypes();
		}

		public static WidgetInfo GetWidgetInfo(Type type)
		{
			return WidgetInfo._widgetInfos[type];
		}

		public WidgetInfo(Type type)
		{
			this.Name = type.Name;
			this.Type = type;
			this.GotCustomUpdate = this.IsMethodOverridden("OnUpdate");
			this.GotCustomLateUpdate = this.IsMethodOverridden("OnLateUpdate");
			this.GotCustomParallelUpdate = this.IsMethodOverridden("OnParallelUpdate");
			this.GotUpdateBrushes = this.IsMethodOverridden("UpdateBrushes");
		}

		private static bool CheckAssemblyReferencesThis(Assembly assembly)
		{
			Assembly assembly2 = typeof(Widget).Assembly;
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < referencedAssemblies.Length; i++)
			{
				if (referencedAssemblies[i].Name == assembly2.GetName().Name)
				{
					return true;
				}
			}
			return false;
		}

		public static List<Type> CollectWidgetTypes()
		{
			List<Type> list = new List<Type>();
			Assembly assembly = typeof(Widget).Assembly;
			foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (WidgetInfo.CheckAssemblyReferencesThis(assembly2) || assembly2 == assembly)
				{
					foreach (Type type in assembly2.GetTypesSafe(null))
					{
						if (typeof(Widget).IsAssignableFrom(type))
						{
							list.Add(type);
						}
					}
				}
			}
			return list;
		}

		private bool IsMethodOverridden(string methodName)
		{
			MethodInfo method = this.Type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			bool flag;
			if (method == null)
			{
				flag = false;
			}
			else
			{
				Type type = this.Type;
				Type type2 = this.Type;
				while (type2 != null)
				{
					if (type2.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null)
					{
						type = type2;
					}
					type2 = type2.BaseType;
				}
				flag = method.DeclaringType != type;
			}
			return flag;
		}

		private static Dictionary<Type, WidgetInfo> _widgetInfos;
	}
}
