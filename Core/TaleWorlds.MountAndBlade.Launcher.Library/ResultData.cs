using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x02000012 RID: 18
	public class ResultData
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000093 RID: 147 RVA: 0x000041E6 File Offset: 0x000023E6
		// (set) Token: 0x06000094 RID: 148 RVA: 0x000041EE File Offset: 0x000023EE
		public string Errors { get; set; } = "";

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000095 RID: 149 RVA: 0x000041F7 File Offset: 0x000023F7
		// (set) Token: 0x06000096 RID: 150 RVA: 0x000041FF File Offset: 0x000023FF
		public List<DLLResult> DLLs { get; set; } = new List<DLLResult>();

		// Token: 0x06000097 RID: 151 RVA: 0x00004208 File Offset: 0x00002408
		public void AddDLLResult(string dllName, bool isSafe, string information)
		{
			this.DLLs.Add(new DLLResult(dllName, isSafe, information));
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00004220 File Offset: 0x00002420
		public override string ToString()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ResultData));
			string text;
			using (StringWriter stringWriter = new StringWriter())
			{
				xmlSerializer.Serialize(stringWriter, this);
				text = stringWriter.ToString();
			}
			return text;
		}
	}
}
