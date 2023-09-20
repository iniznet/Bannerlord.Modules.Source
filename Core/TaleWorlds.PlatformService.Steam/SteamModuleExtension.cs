using System;
using System.Collections.Generic;
using Steamworks;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.PlatformService.Steam
{
	public class SteamModuleExtension : IPlatformModuleExtension
	{
		public SteamModuleExtension()
		{
			this._modulePaths = new List<string>();
		}

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
					if (SteamUGC.GetItemInstallInfo(array[num], ref num2, ref text, 4096U, ref num3))
					{
						this._modulePaths.Add(text);
					}
					num++;
				}
			}
		}

		public string[] GetModulePaths()
		{
			return this._modulePaths.ToArray();
		}

		public void Destroy()
		{
			this._modulePaths.Clear();
		}

		private List<string> _modulePaths;
	}
}
