﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Settlements.Workshops
{
	public sealed class WorkshopType : MBObjectBase
	{
		internal static void AutoGeneratedStaticCollectObjectsWorkshopType(object o, List<object> collectedObjects)
		{
			((WorkshopType)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		public int EquipmentCost { get; private set; }

		public int Frequency { get; private set; }

		public TextObject Name { get; private set; }

		public TextObject JobName { get; private set; }

		public bool IsHidden { get; private set; }

		public string SignMeshName { get; private set; }

		public string PropMeshName1 { get; private set; }

		public string PropMeshName2 { get; private set; }

		public List<string> PropMeshName3List { get; private set; }

		public string PropMeshName4 { get; private set; }

		public string PropMeshName5 { get; private set; }

		public string PropMeshName6 { get; private set; }

		public TextObject Description { get; private set; }

		public MBReadOnlyList<WorkshopType.Production> Productions
		{
			get
			{
				return this._productions;
			}
		}

		public override string ToString()
		{
			return this.Name.ToString();
		}

		public override void Initialize()
		{
			base.Initialize();
			this._productions = new MBList<WorkshopType.Production>(0);
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.Name = new TextObject(node.Attributes["name"].Value, null);
			this.JobName = new TextObject(node.Attributes["jobname"].Value, null);
			this.Description = new TextObject(node.Attributes["description"].Value, null);
			this.EquipmentCost = int.Parse(node.Attributes["equipment_cost"].Value);
			this.Frequency = ((node.Attributes["frequency"] != null) ? int.Parse(node.Attributes["frequency"].Value) : 1);
			this.IsHidden = node.Attributes["isHidden"] != null && bool.Parse(node.Attributes["isHidden"].Value);
			this._productions = new MBList<WorkshopType.Production>();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "Production")
				{
					objectManager.CreateObjectFromXmlNode(xmlNode);
					float num = ((xmlNode.Attributes["conversion_speed"] != null) ? float.Parse(xmlNode.Attributes["conversion_speed"].Value) : 0f);
					WorkshopType.Production production = new WorkshopType.Production(num);
					foreach (object obj2 in xmlNode)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.Name == "Inputs")
						{
							foreach (object obj3 in xmlNode2)
							{
								XmlNode xmlNode3 = (XmlNode)obj3;
								if (xmlNode3.Attributes != null && xmlNode3.Attributes.Count > 0)
								{
									string value = xmlNode3.Attributes[0].Value;
									if (!string.IsNullOrEmpty(value))
									{
										ItemCategory @object = objectManager.GetObject<ItemCategory>(value);
										int num2 = ((xmlNode3.Attributes["input_count"] != null) ? int.Parse(xmlNode3.Attributes["input_count"].Value) : 1);
										if (@object != null)
										{
											production.AddInput(@object, num2);
										}
										else
										{
											Debug.Print("While reading Workshop Node: " + node.ToString() + " Unable to Find Item Category:" + value, 0, Debug.DebugColor.White, 17592186044416UL);
										}
									}
									else
									{
										Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Settlements\\Workshops\\WorkshopType.cs", "Deserialize", 146);
									}
								}
							}
						}
						if (xmlNode2.Name == "Outputs")
						{
							foreach (object obj4 in xmlNode2)
							{
								XmlNode xmlNode4 = (XmlNode)obj4;
								if (xmlNode4.Attributes.Count > 0)
								{
									objectManager.CreateObjectFromXmlNode(xmlNode4);
									ItemCategory itemCategory = objectManager.ReadObjectReferenceFromXml<ItemCategory>("output", xmlNode4);
									int num3 = ((xmlNode4.Attributes["output_count"] != null) ? int.Parse(xmlNode4.Attributes["output_count"].Value) : 1);
									production.AddOutput(itemCategory, num3);
								}
							}
						}
					}
					this._productions.Add(production);
				}
				else if (xmlNode.Name == "Meshes")
				{
					this.SignMeshName = XmlHelper.ReadString(xmlNode, "sign_mesh_name");
					this.PropMeshName1 = XmlHelper.ReadString(xmlNode, "shop_prop_mesh_name_1");
					this.PropMeshName2 = XmlHelper.ReadString(xmlNode, "shop_prop_mesh_name_2");
					this.PropMeshName3List = new List<string>();
					for (int i = 1; i < 4; i++)
					{
						string text = "shop_prop_mesh_name_3_" + i;
						if (xmlNode.Attributes[text] != null)
						{
							this.PropMeshName3List.Add(xmlNode.Attributes[text].Value);
						}
					}
					this.PropMeshName4 = XmlHelper.ReadString(xmlNode, "shop_prop_mesh_name_4");
					this.PropMeshName5 = XmlHelper.ReadString(xmlNode, "shop_prop_mesh_name_5");
					this.PropMeshName6 = XmlHelper.ReadString(xmlNode, "shop_prop_mesh_name_6");
				}
			}
			this._productions.Capacity = this._productions.Count;
		}

		public static WorkshopType Find(string idString)
		{
			return MBObjectManager.Instance.GetObject<WorkshopType>(idString);
		}

		public static WorkshopType FindFirst(Func<WorkshopType, bool> predicate)
		{
			return WorkshopType.All.FirstOrDefault(predicate);
		}

		public static MBReadOnlyList<WorkshopType> All
		{
			get
			{
				return Campaign.Current.Workshops;
			}
		}

		private MBList<WorkshopType.Production> _productions;

		public struct Production
		{
			public Production(float conversionSpeed)
			{
				this._inputs = new MBList<ValueTuple<ItemCategory, int>>();
				this._outputs = new MBList<ValueTuple<ItemCategory, int>>();
				this._conversionSpeed = conversionSpeed;
			}

			public void AddInput(ItemCategory item, int count = 1)
			{
				this._inputs.Add(new ValueTuple<ItemCategory, int>(item, count));
			}

			public void AddOutput(ItemCategory outputCategory, int outputCount)
			{
				this._outputs.Add(new ValueTuple<ItemCategory, int>(outputCategory, outputCount));
			}

			public MBReadOnlyList<ValueTuple<ItemCategory, int>> Inputs
			{
				get
				{
					return this._inputs;
				}
			}

			public MBReadOnlyList<ValueTuple<ItemCategory, int>> Outputs
			{
				get
				{
					return this._outputs;
				}
			}

			public float ConversionSpeed
			{
				get
				{
					return this._conversionSpeed;
				}
			}

			private MBList<ValueTuple<ItemCategory, int>> _inputs;

			private MBList<ValueTuple<ItemCategory, int>> _outputs;

			private float _conversionSpeed;
		}
	}
}