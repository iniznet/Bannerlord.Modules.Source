using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.SaveSystem.Load
{
	public class LoadResult
	{
		public object Root { get; private set; }

		public bool Successful { get; private set; }

		public LoadError[] Errors { get; private set; }

		public MetaData MetaData { get; private set; }

		private LoadResult()
		{
		}

		internal static LoadResult CreateSuccessful(object root, MetaData metaData, LoadCallbackInitializator loadCallbackInitializator)
		{
			return new LoadResult
			{
				Root = root,
				Successful = true,
				MetaData = metaData,
				_loadCallbackInitializator = loadCallbackInitializator
			};
		}

		internal static LoadResult CreateFailed(IEnumerable<LoadError> errors)
		{
			return new LoadResult
			{
				Successful = false,
				Errors = errors.ToArray<LoadError>()
			};
		}

		public void InitializeObjects()
		{
			this._loadCallbackInitializator.InitializeObjects();
		}

		private LoadCallbackInitializator _loadCallbackInitializator;
	}
}
