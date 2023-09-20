using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	// Token: 0x0200003F RID: 63
	public class MapColorGradeManager : ScriptComponentBehavior
	{
		// Token: 0x060002DF RID: 735 RVA: 0x00019CB8 File Offset: 0x00017EB8
		private void Init()
		{
			if (base.Scene.ContainsTerrain)
			{
				Vec2i vec2i;
				float num;
				int num2;
				int num3;
				base.Scene.GetTerrainData(ref vec2i, ref num, ref num2, ref num3);
				this.terrainSize.x = (float)vec2i.X * num;
				this.terrainSize.y = (float)vec2i.Y * num;
			}
			this.colorGradeGridMapping.Add(1, this.defaultColorGradeTextureName);
			this.colorGradeGridMapping.Add(2, "worldmap_colorgrade_night");
			this.ReadColorGradesXml();
			MBMapScene.GetColorGradeGridData(base.Scene, this.colorGradeGrid, this.colorGradeGridName);
			MBMapScene.LoadAtmosphereData(base.Scene);
			this.ApplyAtmosphere(true);
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00019D5F File Offset: 0x00017F5F
		protected override void OnInit()
		{
			base.OnInit();
			this.Init();
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x00019D6D File Offset: 0x00017F6D
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			this.Init();
			this.TimeOfDay = base.Scene.TimeOfDay;
			this.lastSceneTimeOfDay = this.TimeOfDay;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00019D98 File Offset: 0x00017F98
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x00019D9B File Offset: 0x00017F9B
		protected override void OnTick(float dt)
		{
			this.TimeOfDay = base.Scene.TimeOfDay;
			this.SeasonTimeFactor = MBMapScene.GetSeasonTimeFactor(base.Scene);
			this.ApplyAtmosphere(false);
			this.ApplyColorGrade(dt);
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00019DD0 File Offset: 0x00017FD0
		protected override void OnEditorTick(float dt)
		{
			if (base.Scene.TimeOfDay != this.lastSceneTimeOfDay)
			{
				this.TimeOfDay = base.Scene.TimeOfDay;
				this.lastSceneTimeOfDay = this.TimeOfDay;
			}
			if (base.Scene.ContainsTerrain)
			{
				Vec2i vec2i;
				float num;
				int num2;
				int num3;
				base.Scene.GetTerrainData(ref vec2i, ref num, ref num2, ref num3);
				this.terrainSize.x = (float)vec2i.X * num;
				this.terrainSize.y = (float)vec2i.Y * num;
			}
			else
			{
				this.terrainSize.x = 1f;
				this.terrainSize.y = 1f;
			}
			if (this.AtmosphereSimulationEnabled)
			{
				this.TimeOfDay += dt;
				if (this.TimeOfDay >= 24f)
				{
					this.TimeOfDay -= 24f;
				}
				this.ApplyAtmosphere(false);
			}
			if (this.ColorGradeEnabled)
			{
				this.ApplyColorGrade(dt);
			}
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00019EC4 File Offset: 0x000180C4
		protected override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "ColorGradeEnabled")
			{
				if (!this.ColorGradeEnabled)
				{
					base.Scene.SetColorGradeBlend("", "", -1f);
					this.lastColorGrade = 0;
					return;
				}
			}
			else
			{
				if (variableName == "TimeOfDay")
				{
					this.ApplyAtmosphere(false);
					return;
				}
				if (variableName == "SeasonTimeFactor")
				{
					this.ApplyAtmosphere(false);
				}
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00019F38 File Offset: 0x00018138
		private void ReadColorGradesXml()
		{
			List<string> list;
			XmlDocument mergedXmlForNative = MBObjectManager.GetMergedXmlForNative("soln_worldmap_color_grades", ref list);
			if (mergedXmlForNative == null)
			{
				return;
			}
			XmlNode xmlNode = mergedXmlForNative.SelectSingleNode("worldmap_color_grades");
			if (xmlNode == null)
			{
				return;
			}
			XmlNode xmlNode2 = xmlNode.SelectSingleNode("color_grade_grid");
			if (xmlNode2 != null && xmlNode2.Attributes["name"] != null)
			{
				this.colorGradeGridName = xmlNode2.Attributes["name"].Value;
			}
			XmlNode xmlNode3 = xmlNode.SelectSingleNode("color_grade_default");
			if (xmlNode3 != null && xmlNode3.Attributes["name"] != null)
			{
				this.defaultColorGradeTextureName = xmlNode3.Attributes["name"].Value;
				this.colorGradeGridMapping[1] = this.defaultColorGradeTextureName;
			}
			XmlNode xmlNode4 = xmlNode.SelectSingleNode("color_grade_night");
			if (xmlNode4 != null && xmlNode4.Attributes["name"] != null)
			{
				this.colorGradeGridMapping[2] = xmlNode4.Attributes["name"].Value;
			}
			XmlNodeList xmlNodeList = xmlNode.SelectNodes("color_grade");
			if (xmlNodeList != null)
			{
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode5 = (XmlNode)obj;
					byte b;
					if (xmlNode5.Attributes["name"] != null && xmlNode5.Attributes["value"] != null && byte.TryParse(xmlNode5.Attributes["value"].Value, out b))
					{
						this.colorGradeGridMapping.Add(b, xmlNode5.Attributes["name"].Value);
					}
				}
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0001A100 File Offset: 0x00018300
		public void ApplyAtmosphere(bool forceLoadTextures)
		{
			this.TimeOfDay = MBMath.ClampFloat(this.TimeOfDay, 0f, 23.99f);
			this.SeasonTimeFactor = MBMath.ClampFloat(this.SeasonTimeFactor, 0f, 1f);
			MBMapScene.SetFrameForAtmosphere(base.Scene, this.TimeOfDay * 10f, base.Scene.LastFinalRenderCameraFrame.origin.z, forceLoadTextures);
			float num = 0.55f;
			float num2 = -0.1f;
			float seasonTimeFactor = this.SeasonTimeFactor;
			Vec3 vec;
			vec..ctor(0f, 0.65f, 0f, -1f);
			vec.x = MBMath.Lerp(num, num2, seasonTimeFactor, 1E-05f);
			MBMapScene.SetTerrainDynamicParams(base.Scene, vec);
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0001A1C0 File Offset: 0x000183C0
		public void ApplyColorGrade(float dt)
		{
			Vec3 origin = base.Scene.LastFinalRenderCameraFrame.origin;
			int num = MathF.Floor(origin.x / this.terrainSize.X * 512f);
			int num2 = MathF.Floor(origin.y / this.terrainSize.Y * 512f);
			num = MBMath.ClampIndex(num, 0, 512);
			num2 = MBMath.ClampIndex(num2, 0, 512);
			byte b = this.colorGradeGrid[num2 * 512 + num];
			if (origin.z > 400f)
			{
				b = 1;
			}
			if (this.TimeOfDay > 22f || this.TimeOfDay < 2f)
			{
				b = 2;
			}
			if (this.lastColorGrade != b)
			{
				string text = "";
				string text2 = "";
				if (!this.colorGradeGridMapping.TryGetValue(this.lastColorGrade, out text))
				{
					text = this.defaultColorGradeTextureName;
				}
				if (!this.colorGradeGridMapping.TryGetValue(b, out text2))
				{
					text2 = this.defaultColorGradeTextureName;
				}
				if (this.primaryTransitionRecord == null)
				{
					this.primaryTransitionRecord = new MapColorGradeManager.ColorGradeBlendRecord
					{
						color1 = text,
						color2 = text2,
						alpha = 0f
					};
				}
				else
				{
					this.secondaryTransitionRecord = new MapColorGradeManager.ColorGradeBlendRecord
					{
						color1 = this.primaryTransitionRecord.color2,
						color2 = text2,
						alpha = 0f
					};
				}
				this.lastColorGrade = b;
			}
			if (this.primaryTransitionRecord != null)
			{
				if (this.primaryTransitionRecord.alpha < 1f)
				{
					this.primaryTransitionRecord.alpha = MathF.Min(this.primaryTransitionRecord.alpha + dt * 1f, 1f);
					base.Scene.SetColorGradeBlend(this.primaryTransitionRecord.color1, this.primaryTransitionRecord.color2, this.primaryTransitionRecord.alpha);
					return;
				}
				this.primaryTransitionRecord = null;
				if (this.secondaryTransitionRecord != null)
				{
					this.primaryTransitionRecord = new MapColorGradeManager.ColorGradeBlendRecord(this.secondaryTransitionRecord);
					this.secondaryTransitionRecord = null;
				}
			}
		}

		// Token: 0x040001FF RID: 511
		public bool ColorGradeEnabled;

		// Token: 0x04000200 RID: 512
		public bool AtmosphereSimulationEnabled;

		// Token: 0x04000201 RID: 513
		public float TimeOfDay;

		// Token: 0x04000202 RID: 514
		public float SeasonTimeFactor;

		// Token: 0x04000203 RID: 515
		private string colorGradeGridName = "worldmap_colorgrade_grid";

		// Token: 0x04000204 RID: 516
		private const int colorGradeGridSize = 262144;

		// Token: 0x04000205 RID: 517
		private byte[] colorGradeGrid = new byte[262144];

		// Token: 0x04000206 RID: 518
		private Dictionary<byte, string> colorGradeGridMapping = new Dictionary<byte, string>();

		// Token: 0x04000207 RID: 519
		private MapColorGradeManager.ColorGradeBlendRecord primaryTransitionRecord;

		// Token: 0x04000208 RID: 520
		private MapColorGradeManager.ColorGradeBlendRecord secondaryTransitionRecord;

		// Token: 0x04000209 RID: 521
		private byte lastColorGrade;

		// Token: 0x0400020A RID: 522
		private Vec2 terrainSize = new Vec2(1f, 1f);

		// Token: 0x0400020B RID: 523
		private string defaultColorGradeTextureName = "worldmap_colorgrade_stratosphere";

		// Token: 0x0400020C RID: 524
		private const float transitionSpeedFactor = 1f;

		// Token: 0x0400020D RID: 525
		private float lastSceneTimeOfDay;

		// Token: 0x020000B4 RID: 180
		private class ColorGradeBlendRecord
		{
			// Token: 0x0600054A RID: 1354 RVA: 0x00026E4C File Offset: 0x0002504C
			public ColorGradeBlendRecord()
			{
				this.color1 = "";
				this.color2 = "";
				this.alpha = 0f;
			}

			// Token: 0x0600054B RID: 1355 RVA: 0x00026E75 File Offset: 0x00025075
			public ColorGradeBlendRecord(MapColorGradeManager.ColorGradeBlendRecord other)
			{
				this.color1 = other.color1;
				this.color2 = other.color2;
				this.alpha = other.alpha;
			}

			// Token: 0x04000360 RID: 864
			public string color1;

			// Token: 0x04000361 RID: 865
			public string color2;

			// Token: 0x04000362 RID: 866
			public float alpha;
		}
	}
}
