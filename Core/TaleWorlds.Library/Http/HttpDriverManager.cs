using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.Http
{
	// Token: 0x020000A4 RID: 164
	public static class HttpDriverManager
	{
		// Token: 0x060005FA RID: 1530 RVA: 0x00013021 File Offset: 0x00011221
		static HttpDriverManager()
		{
			HttpDriverManager.AddHttpDriver("DotNet", new DotNetHttpDriver());
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x0001303C File Offset: 0x0001123C
		public static void AddHttpDriver(string name, IHttpDriver driver)
		{
			if (HttpDriverManager._httpDrivers.Count == 0)
			{
				HttpDriverManager._defaultHttpDriver = name;
			}
			HttpDriverManager._httpDrivers.Add(name, driver);
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x0001305C File Offset: 0x0001125C
		public static void SetDefault(string name)
		{
			if (HttpDriverManager.GetHttpDriver(name) != null)
			{
				HttpDriverManager._defaultHttpDriver = name;
			}
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x0001306C File Offset: 0x0001126C
		public static IHttpDriver GetHttpDriver(string name)
		{
			IHttpDriver httpDriver;
			HttpDriverManager._httpDrivers.TryGetValue(name, out httpDriver);
			return httpDriver;
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00013088 File Offset: 0x00011288
		public static IHttpDriver GetDefaultHttpDriver()
		{
			return HttpDriverManager.GetHttpDriver(HttpDriverManager._defaultHttpDriver);
		}

		// Token: 0x040001BF RID: 447
		private static Dictionary<string, IHttpDriver> _httpDrivers = new Dictionary<string, IHttpDriver>();

		// Token: 0x040001C0 RID: 448
		private static string _defaultHttpDriver;
	}
}
