using System;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.PlatformService.GOG
{
	// Token: 0x02000007 RID: 7
	public class GOGPlatformAvatarService : IAvatarService
	{
		// Token: 0x06000025 RID: 37 RVA: 0x000023C1 File Offset: 0x000005C1
		public GOGPlatformAvatarService(GOGPlatformServices gogPlatformServices)
		{
			this._gogPlatformServices = gogPlatformServices;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000023D0 File Offset: 0x000005D0
		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			AvatarData avatarData = new AvatarData();
			this.FetchPlayerAvatar(avatarData, playerId);
			return avatarData;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000023EC File Offset: 0x000005EC
		public async void FetchPlayerAvatar(AvatarData avatarData, PlayerId playerId)
		{
			AvatarData avatarData2 = await ((IPlatformServices)this._gogPlatformServices).GetUserAvatar(playerId);
			if (avatarData2 != null)
			{
				if (avatarData2.Width > 0U && avatarData2.Height > 0U)
				{
					avatarData.SetImageData(avatarData2.Image, avatarData2.Width, avatarData2.Height);
				}
				else
				{
					avatarData.SetImageData(avatarData2.Image);
				}
			}
			else
			{
				avatarData.SetFailed();
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002435 File Offset: 0x00000635
		public void Initialize()
		{
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002437 File Offset: 0x00000637
		public void ClearCache()
		{
			this._gogPlatformServices.ClearAvatarCache();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002444 File Offset: 0x00000644
		public bool IsInitialized()
		{
			return true;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002447 File Offset: 0x00000647
		public void Tick(float dt)
		{
		}

		// Token: 0x04000007 RID: 7
		private GOGPlatformServices _gogPlatformServices;
	}
}
