using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public static class ModuleInfoModelExtensions
	{
		public static bool IsCompatibleWith(this IEnumerable<ModuleInfoModel> a, IEnumerable<ModuleInfoModel> b, bool allowOptionalModules)
		{
			bool flag = (from m in a
				where !m.IsOptional
				orderby m.Id
				select m).SequenceEqual(from m in b
				where !m.IsOptional
				orderby m.Id
				select m);
			bool flag2;
			if (!a.Any((ModuleInfoModel m) => m.IsOptional))
			{
				flag2 = b.Any((ModuleInfoModel m) => m.IsOptional);
			}
			else
			{
				flag2 = true;
			}
			bool flag3 = flag2;
			return flag && (allowOptionalModules || !flag3);
		}
	}
}
