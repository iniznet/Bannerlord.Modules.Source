using System;
using System.Collections.Generic;
using Steamworks;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Steam
{
	// Token: 0x02000002 RID: 2
	public class SteamLauncherModuleExtension : IPlatformModuleExtension
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public SteamLauncherModuleExtension()
		{
			this._modulePaths = new List<string>();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000205C File Offset: 0x0000025C
		public void Initialize()
		{
			this._steamInitialized = SteamAPI.Init();
			if (this._steamInitialized)
			{
				if (!SteamUser.BLoggedOn())
				{
					Debug.Print("Steam user is not logged in. Please log in to Steam", 0, Debug.DebugColor.White, 17592186044416UL);
					return;
				}
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
					return;
				}
			}
			else
			{
				Debug.Print("Could not initialize Steam", 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002100 File Offset: 0x00000300
		public string[] GetModulePaths()
		{
			return this._modulePaths.ToArray();
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000210D File Offset: 0x0000030D
		public void Destroy()
		{
			if (this._steamInitialized)
			{
				SteamAPI.Shutdown();
			}
		}

		// Token: 0x04000001 RID: 1
		private bool _steamInitialized;

		// Token: 0x04000002 RID: 2
		private List<string> _modulePaths;
	}
}
