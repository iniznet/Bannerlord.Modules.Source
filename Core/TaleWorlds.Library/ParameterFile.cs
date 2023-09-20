using System;
using System.IO;
using System.Xml;

namespace TaleWorlds.Library
{
	// Token: 0x0200006D RID: 109
	public class ParameterFile
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060003B9 RID: 953 RVA: 0x0000BB54 File Offset: 0x00009D54
		// (set) Token: 0x060003BA RID: 954 RVA: 0x0000BB5C File Offset: 0x00009D5C
		public string Path { get; private set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060003BB RID: 955 RVA: 0x0000BB65 File Offset: 0x00009D65
		// (set) Token: 0x060003BC RID: 956 RVA: 0x0000BB6D File Offset: 0x00009D6D
		public DateTime LastCheckedTime { get; private set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060003BD RID: 957 RVA: 0x0000BB76 File Offset: 0x00009D76
		// (set) Token: 0x060003BE RID: 958 RVA: 0x0000BB7E File Offset: 0x00009D7E
		public ParameterContainer ParameterContainer { get; private set; }

		// Token: 0x060003BF RID: 959 RVA: 0x0000BB87 File Offset: 0x00009D87
		public ParameterFile(string path)
		{
			this.ParameterContainer = new ParameterContainer();
			this.Path = path;
			this.LastCheckedTime = DateTime.MinValue;
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0000BBAC File Offset: 0x00009DAC
		public bool CheckIfNeedsToBeRefreshed()
		{
			return File.GetLastWriteTime(this.Path) > this.LastCheckedTime;
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0000BBC4 File Offset: 0x00009DC4
		public void Refresh()
		{
			this.ParameterContainer.ClearParameters();
			DateTime lastWriteTime = File.GetLastWriteTime(this.Path);
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(this.Path);
			}
			catch
			{
				this._failedAttemptsCount++;
				if (this._failedAttemptsCount >= 100)
				{
					Debug.FailedAssert("Could not load parameters file", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\ParameterFile.cs", "Refresh", 47);
				}
				return;
			}
			this._failedAttemptsCount = 0;
			foreach (object obj in xmlDocument.FirstChild.ChildNodes)
			{
				XmlElement xmlElement = (XmlElement)obj;
				string attribute = xmlElement.GetAttribute("name");
				string attribute2 = xmlElement.GetAttribute("value");
				this.ParameterContainer.AddParameter(attribute, attribute2, true);
			}
			this.LastCheckedTime = lastWriteTime;
		}

		// Token: 0x0400011D RID: 285
		private int _failedAttemptsCount;

		// Token: 0x0400011E RID: 286
		private const int MaxFailedAttemptsCount = 100;
	}
}
