using System;

namespace TaleWorlds.Diamond.AccessProvider.Test
{
	[Serializable]
	public class TestAccessObject
	{
		public string UserName { get; private set; }

		public string Password { get; private set; }

		public TestAccessObject(string userName, string password)
		{
			this.UserName = userName;
			this.Password = password;
		}
	}
}
