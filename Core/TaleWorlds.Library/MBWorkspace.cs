using System;

namespace TaleWorlds.Library
{
	public class MBWorkspace<T> where T : IMBCollection, new()
	{
		public T StartUsingWorkspace()
		{
			this._isBeingUsed = true;
			if (this._workspace == null)
			{
				this._workspace = new T();
			}
			return this._workspace;
		}

		public void StopUsingWorkspace()
		{
			this._isBeingUsed = false;
			this._workspace.Clear();
		}

		public T GetWorkspace()
		{
			return this._workspace;
		}

		private bool _isBeingUsed;

		private T _workspace;
	}
}
