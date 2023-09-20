using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ModuleInfoModel
	{
		public string Id { get; private set; }

		public string Name { get; private set; }

		public ModuleCategory Category { get; private set; }

		public string Version { get; private set; }

		[JsonIgnore]
		public bool IsOptional
		{
			get
			{
				return this.Category == ModuleCategory.MultiplayerOptional;
			}
		}

		[JsonConstructor]
		private ModuleInfoModel(string id, string name, string version, ModuleCategory category)
		{
			this.Id = id;
			this.Name = name;
			this.Version = version;
			this.Category = category;
		}

		internal ModuleInfoModel(ModuleInfo moduleInfo)
			: this(moduleInfo.Id, moduleInfo.Name, moduleInfo.Version.ToString(), moduleInfo.Category)
		{
		}

		public static bool ShouldIncludeInSession(ModuleInfo moduleInfo)
		{
			return !moduleInfo.IsOfficial && moduleInfo.HasMultiplayerCategory;
		}

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

		public override bool Equals(object obj)
		{
			ModuleInfoModel moduleInfoModel;
			return (moduleInfoModel = obj as ModuleInfoModel) != null && this.Id == moduleInfoModel.Id && this.Version == moduleInfoModel.Version;
		}

		public override int GetHashCode()
		{
			return (-612338121 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Id)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Version);
		}
	}
}
