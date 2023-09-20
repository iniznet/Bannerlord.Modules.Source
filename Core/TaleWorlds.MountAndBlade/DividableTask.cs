using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000128 RID: 296
	public class DividableTask
	{
		// Token: 0x06000DD3 RID: 3539 RVA: 0x00026E66 File Offset: 0x00025066
		public DividableTask(DividableTask continueToTask = null)
		{
			this._continueToTask = continueToTask;
			this.ResetTaskStatus();
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x00026E7B File Offset: 0x0002507B
		public void ResetTaskStatus()
		{
			this._isMainTaskFinished = false;
			this._isTaskCompletelyFinished = false;
			this._lastActionCalled = false;
		}

		// Token: 0x06000DD5 RID: 3541 RVA: 0x00026E92 File Offset: 0x00025092
		public void SetTaskFinished(bool callLastAction = false)
		{
			if (callLastAction)
			{
				Action lastAction = this._lastAction;
				if (lastAction != null)
				{
					lastAction();
				}
				this._lastActionCalled = true;
			}
			this._isTaskCompletelyFinished = true;
			this._isMainTaskFinished = true;
		}

		// Token: 0x06000DD6 RID: 3542 RVA: 0x00026EC0 File Offset: 0x000250C0
		public bool Update()
		{
			if (!this._isTaskCompletelyFinished)
			{
				if (!this._isMainTaskFinished && this.UpdateExtra())
				{
					this._isMainTaskFinished = true;
				}
				if (this._isMainTaskFinished)
				{
					DividableTask continueToTask = this._continueToTask;
					this._isTaskCompletelyFinished = continueToTask == null || continueToTask.Update();
				}
			}
			if (this._isTaskCompletelyFinished && !this._lastActionCalled)
			{
				Action lastAction = this._lastAction;
				if (lastAction != null)
				{
					lastAction();
				}
				this._lastActionCalled = true;
			}
			return this._isTaskCompletelyFinished;
		}

		// Token: 0x06000DD7 RID: 3543 RVA: 0x00026F3A File Offset: 0x0002513A
		public void SetLastAction(Action action)
		{
			this._lastAction = action;
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x00026F43 File Offset: 0x00025143
		protected virtual bool UpdateExtra()
		{
			return true;
		}

		// Token: 0x0400036E RID: 878
		private bool _isTaskCompletelyFinished;

		// Token: 0x0400036F RID: 879
		private bool _isMainTaskFinished;

		// Token: 0x04000370 RID: 880
		private bool _lastActionCalled;

		// Token: 0x04000371 RID: 881
		private DividableTask _continueToTask;

		// Token: 0x04000372 RID: 882
		private Action _lastAction;
	}
}
