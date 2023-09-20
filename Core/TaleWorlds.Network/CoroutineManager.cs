using System;
using System.Collections.Generic;

namespace TaleWorlds.Network
{
	// Token: 0x02000004 RID: 4
	public class CoroutineManager
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002504 File Offset: 0x00000704
		// (set) Token: 0x0600001A RID: 26 RVA: 0x0000250C File Offset: 0x0000070C
		public int CurrentTick { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002515 File Offset: 0x00000715
		public int CoroutineCount
		{
			get
			{
				return this._coroutines.Count;
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002522 File Offset: 0x00000722
		public CoroutineManager()
		{
			this._coroutines = new List<Coroutine>();
			this.CurrentTick = 0;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000253C File Offset: 0x0000073C
		public void AddCoroutine(CoroutineDelegate coroutineMethod)
		{
			Coroutine coroutine = new Coroutine();
			coroutine.CoroutineMethod = coroutineMethod;
			coroutine.IsStarted = false;
			this._coroutines.Add(coroutine);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000256C File Offset: 0x0000076C
		public void Tick()
		{
			for (int i = 0; i < this._coroutines.Count; i++)
			{
				Coroutine coroutine = this._coroutines[i];
				bool flag = false;
				if (!coroutine.IsStarted)
				{
					coroutine.IsStarted = true;
					flag = true;
					coroutine.Enumerator = coroutine.CoroutineMethod();
				}
				if (flag || coroutine.CurrentState.IsFinished)
				{
					if (!coroutine.Enumerator.MoveNext())
					{
						this._coroutines.Remove(coroutine);
						i--;
					}
					else
					{
						coroutine.CurrentState = coroutine.Enumerator.Current as CoroutineState;
						coroutine.CurrentState.Initialize(this);
					}
				}
			}
			int currentTick = this.CurrentTick;
			this.CurrentTick = currentTick + 1;
		}

		// Token: 0x04000011 RID: 17
		private List<Coroutine> _coroutines;
	}
}
