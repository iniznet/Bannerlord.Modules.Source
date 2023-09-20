using System;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002DD RID: 733
	public abstract class MultiplayerGameMode
	{
		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x0600283E RID: 10302 RVA: 0x0009BC54 File Offset: 0x00099E54
		// (set) Token: 0x0600283F RID: 10303 RVA: 0x0009BC5C File Offset: 0x00099E5C
		public string Name { get; private set; }

		// Token: 0x06002840 RID: 10304 RVA: 0x0009BC65 File Offset: 0x00099E65
		protected MultiplayerGameMode(string name)
		{
			this.Name = name;
		}

		// Token: 0x06002841 RID: 10305
		public abstract void JoinCustomGame(JoinGameData joinGameData);

		// Token: 0x06002842 RID: 10306
		public abstract void StartMultiplayerGame(string scene);
	}
}
