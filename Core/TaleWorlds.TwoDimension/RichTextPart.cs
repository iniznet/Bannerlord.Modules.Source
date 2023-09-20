﻿using System;
using System.Numerics;

namespace TaleWorlds.TwoDimension
{
	public class RichTextPart
	{
		public string Style { get; set; }

		internal TextMeshGenerator TextMeshGenerator { get; set; }

		public DrawObject2D DrawObject2D { get; set; }

		public Font DefaultFont { get; set; }

		public float WordWidth { get; set; }

		public Vector2 PartPosition { get; set; }

		public Sprite Sprite { get; set; }

		public Vector2 SpritePosition { get; set; }

		public RichTextPartType Type { get; set; }

		public float Extend { get; set; }

		internal RichTextPart()
		{
			this.Style = "Default";
		}
	}
}
