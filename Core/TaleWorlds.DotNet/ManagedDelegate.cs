using System;

namespace TaleWorlds.DotNet
{
	public class ManagedDelegate : DotNetObject
	{
		public ManagedDelegate.DelegateDefinition Instance
		{
			get
			{
				return this._instance;
			}
			set
			{
				this._instance = value;
			}
		}

		[LibraryCallback]
		public void InvokeAux()
		{
			this.Instance();
		}

		private ManagedDelegate.DelegateDefinition _instance;

		public delegate void DelegateDefinition();
	}
}
