using System;
using System.Collections.Concurrent;

namespace TaleWorlds.Library.Http
{
	public static class HttpDriverManager
	{
		public static void AddHttpDriver(string name, IHttpDriver driver)
		{
			if (HttpDriverManager._httpDrivers.Count == 0)
			{
				HttpDriverManager._defaultHttpDriver = name;
			}
			HttpDriverManager._httpDrivers[name] = driver;
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
			if (httpDriver == null)
			{
				Debug.Print("HTTP driver not found:" + (name ?? "not set"), 0, Debug.DebugColor.White, 17592186044416UL);
			}
			return httpDriver;
		}

		public static IHttpDriver GetDefaultHttpDriver()
		{
			if (HttpDriverManager._defaultHttpDriver == null)
			{
				HttpDriverManager.AddHttpDriver("DotNet", new DotNetHttpDriver());
			}
			return HttpDriverManager.GetHttpDriver(HttpDriverManager._defaultHttpDriver);
		}

		private static ConcurrentDictionary<string, IHttpDriver> _httpDrivers = new ConcurrentDictionary<string, IHttpDriver>();

		private static string _defaultHttpDriver;
	}
}
