using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Library;

namespace TaleWorlds.PlayerServices.Avatar
{
	public class TestAvatarService : IAvatarService
	{
		public TestAvatarService()
		{
			this._avatarImageCache = new Dictionary<ulong, AvatarData>();
			this._avatarImagesAsByteArrays = new List<byte[]>();
		}

		public void ClearCache()
		{
			if (!this._isInitialized)
			{
				return;
			}
			this._avatarImageCache.Clear();
			this._avatarImagesAsByteArrays.Clear();
			this._isInitialized = false;
		}

		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			if (this._avatarImagesAsByteArrays.Count == 0)
			{
				return new AvatarData();
			}
			int num = (int)((uint)playerId.Id2 % (uint)this._avatarImagesAsByteArrays.Count);
			return new AvatarData(this._avatarImagesAsByteArrays[num]);
		}

		public void Initialize()
		{
			if (this._isInitialized)
			{
				return;
			}
			if (Directory.Exists(this._resourceFolder))
			{
				foreach (string text in Directory.GetFiles(this._resourceFolder, "*.jpg"))
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

		public void Tick(float dt)
		{
		}

		private readonly Dictionary<ulong, AvatarData> _avatarImageCache;

		private readonly string _resourceFolder = BasePath.Name + "Modules/Native/MultiplayerTestAvatars/";

		private readonly List<byte[]> _avatarImagesAsByteArrays;

		private bool _isInitialized;
	}
}
