using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
	public static class MetaDataExtensions
	{
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

		public static ApplicationVersion GetApplicationVersion(this MetaData metaData)
		{
			string text = ((metaData != null) ? metaData["ApplicationVersion"] : null);
			if (text == null)
			{
				return ApplicationVersion.Empty;
			}
			return ApplicationVersion.FromString(text, 17949);
		}

		public static string[] GetModules(this MetaData metaData)
		{
			string text;
			if (metaData == null || !metaData.TryGetValue("Modules", out text))
			{
				return new string[0];
			}
			return text.Split(new char[] { ';' });
		}

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
