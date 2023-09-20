using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.PlatformService.Epic
{
	public class EpicPlatformAvatarService : IAvatarService
	{
		public EpicPlatformAvatarService()
		{
			this._avatarImagesAsByteArrays = new List<byte[]>();
		}

		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			int num = (int)((uint)playerId.Id2 % (uint)this._avatarImagesAsByteArrays.Count);
			return new AvatarData(this._avatarImagesAsByteArrays[num]);
		}

		public void Initialize()
		{
			if (this._isInitialized)
			{
				return;
			}
			this._avatarImagesAsByteArrays.Clear();
			foreach (string text in Directory.GetFiles(this._resourceFolder, "*.png"))
			{
				this._avatarImagesAsByteArrays.Add(File.ReadAllBytes(text));
			}
			this._isInitialized = true;
		}

		public bool IsInitialized()
		{
			return this._isInitialized;
		}

		public void ClearCache()
		{
			if (!this._isInitialized)
			{
				return;
			}
			this._avatarImagesAsByteArrays.Clear();
			this._isInitialized = false;
		}

		public void Tick(float dt)
		{
		}

		private readonly string _resourceFolder = BasePath.Name + "Modules\\Native\\MultiplayerForcedAvatars\\";

		private readonly List<byte[]> _avatarImagesAsByteArrays;

		private bool _isInitialized;
	}
}
