using System;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class DLLResult
	{
		public string DLLName { get; set; }

		public bool IsSafe { get; set; }

		public string Information { get; set; }

		public DLLResult(string dLLName, bool isSafe, string information)
		{
			this.DLLName = dLLName;
			this.IsSafe = isSafe;
			this.Information = information;
		}

		public DLLResult()
		{
			this.DLLName = "";
			this.IsSafe = false;
			this.Information = "";
		}
	}
}
