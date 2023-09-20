using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	public class MapColorGradeManager : ScriptComponentBehavior
	{
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

		protected override void OnInit()
		{
			base.OnInit();
			this.Init();
		}

		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			this.Init();
			this.TimeOfDay = base.Scene.TimeOfDay;
			this.lastSceneTimeOfDay = this.TimeOfDay;
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

		protected override void OnTick(float dt)
		{
			this.TimeOfDay = base.Scene.TimeOfDay;
			this.SeasonTimeFactor = MBMapScene.GetSeasonTimeFactor(base.Scene);
			this.ApplyAtmosphere(false);
			this.ApplyColorGrade(dt);
		}

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

		public bool ColorGradeEnabled;

		public bool AtmosphereSimulationEnabled;

		public float TimeOfDay;

		public float SeasonTimeFactor;

		private string colorGradeGridName = "worldmap_colorgrade_grid";

		private const int colorGradeGridSize = 262144;

		private byte[] colorGradeGrid = new byte[262144];

		private Dictionary<byte, string> colorGradeGridMapping = new Dictionary<byte, string>();

		private MapColorGradeManager.ColorGradeBlendRecord primaryTransitionRecord;

		private MapColorGradeManager.ColorGradeBlendRecord secondaryTransitionRecord;

		private byte lastColorGrade;

		private Vec2 terrainSize = new Vec2(1f, 1f);

		private string defaultColorGradeTextureName = "worldmap_colorgrade_stratosphere";

		private const float transitionSpeedFactor = 1f;

		private float lastSceneTimeOfDay;

		private class ColorGradeBlendRecord
		{
			public ColorGradeBlendRecord()
			{
				this.color1 = "";
				this.color2 = "";
				this.alpha = 0f;
			}

			public ColorGradeBlendRecord(MapColorGradeManager.ColorGradeBlendRecord other)
			{
				this.color1 = other.color1;
				this.color2 = other.color2;
				this.alpha = other.alpha;
			}

			public string color1;

			public string color2;

			public float alpha;
		}
	}
}
