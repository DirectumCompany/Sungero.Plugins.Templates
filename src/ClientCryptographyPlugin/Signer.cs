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
    /// <param name="pluginName">Имя плагина.</param>
    /// <param name="certificateThumbprint">Отпечаток сертификата.</param>
    /// <param name="signingData">Подписываемые данные закодированные в формате base64.</param>
    /// <param name="userLanguage">Язык интерфейса пользователя.</param>
    /// <param name="result">Подписанные данные закодированные в формате base64, либо текст сообщения об ошибке.</param>
    public static int SignData(string pluginName, string certificateThumbprint, string signingData, string userLanguage, out string result)
    {
      // Пример использования класса логирования.
      Logger.Info(string.Format("Start signing. PluginName: {0} CertificateThumbprint: {1}.", pluginName, certificateThumbprint));
      var data = Convert.FromBase64String(signingData);

      // Можно получить сертификат из реестра.
      var certificate = GetCertificateFromStore(certificateThumbprint);

      // Пример использования диалога пин-кода (может потребоваться для доступа к токену).
      // string pinCode = PinCodeDialog.Get();

      // Пример использования диалога выбора сертификата для получения сертификата из файла.
      // string[] key = KeyPathDialog.Get(null);
      // var privateKeyFilePath = key[0];
      // var privateKeyFilePassword = key[1];
      // var certificate = new X509Certificate2(privateKeyFilePath, privateKeyFilePassword);

      // Формирование ответа для клиента.
      result = null;
      var resultCode = (int)SignDataResult.Success;

      // Пример подписания с использованием SHA512 и RSA.
      var cryptoServiceProvider = (RSACryptoServiceProvider)certificate.PrivateKey;
      using (var hasher = SHA512.Create())
      {
        try
        {
          const string SHA512AlgorithmIdentifier = "2.16.840.1.101.3.4.2.3";
          var signedHash = cryptoServiceProvider.SignHash(hasher.ComputeHash(data), SHA512AlgorithmIdentifier);

          result = Convert.ToBase64String(signedHash);
        }
        catch (Exception ex)
        {
          // Пример использования локализации веб-агента. Полный список стандартных ошибок при подписании находится в файле LocalizerStandardErrors.md.
          result = Localizer.L("CRYPTOGRAPHY.ERR_SIGN");
          resultCode = (int)SignDataResult.UnknownError;
          Logger.Error(string.Format("Signing failed. Reason: {0}.", ex.Message));
        }
      }

      Logger.Info("Signing finished.");
      return resultCode;
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
