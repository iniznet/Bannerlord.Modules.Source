using System;
using System.IO;
using TaleWorlds.Library;

namespace psai.net
{
	// Token: 0x02000015 RID: 21
	internal class PlatformLayerStandalone : IPlatformLayer
	{
		// Token: 0x060001A1 RID: 417 RVA: 0x00008844 File Offset: 0x00006A44
		public PlatformLayerStandalone(Logik logik)
		{
			this.m_logik = logik;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00008853 File Offset: 0x00006A53
		public void Initialize()
		{
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00008855 File Offset: 0x00006A55
		public void Release()
		{
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00008858 File Offset: 0x00006A58
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

		// Token: 0x060001A5 RID: 421 RVA: 0x000088E5 File Offset: 0x00006AE5
		public Stream GetStreamOnPsaiSoundtrackFile(string filepath)
		{
			if (Logik.CheckIfFileExists(filepath))
			{
				return File.OpenRead(filepath);
			}
			return null;
		}

		// Token: 0x040000EB RID: 235
		private Logik m_logik;
	}
}
