using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Library;

namespace TaleWorlds.PlayerServices.Avatar
{
	// Token: 0x0200000D RID: 13
	public class TestAvatarService : IAvatarService
	{
		// Token: 0x0600006D RID: 109 RVA: 0x00003125 File Offset: 0x00001325
		public TestAvatarService()
		{
			this._avatarImageCache = new Dictionary<ulong, AvatarData>();
			this._avatarImagesAsByteArrays = new List<byte[]>();
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003158 File Offset: 0x00001358
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

		// Token: 0x0600006F RID: 111 RVA: 0x00003180 File Offset: 0x00001380
		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			if (this._avatarImagesAsByteArrays.Count == 0)
			{
				return new AvatarData();
			}
			int num = (int)((uint)playerId.Id2 % (uint)this._avatarImagesAsByteArrays.Count);
			return new AvatarData(this._avatarImagesAsByteArrays[num]);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000031C8 File Offset: 0x000013C8
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

		// Token: 0x06000071 RID: 113 RVA: 0x00003226 File Offset: 0x00001426
		public bool IsInitialized()
		{
			return this._isInitialized;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000322E File Offset: 0x0000142E
		public void Tick(float dt)
		{
		}

		// Token: 0x0400002F RID: 47
		private readonly Dictionary<ulong, AvatarData> _avatarImageCache;

		// Token: 0x04000030 RID: 48
		private readonly string _resourceFolder = BasePath.Name + "Modules/Native/MultiplayerTestAvatars/";

		// Token: 0x04000031 RID: 49
		private readonly List<byte[]> _avatarImagesAsByteArrays;

		// Token: 0x04000032 RID: 50
		private bool _isInitialized;
	}
}
