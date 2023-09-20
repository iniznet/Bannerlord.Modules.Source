using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Library;

namespace TaleWorlds.PlayerServices.Avatar
{
	// Token: 0x02000009 RID: 9
	internal class ForcedAvatarService : IAvatarService
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002E93 File Offset: 0x00001093
		public int AvatarCount
		{
			get
			{
				return this._avatarImagesAsByteArrays.Count;
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002EA0 File Offset: 0x000010A0
		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			if (this._avatarImagesAsByteArrays.Count == 0)
			{
				return new AvatarData();
			}
			return this.GetForcedPlayerAvatar(AvatarServices.GetForcedAvatarIndexOfPlayer(playerId));
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002EC1 File Offset: 0x000010C1
		private AvatarData GetForcedPlayerAvatar(int forcedIndex)
		{
			return new AvatarData(this._avatarImagesAsByteArrays[forcedIndex]);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002ED4 File Offset: 0x000010D4
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

		// Token: 0x0600005B RID: 91 RVA: 0x00002F3D File Offset: 0x0000113D
		public bool IsInitialized()
		{
			return this._isInitialized;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002F45 File Offset: 0x00001145
		public void ClearCache()
		{
			if (!this._isInitialized)
			{
				return;
			}
			this._avatarImagesAsByteArrays.Clear();
			this._isInitialized = false;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002F62 File Offset: 0x00001162
		public void Tick(float dt)
		{
		}

		// Token: 0x04000025 RID: 37
		private readonly string _resourceFolder = BasePath.Name + "Modules/Native/MultiplayerForcedAvatars/";

		// Token: 0x04000026 RID: 38
		private readonly List<byte[]> _avatarImagesAsByteArrays = new List<byte[]>();

		// Token: 0x04000027 RID: 39
		private bool _isInitialized;
	}
}
