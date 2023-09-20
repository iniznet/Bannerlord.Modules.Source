using System;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond.InnerProcess
{
	// Token: 0x0200004E RID: 78
	internal class InnerProcessMessageTask
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x000055F5 File Offset: 0x000037F5
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x000055FD File Offset: 0x000037FD
		public SessionCredentials SessionCredentials { get; private set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x00005606 File Offset: 0x00003806
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x0000560E File Offset: 0x0000380E
		public Message Message { get; private set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001CA RID: 458 RVA: 0x00005617 File Offset: 0x00003817
		// (set) Token: 0x060001CB RID: 459 RVA: 0x0000561F File Offset: 0x0000381F
		public InnerProcessMessageTaskType Type { get; private set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001CC RID: 460 RVA: 0x00005628 File Offset: 0x00003828
		// (set) Token: 0x060001CD RID: 461 RVA: 0x00005630 File Offset: 0x00003830
		public bool Finished { get; private set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001CE RID: 462 RVA: 0x00005639 File Offset: 0x00003839
		// (set) Token: 0x060001CF RID: 463 RVA: 0x00005641 File Offset: 0x00003841
		public bool Successful { get; private set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000564A File Offset: 0x0000384A
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x00005652 File Offset: 0x00003852
		public FunctionResult FunctionResult { get; private set; }

		// Token: 0x060001D2 RID: 466 RVA: 0x0000565B File Offset: 0x0000385B
		public InnerProcessMessageTask(SessionCredentials sessionCredentials, Message message, InnerProcessMessageTaskType type)
		{
			this.SessionCredentials = sessionCredentials;
			this.Message = message;
			this.Type = type;
			this._taskCompletionSource = new TaskCompletionSource<bool>();
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00005684 File Offset: 0x00003884
		public async Task WaitUntilFinished()
		{
			await this._taskCompletionSource.Task;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x000056C9 File Offset: 0x000038C9
		public void SetFinishedAsSuccessful(FunctionResult functionResult)
		{
			this.FunctionResult = functionResult;
			this.Successful = true;
			this.Finished = true;
			this._taskCompletionSource.SetResult(true);
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x000056EC File Offset: 0x000038EC
		public void SetFinishedAsFailed()
		{
			this.Successful = false;
			this.Finished = true;
			this._taskCompletionSource.SetResult(true);
		}

		// Token: 0x040000A8 RID: 168
		private TaskCompletionSource<bool> _taskCompletionSource;
	}
}
