using System;
using System.IO;

namespace Library
{
    /// <summary>
    /// Обертка над классом клиентского шифрования.
    /// </summary>
    /// <remarks>
    /// Этот класс - часть API, нельзя его модифицировать.
    /// </remarks>
    public static class EncryptorWrapper
    {
        public static int Encrypt(string decryptedFilePath, string encryptedFilePath, string encryptionOptions, out string errorMessage)
        {
            try
            {
                using (var inStream = new FileStream(decryptedFilePath, FileMode.Open))
                using (var outStream = new FileStream(encryptedFilePath, FileMode.Create))
                {
                    return Encryptor.Encrypt(inStream, outStream, encryptionOptions, out errorMessage);
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return (int)SignDataResult.UnknownError;
            }
        }

        public static int Decrypt(string encryptedFilePath, string decryptedFilePath, string decryptionOptions, out string errorMessage)
        {
            try
            {
                using (var inStream = new FileStream(encryptedFilePath, FileMode.Open))
                using (var outStream = new FileStream(decryptedFilePath, FileMode.Create))
                {
                    return Encryptor.Decrypt(inStream, outStream, decryptionOptions, out errorMessage);
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return (int)SignDataResult.UnknownError;
            }
        }

        public static int ReEncrypt(string inputFilePath, string outputFilePath, string encryptionOptions, out string errorMessage)
        {
            try
            {
                using (var inStream = new FileStream(inputFilePath, FileMode.Open))
                using (var outStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    return Encryptor.ReEncrypt(inStream, outStream, encryptionOptions, out errorMessage);
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return (int)SignDataResult.UnknownError;
            }
        }
    }
}
