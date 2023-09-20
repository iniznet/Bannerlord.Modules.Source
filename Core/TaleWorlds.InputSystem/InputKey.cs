using System;

namespace TaleWorlds.InputSystem
{
	// Token: 0x0200000D RID: 13
	public enum InputKey
	{
		// Token: 0x0400003C RID: 60
		Invalid = -1,
		// Token: 0x0400003D RID: 61
		D1 = 2,
		// Token: 0x0400003E RID: 62
		D2,
		// Token: 0x0400003F RID: 63
		D3,
		// Token: 0x04000040 RID: 64
		D4,
		// Token: 0x04000041 RID: 65
		D5,
		// Token: 0x04000042 RID: 66
		D6,
		// Token: 0x04000043 RID: 67
		D7,
		// Token: 0x04000044 RID: 68
		D8,
		// Token: 0x04000045 RID: 69
		D9,
		// Token: 0x04000046 RID: 70
		D0,
		// Token: 0x04000047 RID: 71
		A = 30,
		// Token: 0x04000048 RID: 72
		B = 48,
		// Token: 0x04000049 RID: 73
		C = 46,
		// Token: 0x0400004A RID: 74
		D = 32,
		// Token: 0x0400004B RID: 75
		E = 18,
		// Token: 0x0400004C RID: 76
		F = 33,
		// Token: 0x0400004D RID: 77
		G,
		// Token: 0x0400004E RID: 78
		H,
		// Token: 0x0400004F RID: 79
		I = 23,
		// Token: 0x04000050 RID: 80
		J = 36,
		// Token: 0x04000051 RID: 81
		K,
		// Token: 0x04000052 RID: 82
		L,
		// Token: 0x04000053 RID: 83
		M = 50,
		// Token: 0x04000054 RID: 84
		N = 49,
		// Token: 0x04000055 RID: 85
		O = 24,
		// Token: 0x04000056 RID: 86
		P,
		// Token: 0x04000057 RID: 87
		Q = 16,
		// Token: 0x04000058 RID: 88
		R = 19,
		// Token: 0x04000059 RID: 89
		S = 31,
		// Token: 0x0400005A RID: 90
		T = 20,
		// Token: 0x0400005B RID: 91
		U = 22,
		// Token: 0x0400005C RID: 92
		V = 47,
		// Token: 0x0400005D RID: 93
		W = 17,
		// Token: 0x0400005E RID: 94
		X = 45,
		// Token: 0x0400005F RID: 95
		Y = 21,
		// Token: 0x04000060 RID: 96
		Z = 44,
		// Token: 0x04000061 RID: 97
		Numpad0 = 82,
		// Token: 0x04000062 RID: 98
		Numpad1 = 79,
		// Token: 0x04000063 RID: 99
		Numpad2,
		// Token: 0x04000064 RID: 100
		Numpad3,
		// Token: 0x04000065 RID: 101
		Numpad4 = 75,
		// Token: 0x04000066 RID: 102
		Numpad5,
		// Token: 0x04000067 RID: 103
		Numpad6,
		// Token: 0x04000068 RID: 104
		Numpad7 = 71,
		// Token: 0x04000069 RID: 105
		Numpad8,
		// Token: 0x0400006A RID: 106
		Numpad9,
		// Token: 0x0400006B RID: 107
		NumLock = 197,
		// Token: 0x0400006C RID: 108
		NumpadSlash = 181,
		// Token: 0x0400006D RID: 109
		NumpadMultiply = 55,
		// Token: 0x0400006E RID: 110
		NumpadMinus = 74,
		// Token: 0x0400006F RID: 111
		NumpadPlus = 78,
		// Token: 0x04000070 RID: 112
		NumpadEnter = 156,
		// Token: 0x04000071 RID: 113
		NumpadPeriod = 83,
		// Token: 0x04000072 RID: 114
		Insert = 210,
		// Token: 0x04000073 RID: 115
		Delete,
		// Token: 0x04000074 RID: 116
		Home = 199,
		// Token: 0x04000075 RID: 117
		End = 207,
		// Token: 0x04000076 RID: 118
		PageUp = 201,
		// Token: 0x04000077 RID: 119
		PageDown = 209,
		// Token: 0x04000078 RID: 120
		Up = 200,
		// Token: 0x04000079 RID: 121
		Down = 208,
		// Token: 0x0400007A RID: 122
		Left = 203,
		// Token: 0x0400007B RID: 123
		Right = 205,
		// Token: 0x0400007C RID: 124
		F1 = 59,
		// Token: 0x0400007D RID: 125
		F2,
		// Token: 0x0400007E RID: 126
		F3,
		// Token: 0x0400007F RID: 127
		F4,
		// Token: 0x04000080 RID: 128
		F5,
		// Token: 0x04000081 RID: 129
		F6,
		// Token: 0x04000082 RID: 130
		F7,
		// Token: 0x04000083 RID: 131
		F8,
		// Token: 0x04000084 RID: 132
		F9,
		// Token: 0x04000085 RID: 133
		F10,
		// Token: 0x04000086 RID: 134
		F11 = 87,
		// Token: 0x04000087 RID: 135
		F12,
		// Token: 0x04000088 RID: 136
		F13 = 100,
		// Token: 0x04000089 RID: 137
		F14,
		// Token: 0x0400008A RID: 138
		F15,
		// Token: 0x0400008B RID: 139
		F16,
		// Token: 0x0400008C RID: 140
		F17,
		// Token: 0x0400008D RID: 141
		F18,
		// Token: 0x0400008E RID: 142
		F19,
		// Token: 0x0400008F RID: 143
		F20,
		// Token: 0x04000090 RID: 144
		F21,
		// Token: 0x04000091 RID: 145
		F22,
		// Token: 0x04000092 RID: 146
		F23,
		// Token: 0x04000093 RID: 147
		F24 = 118,
		// Token: 0x04000094 RID: 148
		Space = 57,
		// Token: 0x04000095 RID: 149
		Escape = 1,
		// Token: 0x04000096 RID: 150
		Enter = 28,
		// Token: 0x04000097 RID: 151
		Tab = 15,
		// Token: 0x04000098 RID: 152
		BackSpace = 14,
		// Token: 0x04000099 RID: 153
		OpenBraces = 26,
		// Token: 0x0400009A RID: 154
		CloseBraces,
		// Token: 0x0400009B RID: 155
		Comma = 51,
		// Token: 0x0400009C RID: 156
		Period,
		// Token: 0x0400009D RID: 157
		Slash,
		// Token: 0x0400009E RID: 158
		BackSlash = 43,
		// Token: 0x0400009F RID: 159
		Equals = 13,
		// Token: 0x040000A0 RID: 160
		Minus = 12,
		// Token: 0x040000A1 RID: 161
		SemiColon = 39,
		// Token: 0x040000A2 RID: 162
		Apostrophe,
		// Token: 0x040000A3 RID: 163
		Tilde,
		// Token: 0x040000A4 RID: 164
		CapsLock = 58,
		// Token: 0x040000A5 RID: 165
		Extended = 86,
		// Token: 0x040000A6 RID: 166
		LeftShift = 42,
		// Token: 0x040000A7 RID: 167
		RightShift = 54,
		// Token: 0x040000A8 RID: 168
		LeftControl = 29,
		// Token: 0x040000A9 RID: 169
		RightControl = 157,
		// Token: 0x040000AA RID: 170
		LeftAlt = 56,
		// Token: 0x040000AB RID: 171
		RightAlt = 184,
		// Token: 0x040000AC RID: 172
		LeftMouseButton = 224,
		// Token: 0x040000AD RID: 173
		RightMouseButton,
		// Token: 0x040000AE RID: 174
		MiddleMouseButton,
		// Token: 0x040000AF RID: 175
		X1MouseButton,
		// Token: 0x040000B0 RID: 176
		X2MouseButton,
		// Token: 0x040000B1 RID: 177
		MouseScrollUp,
		// Token: 0x040000B2 RID: 178
		MouseScrollDown,
		// Token: 0x040000B3 RID: 179
		ControllerLStick = 222,
		// Token: 0x040000B4 RID: 180
		ControllerRStick,
		// Token: 0x040000B5 RID: 181
		ControllerLStickUp = 232,
		// Token: 0x040000B6 RID: 182
		ControllerLStickDown,
		// Token: 0x040000B7 RID: 183
		ControllerLStickLeft,
		// Token: 0x040000B8 RID: 184
		ControllerLStickRight,
		// Token: 0x040000B9 RID: 185
		ControllerRStickUp,
		// Token: 0x040000BA RID: 186
		ControllerRStickDown,
		// Token: 0x040000BB RID: 187
		ControllerRStickLeft,
		// Token: 0x040000BC RID: 188
		ControllerRStickRight,
		// Token: 0x040000BD RID: 189
		ControllerLUp,
		// Token: 0x040000BE RID: 190
		ControllerLDown,
		// Token: 0x040000BF RID: 191
		ControllerLLeft,
		// Token: 0x040000C0 RID: 192
		ControllerLRight,
		// Token: 0x040000C1 RID: 193
		ControllerRUp,
		// Token: 0x040000C2 RID: 194
		ControllerRDown,
		// Token: 0x040000C3 RID: 195
		ControllerRLeft,
		// Token: 0x040000C4 RID: 196
		ControllerRRight,
		// Token: 0x040000C5 RID: 197
		ControllerLBumper,
		// Token: 0x040000C6 RID: 198
		ControllerRBumper,
		// Token: 0x040000C7 RID: 199
		ControllerLOption,
		// Token: 0x040000C8 RID: 200
		ControllerROption,
		// Token: 0x040000C9 RID: 201
		ControllerLThumb,
		// Token: 0x040000CA RID: 202
		ControllerRThumb,
		// Token: 0x040000CB RID: 203
		ControllerLTrigger,
		// Token: 0x040000CC RID: 204
		ControllerRTrigger
	}
}
