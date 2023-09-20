using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	// Token: 0x02000063 RID: 99
	[MessageDescription("CustomBattleServerManager", "CustomBattleServer")]
	[Serializable]
	public class ClientWantsToConnectCustomGameMessage : Message
	{
		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000194 RID: 404 RVA: 0x00003230 File Offset: 0x00001430
		// (set) Token: 0x06000195 RID: 405 RVA: 0x00003238 File Offset: 0x00001438
		public PlayerJoinGameData[] PlayerJoinGameData { get; private set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000196 RID: 406 RVA: 0x00003241 File Offset: 0x00001441
		// (set) Token: 0x06000197 RID: 407 RVA: 0x00003249 File Offset: 0x00001449
		public string Password { get; private set; }

		// Token: 0x06000198 RID: 408 RVA: 0x00003252 File Offset: 0x00001452
		public ClientWantsToConnectCustomGameMessage(PlayerJoinGameData[] playerJoinGameData, string password)
		{
			this.PlayerJoinGameData = playerJoinGameData;
			this.Password = password;
		}
	}
}
