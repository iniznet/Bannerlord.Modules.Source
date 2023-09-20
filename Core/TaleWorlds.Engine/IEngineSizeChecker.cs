using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[ApplicationInterfaceBase]
	internal interface IEngineSizeChecker
	{
		[EngineMethod("get_engine_struct_size", false)]
		int GetEngineStructSize(string str);
	}
}
