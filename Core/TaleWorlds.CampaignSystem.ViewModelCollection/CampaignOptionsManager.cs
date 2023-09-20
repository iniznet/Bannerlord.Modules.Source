using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x0200000A RID: 10
	public static class CampaignOptionsManager
	{
		// Token: 0x06000082 RID: 130 RVA: 0x0000384C File Offset: 0x00001A4C
		public static bool GetOptionWithIdExists(string identifier)
		{
			return !string.IsNullOrEmpty(identifier) && CampaignOptionsManager._currentOptions.Any((ICampaignOptionData x) => x.GetIdentifier() == identifier);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000388C File Offset: 0x00001A8C
		public static void Initialize()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				foreach (Type type in assemblies[i].GetTypes())
				{
					if (type != typeof(ICampaignOptionProvider) && typeof(ICampaignOptionProvider).IsAssignableFrom(type))
					{
						ICampaignOptionProvider campaignOptionProvider = Activator.CreateInstance(type) as ICampaignOptionProvider;
						CampaignOptionsManager._optionProviders.Add(campaignOptionProvider);
					}
				}
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000390E File Offset: 0x00001B0E
		public static void ClearCachedOptions()
		{
			CampaignOptionsManager._currentOptions.Clear();
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000391C File Offset: 0x00001B1C
		public static List<ICampaignOptionData> GetGameplayCampaignOptions()
		{
			CampaignOptionsManager._currentOptions.Clear();
			for (int i = CampaignOptionsManager._optionProviders.Count - 1; i >= 0; i--)
			{
				IEnumerable<ICampaignOptionData> gameplayCampaignOptions = CampaignOptionsManager._optionProviders[i].GetGameplayCampaignOptions();
				if (gameplayCampaignOptions != null)
				{
					foreach (ICampaignOptionData campaignOptionData in gameplayCampaignOptions)
					{
						CampaignOptionsManager._currentOptions.Add(campaignOptionData);
					}
				}
			}
			return CampaignOptionsManager._currentOptions;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000039A4 File Offset: 0x00001BA4
		public static List<ICampaignOptionData> GetCharacterCreationCampaignOptions()
		{
			CampaignOptionsManager._currentOptions.Clear();
			for (int i = CampaignOptionsManager._optionProviders.Count - 1; i >= 0; i--)
			{
				IEnumerable<ICampaignOptionData> characterCreationCampaignOptions = CampaignOptionsManager._optionProviders[i].GetCharacterCreationCampaignOptions();
				if (characterCreationCampaignOptions != null)
				{
					foreach (ICampaignOptionData campaignOptionData in characterCreationCampaignOptions)
					{
						CampaignOptionsManager._currentOptions.Add(campaignOptionData);
					}
				}
			}
			return CampaignOptionsManager._currentOptions;
		}

		// Token: 0x04000053 RID: 83
		private static readonly List<ICampaignOptionProvider> _optionProviders = new List<ICampaignOptionProvider>();

		// Token: 0x04000054 RID: 84
		private static List<ICampaignOptionData> _currentOptions = new List<ICampaignOptionData>();
	}
}
