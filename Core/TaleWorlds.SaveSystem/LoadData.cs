﻿using System;

namespace TaleWorlds.SaveSystem
{
	public class LoadData
	{
		public MetaData MetaData { get; private set; }

		public GameData GameData { get; private set; }

		public LoadData(MetaData metaData, GameData gameData)
		{
			this.MetaData = metaData;
			this.GameData = gameData;
		}
	}
}
