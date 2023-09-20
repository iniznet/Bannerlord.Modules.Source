using System;
using System.IO;
using TaleWorlds.Library;

namespace psai.net
{
	internal class PlatformLayerStandalone : IPlatformLayer
	{
		public PlatformLayerStandalone(Logik logik)
		{
			this.m_logik = logik;
		}

		public void Initialize()
		{
		}

		public void Release()
		{
		}

		public string ConvertFilePathForPlatform(string filepath)
		{
			string text = filepath.Replace('\\', '/');
			string text2;
			if (text.Contains("/"))
			{
				text2 = Path.GetDirectoryName(text) + "/" + Path.GetFileNameWithoutExtension(text);
			}
			else
			{
				text2 = Path.GetFileNameWithoutExtension(text);
			}
			if (ApplicationPlatform.CurrentPlatform == Platform.Orbis)
			{
				return "PS4/" + text2 + ".fsb";
			}
			if (ApplicationPlatform.CurrentPlatform == Platform.Durango)
			{
				return "XboxOne/" + text2 + ".fsb";
			}
			return "PC/" + text2 + ".ogg";
		}

		public Stream GetStreamOnPsaiSoundtrackFile(string filepath)
		{
			if (Logik.CheckIfFileExists(filepath))
			{
				return File.OpenRead(filepath);
			}
			return null;
		}

		private Logik m_logik;
	}
}
