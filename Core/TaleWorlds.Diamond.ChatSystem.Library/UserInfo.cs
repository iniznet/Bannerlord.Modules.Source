using System;

namespace TaleWorlds.Diamond.ChatSystem.Library
{
	[Serializable]
	public class UserInfo
	{
		public string UserName { get; set; }

		public string DisplayName { get; set; }

		public UserInfo(string userName, string displayName)
		{
			this.UserName = userName;
			this.DisplayName = displayName;
		}
	}
}
