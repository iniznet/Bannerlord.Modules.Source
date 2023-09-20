using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000035 RID: 53
	public class WidgetInfo
	{
		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000370 RID: 880 RVA: 0x0000ED38 File Offset: 0x0000CF38
		// (set) Token: 0x06000371 RID: 881 RVA: 0x0000ED40 File Offset: 0x0000CF40
		public string Name { get; private set; }

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000372 RID: 882 RVA: 0x0000ED49 File Offset: 0x0000CF49
		// (set) Token: 0x06000373 RID: 883 RVA: 0x0000ED51 File Offset: 0x0000CF51
		public Type Type { get; private set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000374 RID: 884 RVA: 0x0000ED5A File Offset: 0x0000CF5A
		// (set) Token: 0x06000375 RID: 885 RVA: 0x0000ED62 File Offset: 0x0000CF62
		public bool GotCustomUpdate { get; private set; }

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000376 RID: 886 RVA: 0x0000ED6B File Offset: 0x0000CF6B
		// (set) Token: 0x06000377 RID: 887 RVA: 0x0000ED73 File Offset: 0x0000CF73
		public bool GotCustomLateUpdate { get; private set; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000378 RID: 888 RVA: 0x0000ED7C File Offset: 0x0000CF7C
		// (set) Token: 0x06000379 RID: 889 RVA: 0x0000ED84 File Offset: 0x0000CF84
		public bool GotCustomParallelUpdate { get; private set; }

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x0600037A RID: 890 RVA: 0x0000ED8D File Offset: 0x0000CF8D
		// (set) Token: 0x0600037B RID: 891 RVA: 0x0000ED95 File Offset: 0x0000CF95
		public bool GotUpdateBrushes { get; private set; }

		// Token: 0x0600037C RID: 892 RVA: 0x0000ED9E File Offset: 0x0000CF9E
		static WidgetInfo()
		{
			WidgetInfo.Reload();
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0000EDA8 File Offset: 0x0000CFA8
		public static void Reload()
		{
			WidgetInfo._widgetInfos = new Dictionary<Type, WidgetInfo>();
			foreach (Type type in WidgetInfo.CollectWidgetTypes())
			{
				WidgetInfo._widgetInfos.Add(type, new WidgetInfo(type));
			}
			TextureWidget.RecollectProviderTypes();
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0000EE14 File Offset: 0x0000D014
		public static WidgetInfo GetWidgetInfo(Type type)
		{
			return WidgetInfo._widgetInfos[type];
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0000EE24 File Offset: 0x0000D024
		public WidgetInfo(Type type)
		{
			this.Name = type.Name;
			this.Type = type;
			this.GotCustomUpdate = this.IsMethodOverridden("OnUpdate");
			this.GotCustomLateUpdate = this.IsMethodOverridden("OnLateUpdate");
			this.GotCustomParallelUpdate = this.IsMethodOverridden("OnParallelUpdate");
			this.GotUpdateBrushes = this.IsMethodOverridden("UpdateBrushes");
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0000EE90 File Offset: 0x0000D090
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

		// Token: 0x06000381 RID: 897 RVA: 0x0000EEE0 File Offset: 0x0000D0E0
		public static List<Type> CollectWidgetTypes()
		{
			List<Type> list = new List<Type>();
			Assembly assembly = typeof(Widget).Assembly;
			foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (WidgetInfo.CheckAssemblyReferencesThis(assembly2) || assembly2 == assembly)
				{
					foreach (Type type in assembly2.GetTypes())
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

		// Token: 0x06000382 RID: 898 RVA: 0x0000EF74 File Offset: 0x0000D174
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

		// Token: 0x040001C7 RID: 455
		private static Dictionary<Type, WidgetInfo> _widgetInfos;
	}
}
