using System;
using System.Collections.Generic;
using Steamworks;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Steam
{
	public class SteamLauncherModuleExtension : IPlatformModuleExtension
	{
		public SteamLauncherModuleExtension()
		{
			this._modulePaths = new List<string>();
		}

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
						if (SteamUGC.GetItemInstallInfo(array[num], ref num2, ref text, 4096U, ref num3))
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

		public string[] GetModulePaths()
		{
			return this._modulePaths.ToArray();
		}

		public void Destroy()
		{
			if (this._steamInitialized)
			{
				SteamAPI.Shutdown();
			}
		}

		private bool _steamInitialized;

		private List<string> _modulePaths;
	}
}
