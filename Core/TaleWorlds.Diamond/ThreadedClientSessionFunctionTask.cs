using System;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	internal sealed class ThreadedClientSessionFunctionTask : ThreadedClientSessionTask
	{
		public FunctionResult FunctionResult { get; private set; }

		public ThreadedClientSessionFunctionTask(IClientSession session, Message message)
			: base(session)
		{
			this._message = message;
			this._taskCompletionSource = new TaskCompletionSource<bool>();
		}

		public override void BeginJob()
		{
			this._task = this.CallFunction();
		}

		private async Task CallFunction()
		{
			FunctionResult functionResult = await base.Session.CallFunction<FunctionResult>(this._message);
			this.FunctionResult = functionResult;
		}

		public override void DoMainThreadJob()
		{
			if (this._task.IsCompleted)
			{
				this._taskCompletionSource.SetResult(true);
				base.Finished = true;
			}
		}

		public async Task Wait()
		{
			await this._taskCompletionSource.Task;
		}

		private TaskCompletionSource<bool> _taskCompletionSource;

		private Message _message;

		private Task _task;
	}
}
