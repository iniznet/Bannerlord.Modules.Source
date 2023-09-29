using System;
using TaleWorlds.Library;

namespace TaleWorlds.ModuleManager
{
	public struct DependedModule
	{
		public string ModuleId { get; private set; }

		public ApplicationVersion Version { get; private set; }

		public bool IsOptional { get; private set; }

		public DependedModule(string moduleId, ApplicationVersion version, bool isOptional = false)
		{
			this.ModuleId = moduleId;
			this.Version = version;
			this.IsOptional = isOptional;
		}

		public void UpdateVersionChangeSet()
		{
			this.Version = new ApplicationVersion(this.Version.ApplicationVersionType, this.Version.Major, this.Version.Minor, this.Version.Revision, 27066);
		}
	}
}
