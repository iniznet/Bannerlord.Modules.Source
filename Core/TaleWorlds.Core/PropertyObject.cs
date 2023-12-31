﻿using System;
using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public class PropertyObject : MBObjectBase
	{
		internal static void AutoGeneratedStaticCollectObjectsPropertyObject(object o, List<object> collectedObjects)
		{
			((PropertyObject)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		public TextObject Name
		{
			get
			{
				return this._name;
			}
		}

		public TextObject Description
		{
			get
			{
				return this._description;
			}
		}

		public PropertyObject(string stringId)
			: base(stringId)
		{
		}

		public void Initialize(TextObject name, TextObject description)
		{
			base.Initialize();
			this._name = name;
			this._description = description;
		}

		private TextObject _name;

		private TextObject _description;
	}
}
