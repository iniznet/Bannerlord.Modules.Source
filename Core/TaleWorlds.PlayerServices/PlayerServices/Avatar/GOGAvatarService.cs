using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Library;

namespace TaleWorlds.PlayerServices.Avatar
{
	// Token: 0x0200000A RID: 10
	public class GOGAvatarService : IAvatarService
	{
		// Token: 0x0600005F RID: 95 RVA: 0x00002F8C File Offset: 0x0000118C
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

		// Token: 0x06000060 RID: 96 RVA: 0x00002FDD File Offset: 0x000011DD
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

		// Token: 0x06000061 RID: 97 RVA: 0x00003008 File Offset: 0x00001208
		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			int num = (int)((uint)playerId.Id2 % (uint)this._avatarImagesAsByteArrays.Count);
			return new AvatarData(this._avatarImagesAsByteArrays[num]);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000303B File Offset: 0x0000123B
		public bool IsInitialized()
		{
			return this._isInitalized;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003043 File Offset: 0x00001243
		public void Tick(float dt)
		{
		}

		// Token: 0x04000028 RID: 40
		private readonly Dictionary<ulong, AvatarData> _avatarImageCache = new Dictionary<ulong, AvatarData>();

		// Token: 0x04000029 RID: 41
		private readonly string _resourceFolder = BasePath.Name + "Modules/Native/MultiplayerForcedAvatars/";

		// Token: 0x0400002A RID: 42
		private readonly List<byte[]> _avatarImagesAsByteArrays = new List<byte[]>();

		// Token: 0x0400002B RID: 43
		private bool _isInitalized;
	}
}
