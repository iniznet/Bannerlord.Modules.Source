using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	public class MultiplayerAdminInformationVM : ViewModel
	{
		public MultiplayerAdminInformationVM()
		{
			this.MessageQueue = new MBBindingList<StringItemWithActionVM>();
		}

		public void OnNewMessageReceived(string message)
		{
			StringItemWithActionVM stringItemWithActionVM = new StringItemWithActionVM(new Action<object>(this.ExecuteRemoveMessage), message, message);
			this.MessageQueue.Add(stringItemWithActionVM);
		}

		private void ExecuteRemoveMessage(object messageToRemove)
		{
			int num = this.MessageQueue.FindIndex((StringItemWithActionVM m) => m.ActionText == messageToRemove as string);
			this.MessageQueue.RemoveAt(num);
		}

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

		private MBBindingList<StringItemWithActionVM> _messageQueue;
	}
}
