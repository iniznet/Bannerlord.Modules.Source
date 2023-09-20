using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public static class CampaignOptionsManager
	{
		public static bool GetOptionWithIdExists(string identifier)
		{
			return !string.IsNullOrEmpty(identifier) && CampaignOptionsManager._currentOptions.Any((ICampaignOptionData x) => x.GetIdentifier() == identifier);
		}

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

		public static void ClearCachedOptions()
		{
			CampaignOptionsManager._currentOptions.Clear();
		}

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

		private static readonly List<ICampaignOptionProvider> _optionProviders = new List<ICampaignOptionProvider>();

		private static List<ICampaignOptionData> _currentOptions = new List<ICampaignOptionData>();
	}
}
