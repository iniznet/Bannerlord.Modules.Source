using System;

namespace TaleWorlds.PlayerServices.Avatar
{
	// Token: 0x0200000B RID: 11
	public interface IAvatarService
	{
		// Token: 0x06000065 RID: 101
		AvatarData GetPlayerAvatar(PlayerId playerId);

		// Token: 0x06000066 RID: 102
		void Initialize();

		// Token: 0x06000067 RID: 103
		void ClearCache();

		// Token: 0x06000068 RID: 104
		bool IsInitialized();

		// Token: 0x06000069 RID: 105
		void Tick(float dt);
	}
}
