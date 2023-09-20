using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
	// Token: 0x020000AD RID: 173
	public static class MetaDataExtensions
	{
		// Token: 0x0600086A RID: 2154 RVA: 0x0001C934 File Offset: 0x0001AB34
		public static DateTime GetCreationTime(this MetaData metaData)
		{
			string text = ((metaData != null) ? metaData["CreationTime"] : null);
			if (text != null)
			{
				DateTime dateTime;
				if (DateTime.TryParse(text, out dateTime))
				{
					return dateTime;
				}
				long num;
				if (long.TryParse(text, out num))
				{
					return new DateTime(num);
				}
			}
			return DateTime.MinValue;
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x0001C978 File Offset: 0x0001AB78
		public static ApplicationVersion GetApplicationVersion(this MetaData metaData)
		{
			string text = ((metaData != null) ? metaData["ApplicationVersion"] : null);
			if (text == null)
			{
				return ApplicationVersion.Empty;
			}
			return ApplicationVersion.FromString(text, 17949);
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0001C9AC File Offset: 0x0001ABAC
		public static string[] GetModules(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("Modules", out text))
			{
				return new string[0];
			}
			return text.Split(new char[] { ';' });
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x0001C9E4 File Offset: 0x0001ABE4
		public static ApplicationVersion GetModuleVersion(this MetaData metaData, string moduleName)
		{
			string text = "Module_" + moduleName;
			string text2;
			if (metaData != null && metaData.TryGetValue(text, out text2))
			{
				try
				{
					return ApplicationVersion.FromString(text2, 17949);
				}
				catch (Exception ex)
				{
					Debug.FailedAssert(ex.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\MetaDataExtensions.cs", "GetModuleVersion", 53);
				}
			}
			return ApplicationVersion.Empty;
		}
	}
}
