using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	[LibraryInterfaceBase]
	internal interface ILibrarySizeChecker
	{
		[EngineMethod("get_engine_struct_size", false)]
		int GetEngineStructSize(string str);
	}
}
