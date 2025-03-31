using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace Library
{
  /// <summary>
  /// Класс подписания по выбранному алгоритму.
  /// </summary>
  public static class Signer
  {
    
    /// <summary>
    /// Параметры сертификата в формате json.
    /// </summary>
    public static string certificateParameters = string.Empty;
    
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
      // Параметры сертификата в формате json.
      Logger.Info($"{nameof(certificateParameters)}: {certificateParameters}");
      // Пример преобразования параметров из json в dictionary.
      Dictionary<string, string> сertificateParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(certificateParameters);
      Logger.Info($"Listings certificate parameters:");
      foreach (var item in сertificateParameters)
        Logger.Info($"{item.Key}: {item.Value}");
      
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
    /// Подписать данные.
    /// </summary>
    /// <param name="pluginName">Имя плагина.</param>
    /// <param name="certificatesThumbprints">Коллекция отпечатков сертификатов.</param>
    /// <param name="signingData">Подписываемые данные в формате base64.</param>
    /// <param name="certificatesProperties">Коллекция свойств сертификатов.</param>
    /// <param name="userLanguage">Язык интерфейса пользователя.</param>
    /// <param name="result">Результат подписания в формате json, либо текст сообщения об ошибке.</param>
    /// <returns>Код возврата <see cref="Library.SignDataResult"/>.</returns>
    public static int SignData(
      string pluginName,
      string[] certificatesThumbprints,
      string signingData,
      string[] certificatesProperties,
      string userLanguage,
      out string result)
    {
      Logger.Info($"Start signing. PluginName: {pluginName}, " +
                  $"CertificatesThumbprints: {string.Join(",", certificatesThumbprints)}, " +
                  $"CertificatesProperties: {string.Join(",", certificatesProperties)}.");

      var data = Convert.FromBase64String(signingData);
      var certificate = SelectCertificate(certificatesThumbprints);
      if (certificate == null)
      {
        result = string.Format(Localizer.L("CRYPTOGRAPHY.ERR_CERTIFICATE_NOT_FOUND"),
          string.Join(", ", certificatesThumbprints));
        Logger.Error(result);

        return (int)SignDataResult.IllegalCertificateID;
      }

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
    
    /// <summary>
    /// Выбрать подходящий сертификат.
    /// </summary>
    /// <remarks>
    /// В данном случае выбирается первый подходящий.
    /// </remarks>
    /// <param name="certificatesThumbprints">Коллекция отпечатков сертификатов.</param>
    /// <returns>Сертификат.</returns>
    private static X509Certificate2 SelectCertificate(string[] certificatesThumbprints)
    {
      foreach (var certificateThumbprint in certificatesThumbprints)
      {
        var certificate = GetCertificateFromStore(certificateThumbprint);

        if (certificate != null)
        {
          return certificate;
        }
      }

      return null;
    }
    
    /// <summary>
    /// Установить параметры сертификата.
    /// </summary>
    /// <param name="parameters">Параметры сертификата в json.</param>
    public static void SetCertificateParameters(string parameters)
    {
      certificateParameters = parameters;
    }
  }
}
