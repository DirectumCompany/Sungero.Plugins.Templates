using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Library
{
  /// <summary>
  /// Класс подписания по выбранному алгоритму.
  /// </summary>
  public static class Signer
  {
    /// <summary>
    /// Подписать данные.
    /// </summary>
    /// <param name="requestString">Запрос.</param>
    public static string SignData(string requestString)
    {
      // Получить информацию о подписании из запроса.
      var requestObj = JsonConvert.DeserializeObject<Request>(requestString);
      // Пример использования класса логирования.
      Logger.Info(string.Format("Start signing. PluginName: {0} CertificateID: {1}.", requestObj.PluginName, requestObj.CertificateID));
      var data = Convert.FromBase64String(requestObj.Attributes);
      var thumbprint = requestObj.CertificateID;

      // Можно получить сертификат из реестра.
      var certificate = GetCertificateFromStore(thumbprint);

      // Пример использования диалога пин-кода (может потребоваться для доступа к токену).
      // string pinCode = PinCodeDialog.Get();

      // Пример использования диалога выбора сертификата для получения сертификата из файла.
      // string[] key = KeyPathDialog.Get(null);
      // var privateKeyFilePath = key[0];
      // var privateKeyFilePassword = key[1];
      // var certificate = new X509Certificate2(privateKeyFilePath, privateKeyFilePassword);

      // Формирование ответа для клиента.
      var response = new Response();

      // Пример подписания с использованием SHA512 и RSA.
      var cryptoServiceProvider = (RSACryptoServiceProvider)certificate.PrivateKey;
      using (var hasher = SHA512.Create())
      {
        try
        {
          const string SHA512AlgorithmIdentifier = "2.16.840.1.101.3.4.2.3";
          var signedHash = cryptoServiceProvider.SignHash(hasher.ComputeHash(data), SHA512AlgorithmIdentifier);
          response.Result = CertificateLoadPrivateKeyResult.Success;
          response.SetSignature(signedHash);
        }
        catch (Exception ex)
        {
          // Пример использования локализации веб-агента. Полный список стандартных ошибок при подписании находится в файле LocalizerStandardErrors.md.
          // string error = Localizer.L("CRYPTOGRAPHY.ERR_SIGN");

          Logger.Error(string.Format("Signing failed. Reason: {0}.", ex.Message));
          response.Result = CertificateLoadPrivateKeyResult.UnknownError;
        }
      }

      Logger.Info("Signing successfully finished.");
      return JsonConvert.SerializeObject(response, Formatting.None);
    }

    /// <summary>
    /// Получить сертификат с закрытым ключом из хранилища текущего пользователя.
    /// </summary>
    /// <param name="thumbprint">Отпечаток сертификата.</param>
    /// <returns>Сертификат.</returns>
    private static X509Certificate2 GetCertificateFromStore(string thumbprint)
    {
      var store = new X509Store();
      store.Open(OpenFlags.ReadOnly);
      var privateKeyCertificate = store.Certificates
        .OfType<X509Certificate2>()
        .FirstOrDefault(c => (c.Thumbprint?.Equals(thumbprint, StringComparison.OrdinalIgnoreCase) ?? false) && c.HasPrivateKey);
      store.Close();
      return privateKeyCertificate;
    }
  }
}
