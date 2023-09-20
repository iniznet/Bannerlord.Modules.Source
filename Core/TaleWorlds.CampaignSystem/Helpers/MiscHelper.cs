using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using TaleWorlds.Library;

namespace Helpers
{
	// Token: 0x0200001D RID: 29
	public static class MiscHelper
	{
		// Token: 0x060000F1 RID: 241 RVA: 0x0000C228 File Offset: 0x0000A428
		public static XmlDocument LoadXmlFile(string path)
		{
			Debug.Print("opening " + path, 0, Debug.DebugColor.White, 17592186044416UL);
			XmlDocument xmlDocument = new XmlDocument();
			StreamReader streamReader = new StreamReader(path);
			string text = streamReader.ReadToEnd();
			xmlDocument.LoadXml(text);
			streamReader.Close();
			return xmlDocument;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000C274 File Offset: 0x0000A474
		public static string GenerateCampaignId(int length)
		{
			string text2;
			using (MD5 md = MD5.Create())
			{
				string text = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				byte[] array = md.ComputeHash(bytes);
				MBStringBuilder mbstringBuilder = default(MBStringBuilder);
				mbstringBuilder.Initialize(16, "GenerateCampaignId");
				int num = 0;
				while (num < array.Length && mbstringBuilder.Length < length)
				{
					mbstringBuilder.Append<string>(array[num].ToString("x2"));
					num++;
				}
				text2 = mbstringBuilder.ToStringAndRelease();
			}
			return text2;
		}
	}
}
