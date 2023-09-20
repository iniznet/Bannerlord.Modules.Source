using System;

namespace TaleWorlds.MountAndBlade.Launcher.Library.UserDatas
{
	public class DLLCheckData
	{
		public string DLLName { get; set; }

		public string DLLVerifyInformation { get; set; }

		public uint LatestSizeInBytes { get; set; }

		public bool IsDangerous { get; set; }

		public DLLCheckData(string dllname)
		{
			this.LatestSizeInBytes = 0U;
			this.IsDangerous = true;
			this.DLLName = dllname;
			this.DLLVerifyInformation = "";
		}

		public DLLCheckData()
		{
		}
	}
}
