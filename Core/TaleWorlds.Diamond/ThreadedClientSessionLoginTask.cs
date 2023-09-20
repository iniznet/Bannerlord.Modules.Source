using System;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200002A RID: 42
	internal sealed class ThreadedClientSessionLoginTask : ThreadedClientSessionTask
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x000033C1 File Offset: 0x000015C1
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x000033C9 File Offset: 0x000015C9
		public LoginResult LoginResult { get; private set; }

		// Token: 0x060000C8 RID: 200 RVA: 0x000033D2 File Offset: 0x000015D2
		public ThreadedClientSessionLoginTask(IClientSession session, LoginMessage message)
			: base(session)
		{
			this._message = message;
			this._taskCompletionSource = new TaskCompletionSource<bool>();
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000033ED File Offset: 0x000015ED
		public override void BeginJob()
		{
			this._task = this.Login();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000033FC File Offset: 0x000015FC
		private async Task Login()
		{
			LoginResult loginResult = await base.Session.Login(this._message);
			this.LoginResult = loginResult;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00003441 File Offset: 0x00001641
		public override void DoMainThreadJob()
		{
			if (this._task.IsCompleted)
			{
				this._taskCompletionSource.SetResult(true);
				base.Finished = true;
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00003464 File Offset: 0x00001664
		public async Task Wait()
		{
			await this._taskCompletionSource.Task;
		}

		// Token: 0x04000034 RID: 52
		private TaskCompletionSource<bool> _taskCompletionSource;

		// Token: 0x04000035 RID: 53
		private LoginMessage _message;

		// Token: 0x04000037 RID: 55
		private Task _task;
	}
}
