using System;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.PlatformService.Steam
{
	// Token: 0x02000005 RID: 5
	public class SteamPlatformAvatarService : IAvatarService
	{
		// Token: 0x0600002A RID: 42 RVA: 0x000024A6 File Offset: 0x000006A6
		public SteamPlatformAvatarService(SteamPlatformServices steamPlatformServices)
		{
			this._steamPlatformServices = steamPlatformServices;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000024B8 File Offset: 0x000006B8
		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			AvatarData avatarData = new AvatarData();
			this.FetchPlayerAvatar(avatarData, playerId);
			return avatarData;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000024D4 File Offset: 0x000006D4
		public async void FetchPlayerAvatar(AvatarData avatarData, PlayerId playerId)
		{
			AvatarData avatarData2 = await ((IPlatformServices)this._steamPlatformServices).GetUserAvatar(playerId);
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

		// Token: 0x0600002D RID: 45 RVA: 0x0000251D File Offset: 0x0000071D
		public void Initialize()
		{
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000251F File Offset: 0x0000071F
		public void ClearCache()
		{
			this._steamPlatformServices.ClearAvatarCache();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000252C File Offset: 0x0000072C
		public bool IsInitialized()
		{
			return true;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x0000252F File Offset: 0x0000072F
		public void Tick(float dt)
		{
		}

		// Token: 0x0400000E RID: 14
		private SteamPlatformServices _steamPlatformServices;
	}
}
