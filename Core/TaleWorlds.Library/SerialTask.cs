using System;

namespace TaleWorlds.Library
{
	public class SerialTask : ITask
	{
		public SerialTask(SerialTask.DelegateDefinition function)
		{
			this._instance = function;
		}

		void ITask.Invoke()
		{
			this._instance();
		}

		void ITask.Wait()
		{
		}

		private SerialTask.DelegateDefinition _instance;

		public delegate void DelegateDefinition();
	}
}
