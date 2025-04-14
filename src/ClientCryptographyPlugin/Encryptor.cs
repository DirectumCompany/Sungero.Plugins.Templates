using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Library
{
    /// <summary>
    /// Класс клиентского шифрования.
    /// </summary>
    public static class Encryptor
    {
        /// <summary>
        /// Количество итераций при получении ключа симметричного шифрования.
        /// </summary>
        private const int KeyDeriveIterations = 64;

        /// <summary>
        /// Количество бит в байте.
        /// </summary>
        private const int BitsPerByte = 8;

        /// <summary>
        /// Вектор инициализации.
        /// </summary>
        private static byte[] IV = Convert.FromBase64String("Pg8FDx9EUNhFPPbq1FGbcA==");

        /// <summary>
        /// Материал для ключа.
        /// </summary>
        private static byte[] keyMaterial = Encoding.ASCII.GetBytes("SuperSecret");

        /// <summary>
        /// Материал для нового ключа.
        /// </summary>
        private static byte[] newKeyMaterial = Encoding.ASCII.GetBytes("TopSecret");

        /// <summary>
        /// Метод шифрования данных.
        /// </summary>
        /// <param name="decryptedStream">Поток незашифрованных данных (исходные данные).</param>
        /// <param name="encryptedStream">Поток шифрованных данных (результат).</param>
        /// <param name="encryptOptions">Параметры шифрования переданные с сервера.</param>
        /// <param name="errorMessage">Сообщение об ошибке (если она есть).</param>
        /// <returns>Результат выполнения операции (см. <see cref="SignDataResult"/>).</returns>
        public static int Encrypt(Stream decryptedStream, Stream encryptedStream, string encryptOptions, out string errorMessage)
        {
            Logger.Debug($"Encrypt encryptOptions: {encryptOptions}");
            errorMessage = string.Empty;

            using (var keyGenerator = new Rfc2898DeriveBytes(keyMaterial, IV, KeyDeriveIterations))
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = IV;
                aes.Key = keyGenerator.GetBytes(aes.KeySize / BitsPerByte);
                using (var cryptoStream = new CryptoStream(encryptedStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    decryptedStream.CopyTo(cryptoStream);
                    cryptoStream.FlushFinalBlock();
                }
            }

            return (int)SignDataResult.Success;
        }

        /// <summary>
        /// Метод расшифровки данных.
        /// </summary>
        /// <param name="encryptedStream">Поток шифрованных данных (исходные данные).</param>
        /// <param name="decryptedStream">Поток незашифрованных данных (результат).</param> 
        /// <param name="decryptOptions">Параметры шифрования переданные с сервера.</param>
        /// <param name="errorMessage">Сообщение об ошибке (если она есть).</param>
        /// <returns>Результат выполнения операции (см. <see cref="SignDataResult"/>).</returns>
        public static int Decrypt(Stream encryptedStream, Stream decryptedStream, string decryptOptions, out string errorMessage)
        {
            Logger.Debug($"Decrypt decryptOptions: {decryptOptions}");
            errorMessage = string.Empty;

            using (var keyGenerator = new Rfc2898DeriveBytes(keyMaterial, IV, KeyDeriveIterations))
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = IV;
                aes.Key = keyGenerator.GetBytes(aes.KeySize / BitsPerByte);
                using (var cryptoStream = new CryptoStream(encryptedStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(decryptedStream);
                    decryptedStream.Flush();
                }
            }

            return (int)SignDataResult.Success;
        }

        /// <summary>
        /// Метод перешифрования данных.
        /// </summary>
        /// <param name="inputStream">Поток шифрованных данных (исходные данные).</param>
        /// <param name="outputStream">Поток перешифрованных данных (результат).</param> 
        /// <param name="encryptOptions">Параметры шифрования переданные с сервера.</param>
        /// <param name="errorMessage">Сообщение об ошибке (если она есть).</param>
        /// <returns>Результат выполнения операции (см. <see cref="SignDataResult"/>).</returns>
        public static int ReEncrypt(Stream inputStream, Stream outputStream, string encryptOptions, out string errorMessage)
        {
            Logger.Debug($"ReEncrypt decryptOptions: {encryptOptions}");
            errorMessage = string.Empty;

            using (var ms = new MemoryStream())
            {
                using (var keyGenerator = new Rfc2898DeriveBytes(keyMaterial, IV, KeyDeriveIterations))
                using (var aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.IV = IV;
                    aes.Key = keyGenerator.GetBytes(aes.KeySize / BitsPerByte);
                    using (var cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(ms);
                        ms.Flush();
                    }
                }

                ms.Seek(0, SeekOrigin.Begin);

                using (var keyGenerator = new Rfc2898DeriveBytes(newKeyMaterial, IV, KeyDeriveIterations))
                using (var aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.IV = IV;
                    aes.Key = keyGenerator.GetBytes(aes.KeySize / BitsPerByte);
                    using (var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        ms.CopyTo(cryptoStream);
                        cryptoStream.FlushFinalBlock();
                    }
                }
            }
 
            return (int)SignDataResult.Success;
        }
    }
}
