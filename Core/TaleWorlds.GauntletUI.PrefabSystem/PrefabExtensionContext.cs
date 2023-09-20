using System;
using System.Collections.Generic;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class PrefabExtensionContext
	{
		public IEnumerable<PrefabExtension> PrefabExtensions
		{
			get
			{
				return this._prefabExtensions;
			}
		}

		public PrefabExtensionContext()
		{
			this._prefabExtensions = new List<PrefabExtension>();
		}

		public void AddExtension(PrefabExtension prefabExtension)
		{
			this._prefabExtensions.Add(prefabExtension);
		}

		private List<PrefabExtension> _prefabExtensions;
	}
}
