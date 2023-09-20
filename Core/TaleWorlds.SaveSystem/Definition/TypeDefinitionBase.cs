﻿using System;

namespace TaleWorlds.SaveSystem.Definition
{
	public class TypeDefinitionBase
	{
		public SaveId SaveId { get; private set; }

		public Type Type { get; private set; }

		public byte TypeLevel { get; private set; }

		protected TypeDefinitionBase(Type type, SaveId saveId)
		{
			this.Type = type;
			this.SaveId = saveId;
			this.TypeLevel = TypeDefinitionBase.GetClassLevel(type);
		}

		public static byte GetClassLevel(Type type)
		{
			byte b = 1;
			if (type.IsClass)
			{
				Type type2 = type;
				while (type2 != typeof(object))
				{
					b += 1;
					type2 = type2.BaseType;
				}
			}
			return b;
		}
	}
}
