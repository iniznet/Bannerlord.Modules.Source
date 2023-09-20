using System;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherDLLData
	{
		public SubModuleInfo SubModule { get; private set; }

		public bool IsDangerous { get; private set; }

		public string VerifyInformation { get; private set; }

		public uint Size { get; private set; }

		public LauncherDLLData(SubModuleInfo subModule, bool isDangerous, string verifyInformation, uint size)
		{
			this.SubModule = subModule;
			this.IsDangerous = isDangerous;
			this.VerifyInformation = verifyInformation;
			this.Size = size;
		}

		public void SetIsDLLDangerous(bool isDangerous)
		{
			this.IsDangerous = isDangerous;
		}

		public void SetDLLSize(uint size)
		{
			this.Size = size;
		}

		public void SetDLLVerifyInformation(string info)
		{
			this.VerifyInformation = info;
		}
	}
}
