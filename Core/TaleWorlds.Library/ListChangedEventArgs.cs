using System;

namespace TaleWorlds.Library
{
	public class ListChangedEventArgs : EventArgs
	{
		public ListChangedEventArgs(ListChangedType listChangedType, int newIndex)
		{
			this.ListChangedType = listChangedType;
			this.NewIndex = newIndex;
			this.OldIndex = -1;
		}

		public ListChangedEventArgs(ListChangedType listChangedType, int newIndex, int oldIndex)
		{
			this.ListChangedType = listChangedType;
			this.NewIndex = newIndex;
			this.OldIndex = oldIndex;
		}

		public ListChangedType ListChangedType { get; private set; }

		public int NewIndex { get; private set; }

		public int OldIndex { get; private set; }
	}
}
