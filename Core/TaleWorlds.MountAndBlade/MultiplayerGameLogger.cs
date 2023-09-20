using System;
using System.Collections.Generic;
using System.Threading;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002FB RID: 763
	public class MultiplayerGameLogger : GameHandler
	{
		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x06002979 RID: 10617 RVA: 0x000A0842 File Offset: 0x0009EA42
		public IReadOnlyList<GameLog> GameLogs
		{
			get
			{
				return this._gameLogs.AsReadOnly();
			}
		}

		// Token: 0x0600297A RID: 10618 RVA: 0x000A084F File Offset: 0x0009EA4F
		public void Log(GameLog log)
		{
			log.Id = Interlocked.Increment(ref this._lastLogId);
			this._gameLogs.Add(log);
		}

		// Token: 0x0600297B RID: 10619 RVA: 0x000A086E File Offset: 0x0009EA6E
		protected override void OnGameStart()
		{
		}

		// Token: 0x0600297C RID: 10620 RVA: 0x000A0870 File Offset: 0x0009EA70
		public override void OnBeforeSave()
		{
		}

		// Token: 0x0600297D RID: 10621 RVA: 0x000A0872 File Offset: 0x0009EA72
		public override void OnAfterSave()
		{
		}

		// Token: 0x0600297E RID: 10622 RVA: 0x000A0874 File Offset: 0x0009EA74
		protected override void OnGameNetworkBegin()
		{
			this._lastLogId = 0;
			this._gameLogs = new List<GameLog>();
			this._chatBox = Game.Current.GetGameHandler<ChatBox>();
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
			if (GameNetwork.IsServer)
			{
				networkMessageHandlerRegisterer.Register<PlayerMessageAll>(new GameNetworkMessage.ClientMessageHandlerDelegate<PlayerMessageAll>(this.HandleClientEventPlayerMessageAll));
				networkMessageHandlerRegisterer.Register<PlayerMessageTeam>(new GameNetworkMessage.ClientMessageHandlerDelegate<PlayerMessageTeam>(this.HandleClientEventPlayerMessageTeam));
			}
		}

		// Token: 0x0600297F RID: 10623 RVA: 0x000A08D8 File Offset: 0x0009EAD8
		private bool HandleClientEventPlayerMessageAll(NetworkCommunicator networkPeer, PlayerMessageAll message)
		{
			GameLog gameLog = new GameLog(GameLogType.ChatMessage, networkPeer.VirtualPlayer.Id, MBCommon.GetTotalMissionTime());
			gameLog.Data.Add("Message", message.Message);
			gameLog.Data.Add("IsTeam", false.ToString());
			Dictionary<string, string> data = gameLog.Data;
			string text = "IsMuted";
			ChatBox chatBox = this._chatBox;
			data.Add(text, ((chatBox != null) ? new bool?(chatBox.IsPlayerMuted(networkPeer.VirtualPlayer.Id)) : null).ToString());
			gameLog.Data.Add("IsGlobalMuted", networkPeer.IsMuted.ToString());
			this.Log(gameLog);
			return true;
		}

		// Token: 0x06002980 RID: 10624 RVA: 0x000A099C File Offset: 0x0009EB9C
		private bool HandleClientEventPlayerMessageTeam(NetworkCommunicator networkPeer, PlayerMessageTeam message)
		{
			GameLog gameLog = new GameLog(GameLogType.ChatMessage, networkPeer.VirtualPlayer.Id, MBCommon.GetTotalMissionTime());
			gameLog.Data.Add("Message", message.Message);
			gameLog.Data.Add("IsTeam", true.ToString());
			Dictionary<string, string> data = gameLog.Data;
			string text = "IsMuted";
			ChatBox chatBox = this._chatBox;
			data.Add(text, ((chatBox != null) ? new bool?(chatBox.IsPlayerMuted(networkPeer.VirtualPlayer.Id)) : null).ToString());
			gameLog.Data.Add("IsGlobalMuted", networkPeer.IsMuted.ToString());
			this.Log(gameLog);
			return true;
		}

		// Token: 0x04000F7A RID: 3962
		public const int PreInitialLogId = 0;

		// Token: 0x04000F7B RID: 3963
		private ChatBox _chatBox;

		// Token: 0x04000F7C RID: 3964
		private int _lastLogId;

		// Token: 0x04000F7D RID: 3965
		private List<GameLog> _gameLogs;
	}
}
