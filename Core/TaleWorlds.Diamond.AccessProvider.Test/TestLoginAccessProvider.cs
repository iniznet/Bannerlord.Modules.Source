using System;
using System.Security.Cryptography;
using System.Text;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Diamond.AccessProvider.Test
{
	public class TestLoginAccessProvider : ILoginAccessProvider
	{
		void ILoginAccessProvider.Initialize(string preferredUserName, PlatformInitParams initParams)
		{
			this._userName = preferredUserName;
		}

		string ILoginAccessProvider.GetUserName()
		{
			return this._userName;
		}

		PlayerId ILoginAccessProvider.GetPlayerId()
		{
			return TestLoginAccessProvider.GetPlayerIdFromUserName(this._userName);
		}

		AccessObjectResult ILoginAccessProvider.CreateAccessObject()
		{
			return AccessObjectResult.CreateSuccess(new TestAccessObject(this._userName, Environment.GetEnvironmentVariable("TestLoginAccessProvider.Password")));
		}

		public static ulong GetInt64HashCode(string strText)
		{
			ulong num = 0UL;
			if (!string.IsNullOrEmpty(strText))
			{
				byte[] bytes = Encoding.Unicode.GetBytes(strText);
				SHA256CryptoServiceProvider sha256CryptoServiceProvider = new SHA256CryptoServiceProvider();
				byte[] array = sha256CryptoServiceProvider.ComputeHash(bytes);
				ulong num2 = BitConverter.ToUInt64(array, 0);
				ulong num3 = BitConverter.ToUInt64(array, 8);
				ulong num4 = BitConverter.ToUInt64(array, 16);
				ulong num5 = BitConverter.ToUInt64(array, 24);
				num = num2 ^ num3 ^ num4 ^ num5;
				sha256CryptoServiceProvider.Dispose();
			}
			return num;
		}

		public static PlayerId GetPlayerIdFromUserName(string userName)
		{
			return new PlayerId(1, 0UL, TestLoginAccessProvider.GetInt64HashCode(userName));
		}

		private string _userName;
	}
}
