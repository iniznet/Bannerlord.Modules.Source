using System;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	internal sealed class ThreadedClientSessionLoginTask : ThreadedClientSessionTask
	{
		public LoginResult LoginResult { get; private set; }

		public ThreadedClientSessionLoginTask(IClientSession session, LoginMessage message)
			: base(session)
		{
			this._message = message;
			this._taskCompletionSource = new TaskCompletionSource<bool>();
		}

		public override void BeginJob()
		{
			this._task = this.Login();
		}

		private async Task Login()
		{
			LoginResult loginResult = await base.Session.Login(this._message);
			this.LoginResult = loginResult;
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

		private LoginMessage _message;

		private Task _task;
	}
}
