using System;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.PlatformService.GOG
{
	public class GOGPlatformAvatarService : IAvatarService
	{
		public GOGPlatformAvatarService(GOGPlatformServices gogPlatformServices)
		{
			this._gogPlatformServices = gogPlatformServices;
		}

		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			AvatarData avatarData = new AvatarData();
			this.FetchPlayerAvatar(avatarData, playerId);
			return avatarData;
		}

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

		public void Initialize()
		{
		}

		public void ClearCache()
		{
			this._gogPlatformServices.ClearAvatarCache();
		}

		public bool IsInitialized()
		{
			return true;
		}

		public void Tick(float dt)
		{
		}

		private GOGPlatformServices _gogPlatformServices;
	}
}
