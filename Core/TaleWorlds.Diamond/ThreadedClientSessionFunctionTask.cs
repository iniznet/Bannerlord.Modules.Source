using System;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200002C RID: 44
	internal sealed class ThreadedClientSessionFunctionTask : ThreadedClientSessionTask
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x000034E6 File Offset: 0x000016E6
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x000034EE File Offset: 0x000016EE
		public FunctionResult FunctionResult { get; private set; }

		// Token: 0x060000D4 RID: 212 RVA: 0x000034F7 File Offset: 0x000016F7
		public ThreadedClientSessionFunctionTask(IClientSession session, Message message)
			: base(session)
		{
			this._message = message;
			this._taskCompletionSource = new TaskCompletionSource<bool>();
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00003512 File Offset: 0x00001712
		public override void BeginJob()
		{
			this._task = this.CallFunction();
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00003520 File Offset: 0x00001720
		private async Task CallFunction()
		{
			FunctionResult functionResult = await base.Session.CallFunction<FunctionResult>(this._message);
			this.FunctionResult = functionResult;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00003565 File Offset: 0x00001765
		public override void DoMainThreadJob()
		{
			if (this._task.IsCompleted)
			{
				this._taskCompletionSource.SetResult(true);
				base.Finished = true;
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00003588 File Offset: 0x00001788
		public async Task Wait()
		{
			await this._taskCompletionSource.Task;
		}

		// Token: 0x04000039 RID: 57
		private TaskCompletionSource<bool> _taskCompletionSource;

		// Token: 0x0400003A RID: 58
		private Message _message;

		// Token: 0x0400003C RID: 60
		private Task _task;
	}
}
