using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.Http
{
	public static class HttpDriverManager
	{
		static HttpDriverManager()
		{
			HttpDriverManager.AddHttpDriver("DotNet", new DotNetHttpDriver());
		}

		public static void AddHttpDriver(string name, IHttpDriver driver)
		{
			if (HttpDriverManager._httpDrivers.Count == 0)
			{
				HttpDriverManager._defaultHttpDriver = name;
			}
			HttpDriverManager._httpDrivers.Add(name, driver);
		}

		public static void SetDefault(string name)
		{
			if (HttpDriverManager.GetHttpDriver(name) != null)
			{
				HttpDriverManager._defaultHttpDriver = name;
			}
		}

		public static IHttpDriver GetHttpDriver(string name)
		{
			IHttpDriver httpDriver;
			HttpDriverManager._httpDrivers.TryGetValue(name, out httpDriver);
			return httpDriver;
		}

		public static IHttpDriver GetDefaultHttpDriver()
		{
			return HttpDriverManager.GetHttpDriver(HttpDriverManager._defaultHttpDriver);
		}

		private static Dictionary<string, IHttpDriver> _httpDrivers = new Dictionary<string, IHttpDriver>();

		private static string _defaultHttpDriver;
	}
}
