using System;
using TaleWorlds.Core;

namespace StoryMode.StoryModeObjects
{
	public class StoryModeBannerEffects
	{
		public static BannerEffect DragonBannerEffect
		{
			get
			{
				return StoryModeManager.Current.StoryModeBannerEffects._dragonBannerEffect;
			}
		}

		public StoryModeBannerEffects()
		{
			this.RegisterAll();
		}

		private void RegisterAll()
		{
			this._dragonBannerEffect = this.Create("dragon_banner_effect");
			this.InitializeAll();
		}

		private BannerEffect Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<BannerEffect>(new BannerEffect(stringId));
		}

		private void InitializeAll()
		{
			this._dragonBannerEffect.Initialize("{=!}Not Implemented.", "{=!}Not Implemented.", 0f, 0f, 0f, -1);
		}

		private const string NotImplementedText = "{=!}Not Implemented.";

		private BannerEffect _dragonBannerEffect;
	}
}
