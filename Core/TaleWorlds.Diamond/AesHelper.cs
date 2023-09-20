using System;
using System.IO;
using System.Security.Cryptography;

namespace TaleWorlds.Diamond
{
	public static class AesHelper
	{
		public static byte[] Encrypt(byte[] plainData, byte[] key, byte[] initializationVector)
		{
			byte[] array;
			using (Aes aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = initializationVector;
				array = AesHelper.EncryptStringToBytes(plainData, aes.Key, aes.IV);
			}
			return array;
		}

		public static byte[] Decrypt(byte[] encrypted, byte[] key, byte[] initializationVector)
		{
			byte[] array;
			using (Aes aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = initializationVector;
				array = AesHelper.DecryptStringFromBytes(encrypted, aes.Key, aes.IV);
			}
			return array;
		}

		private static byte[] EncryptStringToBytes(byte[] plainData, byte[] Key, byte[] IV)
		{
			if (plainData == null || plainData.Length == 0)
			{
				throw new ArgumentNullException("plainText");
			}
			if (Key == null || Key.Length == 0)
			{
				throw new ArgumentNullException("Key");
			}
			if (IV == null || IV.Length == 0)
			{
				throw new ArgumentNullException("IV");
			}
			byte[] array;
			using (Aes aes = Aes.Create())
			{
				aes.Key = Key;
				aes.IV = IV;
				ICryptoTransform cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(cryptoStream))
						{
							binaryWriter.Write(plainData);
						}
						array = memoryStream.ToArray();
					}
				}
			}
			return array;
		}

		private static byte[] DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
		{
			if (cipherText == null || cipherText.Length == 0)
			{
				throw new ArgumentNullException("cipherText");
			}
			if (Key == null || Key.Length == 0)
			{
				throw new ArgumentNullException("Key");
			}
			if (IV == null || IV.Length == 0)
			{
				throw new ArgumentNullException("IV");
			}
			byte[] array = null;
			using (Aes aes = Aes.Create())
			{
				aes.Key = Key;
				aes.IV = IV;
				ICryptoTransform cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);
				using (MemoryStream memoryStream = new MemoryStream(cipherText))
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
					{
						using (BinaryReader binaryReader = new BinaryReader(cryptoStream))
						{
							array = AesHelper.ReadAllBytes(binaryReader);
						}
					}
				}
			}
			return array;
		}

		private static byte[] ReadAllBytes(BinaryReader reader)
		{
			byte[] array2;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] array = new byte[4096];
				int num;
				while ((num = reader.Read(array, 0, array.Length)) != 0)
				{
					memoryStream.Write(array, 0, num);
				}
				array2 = memoryStream.ToArray();
			}
			return array2;
		}
	}
}
