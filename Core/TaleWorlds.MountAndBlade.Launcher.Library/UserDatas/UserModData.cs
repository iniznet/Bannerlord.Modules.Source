using System;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library.UserDatas
{
	public class UserModData
	{
		public string Id { get; set; }

		public string LastKnownVersion { get; set; }

		public bool IsSelected { get; set; }

		public UserModData()
		{
		}

		public UserModData(string id, string lastKnownVersion, bool isSelected)
		{
			this.Id = id;
			this.LastKnownVersion = lastKnownVersion;
			this.IsSelected = isSelected;
		}

		public bool IsUpdatedToBeDefault(ModuleInfo module)
		{
			return this.LastKnownVersion != module.Version.ToString() && module.IsDefault;
		}
	}
}
