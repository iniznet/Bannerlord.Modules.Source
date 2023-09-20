using System;
using System.Collections.Concurrent;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000002 RID: 2
	public class ChatLogMessageManager : MessageManagerBase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public ChatLogMessageManager(MPChatVM chatDataSource)
		{
			this._chatDataSource = chatDataSource;
			this._queue = new ConcurrentQueue<ChatLogMessageManager.ChatLineData>();
			InformationManager.DisplayMessageInternal += this.OnDisplayMessageReceived;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002073 File Offset: 0x00000273
		private void OnDisplayMessageReceived(InformationMessage message)
		{
			if (!string.IsNullOrEmpty(message.SoundEventPath))
			{
				SoundEvent.PlaySound2D(message.SoundEventPath);
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002090 File Offset: 0x00000290
		public void Update()
		{
			ChatLogMessageManager.ChatLineData chatLineData;
			while (this._queue.TryDequeue(out chatLineData))
			{
				InformationManager.DisplayMessage(new InformationMessage(chatLineData.Text, Color.FromUint(chatLineData.Color), "Default"));
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020CE File Offset: 0x000002CE
		protected override void PostWarningLine(string text)
		{
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020D0 File Offset: 0x000002D0
		protected override void PostSuccessLine(string text)
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020D2 File Offset: 0x000002D2
		protected override void PostMessageLineFormatted(string text, uint color)
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020D4 File Offset: 0x000002D4
		protected override void PostMessageLine(string text, uint color)
		{
		}

		// Token: 0x04000001 RID: 1
		private const uint WarningColor = 4292235858U;

		// Token: 0x04000002 RID: 2
		private const uint SuccessColor = 4285126986U;

		// Token: 0x04000003 RID: 3
		private MPChatVM _chatDataSource;

		// Token: 0x04000004 RID: 4
		private ConcurrentQueue<ChatLogMessageManager.ChatLineData> _queue;

		// Token: 0x0200004C RID: 76
		public struct ChatLineData
		{
			// Token: 0x06000393 RID: 915 RVA: 0x00015064 File Offset: 0x00013264
			public ChatLineData(string text, uint color)
			{
				this.Text = text;
				this.Color = color;
			}

			// Token: 0x040001FE RID: 510
			public string Text;

			// Token: 0x040001FF RID: 511
			public uint Color;
		}
	}
}
