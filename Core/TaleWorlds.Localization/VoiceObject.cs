using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Localization
{
	// Token: 0x0200000C RID: 12
	public class VoiceObject
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000087 RID: 135 RVA: 0x0000421F File Offset: 0x0000241F
		public MBReadOnlyList<string> VoicePaths
		{
			get
			{
				return this._voicePaths;
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004227 File Offset: 0x00002427
		private VoiceObject()
		{
			this._voicePaths = new MBList<string>();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000423A File Offset: 0x0000243A
		private void AddVoicePath(string voicePath)
		{
			this._voicePaths.Add(voicePath);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004248 File Offset: 0x00002448
		public void AddVoicePaths(XmlNode node, string modulePath)
		{
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "Voice")
				{
					string text = modulePath + "/" + xmlNode.Attributes["path"].InnerText;
					this.AddVoicePath(text);
				}
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000042D4 File Offset: 0x000024D4
		public static VoiceObject Deserialize(XmlNode node, string modulePath)
		{
			VoiceObject voiceObject = new VoiceObject();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "Voice")
				{
					string text = modulePath + "/" + xmlNode.Attributes["path"].InnerText;
					voiceObject.AddVoicePath(text);
				}
			}
			return voiceObject;
		}

		// Token: 0x04000026 RID: 38
		private readonly MBList<string> _voicePaths;
	}
}
