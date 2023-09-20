using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	public readonly struct CampaignSaveMetaDataArgs
	{
		public CampaignSaveMetaDataArgs(string[] moduleName, params KeyValuePair<string, string>[] otherArgs)
		{
			this.ModuleNames = moduleName;
			this.OtherData = otherArgs;
		}

		public readonly string[] ModuleNames;

		public readonly KeyValuePair<string, string>[] OtherData;
	}
}
