using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.Library
{
	// Token: 0x02000064 RID: 100
	public static class MBMath
	{
		// Token: 0x06000349 RID: 841 RVA: 0x0000A2E2 File Offset: 0x000084E2
		public static float ToRadians(this float f)
		{
			return f * 0.017453292f;
		}

		// Token: 0x0600034A RID: 842 RVA: 0x0000A2EB File Offset: 0x000084EB
		public static float ToDegrees(this float f)
		{
			return f * 57.295776f;
		}

		// Token: 0x0600034B RID: 843 RVA: 0x0000A2F4 File Offset: 0x000084F4
		public static bool ApproximatelyEqualsTo(this float f, float comparedValue, float epsilon = 1E-05f)
		{
			return Math.Abs(f - comparedValue) <= epsilon;
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0000A304 File Offset: 0x00008504
		public static bool ApproximatelyEquals(float first, float second, float epsilon = 1E-05f)
		{
			return Math.Abs(first - second) <= epsilon;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x0000A314 File Offset: 0x00008514
		public static bool IsValidValue(float f)
		{
			return !float.IsNaN(f) && !float.IsInfinity(f);
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0000A329 File Offset: 0x00008529
		public static int ClampIndex(int value, int minValue, int maxValue)
		{
			return MBMath.ClampInt(value, minValue, maxValue - 1);
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0000A335 File Offset: 0x00008535
		public static int ClampInt(int value, int minValue, int maxValue)
		{
			return Math.Max(Math.Min(value, maxValue), minValue);
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0000A344 File Offset: 0x00008544
		public static float ClampFloat(float value, float minValue, float maxValue)
		{
			return Math.Max(Math.Min(value, maxValue), minValue);
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0000A353 File Offset: 0x00008553
		public static void ClampUnit(ref float value)
		{
			value = MBMath.ClampFloat(value, 0f, 1f);
		}

		// Token: 0x06000352 RID: 850 RVA: 0x0000A368 File Offset: 0x00008568
		public static int GetNumberOfBitsToRepresentNumber(uint value)
		{
			int num = 0;
			for (uint num2 = value; num2 > 0U; num2 >>= 1)
			{
				num++;
			}
			return num;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0000A388 File Offset: 0x00008588
		public static IEnumerable<ValueTuple<T, int>> DistributeShares<T>(int totalAward, IEnumerable<T> stakeHolders, Func<T, int> shareFunction)
		{
			List<ValueTuple<T, int>> sharesList = new List<ValueTuple<T, int>>(20);
			int num = 0;
			foreach (T t in stakeHolders)
			{
				int num2 = shareFunction(t);
				sharesList.Add(new ValueTuple<T, int>(t, num2));
				num += num2;
			}
			if (num > 0)
			{
				int remainingShares = num;
				int remaingAward = totalAward;
				int i = 0;
				while (i < sharesList.Count && remaingAward > 0)
				{
					int item = sharesList[i].Item2;
					int num3 = MathF.Round((float)remaingAward * (float)item / (float)remainingShares);
					if (num3 > remaingAward)
					{
						num3 = remaingAward;
					}
					remaingAward -= num3;
					remainingShares -= item;
					yield return new ValueTuple<T, int>(sharesList[i].Item1, num3);
					int num4 = i + 1;
					i = num4;
				}
			}
			yield break;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0000A3A8 File Offset: 0x000085A8
		public static int GetNumberOfBitsToRepresentNumber(ulong value)
		{
			int num = 0;
			for (ulong num2 = value; num2 > 0UL; num2 >>= 1)
			{
				num++;
			}
			return num;
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0000A3C9 File Offset: 0x000085C9
		public static float Lerp(float valueFrom, float valueTo, float amount, float minimumDifference = 1E-05f)
		{
			if (Math.Abs(valueFrom - valueTo) <= minimumDifference)
			{
				return valueTo;
			}
			return valueFrom + (valueTo - valueFrom) * amount;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0000A3DF File Offset: 0x000085DF
		public static float LinearExtrapolation(float valueFrom, float valueTo, float amount)
		{
			return valueFrom + (valueTo - valueFrom) * amount;
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0000A3E8 File Offset: 0x000085E8
		public static Vec3 Lerp(Vec3 vecFrom, Vec3 vecTo, float amount, float minimumDifference)
		{
			return new Vec3(MBMath.Lerp(vecFrom.x, vecTo.x, amount, minimumDifference), MBMath.Lerp(vecFrom.y, vecTo.y, amount, minimumDifference), MBMath.Lerp(vecFrom.z, vecTo.z, amount, minimumDifference), -1f);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x0000A438 File Offset: 0x00008638
		public static Vec2 Lerp(Vec2 vecFrom, Vec2 vecTo, float amount, float minimumDifference)
		{
			return new Vec2(MBMath.Lerp(vecFrom.x, vecTo.x, amount, minimumDifference), MBMath.Lerp(vecFrom.y, vecTo.y, amount, minimumDifference));
		}

		// Token: 0x06000359 RID: 857 RVA: 0x0000A465 File Offset: 0x00008665
		public static float Map(float input, float inputMinimum, float inputMaximum, float outputMinimum, float outputMaximum)
		{
			input = MBMath.ClampFloat(input, inputMinimum, inputMaximum);
			return (input - inputMinimum) * (outputMaximum - outputMinimum) / (inputMaximum - inputMinimum) + outputMinimum;
		}

		// Token: 0x0600035A RID: 858 RVA: 0x0000A47F File Offset: 0x0000867F
		public static Mat3 Lerp(ref Mat3 matFrom, ref Mat3 matTo, float amount, float minimumDifference)
		{
			return new Mat3(MBMath.Lerp(matFrom.s, matTo.s, amount, minimumDifference), MBMath.Lerp(matFrom.f, matTo.f, amount, minimumDifference), MBMath.Lerp(matFrom.u, matTo.u, amount, minimumDifference));
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0000A4C0 File Offset: 0x000086C0
		public static float LerpRadians(float valueFrom, float valueTo, float amount, float minChange, float maxChange)
		{
			float smallestDifferenceBetweenTwoAngles = MBMath.GetSmallestDifferenceBetweenTwoAngles(valueFrom, valueTo);
			if (Math.Abs(smallestDifferenceBetweenTwoAngles) <= minChange)
			{
				return valueTo;
			}
			float num = (float)Math.Sign(smallestDifferenceBetweenTwoAngles) * MBMath.ClampFloat(Math.Abs(smallestDifferenceBetweenTwoAngles * amount), minChange, maxChange);
			return MBMath.WrapAngle(valueFrom + num);
		}

		// Token: 0x0600035C RID: 860 RVA: 0x0000A504 File Offset: 0x00008704
		public static float SplitLerp(float value1, float value2, float value3, float cutOff, float amount, float minimumDifference)
		{
			if (amount <= cutOff)
			{
				float num = amount / cutOff;
				return MBMath.Lerp(value1, value2, num, minimumDifference);
			}
			float num2 = 1f - cutOff;
			float num3 = (amount - cutOff) / num2;
			return MBMath.Lerp(value2, value3, num3, minimumDifference);
		}

		// Token: 0x0600035D RID: 861 RVA: 0x0000A541 File Offset: 0x00008741
		public static float InverseLerp(float valueFrom, float valueTo, float value)
		{
			return (value - valueFrom) / (valueTo - valueFrom);
		}

		// Token: 0x0600035E RID: 862 RVA: 0x0000A54C File Offset: 0x0000874C
		public static float SmoothStep(float edge0, float edge1, float value)
		{
			float num = MBMath.ClampFloat((value - edge0) / (edge1 - edge0), 0f, 1f);
			return num * num * (3f - 2f * num);
		}

		// Token: 0x0600035F RID: 863 RVA: 0x0000A584 File Offset: 0x00008784
		public static float BilinearLerp(float topLeft, float topRight, float botLeft, float botRight, float x, float y)
		{
			float num = MBMath.Lerp(topLeft, topRight, x, 1E-05f);
			float num2 = MBMath.Lerp(botLeft, botRight, x, 1E-05f);
			return MBMath.Lerp(num, num2, y, 1E-05f);
		}

		// Token: 0x06000360 RID: 864 RVA: 0x0000A5BC File Offset: 0x000087BC
		public static float GetSmallestDifferenceBetweenTwoAngles(float fromAngle, float toAngle)
		{
			float num = toAngle - fromAngle;
			if (num > 3.1415927f)
			{
				num = -6.2831855f + num;
			}
			if (num < -3.1415927f)
			{
				num = 6.2831855f + num;
			}
			return num;
		}

		// Token: 0x06000361 RID: 865 RVA: 0x0000A5F0 File Offset: 0x000087F0
		public static float ClampAngle(float angle, float restrictionCenter, float restrictionRange)
		{
			restrictionRange /= 2f;
			float smallestDifferenceBetweenTwoAngles = MBMath.GetSmallestDifferenceBetweenTwoAngles(restrictionCenter, angle);
			if (smallestDifferenceBetweenTwoAngles > restrictionRange)
			{
				angle = restrictionCenter + restrictionRange;
			}
			else if (smallestDifferenceBetweenTwoAngles < -restrictionRange)
			{
				angle = restrictionCenter - restrictionRange;
			}
			if (angle > 3.1415927f)
			{
				angle -= 6.2831855f;
			}
			else if (angle < -3.1415927f)
			{
				angle += 6.2831855f;
			}
			return angle;
		}

		// Token: 0x06000362 RID: 866 RVA: 0x0000A648 File Offset: 0x00008848
		public static float WrapAngle(float angle)
		{
			angle = (float)Math.IEEERemainder((double)angle, 6.283185307179586);
			if (angle <= -3.1415927f)
			{
				angle += 6.2831855f;
			}
			else if (angle > 3.1415927f)
			{
				angle -= 6.2831855f;
			}
			return angle;
		}

		// Token: 0x06000363 RID: 867 RVA: 0x0000A682 File Offset: 0x00008882
		public static bool IsBetween(float numberToCheck, float bottom, float top)
		{
			return numberToCheck > bottom && numberToCheck < top;
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0000A68E File Offset: 0x0000888E
		public static bool IsBetween(int value, int minValue, int maxValue)
		{
			return value >= minValue && value < maxValue;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x0000A69A File Offset: 0x0000889A
		public static bool IsBetweenInclusive(float numberToCheck, float bottom, float top)
		{
			return numberToCheck >= bottom && numberToCheck <= top;
		}

		// Token: 0x06000366 RID: 870 RVA: 0x0000A6A9 File Offset: 0x000088A9
		public static uint ColorFromRGBA(float red, float green, float blue, float alpha)
		{
			return ((uint)(alpha * 255f) << 24) + ((uint)(red * 255f) << 16) + ((uint)(green * 255f) << 8) + (uint)(blue * 255f);
		}

		// Token: 0x06000367 RID: 871 RVA: 0x0000A6D8 File Offset: 0x000088D8
		public static Color HSBtoRGB(float hue, float saturation, float brightness, float outputAlpha)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = brightness * saturation;
			float num5 = num4 * (1f - MathF.Abs(hue * 0.016666668f % 2f - 1f));
			float num6 = brightness - num4;
			switch ((int)(hue * 0.016666668f % 6f))
			{
			case 0:
				num = num4;
				num2 = num5;
				num3 = 0f;
				break;
			case 1:
				num = num5;
				num2 = num4;
				num3 = 0f;
				break;
			case 2:
				num = 0f;
				num2 = num4;
				num3 = num5;
				break;
			case 3:
				num = 0f;
				num2 = num5;
				num3 = num4;
				break;
			case 4:
				num = num5;
				num2 = 0f;
				num3 = num4;
				break;
			case 5:
				num = num4;
				num2 = 0f;
				num3 = num5;
				break;
			}
			return new Color(num + num6, num2 + num6, num3 + num6, outputAlpha);
		}

		// Token: 0x06000368 RID: 872 RVA: 0x0000A7B4 File Offset: 0x000089B4
		public static Vec3 RGBtoHSB(Color rgb)
		{
			Vec3 vec = new Vec3(0f, 0f, 0f, -1f);
			float num = MathF.Min(MathF.Min(rgb.Red, rgb.Green), rgb.Blue);
			float num2 = MathF.Max(MathF.Max(rgb.Red, rgb.Green), rgb.Blue);
			float num3 = num2 - num;
			vec.z = num2;
			if (MathF.Abs(num3) < 0.0001f)
			{
				vec.x = 0f;
			}
			else if (MathF.Abs(num2 - rgb.Red) < 0.0001f)
			{
				vec.x = 60f * ((rgb.Green - rgb.Blue) / num3 % 6f);
			}
			else if (MathF.Abs(num2 - rgb.Green) < 0.0001f)
			{
				vec.x = 60f * ((rgb.Blue - rgb.Red) / num3 + 2f);
			}
			else
			{
				vec.x = 60f * ((rgb.Red - rgb.Green) / num3 + 4f);
			}
			vec.x %= 360f;
			if (vec.x < 0f)
			{
				vec.x += 360f;
			}
			if (MathF.Abs(num2) < 0.0001f)
			{
				vec.y = 0f;
			}
			else
			{
				vec.y = num3 / num2;
			}
			return vec;
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0000A928 File Offset: 0x00008B28
		public static Vec3 GammaCorrectRGB(float gamma, Vec3 rgb)
		{
			float num = 1f / gamma;
			rgb.x = MathF.Pow(rgb.x, num);
			rgb.y = MathF.Pow(rgb.y, num);
			rgb.z = MathF.Pow(rgb.z, num);
			return rgb;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0000A978 File Offset: 0x00008B78
		public static Vec3 GetClosestPointInLineSegmentToPoint(Vec3 point, Vec3 lineSegmentBegin, Vec3 lineSegmentEnd)
		{
			Vec3 vec = lineSegmentEnd - lineSegmentBegin;
			if (!vec.IsNonZero)
			{
				return lineSegmentBegin;
			}
			float num = Vec3.DotProduct(point - lineSegmentBegin, vec) / Vec3.DotProduct(vec, vec);
			if (num < 0f)
			{
				return lineSegmentBegin;
			}
			if (num > 1f)
			{
				return lineSegmentEnd;
			}
			return lineSegmentBegin + vec * num;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x0000A9D0 File Offset: 0x00008BD0
		public static bool GetRayPlaneIntersectionPoint(in Vec3 planeNormal, in Vec3 planeCenter, in Vec3 rayOrigin, in Vec3 rayDirection, out float t)
		{
			float num = Vec3.DotProduct(planeNormal, rayDirection);
			if (num > 1E-06f)
			{
				Vec3 vec = planeCenter - rayOrigin;
				t = Vec3.DotProduct(vec, planeNormal) / num;
				return t >= 0f;
			}
			t = -1f;
			return false;
		}

		// Token: 0x0600036C RID: 876 RVA: 0x0000AA34 File Offset: 0x00008C34
		public static Vec2 GetClosestPointInLineSegmentToPoint(Vec2 point, Vec2 lineSegmentBegin, Vec2 lineSegmentEnd)
		{
			Vec2 vec = lineSegmentEnd - lineSegmentBegin;
			if (!vec.IsNonZero())
			{
				return lineSegmentBegin;
			}
			float num = Vec2.DotProduct(point - lineSegmentBegin, vec) / Vec2.DotProduct(vec, vec);
			if (num < 0f)
			{
				return lineSegmentBegin;
			}
			if (num > 1f)
			{
				return lineSegmentEnd;
			}
			return lineSegmentBegin + vec * num;
		}

		// Token: 0x0600036D RID: 877 RVA: 0x0000AA8C File Offset: 0x00008C8C
		public static bool CheckLineToLineSegmentIntersection(Vec2 lineOrigin, Vec2 lineDirection, Vec2 segmentA, Vec2 segmentB, out float t, out Vec2 intersect)
		{
			t = float.MaxValue;
			intersect = Vec2.Zero;
			Vec2 vec = lineOrigin - segmentA;
			Vec2 vec2 = segmentB - segmentA;
			Vec2 vec3 = new Vec2(-lineDirection.y, lineDirection.x);
			float num = vec2.DotProduct(vec3);
			if (MathF.Abs(num) < 1E-05f)
			{
				return false;
			}
			float num2 = vec2.x * vec.y - vec2.y * vec.x;
			t = num2 / num;
			intersect = lineOrigin + lineDirection * t;
			float num3 = vec.DotProduct(vec3) / num;
			return num3 >= 0f && num3 <= 1f;
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0000AB48 File Offset: 0x00008D48
		public static float GetClosestPointOnLineSegment(Vec2 point, Vec2 segmentA, Vec2 segmentB, out Vec2 closest)
		{
			Vec2 vec = point - segmentA;
			Vec2 vec2 = segmentB - segmentA;
			float num = vec.DotProduct(vec2) / Math.Max(vec2.LengthSquared, 1E-05f);
			if (num < 0f)
			{
				closest = segmentA;
			}
			else if (num > 1f)
			{
				closest = segmentB;
			}
			else
			{
				closest = segmentA + vec2 * num;
			}
			return point.Distance(closest);
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0000ABC4 File Offset: 0x00008DC4
		public static bool IntersectRayWithBoundaryList(Vec2 rayOrigin, Vec2 rayDir, List<Vec2> boundaries, out Vec2 intersectionPoint)
		{
			List<ValueTuple<float, Vec2>> list = new List<ValueTuple<float, Vec2>>();
			for (int j = 0; j < boundaries.Count; j++)
			{
				Vec2 vec = boundaries[j];
				Vec2 vec2 = boundaries[(j + 1) % boundaries.Count];
				float num;
				Vec2 vec3;
				if (MBMath.CheckLineToLineSegmentIntersection(rayOrigin, rayDir, vec, vec2, out num, out vec3) && num > 0f)
				{
					list.Add(new ValueTuple<float, Vec2>(num, vec3));
				}
			}
			list = list.OrderBy((ValueTuple<float, Vec2> i) => i.Item1).ToList<ValueTuple<float, Vec2>>();
			if (list.Count != 0)
			{
				intersectionPoint = list[0].Item2;
				return true;
			}
			intersectionPoint = rayOrigin;
			return false;
		}

		// Token: 0x06000370 RID: 880 RVA: 0x0000AC78 File Offset: 0x00008E78
		public static string ToOrdinal(int number)
		{
			if (number < 0)
			{
				return number.ToString();
			}
			long num = (long)(number % 100);
			if (num >= 11L && num <= 13L)
			{
				return number + "th";
			}
			switch (number % 10)
			{
			case 1:
				return number + "st";
			case 2:
				return number + "nd";
			case 3:
				return number + "rd";
			default:
				return number + "th";
			}
		}

		// Token: 0x06000371 RID: 881 RVA: 0x0000AD14 File Offset: 0x00008F14
		public static int IndexOfMax<T>(MBReadOnlyList<T> array, Func<T, int> func)
		{
			int num = int.MinValue;
			int num2 = -1;
			for (int i = 0; i < array.Count; i++)
			{
				int num3 = func(array[i]);
				if (num3 > num)
				{
					num = num3;
					num2 = i;
				}
			}
			return num2;
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0000AD54 File Offset: 0x00008F54
		public static T MaxElement<T>(IEnumerable<T> collection, Func<T, float> func)
		{
			float num = float.MinValue;
			T t = default(T);
			foreach (T t2 in collection)
			{
				float num2 = func(t2);
				if (num2 > num)
				{
					num = num2;
					t = t2;
				}
			}
			return t;
		}

		// Token: 0x06000373 RID: 883 RVA: 0x0000ADB8 File Offset: 0x00008FB8
		public static ValueTuple<T, T> MaxElements2<T>(IEnumerable<T> collection, Func<T, float> func)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			T t = default(T);
			T t2 = default(T);
			foreach (T t3 in collection)
			{
				float num3 = func(t3);
				if (num3 > num2)
				{
					if (num3 > num)
					{
						num2 = num;
						t2 = t;
						num = num3;
						t = t3;
					}
					else
					{
						num2 = num3;
						t2 = t3;
					}
				}
			}
			return new ValueTuple<T, T>(t, t2);
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0000AE48 File Offset: 0x00009048
		public static ValueTuple<T, T, T> MaxElements3<T>(IEnumerable<T> collection, Func<T, float> func)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			float num3 = float.MinValue;
			T t = default(T);
			T t2 = default(T);
			T t3 = default(T);
			foreach (T t4 in collection)
			{
				float num4 = func(t4);
				if (num4 > num3)
				{
					if (num4 > num2)
					{
						num3 = num2;
						t3 = t2;
						if (num4 > num)
						{
							num2 = num;
							t2 = t;
							num = num4;
							t = t4;
						}
						else
						{
							num2 = num4;
							t2 = t4;
						}
					}
					else
					{
						num3 = num4;
						t3 = t4;
					}
				}
			}
			return new ValueTuple<T, T, T>(t, t2, t3);
		}

		// Token: 0x06000375 RID: 885 RVA: 0x0000AF00 File Offset: 0x00009100
		public static ValueTuple<T, T, T, T> MaxElements4<T>(IEnumerable<T> collection, Func<T, float> func)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			T t = default(T);
			T t2 = default(T);
			T t3 = default(T);
			T t4 = default(T);
			foreach (T t5 in collection)
			{
				float num5 = func(t5);
				if (num5 > num4)
				{
					if (num5 > num3)
					{
						num4 = num3;
						t4 = t3;
						if (num5 > num2)
						{
							num3 = num2;
							t3 = t2;
							if (num5 > num)
							{
								num2 = num;
								t2 = t;
								num = num5;
								t = t5;
							}
							else
							{
								num2 = num5;
								t2 = t5;
							}
						}
						else
						{
							num3 = num5;
							t3 = t5;
						}
					}
					else
					{
						num4 = num5;
						t4 = t5;
					}
				}
			}
			return new ValueTuple<T, T, T, T>(t, t2, t3, t4);
		}

		// Token: 0x06000376 RID: 886 RVA: 0x0000AFE0 File Offset: 0x000091E0
		public static ValueTuple<T, T, T, T, T> MaxElements5<T>(IEnumerable<T> collection, Func<T, float> func)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			float num5 = float.MinValue;
			T t = default(T);
			T t2 = default(T);
			T t3 = default(T);
			T t4 = default(T);
			T t5 = default(T);
			foreach (T t6 in collection)
			{
				float num6 = func(t6);
				if (num6 > num5)
				{
					if (num6 > num4)
					{
						num5 = num4;
						t5 = t4;
						if (num6 > num3)
						{
							num4 = num3;
							t4 = t3;
							if (num6 > num2)
							{
								num3 = num2;
								t3 = t2;
								if (num6 > num)
								{
									num2 = num;
									t2 = t;
									num = num6;
									t = t6;
								}
								else
								{
									num2 = num6;
									t2 = t6;
								}
							}
							else
							{
								num3 = num6;
								t3 = t6;
							}
						}
						else
						{
							num4 = num6;
							t4 = t6;
						}
					}
					else
					{
						num5 = num6;
						t5 = t6;
					}
				}
			}
			return new ValueTuple<T, T, T, T, T>(t, t2, t3, t4, t5);
		}

		// Token: 0x06000377 RID: 887 RVA: 0x0000B0E8 File Offset: 0x000092E8
		public static IList<T> TopologySort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
		{
			List<T> list = new List<T>();
			Dictionary<T, bool> dictionary = new Dictionary<T, bool>();
			foreach (T t in source)
			{
				MBMath.Visit<T>(t, getDependencies, list, dictionary);
			}
			return list;
		}

		// Token: 0x06000378 RID: 888 RVA: 0x0000B140 File Offset: 0x00009340
		private static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
		{
			bool flag;
			if (visited.TryGetValue(item, out flag))
			{
				return;
			}
			visited[item] = true;
			IEnumerable<T> enumerable = getDependencies(item);
			if (enumerable != null)
			{
				foreach (T t in enumerable)
				{
					MBMath.Visit<T>(t, getDependencies, sorted, visited);
				}
			}
			visited[item] = false;
			sorted.Add(item);
		}

		// Token: 0x0400010A RID: 266
		public const float TwoPI = 6.2831855f;

		// Token: 0x0400010B RID: 267
		public const float PI = 3.1415927f;

		// Token: 0x0400010C RID: 268
		public const float HalfPI = 1.5707964f;

		// Token: 0x0400010D RID: 269
		public const float E = 2.7182817f;

		// Token: 0x0400010E RID: 270
		public const float DegreesToRadians = 0.017453292f;

		// Token: 0x0400010F RID: 271
		public const float RadiansToDegrees = 57.295776f;

		// Token: 0x04000110 RID: 272
		public const float Epsilon = 1E-05f;
	}
}
