using System;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	// Token: 0x02000004 RID: 4
	public class GameAxisKey
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600003E RID: 62 RVA: 0x000026A6 File Offset: 0x000008A6
		// (set) Token: 0x0600003F RID: 63 RVA: 0x000026AE File Offset: 0x000008AE
		public string Id { get; private set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000040 RID: 64 RVA: 0x000026B7 File Offset: 0x000008B7
		// (set) Token: 0x06000041 RID: 65 RVA: 0x000026BF File Offset: 0x000008BF
		public Key AxisKey { get; internal set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000026C8 File Offset: 0x000008C8
		// (set) Token: 0x06000043 RID: 67 RVA: 0x000026D0 File Offset: 0x000008D0
		public Key DefaultAxisKey { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000044 RID: 68 RVA: 0x000026D9 File Offset: 0x000008D9
		// (set) Token: 0x06000045 RID: 69 RVA: 0x000026E1 File Offset: 0x000008E1
		public GameKey PositiveKey { get; internal set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000046 RID: 70 RVA: 0x000026EA File Offset: 0x000008EA
		// (set) Token: 0x06000047 RID: 71 RVA: 0x000026F2 File Offset: 0x000008F2
		public GameKey NegativeKey { get; internal set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000026FB File Offset: 0x000008FB
		// (set) Token: 0x06000049 RID: 73 RVA: 0x00002703 File Offset: 0x00000903
		public GameAxisKey.AxisType Type { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600004A RID: 74 RVA: 0x0000270C File Offset: 0x0000090C
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002714 File Offset: 0x00000914
		internal bool IsBinded { get; private set; }

		// Token: 0x0600004C RID: 76 RVA: 0x00002720 File Offset: 0x00000920
		public GameAxisKey(string id, InputKey axisKey, GameKey positiveKey, GameKey negativeKey, GameAxisKey.AxisType type = GameAxisKey.AxisType.X)
		{
			this.Id = id;
			this.AxisKey = new Key(axisKey);
			this.DefaultAxisKey = new Key(axisKey);
			this.PositiveKey = positiveKey;
			this.NegativeKey = negativeKey;
			this.Type = type;
			this.IsBinded = this.PositiveKey != null || this.NegativeKey != null;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002783 File Offset: 0x00000983
		private bool IsKeyAllowed(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return (isKeysAllowed || !key.IsKeyboardInput) && (isMouseButtonAllowed || !key.IsMouseButtonInput) && (isMouseWheelAllowed || !key.IsMouseWheelInput) && (isControllerAllowed || !key.IsControllerInput);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000027B8 File Offset: 0x000009B8
		public float GetAxisState(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			GameKey positiveKey = this.PositiveKey;
			bool flag = positiveKey != null && positiveKey.IsDown(isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed, false);
			GameKey negativeKey = this.NegativeKey;
			bool flag2 = negativeKey != null && negativeKey.IsDown(isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed, false);
			if (flag || flag2)
			{
				return (flag ? 1f : 0f) - (flag2 ? 1f : 0f);
			}
			Vec2 keyState = new Vec2(0f, 0f);
			if (this.AxisKey != null && this.IsKeyAllowed(this.AxisKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				keyState = this.AxisKey.GetKeyState();
			}
			if (this.Type == GameAxisKey.AxisType.X)
			{
				return keyState.X;
			}
			if (this.Type == GameAxisKey.AxisType.Y)
			{
				return keyState.Y;
			}
			return 0f;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002880 File Offset: 0x00000A80
		public override string ToString()
		{
			string text = "";
			if (this.AxisKey != null)
			{
				text = this.AxisKey.ToString();
			}
			return text;
		}

		// Token: 0x02000011 RID: 17
		public enum AxisType
		{
			// Token: 0x0400014E RID: 334
			X,
			// Token: 0x0400014F RID: 335
			Y
		}
	}
}
