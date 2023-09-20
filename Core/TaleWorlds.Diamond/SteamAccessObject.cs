using System;

namespace TaleWorlds.Diamond
{
	[Serializable]
	public class SteamAccessObject
	{
		public string UserName { get; private set; }

		public string ExternalAccessToken { get; private set; }

		public int AppId { get; private set; }

		public SteamAccessObject(string userName, string externalAccessToken, int appId)
		{
			this.UserName = userName;
			this.ExternalAccessToken = externalAccessToken;
			this.AppId = appId;
		}
	}
}
