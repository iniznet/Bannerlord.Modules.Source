using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Localization
{
	public class VoiceObject
	{
		public MBReadOnlyList<string> VoicePaths
		{
			get
			{
				return this._voicePaths;
			}
		}

		private VoiceObject()
		{
			this._voicePaths = new MBList<string>();
		}

		private void AddVoicePath(string voicePath)
		{
			this._voicePaths.Add(voicePath);
		}

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

		private readonly MBList<string> _voicePaths;
	}
}
