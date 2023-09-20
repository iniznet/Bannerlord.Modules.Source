using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Launcher.Library.UserDatas
{
	public class UserGameTypeData
	{
		public List<UserModData> ModDatas { get; set; }

		public UserGameTypeData()
		{
			this.ModDatas = new List<UserModData>();
		}
	}
}
