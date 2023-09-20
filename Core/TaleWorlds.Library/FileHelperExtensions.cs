using System;
using System.Xml;

namespace TaleWorlds.Library
{
	// Token: 0x0200002D RID: 45
	public static class FileHelperExtensions
	{
		// Token: 0x0600016B RID: 363 RVA: 0x00005EC8 File Offset: 0x000040C8
		public static void Load(this XmlDocument document, PlatformFilePath path)
		{
			string fileContentString = FileHelper.GetFileContentString(path);
			if (!string.IsNullOrEmpty(fileContentString))
			{
				document.LoadXml(fileContentString);
			}
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00005EEC File Offset: 0x000040EC
		public static void Save(this XmlDocument document, PlatformFilePath path)
		{
			string outerXml = document.OuterXml;
			FileHelper.SaveFileString(path, outerXml);
		}
	}
}
