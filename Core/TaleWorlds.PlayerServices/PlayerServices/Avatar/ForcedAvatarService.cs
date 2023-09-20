using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Library;

namespace TaleWorlds.PlayerServices.Avatar
{
	internal class ForcedAvatarService : IAvatarService
	{
		public int AvatarCount
		{
			get
			{
				return this._avatarImagesAsByteArrays.Count;
			}
		}

		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			if (this._avatarImagesAsByteArrays.Count == 0)
			{
				return new AvatarData();
			}
			return this.GetForcedPlayerAvatar(AvatarServices.GetForcedAvatarIndexOfPlayer(playerId));
		}

		private AvatarData GetForcedPlayerAvatar(int forcedIndex)
		{
			return new AvatarData(this._avatarImagesAsByteArrays[forcedIndex]);
		}

		public void Initialize()
		{
			if (this._isInitialized)
			{
				return;
			}
			this._avatarImagesAsByteArrays.Clear();
			if (Directory.Exists(this._resourceFolder))
			{
				foreach (string text in Directory.GetFiles(this._resourceFolder, "*.png"))
				{
					this._avatarImagesAsByteArrays.Add(File.ReadAllBytes(text));
				}
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

		private readonly string _resourceFolder = BasePath.Name + "Modules/Native/MultiplayerForcedAvatars/";

		private readonly List<byte[]> _avatarImagesAsByteArrays = new List<byte[]>();

		private bool _isInitialized;
	}
}
