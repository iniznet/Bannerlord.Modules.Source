using System;
using System.Collections.Concurrent;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class ChatLogMessageManager : MessageManagerBase
	{
		public ChatLogMessageManager(MPChatVM chatDataSource)
		{
			this._chatDataSource = chatDataSource;
			this._queue = new ConcurrentQueue<ChatLogMessageManager.ChatLineData>();
			InformationManager.DisplayMessageInternal += this.OnDisplayMessageReceived;
		}

		private void OnDisplayMessageReceived(InformationMessage message)
		{
			if (!string.IsNullOrEmpty(message.SoundEventPath))
			{
				SoundEvent.PlaySound2D(message.SoundEventPath);
			}
		}

		public void Update()
		{
			ChatLogMessageManager.ChatLineData chatLineData;
			while (this._queue.TryDequeue(out chatLineData))
			{
				InformationManager.DisplayMessage(new InformationMessage(chatLineData.Text, Color.FromUint(chatLineData.Color), "Default"));
			}
		}

		protected override void PostWarningLine(string text)
		{
		}

		protected override void PostSuccessLine(string text)
		{
		}

		protected override void PostMessageLineFormatted(string text, uint color)
		{
		}

		protected override void PostMessageLine(string text, uint color)
		{
		}

		private const uint WarningColor = 4292235858U;

		private const uint SuccessColor = 4285126986U;

		private MPChatVM _chatDataSource;

		private ConcurrentQueue<ChatLogMessageManager.ChatLineData> _queue;

		public struct ChatLineData
		{
			public ChatLineData(string text, uint color)
			{
				this.Text = text;
				this.Color = color;
			}

			public string Text;

			public uint Color;
		}
	}
}
