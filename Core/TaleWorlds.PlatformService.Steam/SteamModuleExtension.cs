using System;
using System.Collections.Generic;
using Steamworks;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.PlatformService.Steam
{
	// Token: 0x02000004 RID: 4
	public class SteamModuleExtension : IPlatformModuleExtension
	{
		// Token: 0x06000026 RID: 38 RVA: 0x0000241A File Offset: 0x0000061A
		public SteamModuleExtension()
		{
			this._modulePaths = new List<string>();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002430 File Offset: 0x00000630
		public void Initialize()
		{
			uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
			if (numSubscribedItems > 0U)
			{
				PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
				SteamUGC.GetSubscribedItems(array, numSubscribedItems);
				int num = 0;
				while ((long)num < (long)((ulong)numSubscribedItems))
				{
					ulong num2;
					string text;
					uint num3;
					if (SteamUGC.GetItemInstallInfo(array[num], out num2, out text, 4096U, out num3))
					{
						this._modulePaths.Add(text);
					}
					num++;
				}
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x0000248C File Offset: 0x0000068C
		public string[] GetModulePaths()
		{
			return this._modulePaths.ToArray();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002499 File Offset: 0x00000699
		public void Destroy()
		{
			this._modulePaths.Clear();
		}

		// Token: 0x0400000D RID: 13
		private List<string> _modulePaths;
	}
}
