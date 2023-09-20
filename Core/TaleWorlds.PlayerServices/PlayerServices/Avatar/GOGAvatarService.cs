using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Library;

namespace TaleWorlds.PlayerServices.Avatar
{
	public class GOGAvatarService : IAvatarService
	{
		public void Initialize()
		{
			if (this._isInitalized)
			{
				return;
			}
			foreach (string text in Directory.GetFiles(this._resourceFolder, "*.png"))
			{
				this._avatarImagesAsByteArrays.Add(File.ReadAllBytes(text));
			}
			this._isInitalized = true;
		}

		public void ClearCache()
		{
			if (!this._isInitalized)
			{
				return;
			}
			this._avatarImageCache.Clear();
			this._avatarImagesAsByteArrays.Clear();
			this._isInitalized = false;
		}

		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			int num = (int)((uint)playerId.Id2 % (uint)this._avatarImagesAsByteArrays.Count);
			return new AvatarData(this._avatarImagesAsByteArrays[num]);
		}

		public bool IsInitialized()
		{
			return this._isInitalized;
		}

		public void Tick(float dt)
		{
		}

		private readonly Dictionary<ulong, AvatarData> _avatarImageCache = new Dictionary<ulong, AvatarData>();

		private readonly string _resourceFolder = BasePath.Name + "Modules/Native/MultiplayerForcedAvatars/";

		private readonly List<byte[]> _avatarImagesAsByteArrays = new List<byte[]>();

		private bool _isInitalized;
	}
}
