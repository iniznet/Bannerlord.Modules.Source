using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000040 RID: 64
	public class MultiplayerAdminInformationVM : ViewModel
	{
		// Token: 0x06000571 RID: 1393 RVA: 0x00017597 File Offset: 0x00015797
		public MultiplayerAdminInformationVM()
		{
			this.MessageQueue = new MBBindingList<StringItemWithActionVM>();
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x000175AC File Offset: 0x000157AC
		public void OnNewMessageReceived(string message)
		{
			StringItemWithActionVM stringItemWithActionVM = new StringItemWithActionVM(new Action<object>(this.ExecuteRemoveMessage), message, message);
			this.MessageQueue.Add(stringItemWithActionVM);
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x000175DC File Offset: 0x000157DC
		private void ExecuteRemoveMessage(object messageToRemove)
		{
			int num = this.MessageQueue.FindIndex((StringItemWithActionVM m) => m.ActionText == messageToRemove as string);
			this.MessageQueue.RemoveAt(num);
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000574 RID: 1396 RVA: 0x0001761A File Offset: 0x0001581A
		// (set) Token: 0x06000575 RID: 1397 RVA: 0x00017622 File Offset: 0x00015822
		[DataSourceProperty]
		public MBBindingList<StringItemWithActionVM> MessageQueue
		{
			get
			{
				return this._messageQueue;
			}
			set
			{
				if (value != this._messageQueue)
				{
					this._messageQueue = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringItemWithActionVM>>(value, "MessageQueue");
				}
			}
		}

		// Token: 0x040002C8 RID: 712
		private MBBindingList<StringItemWithActionVM> _messageQueue;
	}
}
