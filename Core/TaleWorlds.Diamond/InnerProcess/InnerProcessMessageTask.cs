using System;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond.InnerProcess
{
	internal class InnerProcessMessageTask
	{
		public SessionCredentials SessionCredentials { get; private set; }

		public Message Message { get; private set; }

		public InnerProcessMessageTaskType Type { get; private set; }

		public bool Finished { get; private set; }

		public bool Successful { get; private set; }

		public FunctionResult FunctionResult { get; private set; }

		public InnerProcessMessageTask(SessionCredentials sessionCredentials, Message message, InnerProcessMessageTaskType type)
		{
			this.SessionCredentials = sessionCredentials;
			this.Message = message;
			this.Type = type;
			this._taskCompletionSource = new TaskCompletionSource<bool>();
		}

		public async Task WaitUntilFinished()
		{
			await this._taskCompletionSource.Task;
		}

		public void SetFinishedAsSuccessful(FunctionResult functionResult)
		{
			this.FunctionResult = functionResult;
			this.Successful = true;
			this.Finished = true;
			this._taskCompletionSource.SetResult(true);
		}

		public void SetFinishedAsFailed()
		{
			this.Successful = false;
			this.Finished = true;
			this._taskCompletionSource.SetResult(true);
		}

		private TaskCompletionSource<bool> _taskCompletionSource;
	}
}
