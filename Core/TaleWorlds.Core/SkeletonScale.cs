using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public sealed class SkeletonScale : MBObjectBase
	{
		public string SkeletonModel { get; private set; }

		public Vec3 MountSitBoneScale { get; private set; }

		public float MountRadiusAdder { get; private set; }

		public Vec3[] Scales { get; private set; }

		public List<string> BoneNames { get; private set; }

		public sbyte[] BoneIndices { get; private set; }

		public SkeletonScale()
		{
			this.BoneNames = null;
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.SkeletonModel = node.Attributes["skeleton"].InnerText;
			XmlAttribute xmlAttribute = node.Attributes["mount_sit_bone_scale"];
			Vec3 vec = new Vec3(1f, 1f, 1f, -1f);
			if (xmlAttribute != null)
			{
				string[] array = xmlAttribute.Value.Split(new char[] { ',' });
				if (array.Length == 3)
				{
					float.TryParse(array[0], out vec.x);
					float.TryParse(array[1], out vec.y);
					float.TryParse(array[2], out vec.z);
				}
			}
			this.MountSitBoneScale = vec;
			XmlAttribute xmlAttribute2 = node.Attributes["mount_radius_adder"];
			if (xmlAttribute2 != null)
			{
				this.MountRadiusAdder = float.Parse(xmlAttribute2.Value);
			}
			this.BoneNames = new List<string>();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string text = xmlNode.Name;
				if (text == "BoneScales")
				{
					List<Vec3> list = new List<Vec3>();
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.Attributes != null)
						{
							text = xmlNode2.Name;
							if (text == "BoneScale")
							{
								XmlAttribute xmlAttribute3 = xmlNode2.Attributes["scale"];
								Vec3 vec2 = default(Vec3);
								if (xmlAttribute3 != null)
								{
									string[] array2 = xmlAttribute3.Value.Split(new char[] { ',' });
									if (array2.Length == 3)
									{
										float.TryParse(array2[0], out vec2.x);
										float.TryParse(array2[1], out vec2.y);
										float.TryParse(array2[2], out vec2.z);
									}
								}
								this.BoneNames.Add(xmlNode2.Attributes["bone_name"].InnerText);
								list.Add(vec2);
							}
						}
					}
					this.Scales = list.ToArray();
				}
			}
		}

		public void SetBoneIndices(sbyte[] boneIndices)
		{
			this.BoneIndices = boneIndices;
			this.BoneNames = null;
		}
	}
}
