using System;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.ClientApplication
{
	public abstract class DiamondClientApplicationObject
	{
		public DiamondClientApplication Application
		{
			get
			{
				return this._application;
			}
		}

		public ApplicationVersion ApplicationVersion
		{
			get
			{
				return this.Application.ApplicationVersion;
			}
		}

		protected DiamondClientApplicationObject(DiamondClientApplication application)
		{
			this._application = application;
		}

		private DiamondClientApplication _application;
	}
}
