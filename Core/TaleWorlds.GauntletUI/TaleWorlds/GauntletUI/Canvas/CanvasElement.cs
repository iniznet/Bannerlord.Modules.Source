using System;
using System.Globalization;
using System.Numerics;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	// Token: 0x02000048 RID: 72
	public abstract class CanvasElement : CanvasObject
	{
		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060004D9 RID: 1241 RVA: 0x00015C48 File Offset: 0x00013E48
		// (set) Token: 0x060004DA RID: 1242 RVA: 0x00015C50 File Offset: 0x00013E50
		public float PositionX { get; set; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060004DB RID: 1243 RVA: 0x00015C59 File Offset: 0x00013E59
		// (set) Token: 0x060004DC RID: 1244 RVA: 0x00015C61 File Offset: 0x00013E61
		public float PositionY { get; set; }

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060004DD RID: 1245 RVA: 0x00015C6A File Offset: 0x00013E6A
		// (set) Token: 0x060004DE RID: 1246 RVA: 0x00015C72 File Offset: 0x00013E72
		public float RelativePositionX
		{
			get
			{
				return this._relativePositionX;
			}
			set
			{
				this._relativePositionX = value;
				this._usingRelativeX = true;
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060004DF RID: 1247 RVA: 0x00015C82 File Offset: 0x00013E82
		// (set) Token: 0x060004E0 RID: 1248 RVA: 0x00015C8A File Offset: 0x00013E8A
		public float RelativePositionY
		{
			get
			{
				return this._relativePositionY;
			}
			set
			{
				this._relativePositionY = value;
				this._usingRelativeY = true;
			}
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060004E1 RID: 1249 RVA: 0x00015C9A File Offset: 0x00013E9A
		// (set) Token: 0x060004E2 RID: 1250 RVA: 0x00015CA2 File Offset: 0x00013EA2
		public float PivotX { get; set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060004E3 RID: 1251 RVA: 0x00015CAB File Offset: 0x00013EAB
		// (set) Token: 0x060004E4 RID: 1252 RVA: 0x00015CB3 File Offset: 0x00013EB3
		public float PivotY { get; set; }

		// Token: 0x060004E5 RID: 1253 RVA: 0x00015CBC File Offset: 0x00013EBC
		protected CanvasElement(CanvasObject parent, FontFactory fontFactory, SpriteData spriteData)
			: base(parent, fontFactory, spriteData)
		{
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x00015CC8 File Offset: 0x00013EC8
		public virtual void LoadFrom(XmlNode canvasImageNode)
		{
			foreach (object obj in canvasImageNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				string value = xmlAttribute.Value;
				if (name == "PositionX")
				{
					this.PositionX = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
				else if (name == "PositionY")
				{
					this.PositionY = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
				else if (name == "RelativePositionX")
				{
					this.RelativePositionX = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
				else if (name == "RelativePositionY")
				{
					this.RelativePositionY = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
				else if (name == "PivotX")
				{
					this.PivotX = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
				else if (name == "PivotY")
				{
					this.PivotY = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
			}
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x00015DF0 File Offset: 0x00013FF0
		protected sealed override Vector2 Layout()
		{
			Vector2 vector = new Vector2(this.PositionX, this.PositionY);
			if (this._usingRelativeX)
			{
				vector.X = base.Parent.Width * this.RelativePositionX;
			}
			else
			{
				vector.X *= base.Scale;
			}
			if (this._usingRelativeY)
			{
				vector.Y = base.Parent.Height * this.RelativePositionY;
			}
			else
			{
				vector.Y *= base.Scale;
			}
			vector.X -= this.PivotX * base.Width;
			vector.Y -= this.PivotY * base.Height;
			return vector;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x00015EA9 File Offset: 0x000140A9
		public override Vector2 GetMarginSize()
		{
			return new Vector2(this.PositionX * base.Scale, this.PositionY * base.Scale);
		}

		// Token: 0x04000266 RID: 614
		public bool _usingRelativeX;

		// Token: 0x04000267 RID: 615
		public bool _usingRelativeY;

		// Token: 0x0400026A RID: 618
		private float _relativePositionX;

		// Token: 0x0400026B RID: 619
		private float _relativePositionY;
	}
}
