using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200000B RID: 11
	public class BrushFactory
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00003A41 File Offset: 0x00001C41
		public IEnumerable<Brush> Brushes
		{
			get
			{
				return this._brushes.Values;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00003A4E File Offset: 0x00001C4E
		public Brush DefaultBrush
		{
			get
			{
				if (this._brushes.ContainsKey("DefaultBrush"))
				{
					return this._brushes["DefaultBrush"];
				}
				return null;
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00003A74 File Offset: 0x00001C74
		public BrushFactory(ResourceDepot resourceDepot, string resourceFolder, SpriteData spriteData, FontFactory fontFactory)
		{
			this._spriteData = spriteData;
			this._fontFactory = fontFactory;
			this._brushes = new Dictionary<string, Brush>();
			this._brushCategories = new Dictionary<string, string>();
			this._resourceDepot = resourceDepot;
			this._resourceDepot.OnResourceChange += this.OnResourceChange;
			this._resourceFolder = resourceFolder;
			this._lastWriteTimes = new Dictionary<string, DateTime>();
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00003ADC File Offset: 0x00001CDC
		private void OnResourceChange()
		{
			this.CheckForUpdates();
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00003AE4 File Offset: 0x00001CE4
		public void Initialize()
		{
			this.LoadBrushes();
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00003AEC File Offset: 0x00001CEC
		private BrushAnimation LoadBrushAnimationFrom(XmlNode animationNode)
		{
			BrushAnimation brushAnimation = new BrushAnimation();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (object obj in animationNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				string value = xmlAttribute.Value;
				dictionary.Add(name, value);
			}
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				string key = keyValuePair.Key;
				string value2 = keyValuePair.Value;
				if (key == "Name")
				{
					brushAnimation.Name = value2;
				}
				else if (key == "Duration")
				{
					brushAnimation.Duration = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "Loop")
				{
					brushAnimation.Loop = value2 == "true";
				}
			}
			foreach (object obj2 in animationNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj2;
				XmlAttribute xmlAttribute2 = xmlNode.Attributes["LayerName"];
				string text = null;
				if (xmlAttribute2 != null)
				{
					text = xmlAttribute2.Value;
				}
				string value3 = xmlNode.Attributes["PropertyName"].Value;
				BrushAnimationProperty brushAnimationProperty = new BrushAnimationProperty();
				if (Enum.TryParse<BrushAnimationProperty.BrushAnimationPropertyType>(value3, out brushAnimationProperty.PropertyType))
				{
					brushAnimationProperty.LayerName = text;
					brushAnimation.AddAnimationProperty(brushAnimationProperty);
					foreach (object obj3 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj3;
						float num = Convert.ToSingle(xmlNode2.Attributes["Time"].Value, CultureInfo.InvariantCulture);
						XmlAttribute xmlAttribute3 = xmlNode2.Attributes["Value"];
						BrushAnimationKeyFrame brushAnimationKeyFrame = new BrushAnimationKeyFrame();
						switch (brushAnimationProperty.PropertyType)
						{
						case BrushAnimationProperty.BrushAnimationPropertyType.ColorFactor:
						case BrushAnimationProperty.BrushAnimationPropertyType.AlphaFactor:
						case BrushAnimationProperty.BrushAnimationPropertyType.HueFactor:
						case BrushAnimationProperty.BrushAnimationPropertyType.SaturationFactor:
						case BrushAnimationProperty.BrushAnimationPropertyType.ValueFactor:
						case BrushAnimationProperty.BrushAnimationPropertyType.OverlayXOffset:
						case BrushAnimationProperty.BrushAnimationPropertyType.OverlayYOffset:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineAmount:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowRadius:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextBlur:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowOffset:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowAngle:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextColorFactor:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextAlphaFactor:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextHueFactor:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextSaturationFactor:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextValueFactor:
						case BrushAnimationProperty.BrushAnimationPropertyType.XOffset:
						case BrushAnimationProperty.BrushAnimationPropertyType.YOffset:
						case BrushAnimationProperty.BrushAnimationPropertyType.OverridenWidth:
						case BrushAnimationProperty.BrushAnimationPropertyType.OverridenHeight:
						case BrushAnimationProperty.BrushAnimationPropertyType.ExtendLeft:
						case BrushAnimationProperty.BrushAnimationPropertyType.ExtendRight:
						case BrushAnimationProperty.BrushAnimationPropertyType.ExtendTop:
						case BrushAnimationProperty.BrushAnimationPropertyType.ExtendBottom:
						case BrushAnimationProperty.BrushAnimationPropertyType.FontSize:
							brushAnimationKeyFrame.InitializeAsFloat(num, Convert.ToSingle(xmlAttribute3.Value, CultureInfo.InvariantCulture));
							break;
						case BrushAnimationProperty.BrushAnimationPropertyType.Color:
						case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor:
						case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineColor:
							brushAnimationKeyFrame.InitializeAsColor(num, Color.ConvertStringToColor(xmlAttribute3.Value));
							break;
						case BrushAnimationProperty.BrushAnimationPropertyType.Sprite:
						case BrushAnimationProperty.BrushAnimationPropertyType.OverlaySprite:
							brushAnimationKeyFrame.InitializeAsSprite(num, this._spriteData.GetSprite(xmlAttribute3.Value));
							break;
						}
						brushAnimationProperty.AddKeyFrame(brushAnimationKeyFrame);
					}
				}
			}
			return brushAnimation;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00003E80 File Offset: 0x00002080
		private void LoadBrushLayerInto(XmlNode styleSpriteNode, IBrushLayerData brushLayer)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (object obj in styleSpriteNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				string value = xmlAttribute.Value;
				dictionary.Add(name, value);
			}
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				string key = keyValuePair.Key;
				string value2 = keyValuePair.Value;
				if (key == "Sprite")
				{
					brushLayer.Sprite = this._spriteData.GetSprite(value2);
				}
				else if (key == "Color")
				{
					brushLayer.Color = Color.ConvertStringToColor(value2);
				}
				else if (key == "ColorFactor")
				{
					brushLayer.ColorFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "AlphaFactor")
				{
					brushLayer.AlphaFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "HueFactor")
				{
					brushLayer.HueFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "SaturationFactor")
				{
					brushLayer.SaturationFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "ValueFactor")
				{
					brushLayer.ValueFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "XOffset")
				{
					brushLayer.XOffset = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "YOffset")
				{
					brushLayer.YOffset = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "OverridenWidth")
				{
					brushLayer.OverridenWidth = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "OverridenHeight")
				{
					brushLayer.OverridenHeight = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "IsHidden")
				{
					brushLayer.IsHidden = value2 == "true";
				}
				else if (key == "UseOverlayAlphaAsMask")
				{
					brushLayer.UseOverlayAlphaAsMask = value2 == "true";
				}
				else if (key == "WidthPolicy")
				{
					brushLayer.WidthPolicy = (BrushLayerSizePolicy)Enum.Parse(typeof(BrushLayerSizePolicy), value2);
				}
				else if (key == "HeightPolicy")
				{
					brushLayer.HeightPolicy = (BrushLayerSizePolicy)Enum.Parse(typeof(BrushLayerSizePolicy), value2);
				}
				else if (key == "HorizontalFlip")
				{
					brushLayer.HorizontalFlip = value2 == "true";
				}
				else if (key == "VerticalFlip")
				{
					brushLayer.VerticalFlip = value2 == "true";
				}
				else if (key == "OverlayMethod")
				{
					brushLayer.OverlayMethod = (BrushOverlayMethod)Enum.Parse(typeof(BrushOverlayMethod), value2);
				}
				else if (key == "OverlaySprite")
				{
					brushLayer.OverlaySprite = this._spriteData.GetSprite(value2);
				}
				else if (key == "ExtendTop")
				{
					brushLayer.ExtendTop = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "ExtendBottom")
				{
					brushLayer.ExtendBottom = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "ExtendLeft")
				{
					brushLayer.ExtendLeft = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "ExtendRight")
				{
					brushLayer.ExtendRight = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "OverlayXOffset")
				{
					brushLayer.OverlayXOffset = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "OverlayYOffset")
				{
					brushLayer.OverlayYOffset = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "UseRandomBaseOverlayXOffset")
				{
					brushLayer.UseRandomBaseOverlayXOffset = value2 == "true";
				}
				else if (key == "UseRandomBaseOverlayYOffset")
				{
					brushLayer.UseRandomBaseOverlayYOffset = value2 == "true";
				}
				else if (key == "Name" && string.IsNullOrEmpty(brushLayer.Name))
				{
					brushLayer.Name = value2;
				}
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00004374 File Offset: 0x00002574
		private void LoadStyleInto(XmlNode styleNode, Style style)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (object obj in styleNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				string value = xmlAttribute.Value;
				dictionary.Add(name, value);
			}
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				string key = keyValuePair.Key;
				string value2 = keyValuePair.Value;
				if (key == "Name")
				{
					style.Name = value2;
				}
				else if (key == "FontColor")
				{
					style.FontColor = Color.ConvertStringToColor(value2);
				}
				else if (key == "AnimationMode")
				{
					style.AnimationMode = (StyleAnimationMode)Enum.Parse(typeof(StyleAnimationMode), value2);
				}
				else if (key == "AnimationToPlayOnBegin")
				{
					style.AnimationToPlayOnBegin = value2;
				}
				else if (key == "TextGlowColor")
				{
					style.TextGlowColor = Color.ConvertStringToColor(value2);
				}
				else if (key == "TextOutlineColor")
				{
					style.TextOutlineColor = Color.ConvertStringToColor(value2);
				}
				else if (key == "TextOutlineAmount")
				{
					style.TextOutlineAmount = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "TextGlowRadius")
				{
					style.TextGlowRadius = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "TextBlur")
				{
					style.TextBlur = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "TextShadowOffset")
				{
					style.TextShadowOffset = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "TextShadowAngle")
				{
					style.TextShadowAngle = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "TextColorFactor")
				{
					style.TextColorFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "TextAlphaFactor")
				{
					style.TextAlphaFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "TextHueFactor")
				{
					style.TextHueFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "TextSaturationFactor")
				{
					style.TextSaturationFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "TextValueFactor")
				{
					style.TextValueFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "Font")
				{
					style.Font = this._fontFactory.GetFont(value2);
				}
				else if (key == "FontSize")
				{
					style.FontSize = Convert.ToInt32(value2);
				}
			}
			foreach (object obj2 in styleNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj2;
				string value3 = xmlNode.Attributes["Name"].Value;
				StyleLayer layer = style.GetLayer(value3);
				this.LoadBrushLayerInto(xmlNode, layer);
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00004738 File Offset: 0x00002938
		private void LoadSoundPropertiesInto(XmlNode soundPropertiesNode, SoundProperties soundProperties)
		{
			XmlNode xmlNode = soundPropertiesNode.SelectSingleNode("StateSounds");
			XmlNode xmlNode2 = soundPropertiesNode.SelectSingleNode("EventSounds");
			if (xmlNode != null)
			{
				foreach (object obj in xmlNode)
				{
					XmlNode xmlNode3 = (XmlNode)obj;
					AudioProperty audioProperty = new AudioProperty();
					string value = xmlNode3.Attributes["StateName"].Value;
					string value2 = xmlNode3.Attributes["Audio"].Value;
					audioProperty.AudioName = value2;
					soundProperties.AddStateSound(value, audioProperty);
				}
			}
			if (xmlNode2 != null)
			{
				foreach (object obj2 in xmlNode2)
				{
					XmlNode xmlNode4 = (XmlNode)obj2;
					AudioProperty audioProperty2 = new AudioProperty();
					string value3 = xmlNode4.Attributes["EventName"].Value;
					string value4 = xmlNode4.Attributes["Audio"].Value;
					audioProperty2.AudioName = value4;
					soundProperties.AddEventSound(value3, audioProperty2);
				}
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004870 File Offset: 0x00002A70
		private Brush LoadBrushFrom(XmlNode brushNode)
		{
			Brush brush = new Brush();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (object obj in brushNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				string value = xmlAttribute.Value;
				dictionary.Add(name, value);
			}
			if (dictionary.ContainsKey("BaseBrush"))
			{
				string text = dictionary["BaseBrush"];
				if (this._brushes.ContainsKey(text))
				{
					Brush brush2 = this._brushes[text];
					brush.FillFrom(brush2);
				}
			}
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				string key = keyValuePair.Key;
				string value2 = keyValuePair.Value;
				if (key == "Name")
				{
					brush.Name = value2;
				}
				else if (key == "Font")
				{
					brush.Font = this._fontFactory.GetFont(value2);
				}
				else if (key == "FontSize")
				{
					brush.FontSize = Convert.ToInt32(value2);
				}
				else if (key == "TransitionDuration")
				{
					brush.TransitionDuration = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "TextHorizontalAlignment")
				{
					brush.TextHorizontalAlignment = (TextHorizontalAlignment)Enum.Parse(typeof(TextHorizontalAlignment), value2);
				}
				else if (key == "TextVerticalAlignment")
				{
					brush.TextVerticalAlignment = (TextVerticalAlignment)Enum.Parse(typeof(TextVerticalAlignment), value2);
				}
				else if (key == "GlobalColorFactor")
				{
					brush.GlobalColorFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "GlobalAlphaFactor")
				{
					brush.GlobalAlphaFactor = Convert.ToSingle(value2, CultureInfo.InvariantCulture);
				}
				else if (key == "GlobalColor")
				{
					brush.GlobalColor = Color.ConvertStringToColor(value2);
				}
			}
			XmlNode xmlNode = brushNode.SelectSingleNode("Layers");
			if (xmlNode != null)
			{
				foreach (object obj2 in xmlNode)
				{
					XmlNode xmlNode2 = (XmlNode)obj2;
					string value3 = xmlNode2.Attributes["Name"].Value;
					BrushLayer brushLayer = brush.GetLayer(value3);
					if (brushLayer != null)
					{
						this.LoadBrushLayerInto(xmlNode2, brushLayer);
					}
					else
					{
						brushLayer = new BrushLayer();
						this.LoadBrushLayerInto(xmlNode2, brushLayer);
						brush.AddLayer(brushLayer);
					}
				}
			}
			XmlNode xmlNode3 = brushNode.SelectSingleNode("Styles");
			if (xmlNode3 != null)
			{
				foreach (object obj3 in xmlNode3)
				{
					XmlNode xmlNode4 = (XmlNode)obj3;
					string value4 = xmlNode4.Attributes["Name"].Value;
					Style style = brush.GetStyle(value4);
					if (style != null)
					{
						style.DefaultStyle = brush.DefaultStyle;
						this.LoadStyleInto(xmlNode4, style);
					}
					else
					{
						style = new Style(brush.Layers);
						style.DefaultStyle = brush.DefaultStyle;
						this.LoadStyleInto(xmlNode4, style);
						brush.AddStyle(style);
					}
				}
			}
			XmlNode xmlNode5 = brushNode.SelectSingleNode("Animations");
			if (xmlNode5 != null)
			{
				foreach (object obj4 in xmlNode5)
				{
					XmlNode xmlNode6 = (XmlNode)obj4;
					BrushAnimation brushAnimation = this.LoadBrushAnimationFrom(xmlNode6);
					brush.AddAnimation(brushAnimation);
				}
			}
			XmlNode xmlNode7 = brushNode.SelectSingleNode("SoundProperties");
			if (xmlNode7 != null)
			{
				this.LoadSoundPropertiesInto(xmlNode7, brush.SoundProperties);
			}
			else if (this.DefaultBrush != null && !brush.SoundProperties.RegisteredEventSounds.Any<KeyValuePair<string, AudioProperty>>() && !brush.SoundProperties.RegisteredStateSounds.Any<KeyValuePair<string, AudioProperty>>())
			{
				brush.SoundProperties.FillFrom(this.DefaultBrush.SoundProperties);
			}
			return brush;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00004D2C File Offset: 0x00002F2C
		private void SaveBrushTo(XmlNode brushNode, Brush brush)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (PropertyInfo propertyInfo in brush.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				object value = propertyInfo.GetValue(brush);
				if ((value == null && propertyInfo.GetValue(this.DefaultBrush) != null) || (value != null && !value.Equals(propertyInfo.GetValue(this.DefaultBrush))))
				{
					this.AddAttributeTo(propertyInfo, value, dictionary);
				}
			}
			brushNode.Attributes.RemoveAll();
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				XmlAttribute xmlAttribute = brushNode.OwnerDocument.CreateAttribute(keyValuePair.Key);
				xmlAttribute.InnerText = keyValuePair.Value;
				brushNode.Attributes.Append(xmlAttribute);
			}
			XmlNode xmlNode = brushNode.SelectSingleNode("Layers");
			if (xmlNode == null)
			{
				xmlNode = brushNode.OwnerDocument.CreateElement("Layers");
				brushNode.AppendChild(xmlNode);
			}
			else
			{
				xmlNode.RemoveAll();
			}
			foreach (BrushLayer brushLayer in brush.Layers)
			{
				XmlNode xmlNode2 = brushNode.OwnerDocument.CreateElement("BrushLayer");
				xmlNode.AppendChild(xmlNode2);
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				foreach (PropertyInfo propertyInfo2 in brushLayer.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					object value2 = propertyInfo2.GetValue(brushLayer);
					if ((value2 == null && propertyInfo2.GetValue(this.DefaultBrush.DefaultLayer) != null) || (value2 != null && !value2.Equals(propertyInfo2.GetValue(this.DefaultBrush.DefaultLayer))) || propertyInfo2.Name == "Name")
					{
						this.AddAttributeTo(propertyInfo2, value2, dictionary2);
					}
				}
				foreach (KeyValuePair<string, string> keyValuePair2 in dictionary2)
				{
					XmlAttribute xmlAttribute2 = brushNode.OwnerDocument.CreateAttribute(keyValuePair2.Key);
					xmlAttribute2.InnerText = keyValuePair2.Value;
					xmlNode2.Attributes.Append(xmlAttribute2);
				}
			}
			XmlNode xmlNode3 = brushNode.SelectSingleNode("Styles");
			if (xmlNode3 == null)
			{
				xmlNode3 = brushNode.OwnerDocument.CreateElement("Styles");
				brushNode.AppendChild(xmlNode3);
			}
			else
			{
				xmlNode3.RemoveAll();
			}
			foreach (Style style in brush.Styles)
			{
				XmlNode xmlNode4 = brushNode.OwnerDocument.CreateElement("Style");
				xmlNode3.AppendChild(xmlNode4);
				Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
				foreach (PropertyInfo propertyInfo3 in style.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					object value3 = propertyInfo3.GetValue(style);
					if ((value3 == null && propertyInfo3.GetValue(this.DefaultBrush.DefaultStyle) != null) || (value3 != null && !value3.Equals(propertyInfo3.GetValue(this.DefaultBrush.DefaultStyle))) || propertyInfo3.Name == "Name")
					{
						this.AddAttributeTo(propertyInfo3, value3, dictionary3);
					}
				}
				foreach (KeyValuePair<string, string> keyValuePair3 in dictionary3)
				{
					XmlAttribute xmlAttribute3 = brushNode.OwnerDocument.CreateAttribute(keyValuePair3.Key);
					xmlAttribute3.InnerText = keyValuePair3.Value;
					xmlNode4.Attributes.Append(xmlAttribute3);
				}
				foreach (StyleLayer styleLayer in style.Layers)
				{
					XmlNode xmlNode5 = brushNode.OwnerDocument.CreateElement("BrushLayer");
					xmlNode4.AppendChild(xmlNode5);
					Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
					foreach (PropertyInfo propertyInfo4 in styleLayer.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						BrushAnimationProperty.BrushAnimationPropertyType brushAnimationPropertyType;
						if (Enum.TryParse<BrushAnimationProperty.BrushAnimationPropertyType>(propertyInfo4.Name, true, out brushAnimationPropertyType) && (styleLayer.GetIsValueChanged(brushAnimationPropertyType) || brushAnimationPropertyType == BrushAnimationProperty.BrushAnimationPropertyType.Name))
						{
							object value4 = propertyInfo4.GetValue(styleLayer);
							this.AddAttributeTo(propertyInfo4, value4, dictionary4);
						}
					}
					foreach (KeyValuePair<string, string> keyValuePair4 in dictionary4)
					{
						XmlAttribute xmlAttribute4 = brushNode.OwnerDocument.CreateAttribute(keyValuePair4.Key);
						xmlAttribute4.InnerText = keyValuePair4.Value;
						xmlNode5.Attributes.Append(xmlAttribute4);
					}
				}
			}
			if (brush.GetAnimations().Any<BrushAnimation>())
			{
				XmlNode xmlNode6 = brushNode.SelectSingleNode("Animations");
				if (xmlNode6 == null)
				{
					xmlNode6 = brushNode.OwnerDocument.CreateElement("Animations");
					brushNode.AppendChild(xmlNode6);
				}
				else
				{
					xmlNode6.RemoveAll();
				}
				using (IEnumerator<BrushAnimation> enumerator5 = brush.GetAnimations().GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						BrushAnimation brushAnimation = enumerator5.Current;
						XmlNode xmlNode7 = brushNode.OwnerDocument.CreateElement("Animation");
						xmlNode6.AppendChild(xmlNode7);
						XmlAttribute xmlAttribute5 = xmlNode7.OwnerDocument.CreateAttribute("Name");
						xmlAttribute5.InnerText = brushAnimation.Name;
						xmlNode7.Attributes.Append(xmlAttribute5);
						XmlAttribute xmlAttribute6 = xmlNode7.OwnerDocument.CreateAttribute("Duration");
						xmlAttribute6.InnerText = brushAnimation.Duration.ToString();
						xmlNode7.Attributes.Append(xmlAttribute6);
						XmlAttribute xmlAttribute7 = xmlNode7.OwnerDocument.CreateAttribute("Loop");
						xmlAttribute7.InnerText = (brushAnimation.Loop ? "true" : "false");
						xmlNode7.Attributes.Append(xmlAttribute7);
						foreach (BrushLayerAnimation brushLayerAnimation in brushAnimation.GetLayerAnimations())
						{
							foreach (BrushAnimationProperty brushAnimationProperty in brushLayerAnimation.Collections)
							{
								XmlNode xmlNode8 = xmlNode7.OwnerDocument.CreateElement("AnimationProperty");
								xmlNode7.AppendChild(xmlNode8);
								if (!string.IsNullOrEmpty(brushAnimationProperty.LayerName))
								{
									XmlAttribute xmlAttribute8 = xmlNode8.OwnerDocument.CreateAttribute("LayerName");
									xmlAttribute8.InnerText = brushAnimationProperty.LayerName;
									xmlNode8.Attributes.Append(xmlAttribute8);
								}
								XmlAttribute xmlAttribute9 = xmlNode8.OwnerDocument.CreateAttribute("PropertyName");
								xmlAttribute9.InnerText = brushAnimationProperty.PropertyType.ToString();
								xmlNode8.Attributes.Append(xmlAttribute9);
								PropertyInfo property = typeof(BrushLayer).GetProperty(brushAnimationProperty.PropertyType.ToString());
								foreach (BrushAnimationKeyFrame brushAnimationKeyFrame in brushAnimationProperty.KeyFrames)
								{
									XmlNode xmlNode9 = xmlNode7.OwnerDocument.CreateElement("KeyFrame");
									xmlNode7.AppendChild(xmlNode9);
									Dictionary<string, string> dictionary5 = new Dictionary<string, string>();
									this.AddAttributeTo(property, brushAnimationKeyFrame.GetValueAsObject(), dictionary5);
									foreach (KeyValuePair<string, string> keyValuePair5 in dictionary5)
									{
										XmlAttribute xmlAttribute10 = xmlNode9.OwnerDocument.CreateAttribute("Time");
										xmlAttribute10.InnerText = brushAnimationKeyFrame.Time.ToString();
										xmlNode9.Attributes.Append(xmlAttribute10);
										XmlAttribute xmlAttribute11 = xmlNode9.OwnerDocument.CreateAttribute("Value");
										xmlAttribute11.InnerText = keyValuePair5.Value;
										xmlNode9.Attributes.Append(xmlAttribute11);
									}
								}
							}
						}
					}
					goto IL_83D;
				}
			}
			XmlNode xmlNode10 = brushNode.SelectSingleNode("Animations");
			if (xmlNode10 != null)
			{
				brushNode.RemoveChild(xmlNode10);
			}
			IL_83D:
			if (brush.SoundProperties != null)
			{
				XmlNode xmlNode11 = brushNode.SelectSingleNode("SoundProperties");
				if (xmlNode11 == null)
				{
					xmlNode11 = brushNode.OwnerDocument.CreateElement("SoundProperties");
					brushNode.AppendChild(xmlNode11);
				}
				else
				{
					xmlNode11.RemoveAll();
				}
				XmlNode xmlNode12 = brushNode.OwnerDocument.CreateElement("StateSounds");
				xmlNode11.AppendChild(xmlNode12);
				foreach (KeyValuePair<string, AudioProperty> keyValuePair6 in brush.SoundProperties.RegisteredStateSounds)
				{
					XmlNode xmlNode13 = brushNode.OwnerDocument.CreateElement("StateSound");
					xmlNode12.AppendChild(xmlNode13);
					XmlAttribute xmlAttribute12 = xmlNode13.OwnerDocument.CreateAttribute("StateName");
					xmlAttribute12.InnerText = keyValuePair6.Key;
					xmlNode13.Attributes.Append(xmlAttribute12);
					XmlAttribute xmlAttribute13 = xmlNode13.OwnerDocument.CreateAttribute("Audio");
					xmlAttribute13.InnerText = keyValuePair6.Value.AudioName;
					xmlNode13.Attributes.Append(xmlAttribute13);
				}
				XmlNode xmlNode14 = brushNode.OwnerDocument.CreateElement("EventSounds");
				xmlNode11.AppendChild(xmlNode14);
				foreach (KeyValuePair<string, AudioProperty> keyValuePair7 in brush.SoundProperties.RegisteredEventSounds)
				{
					XmlNode xmlNode15 = brushNode.OwnerDocument.CreateElement("EventSound");
					xmlNode14.AppendChild(xmlNode15);
					XmlAttribute xmlAttribute14 = xmlNode15.OwnerDocument.CreateAttribute("EventName");
					xmlAttribute14.InnerText = keyValuePair7.Key;
					xmlNode15.Attributes.Append(xmlAttribute14);
					XmlAttribute xmlAttribute15 = xmlNode15.OwnerDocument.CreateAttribute("Audio");
					xmlAttribute15.InnerText = keyValuePair7.Value.AudioName;
					xmlNode15.Attributes.Append(xmlAttribute15);
				}
			}
			Uri uri = new Uri(brushNode.OwnerDocument.BaseURI);
			brushNode.OwnerDocument.Save(uri.LocalPath);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000058DC File Offset: 0x00003ADC
		private void AddAttributeTo(PropertyInfo targetPropertyInfo, object targetPropertyValue, Dictionary<string, string> attributePairs)
		{
			if (targetPropertyInfo.PropertyType != typeof(string) && (targetPropertyInfo.PropertyType.GetInterface("IEnumerable") != null || targetPropertyInfo.PropertyType.GetInterface("ICollection") != null || targetPropertyInfo.PropertyType.GetInterface("IList") != null))
			{
				return;
			}
			object[] customAttributes = targetPropertyInfo.GetCustomAttributes(typeof(EditorAttribute), true);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				if (targetPropertyInfo.PropertyType == typeof(Color))
				{
					Color color = (Color)targetPropertyValue;
					attributePairs.Add(targetPropertyInfo.Name, color.ToString());
					return;
				}
				if (targetPropertyInfo.PropertyType == typeof(Brush))
				{
					Brush brush = targetPropertyValue as Brush;
					if (brush != null)
					{
						attributePairs.Add(targetPropertyInfo.Name, brush.Name);
						return;
					}
				}
				else if (targetPropertyInfo.PropertyType == typeof(Font))
				{
					Font font = targetPropertyValue as Font;
					if (font != null)
					{
						attributePairs.Add(targetPropertyInfo.Name, this._fontFactory.GetFontName(font));
						return;
					}
				}
				else
				{
					if (targetPropertyValue == null)
					{
						attributePairs.Add(targetPropertyInfo.Name, "");
						return;
					}
					attributePairs.Add(targetPropertyInfo.Name, targetPropertyValue.ToString());
				}
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00005A38 File Offset: 0x00003C38
		private void LoadBrushes()
		{
			this._brushes.Clear();
			this._brushCategories.Clear();
			this._lastWriteTimes.Clear();
			List<string> brushesNames = this.GetBrushesNames();
			this.LoadBrushFile("Base");
			foreach (string text in brushesNames)
			{
				if (text != "Base")
				{
					this.LoadBrushFile(text);
				}
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00005AC4 File Offset: 0x00003CC4
		public void LoadBrushFile(string name)
		{
			string filePath = this._resourceDepot.GetFilePath(this._resourceFolder + "/" + name + ".xml");
			DateTime lastWriteTime = File.GetLastWriteTime(filePath);
			if (this._lastWriteTimes.ContainsKey(name))
			{
				this._lastWriteTimes[name] = lastWriteTime;
			}
			else
			{
				this._lastWriteTimes.Add(name, lastWriteTime);
			}
			XmlDocument xmlDocument = new XmlDocument();
			using (XmlReader xmlReader = XmlReader.Create(filePath, new XmlReaderSettings
			{
				IgnoreComments = true
			}))
			{
				xmlDocument.Load(xmlReader);
			}
			foreach (object obj in xmlDocument.SelectSingleNode("Brushes").ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				Brush brush = this.LoadBrushFrom(xmlNode);
				if (this._brushes.ContainsKey(brush.Name))
				{
					this._brushes[brush.Name] = brush;
				}
				else
				{
					this._brushes.Add(brush.Name, brush);
				}
				if (this._brushCategories.ContainsKey(brush.Name))
				{
					this._brushCategories[brush.Name] = filePath;
				}
				else
				{
					this._brushCategories.Add(brush.Name, filePath);
				}
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00005C44 File Offset: 0x00003E44
		public Brush GetBrush(string name)
		{
			Brush brush;
			if (this._brushes.TryGetValue(name, out brush))
			{
				return brush;
			}
			return null;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00005C64 File Offset: 0x00003E64
		public bool SaveBrushAs(string name, Brush brush)
		{
			if (!this._brushCategories.ContainsKey(name))
			{
				Debug.FailedAssert("Brush not found", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushFactory.cs", "SaveBrushAs", 1084);
			}
			string text = this._brushCategories[name];
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(text);
			foreach (object obj in xmlDocument.SelectSingleNode("Brushes").ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				foreach (object obj2 in xmlNode.Attributes)
				{
					XmlAttribute xmlAttribute = (XmlAttribute)obj2;
					if (xmlAttribute.Name.Equals("Name") && xmlAttribute.Value.Equals(name))
					{
						this.SaveBrushTo(xmlNode, brush);
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00005D7C File Offset: 0x00003F7C
		private List<string> GetBrushesNames()
		{
			string[] files = this._resourceDepot.GetFiles(this._resourceFolder, ".xml", false);
			List<string> list = new List<string>();
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(array[i]);
				list.Add(fileNameWithoutExtension);
			}
			return list;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00005DC8 File Offset: 0x00003FC8
		public void CheckForUpdates()
		{
			bool flag = false;
			List<string> brushesNames = this.GetBrushesNames();
			foreach (string text in this._lastWriteTimes.Keys)
			{
				if (!brushesNames.Contains(text))
				{
					flag = true;
				}
			}
			foreach (string text2 in brushesNames)
			{
				DateTime lastWriteTime = File.GetLastWriteTime(this._resourceDepot.GetFilePath(this._resourceFolder + "/" + text2 + ".xml"));
				if (this._lastWriteTimes.ContainsKey(text2))
				{
					if (this._lastWriteTimes[text2] != lastWriteTime)
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.LoadBrushes();
				Action brushChange = this.BrushChange;
				if (brushChange == null)
				{
					return;
				}
				brushChange();
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000B6 RID: 182 RVA: 0x00005ED4 File Offset: 0x000040D4
		// (remove) Token: 0x060000B7 RID: 183 RVA: 0x00005F0C File Offset: 0x0000410C
		public event Action BrushChange;

		// Token: 0x04000038 RID: 56
		private Dictionary<string, Brush> _brushes;

		// Token: 0x04000039 RID: 57
		private Dictionary<string, string> _brushCategories;

		// Token: 0x0400003A RID: 58
		private ResourceDepot _resourceDepot;

		// Token: 0x0400003B RID: 59
		private readonly string _resourceFolder;

		// Token: 0x0400003C RID: 60
		private Dictionary<string, DateTime> _lastWriteTimes;

		// Token: 0x0400003D RID: 61
		private SpriteData _spriteData;

		// Token: 0x0400003E RID: 62
		private FontFactory _fontFactory;
	}
}
