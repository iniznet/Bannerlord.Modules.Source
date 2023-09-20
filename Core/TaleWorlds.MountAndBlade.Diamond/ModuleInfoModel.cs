using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000131 RID: 305
	[Serializable]
	public class ModuleInfoModel
	{
		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000740 RID: 1856 RVA: 0x0000BA67 File Offset: 0x00009C67
		// (set) Token: 0x06000741 RID: 1857 RVA: 0x0000BA6F File Offset: 0x00009C6F
		public string Id { get; private set; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000742 RID: 1858 RVA: 0x0000BA78 File Offset: 0x00009C78
		// (set) Token: 0x06000743 RID: 1859 RVA: 0x0000BA80 File Offset: 0x00009C80
		public string Name { get; private set; }

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x0000BA89 File Offset: 0x00009C89
		// (set) Token: 0x06000745 RID: 1861 RVA: 0x0000BA91 File Offset: 0x00009C91
		public ModuleCategory Category { get; private set; }

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x0000BA9A File Offset: 0x00009C9A
		// (set) Token: 0x06000747 RID: 1863 RVA: 0x0000BAA2 File Offset: 0x00009CA2
		public string Version { get; private set; }

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000748 RID: 1864 RVA: 0x0000BAAB File Offset: 0x00009CAB
		[JsonIgnore]
		public bool IsOptional
		{
			get
			{
				return this.Category == ModuleCategory.MultiplayerOptional;
			}
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x0000BAB6 File Offset: 0x00009CB6
		[JsonConstructor]
		private ModuleInfoModel(string id, string name, string version, ModuleCategory category)
		{
			this.Id = id;
			this.Name = name;
			this.Version = version;
			this.Category = category;
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x0000BADC File Offset: 0x00009CDC
		internal ModuleInfoModel(ModuleInfo moduleInfo)
			: this(moduleInfo.Id, moduleInfo.Name, moduleInfo.Version.ToString(), moduleInfo.Category)
		{
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x0000BB15 File Offset: 0x00009D15
		public static bool ShouldIncludeInSession(ModuleInfo moduleInfo)
		{
			return !moduleInfo.IsOfficial && moduleInfo.HasMultiplayerCategory;
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x0000BB27 File Offset: 0x00009D27
		public static bool TryCreateForSession(ModuleInfo moduleInfo, out ModuleInfoModel moduleInfoModel)
		{
			if (ModuleInfoModel.ShouldIncludeInSession(moduleInfo))
			{
				moduleInfoModel = new ModuleInfoModel(moduleInfo);
				return true;
			}
			moduleInfoModel = null;
			return false;
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x0000BB40 File Offset: 0x00009D40
		public override bool Equals(object obj)
		{
			ModuleInfoModel moduleInfoModel;
			return (moduleInfoModel = obj as ModuleInfoModel) != null && this.Id == moduleInfoModel.Id && this.Version == moduleInfoModel.Version;
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x0000BB7D File Offset: 0x00009D7D
		public override int GetHashCode()
		{
			return (-612338121 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Id)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Version);
		}
	}
}
