using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x020000BD RID: 189
	public sealed class SkeletonScale : MBObjectBase
	{
		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06000970 RID: 2416 RVA: 0x0001F600 File Offset: 0x0001D800
		// (set) Token: 0x06000971 RID: 2417 RVA: 0x0001F608 File Offset: 0x0001D808
		public string SkeletonModel { get; private set; }

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06000972 RID: 2418 RVA: 0x0001F611 File Offset: 0x0001D811
		// (set) Token: 0x06000973 RID: 2419 RVA: 0x0001F619 File Offset: 0x0001D819
		public Vec3 MountSitBoneScale { get; private set; }

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06000974 RID: 2420 RVA: 0x0001F622 File Offset: 0x0001D822
		// (set) Token: 0x06000975 RID: 2421 RVA: 0x0001F62A File Offset: 0x0001D82A
		public float MountRadiusAdder { get; private set; }

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06000976 RID: 2422 RVA: 0x0001F633 File Offset: 0x0001D833
		// (set) Token: 0x06000977 RID: 2423 RVA: 0x0001F63B File Offset: 0x0001D83B
		public Vec3[] Scales { get; private set; }

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06000978 RID: 2424 RVA: 0x0001F644 File Offset: 0x0001D844
		// (set) Token: 0x06000979 RID: 2425 RVA: 0x0001F64C File Offset: 0x0001D84C
		public List<string> BoneNames { get; private set; }

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x0600097A RID: 2426 RVA: 0x0001F655 File Offset: 0x0001D855
		// (set) Token: 0x0600097B RID: 2427 RVA: 0x0001F65D File Offset: 0x0001D85D
		public sbyte[] BoneIndices { get; private set; }

		// Token: 0x0600097C RID: 2428 RVA: 0x0001F666 File Offset: 0x0001D866
		public SkeletonScale()
		{
			this.BoneNames = null;
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x0001F678 File Offset: 0x0001D878
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

		// Token: 0x0600097E RID: 2430 RVA: 0x0001F908 File Offset: 0x0001DB08
		public void SetBoneIndices(sbyte[] boneIndices)
		{
			this.BoneIndices = boneIndices;
			this.BoneNames = null;
		}
	}
}
