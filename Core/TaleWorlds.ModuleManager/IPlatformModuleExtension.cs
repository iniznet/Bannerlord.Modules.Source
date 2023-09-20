﻿using System;

namespace TaleWorlds.ModuleManager
{
	public interface IPlatformModuleExtension
	{
		void Initialize();

		void Destroy();

		string[] GetModulePaths();
	}
}
