using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.PlatformService.Epic
{
	// Token: 0x02000004 RID: 4
	public class EpicPlatformAvatarService : IAvatarService
	{
		// Token: 0x0600001D RID: 29 RVA: 0x000022B8 File Offset: 0x000004B8
		public EpicPlatformAvatarService()
		{
			this._avatarImagesAsByteArrays = new List<byte[]>();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000022E0 File Offset: 0x000004E0
		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			int num = (int)((uint)playerId.Id2 % (uint)this._avatarImagesAsByteArrays.Count);
			return new AvatarData(this._avatarImagesAsByteArrays[num]);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002314 File Offset: 0x00000514
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

		// Token: 0x06000020 RID: 32 RVA: 0x00002370 File Offset: 0x00000570
		public bool IsInitialized()
		{
			return this._isInitialized;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002378 File Offset: 0x00000578
		public void ClearCache()
		{
			if (!this._isInitialized)
			{
				return;
			}
			this._avatarImagesAsByteArrays.Clear();
			this._isInitialized = false;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002395 File Offset: 0x00000595
		public void Tick(float dt)
		{
		}

		// Token: 0x04000006 RID: 6
		private readonly string _resourceFolder = BasePath.Name + "Modules\\Native\\MultiplayerForcedAvatars\\";

		// Token: 0x04000007 RID: 7
		private readonly List<byte[]> _avatarImagesAsByteArrays;

		// Token: 0x04000008 RID: 8
		private bool _isInitialized;
	}
}
