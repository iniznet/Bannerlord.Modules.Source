﻿using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.Data
{
	public interface IGauntletMovie
	{
		Widget RootWidget { get; }

		string MovieName { get; }

		void Update();

		void Release();

		void RefreshBindingWithChildren();
	}
}
