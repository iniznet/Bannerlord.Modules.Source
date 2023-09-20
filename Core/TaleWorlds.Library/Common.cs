using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaleWorlds.Library
{
	public static class Common
	{
		public static IPlatformFileHelper PlatformFileHelper
		{
			get
			{
				return Common._fileHelper;
			}
			set
			{
				Common._fileHelper = value;
			}
		}

		public static byte[] CombineBytes(byte[] arr1, byte[] arr2, byte[] arr3 = null, byte[] arr4 = null, byte[] arr5 = null)
		{
			byte[] array = new byte[arr1.Length + arr2.Length + ((arr3 != null) ? arr3.Length : 0) + ((arr4 != null) ? arr4.Length : 0) + ((arr5 != null) ? arr5.Length : 0)];
			int num = 0;
			if (arr1.Length != 0)
			{
				Buffer.BlockCopy(arr1, 0, array, num, arr1.Length);
				num += arr1.Length;
			}
			if (arr2.Length != 0)
			{
				Buffer.BlockCopy(arr2, 0, array, num, arr2.Length);
				num += arr2.Length;
			}
			if (arr3 != null && arr3.Length != 0)
			{
				Buffer.BlockCopy(arr3, 0, array, num, arr3.Length);
				num += arr3.Length;
			}
			if (arr4 != null && arr4.Length != 0)
			{
				Buffer.BlockCopy(arr4, 0, array, num, arr4.Length);
				num += arr4.Length;
			}
			if (arr5 != null && arr5.Length != 0)
			{
				Buffer.BlockCopy(arr5, 0, array, num, arr5.Length);
			}
			return array;
		}

		public static string CalculateMD5Hash(string input)
		{
			MD5 md = MD5.Create();
			byte[] bytes = Encoding.ASCII.GetBytes(input);
			byte[] array = md.ComputeHash(bytes);
			md.Dispose();
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(32, "CalculateMD5Hash");
			for (int i = 0; i < array.Length; i++)
			{
				mbstringBuilder.Append<string>(array[i].ToString("X2"));
			}
			return mbstringBuilder.ToStringAndRelease();
		}

		public static string ToRoman(int number)
		{
			if (number < 0 || number > 3999)
			{
				Debug.FailedAssert("Requested roman number has to be between 1 and 3999. Fix number!", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\Common.cs", "ToRoman", 88);
			}
			if (number < 1)
			{
				return string.Empty;
			}
			if (number >= 1000)
			{
				return "M" + Common.ToRoman(number - 1000);
			}
			if (number >= 900)
			{
				return "CM" + Common.ToRoman(number - 900);
			}
			if (number >= 500)
			{
				return "D" + Common.ToRoman(number - 500);
			}
			if (number >= 400)
			{
				return "CD" + Common.ToRoman(number - 400);
			}
			if (number >= 100)
			{
				return "C" + Common.ToRoman(number - 100);
			}
			if (number >= 90)
			{
				return "XC" + Common.ToRoman(number - 90);
			}
			if (number >= 50)
			{
				return "L" + Common.ToRoman(number - 50);
			}
			if (number >= 40)
			{
				return "XL" + Common.ToRoman(number - 40);
			}
			if (number >= 10)
			{
				return "X" + Common.ToRoman(number - 10);
			}
			if (number >= 9)
			{
				return "IX" + Common.ToRoman(number - 9);
			}
			if (number >= 5)
			{
				return "V" + Common.ToRoman(number - 5);
			}
			if (number >= 4)
			{
				return "IV" + Common.ToRoman(number - 4);
			}
			if (number >= 1)
			{
				return "I" + Common.ToRoman(number - 1);
			}
			Debug.FailedAssert("ToRoman error", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\Common.cs", "ToRoman", 104);
			return "";
		}

		public static int GetDJB2(string str)
		{
			int num = 5381;
			for (int i = 0; i < str.Length; i++)
			{
				num = (num << 5) + num + (int)str[i];
			}
			return num;
		}

		public static byte[] SerializeObjectAsJson(object o)
		{
			string text = JsonConvert.SerializeObject(o, 1);
			return Encoding.UTF8.GetBytes(text);
		}

		public static byte[] SerializeObject(object sObject)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			try
			{
				binaryFormatter.Serialize(memoryStream, sObject);
			}
			catch (Exception ex)
			{
				Debug.Print(ex.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
			}
			return memoryStream.ToArray();
		}

		public static object DeserializeObject(byte[] serializeData)
		{
			return Common.DeserializeObject(serializeData, 0, serializeData.Length);
		}

		public static object DeserializeObject(byte[] serializeData, int index, int length)
		{
			MemoryStream memoryStream = new MemoryStream(serializeData, index, length, false);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			object obj;
			try
			{
				obj = binaryFormatter.Deserialize(memoryStream);
			}
			catch (Exception)
			{
				throw;
			}
			return obj;
		}

		public static string ConfigName
		{
			get
			{
				return new DirectoryInfo(Directory.GetCurrentDirectory()).Name;
			}
		}

		public static Type FindType(string typeName)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type type = assemblies[i].GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		public static void MemoryCleanupGC(bool forceTimer = false)
		{
			GC.Collect();
			Common.lastGCTime = DateTime.Now;
		}

		public static object DynamicInvokeWithLog(this Delegate method, params object[] args)
		{
			object obj = null;
			try
			{
				obj = method.DynamicInvoke(args);
			}
			catch (Exception ex)
			{
				Common.PrintDynamicInvokeDebugInfo(ex, method.Method, method.Target, args);
				obj = null;
				throw;
			}
			return obj;
		}

		public static object InvokeWithLog(this MethodInfo methodInfo, object obj, params object[] args)
		{
			object obj2 = null;
			try
			{
				obj2 = methodInfo.Invoke(obj, args);
			}
			catch (Exception ex)
			{
				Common.PrintDynamicInvokeDebugInfo(ex, methodInfo, obj, args);
				obj2 = null;
				throw;
			}
			return obj2;
		}

		public static object InvokeWithLog(this ConstructorInfo constructorInfo, params object[] args)
		{
			object obj = null;
			try
			{
				obj = constructorInfo.Invoke(args);
			}
			catch (Exception ex)
			{
				MethodInfo methodInfo = Common.GetMethodInfo<object[]>((object[] a) => constructorInfo.Invoke(a));
				Common.PrintDynamicInvokeDebugInfo(ex, methodInfo, null, args);
				obj = null;
				throw;
			}
			return obj;
		}

		private static string GetStackTraceRaw(Exception e, int skipCount = 0)
		{
			StackTrace stackTrace = new StackTrace(e, 0, false);
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "GetStackTraceRaw");
			for (int i = 0; i < stackTrace.FrameCount; i++)
			{
				if (i >= skipCount)
				{
					string text = "unknown_module.dll";
					try
					{
						StackFrame frame = stackTrace.GetFrame(i);
						MethodBase method = frame.GetMethod();
						text = method.Module.Assembly.Location;
						int iloffset = frame.GetILOffset();
						int metadataToken = method.MetadataToken;
						mbstringBuilder.AppendLine<string>(string.Concat(new object[] { text, "@", metadataToken, "@", iloffset }));
					}
					catch
					{
						mbstringBuilder.AppendLine<string>(text + "@-1@-1");
					}
				}
			}
			return mbstringBuilder.ToStringAndRelease();
		}

		private static void WalkInnerExceptionRecursive(Exception InnerException, ref string StackStr)
		{
			if (InnerException == null)
			{
				return;
			}
			Common.WalkInnerExceptionRecursive(InnerException.InnerException, ref StackStr);
			string stackTraceRaw = Common.GetStackTraceRaw(InnerException, 0);
			StackStr += stackTraceRaw;
			StackStr += "---End of stack trace from previous location where exception was thrown ---";
			StackStr += Environment.NewLine;
		}

		private static void PrintDynamicInvokeDebugInfo(Exception e, MethodInfo methodInfo, object obj, params object[] args)
		{
			string text = "Exception occurred inside invoke: " + methodInfo.Name;
			if (obj != null)
			{
				text = text + "\nTarget type: " + obj.GetType().FullName;
			}
			if (args != null)
			{
				text = text + "\nArgument count: " + args.Length;
				foreach (object obj2 in args)
				{
					if (obj2 == null)
					{
						text += "\nArgument is null";
					}
					else
					{
						text = text + "\nArgument type is " + obj2.GetType().FullName;
					}
				}
			}
			string text2 = "";
			if (e.InnerException != null)
			{
				Common.WalkInnerExceptionRecursive(e, ref text2);
			}
			Exception ex = e;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			text = text + "\nInner message: " + ex.Message;
			Debug.SetCrashReportCustomString(text);
			Debug.SetCrashReportCustomStack(text2);
			Debug.Print(text, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		public static bool TextContainsSpecialCharacters(string text)
		{
			return text.Any((char x) => !char.IsWhiteSpace(x) && !char.IsLetterOrDigit(x) && !char.IsPunctuation(x));
		}

		public static uint ParseIpAddress(string address)
		{
			byte[] addressBytes = IPAddress.Parse(address).GetAddressBytes();
			return (uint)(((int)addressBytes[0] << 24) + ((int)addressBytes[1] << 16) + ((int)addressBytes[2] << 8) + (int)addressBytes[3]);
		}

		public static bool IsAllLetters(string text)
		{
			if (text == null)
			{
				return false;
			}
			for (int i = 0; i < text.Length; i++)
			{
				if (!char.IsLetter(text[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsAllLettersOrWhiteSpaces(string text)
		{
			if (text == null)
			{
				return false;
			}
			foreach (char c in text)
			{
				if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsCharAsian(char character)
		{
			return (character >= '一' && character <= '\u9fff') || (character >= '㐀' && character <= '䶿') || (character >= '㐀' && character <= '䶿') || ((int)character >= 131072 && (int)character <= 183983) || (character >= '⺀' && character <= '\u31ef') || (character >= '豈' && character <= '\ufaff') || (character >= '︰' && character <= '\ufe4f') || ((int)character >= 993280 && (int)character <= 195103);
		}

		public static void SetInvariantCulture()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
		}

		public static MethodInfo GetMethodInfo(Expression<Action> expression)
		{
			return Common.GetMethodInfo(expression);
		}

		public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
		{
			return Common.GetMethodInfo(expression);
		}

		public static MethodInfo GetMethodInfo<T, TResult>(Expression<Func<T, TResult>> expression)
		{
			return Common.GetMethodInfo(expression);
		}

		public static MethodInfo GetMethodInfo(LambdaExpression expression)
		{
			MethodCallExpression methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression == null)
			{
				throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
			}
			return methodCallExpression.Method;
		}

		public static ParallelOptions ParallelOptions
		{
			get
			{
				if (Common._parallelOptions == null)
				{
					Common._parallelOptions = new ParallelOptions();
					Common._parallelOptions.MaxDegreeOfParallelism = MathF.Max(Environment.ProcessorCount - 2, 1);
				}
				return Common._parallelOptions;
			}
		}

		private static IPlatformFileHelper _fileHelper = null;

		private static DateTime lastGCTime = DateTime.Now;

		private static ParallelOptions _parallelOptions = null;
	}
}
