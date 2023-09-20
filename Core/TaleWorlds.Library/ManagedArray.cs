using System;

namespace TaleWorlds.Library
{
	[Serializable]
	public struct ManagedArray
	{
		public ManagedArray(IntPtr array, int length)
		{
			this.Array = array;
			this.Length = length;
		}

		internal IntPtr Array;

		internal int Length;
	}
}
