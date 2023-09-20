using System;
using System.Collections.Generic;

namespace TaleWorlds.Network
{
	public class CoroutineManager
	{
		public int CurrentTick { get; private set; }

		public int CoroutineCount
		{
			get
			{
				return this._coroutines.Count;
			}
		}

		public CoroutineManager()
		{
			this._coroutines = new List<Coroutine>();
			this.CurrentTick = 0;
		}

		public void AddCoroutine(CoroutineDelegate coroutineMethod)
		{
			Coroutine coroutine = new Coroutine();
			coroutine.CoroutineMethod = coroutineMethod;
			coroutine.IsStarted = false;
			this._coroutines.Add(coroutine);
		}

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

		private List<Coroutine> _coroutines;
	}
}
