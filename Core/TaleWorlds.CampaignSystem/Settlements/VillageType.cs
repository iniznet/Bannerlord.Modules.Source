﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Settlements
{
	public sealed class VillageType : MBObjectBase
	{
		internal static void AutoGeneratedStaticCollectObjectsVillageType(object o, List<object> collectedObjects)
		{
			((VillageType)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		public static MBReadOnlyList<VillageType> All
		{
			get
			{
				return Campaign.Current.AllVillageTypes;
			}
		}

		public MBReadOnlyList<ValueTuple<ItemObject, float>> Productions
		{
			get
			{
				return this._productions;
			}
		}

		public ItemObject PrimaryProduction
		{
			get
			{
				ValueTuple<ItemObject, float> valueTuple = this._productions[0];
				float num = 0f;
				foreach (ValueTuple<ItemObject, float> valueTuple2 in this._productions)
				{
					if (valueTuple2.Item2 * (float)valueTuple2.Item1.Value > num)
					{
						valueTuple = valueTuple2;
						num = valueTuple2.Item2 * (float)valueTuple2.Item1.Value;
					}
				}
				return valueTuple.Item1;
			}
		}

		public VillageType(string stringId)
			: base(stringId)
		{
		}

		public VillageType Initialize(TextObject shortName, string meshName, string meshNameUnderConstruction, string meshNameBurned, ValueTuple<ItemObject, float>[] productions)
		{
			this.ShortName = shortName;
			this.MeshName = meshName;
			this.MeshNameUnderConstruction = meshNameUnderConstruction;
			this.MeshNameBurned = meshNameBurned;
			this._productions = productions.ToMBList<ValueTuple<ItemObject, float>>();
			base.AfterInitialized();
			return this;
		}

		public override string ToString()
		{
			return this.ShortName.ToString();
		}

		public void AddProductions(IEnumerable<ValueTuple<ItemObject, float>> productions)
		{
			this._productions = productions.Concat(this._productions).ToMBList<ValueTuple<ItemObject, float>>();
		}

		public float GetProductionPerDay(ItemObject item)
		{
			foreach (ValueTuple<ItemObject, float> valueTuple in this._productions)
			{
				if (valueTuple.Item1 == item)
				{
					return valueTuple.Item2;
				}
			}
			return 0f;
		}

		public float GetProductionPerDay(ItemCategory itemCategory)
		{
			float num = 0f;
			foreach (ValueTuple<ItemObject, float> valueTuple in this._productions)
			{
				if (valueTuple.Item1 != null && valueTuple.Item1.ItemCategory == itemCategory)
				{
					num += valueTuple.Item2;
				}
			}
			return num;
		}

		private MBList<ValueTuple<ItemObject, float>> _productions;

		public TextObject ShortName;

		public string MeshName;

		public string MeshNameUnderConstruction;

		public string MeshNameBurned;
	}
}
