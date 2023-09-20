﻿using System;

namespace TaleWorlds.InputSystem
{
	// Token: 0x0200000E RID: 14
	public enum VirtualKeyCode
	{
		// Token: 0x040000CE RID: 206
		Invalid = -1,
		// Token: 0x040000CF RID: 207
		D1 = 49,
		// Token: 0x040000D0 RID: 208
		D2,
		// Token: 0x040000D1 RID: 209
		D3,
		// Token: 0x040000D2 RID: 210
		D4,
		// Token: 0x040000D3 RID: 211
		D5,
		// Token: 0x040000D4 RID: 212
		D6,
		// Token: 0x040000D5 RID: 213
		D7,
		// Token: 0x040000D6 RID: 214
		D8,
		// Token: 0x040000D7 RID: 215
		D9,
		// Token: 0x040000D8 RID: 216
		D0 = 48,
		// Token: 0x040000D9 RID: 217
		A = 65,
		// Token: 0x040000DA RID: 218
		B,
		// Token: 0x040000DB RID: 219
		C,
		// Token: 0x040000DC RID: 220
		D,
		// Token: 0x040000DD RID: 221
		E,
		// Token: 0x040000DE RID: 222
		F,
		// Token: 0x040000DF RID: 223
		G,
		// Token: 0x040000E0 RID: 224
		H,
		// Token: 0x040000E1 RID: 225
		I,
		// Token: 0x040000E2 RID: 226
		J,
		// Token: 0x040000E3 RID: 227
		K,
		// Token: 0x040000E4 RID: 228
		L,
		// Token: 0x040000E5 RID: 229
		M,
		// Token: 0x040000E6 RID: 230
		N,
		// Token: 0x040000E7 RID: 231
		O,
		// Token: 0x040000E8 RID: 232
		P,
		// Token: 0x040000E9 RID: 233
		Q,
		// Token: 0x040000EA RID: 234
		R,
		// Token: 0x040000EB RID: 235
		S,
		// Token: 0x040000EC RID: 236
		T,
		// Token: 0x040000ED RID: 237
		U,
		// Token: 0x040000EE RID: 238
		V,
		// Token: 0x040000EF RID: 239
		W,
		// Token: 0x040000F0 RID: 240
		X,
		// Token: 0x040000F1 RID: 241
		Y,
		// Token: 0x040000F2 RID: 242
		Z,
		// Token: 0x040000F3 RID: 243
		Numpad0 = 96,
		// Token: 0x040000F4 RID: 244
		Numpad1,
		// Token: 0x040000F5 RID: 245
		Numpad2,
		// Token: 0x040000F6 RID: 246
		Numpad3,
		// Token: 0x040000F7 RID: 247
		Numpad4,
		// Token: 0x040000F8 RID: 248
		Numpad5,
		// Token: 0x040000F9 RID: 249
		Numpad6,
		// Token: 0x040000FA RID: 250
		Numpad7,
		// Token: 0x040000FB RID: 251
		Numpad8,
		// Token: 0x040000FC RID: 252
		Numpad9,
		// Token: 0x040000FD RID: 253
		NumLock = 144,
		// Token: 0x040000FE RID: 254
		NumpadSlash = 111,
		// Token: 0x040000FF RID: 255
		NumpadMultiply = 106,
		// Token: 0x04000100 RID: 256
		NumpadMinus = 109,
		// Token: 0x04000101 RID: 257
		NumpadPlus = 107,
		// Token: 0x04000102 RID: 258
		NumpadEnter,
		// Token: 0x04000103 RID: 259
		NumpadPeriod = 110,
		// Token: 0x04000104 RID: 260
		Insert = 45,
		// Token: 0x04000105 RID: 261
		Delete,
		// Token: 0x04000106 RID: 262
		Home = 36,
		// Token: 0x04000107 RID: 263
		End = 35,
		// Token: 0x04000108 RID: 264
		PageUp = 33,
		// Token: 0x04000109 RID: 265
		PageDown,
		// Token: 0x0400010A RID: 266
		Up = 38,
		// Token: 0x0400010B RID: 267
		Down = 40,
		// Token: 0x0400010C RID: 268
		Left = 37,
		// Token: 0x0400010D RID: 269
		Right = 39,
		// Token: 0x0400010E RID: 270
		F1 = 112,
		// Token: 0x0400010F RID: 271
		F2,
		// Token: 0x04000110 RID: 272
		F3,
		// Token: 0x04000111 RID: 273
		F4,
		// Token: 0x04000112 RID: 274
		F5,
		// Token: 0x04000113 RID: 275
		F6,
		// Token: 0x04000114 RID: 276
		F7,
		// Token: 0x04000115 RID: 277
		F8,
		// Token: 0x04000116 RID: 278
		F9,
		// Token: 0x04000117 RID: 279
		F10,
		// Token: 0x04000118 RID: 280
		F11,
		// Token: 0x04000119 RID: 281
		F12,
		// Token: 0x0400011A RID: 282
		F13,
		// Token: 0x0400011B RID: 283
		F14,
		// Token: 0x0400011C RID: 284
		F15,
		// Token: 0x0400011D RID: 285
		F16,
		// Token: 0x0400011E RID: 286
		F17,
		// Token: 0x0400011F RID: 287
		F18,
		// Token: 0x04000120 RID: 288
		F19,
		// Token: 0x04000121 RID: 289
		F20,
		// Token: 0x04000122 RID: 290
		F21,
		// Token: 0x04000123 RID: 291
		F22,
		// Token: 0x04000124 RID: 292
		F23,
		// Token: 0x04000125 RID: 293
		F24,
		// Token: 0x04000126 RID: 294
		Space = 32,
		// Token: 0x04000127 RID: 295
		Escape = 27,
		// Token: 0x04000128 RID: 296
		Enter = 13,
		// Token: 0x04000129 RID: 297
		Tab = 9,
		// Token: 0x0400012A RID: 298
		BackSpace = 8,
		// Token: 0x0400012B RID: 299
		OpenBraces = 219,
		// Token: 0x0400012C RID: 300
		CloseBraces = 221,
		// Token: 0x0400012D RID: 301
		Comma = 188,
		// Token: 0x0400012E RID: 302
		Period = 190,
		// Token: 0x0400012F RID: 303
		Slash,
		// Token: 0x04000130 RID: 304
		BackSlash = 220,
		// Token: 0x04000131 RID: 305
		Equals = 187,
		// Token: 0x04000132 RID: 306
		Minus = 189,
		// Token: 0x04000133 RID: 307
		SemiColon = 186,
		// Token: 0x04000134 RID: 308
		Apostrophe = 222,
		// Token: 0x04000135 RID: 309
		Tilde = 192,
		// Token: 0x04000136 RID: 310
		CapsLock = 20,
		// Token: 0x04000137 RID: 311
		Extended1 = 223,
		// Token: 0x04000138 RID: 312
		Extended2 = 226,
		// Token: 0x04000139 RID: 313
		LeftShift = 160,
		// Token: 0x0400013A RID: 314
		RightShift,
		// Token: 0x0400013B RID: 315
		LeftControl,
		// Token: 0x0400013C RID: 316
		RightControl,
		// Token: 0x0400013D RID: 317
		LeftAlt,
		// Token: 0x0400013E RID: 318
		RightAlt
	}
}
