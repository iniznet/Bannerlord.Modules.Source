using System;
using TaleWorlds.Core;

namespace StoryMode.StoryModeObjects
{
	// Token: 0x02000016 RID: 22
	public class StoryModeBannerEffects
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00004FA0 File Offset: 0x000031A0
		public static BannerEffect DragonBannerEffect
		{
			get
			{
				return StoryModeManager.Current.StoryModeBannerEffects._dragonBannerEffect;
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004FB1 File Offset: 0x000031B1
		public StoryModeBannerEffects()
		{
			this.RegisterAll();
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00004FBF File Offset: 0x000031BF
		private void RegisterAll()
		{
			this._dragonBannerEffect = this.Create("dragon_banner_effect");
			this.InitializeAll();
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004FD8 File Offset: 0x000031D8
		private BannerEffect Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<BannerEffect>(new BannerEffect(stringId));
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00004FEF File Offset: 0x000031EF
		private void InitializeAll()
		{
			this._dragonBannerEffect.Initialize("{=!}Not Implemented.", "{=!}Not Implemented.", 0f, 0f, 0f, -1);
		}

		// Token: 0x0400002F RID: 47
		private const string NotImplementedText = "{=!}Not Implemented.";

		// Token: 0x04000030 RID: 48
		private BannerEffect _dragonBannerEffect;
	}
}
