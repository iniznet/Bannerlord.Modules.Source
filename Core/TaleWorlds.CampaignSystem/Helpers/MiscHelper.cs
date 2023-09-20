using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using TaleWorlds.Library;

namespace Helpers
{
	public static class MiscHelper
	{
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
