using System;
using System.Collections.Generic;
using System.Threading;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerGameLogger : GameHandler
	{
		public IReadOnlyList<GameLog> GameLogs
		{
			get
			{
				return this._gameLogs.AsReadOnly();
			}
		}

		public void Log(GameLog log)
		{
			log.Id = Interlocked.Increment(ref this._lastLogId);
			this._gameLogs.Add(log);
		}

		protected override void OnGameStart()
		{
		}

		public override void OnBeforeSave()
		{
		}

		public override void OnAfterSave()
		{
		}

		protected override void OnGameNetworkBegin()
		{
			this._lastLogId = 0;
			this._gameLogs = new List<GameLog>();
			this._chatBox = Game.Current.GetGameHandler<ChatBox>();
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(0);
			if (GameNetwork.IsServer)
			{
				networkMessageHandlerRegisterer.Register<PlayerMessageAll>(new GameNetworkMessage.ClientMessageHandlerDelegate<PlayerMessageAll>(this.HandleClientEventPlayerMessageAll));
				networkMessageHandlerRegisterer.Register<PlayerMessageTeam>(new GameNetworkMessage.ClientMessageHandlerDelegate<PlayerMessageTeam>(this.HandleClientEventPlayerMessageTeam));
			}
		}

		private bool HandleClientEventPlayerMessageAll(NetworkCommunicator networkPeer, PlayerMessageAll message)
		{
			GameLog gameLog = new GameLog(2, networkPeer.VirtualPlayer.Id, MBCommon.GetTotalMissionTime());
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

		private bool HandleClientEventPlayerMessageTeam(NetworkCommunicator networkPeer, PlayerMessageTeam message)
		{
			GameLog gameLog = new GameLog(2, networkPeer.VirtualPlayer.Id, MBCommon.GetTotalMissionTime());
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

		public const int PreInitialLogId = 0;

		private ChatBox _chatBox;

		private int _lastLogId;

		private List<GameLog> _gameLogs;
	}
}
