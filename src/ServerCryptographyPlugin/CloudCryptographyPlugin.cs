using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Sungero.Cryptography.Shared;
using Sungero.Plugins.Sdk;

namespace CryptographyPlugin
{
  /*
   * Облачное подписание выполняется в облачном криптопровайдере, например, КриптоПро DSS.
   * Закрытый ключ пользователя также хранится в облаке, поэтому нет необходимости взаимодействовать с устройством пользователя через веб-агент.
   * Платформа Sungero ожидает, что плагин облачного подписания вернет готовую CMS-совместимую подпись, которая затем будет сохранена в базу данных.
   * В настроящее время поддерживается только облачное подписание (интерфейс ICloudSignPlugin), но не облачная проверка подписи и сертификатов.
   * Поэтому подписи и сертификатов всегда проверяются локально на сервере приложений.
   * Проверка подписей и сертификатов может быть реализована в этом же плагине, либо в другом плагине.
   * Для простоты примера в этом классе будут написаны только методы, необходимые для подписания.
   * Пример проверки подписи и сертификатов см. в классах CryptographyPlugin и Signer.
  */
  
  /// <summary>
  /// Плагин облачного подписания.
  /// </summary>
  public class CloudCryptographyPlugin : ICryptographyPlugin, ICloudSignPlugin
  {
    #region Поля и свойства

    /// <summary>
    /// Адрес облачного криптопровайдера.
    /// </summary>
    private Uri cryptoProviderAddress;

    /// <summary>
    /// HTTP-клиент.
    /// </summary>
    private static readonly HttpClient client = new HttpClient();

    #endregion

    #region Методы

    /// <summary>
    /// Получить JWT-токен доступа.
    /// </summary>
    /// <returns>JWT-токен доступа.</returns>
    private async Task<string> GetAccessTokenAsync()
    {
      // В платформе есть механизм кеширования токенов доступа пользователей, которым можно воспользоваться из плагина.
      // Возможно токен уже есть в кеше, и ничего получать не нужно.
      var token = CloudTokens.Get(this.Id.ToString());
      if (!string.IsNullOrEmpty(token))
        return token;
      
      // В примере JWT-токены получаются по OAuth2 token exchange flow.
      // Сначала получается JWT-токен в RX. В токен можно добавить некоторые утверждения, необходимые криптопровайдеру.
      var testClaims = new Dictionary<string, string>
      {
        { "someClaim", "someValue" },
        { "role", "Users" }
      };
      var platformToken = PlatformTokens.CreateToken("testSignCloudService", TimeSpan.FromMinutes(5), testClaims);

      // Затем этот JWT-токен RX обменивается на JWT-токен криптопровайдера.
      using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(this.cryptoProviderAddress, "/tokens")))
      {
        var requestParameters = new Dictionary<string, string>()
        {
          { "grant_type", "urn:ietf:params:oauth:grant-type:token-exchange" },
          { "client_id", "OurCompanyName"},
          { "subject_token", platformToken },
          { "subject_token_type", "urn:ietf:params:oauth:token-type:jwt" }
        };
        
        request.Content = new FormUrlEncodedContent(requestParameters);
        using (var response = await client.SendAsync(request))
        {
          response.EnsureSuccessStatusCode();
          token = await response.Content.ReadAsStringAsync();
        }

        // Сохраним токен в кеш для следующего раза.
        CloudTokens.Save(this.Id.ToString(), token, TimeSpan.FromHours(1));
        return token;
      }
    }

    /// <summary>
    /// Попытаться получить ПИН-код сертификата у пользователя.
    /// </summary>
    /// <param name="certificateThumbprint">Отпечаток сертификата.</param>
    /// <param name="pin">ПИН-код.</param>
    /// <returns>True, если ПИН-код успешно получен.</returns>
    private bool TryGetPin(string certificateThumbprint, out string pin)
    {
      // Запрос ПИН-кода у пользователя работает только в веб-клиенте.
      // Если подписание происходит на сервере, то будет сгенерировано исключение.
      return UserInteraction.GetPinFromUser(this.Id, certificateThumbprint, false, false, out pin, out var _);
    }

    /// <summary>
    /// Подписать по хешу.
    /// </summary>
    /// <param name="dataProvider">Поставщик данных.</param>
    /// <param name="accessToken">Токен доступа.</param>
    /// <param name="pin">ПИН-код.</param> 
    /// <returns>Подпись.</returns>
    private async Task<byte[]> SignByHashAsync(ICryptographyPluginDataProvider dataProvider, string accessToken, string pin)
    {
      // Поставщик данных может рассчитать хеш от данных по нужному алгоритму, если он реализован в этом или другом криптографическом плагине.
      var hash = dataProvider.ComputeHash("1.1.1.1.123.456");
      using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(this.cryptoProviderAddress, "/signByHash")))
      {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Add("PIN", pin);
        request.Content = new StringContent(Convert.ToBase64String(hash));
        using (var response = await client.SendAsync(request))
        {
          response.EnsureSuccessStatusCode();
          var signatureBase64 = await response.Content.ReadAsStringAsync();
          return Convert.FromBase64String(signatureBase64);
        }
      }
    }

    /// <summary>
    /// Подписать по потоку данных.
    /// </summary>
    /// <param name="dataProvider">Поставщик данных.</param>
    /// <param name="accessToken">Токен доступа.</param>
    /// <param name="pin">ПИН-код.</param>
    /// <returns>Подпись.</returns>    
    private async Task<byte[]> SignByStreamAsync(ICryptographyPluginDataProvider dataProvider, string accessToken, string pin)
    {
      var stream = dataProvider.GetData(out var needDispose);
      try
      {
        using (var memoryStream = new MemoryStream())
        {
          await stream.CopyToAsync(memoryStream);
          using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(this.cryptoProviderAddress, "/signByBody")))
          {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("PIN", pin);
            request.Content = new StringContent(Convert.ToBase64String(memoryStream.ToArray()));
            using (var response = await client.SendAsync(request))
            {
              response.EnsureSuccessStatusCode();
              var signatureBase64 = await response.Content.ReadAsStringAsync();
              return Convert.FromBase64String(signatureBase64);
            }
          }
        }
      }
      finally
      {
        // Потоки и их жизненный цикл могут быть разными, в зависимости от того какие данные подписываются.
        // Поэтому платформа Sungero передает специальный флаг needDispose, определяющий,
        // нужно ли в плагине после использования потока вызвать для него Dispose() или не нужно.
        
        // Также нужно учитывать, что StreamContent для потоковой передачи данных всегда вызывает Dispose() у своего внутреннего потока.
        // Поэтому при использовании StreamContent полученный у поставщика данных поток необходимо скопировать в другой или защитить от освобождения как-либо иначе.
        if (needDispose)
          stream.Dispose();
      }
    }    

    #endregion
    
    #region ICryptographyPlugin

    public void ApplySettings(IReadOnlyDictionary<string, string> settings)
    {
      if (settings.ContainsKey("cryptoProviderAddress"))
        this.cryptoProviderAddress = new Uri(settings["cryptoProviderAddress"]);
    }

    // TODO: Для каждого плагина подписания должен быть свой уникальный GUID. Данный идентификатор указан для примера и его следует поменять.
    public Guid Id { get; } = Guid.Parse("FB84311C-A781-4C00-84F8-8E4FD4E418B4");
    
    #endregion
    
    #region ICloudSignPlugin

    public byte[] Sign(ICryptographyPluginDataProvider dataProvider, X509Certificate2 certificate)
    {
      if (this.cryptoProviderAddress == null)
        throw new CloudPluginException("Settings is wrong or corrupted");

      // В примере используется аутентификация в криптопровайдере по JWT-токенам.
      var accessToken = this.GetAccessTokenAsync().Result;
      if (!TryGetPin(certificate.Thumbprint, out var pin))
      {
        // Пользователь отказался от ввода ПИН-кода.
        // Генерировать и показывать исключения в этом случае не нужно. Нужно просто выйти из подписания.
        return null;
      }

      // Платформа sungero может дать как хеш подписываемых данных, так и поток с ними.
      if (certificate.SubjectName.Name == "Иванов Иван")
        return this.SignByStreamAsync(dataProvider, accessToken, pin).Result;

      return this.SignByHashAsync(dataProvider, accessToken, pin).Result;
    }
    
    #endregion        

    #region Методы ICryptographyPlugin опущенные для простоты.
    
    public bool IsSignAlgorithmSupported(string signAlgorithmId)
    {
      return false;
    }    
    
    public bool IsHashAlgorithmSupported(string hashAlgorithmId)
    {
      return false;
    }

    public string GetHashAlgorithmIdBySignAlgorithmId(string signAlgorithmId)
    {
      throw new NotSupportedException();
    }

    public HashAlgorithmWrapper GetHashAlgorithmWrapperByHashAlgorithmId(string hashAlgorithmId)
    {
      throw new NotSupportedException();
    }

    public ISigner GetSignerBySignAlgorithmId(string signAlgorithmId)
    {
      throw new NotSupportedException();
    }

    public IEnumerable<X509Certificate2> GetAdditionalCertificates(X509Certificate2 certificate)
    {
      return Enumerable.Empty<X509Certificate2>();
    }

    public IEnumerable<string> ValidateCertificate(X509Certificate2 certificate, DateTime verificationDateTime)
    {
      return Enumerable.Empty<string>();
    }

    public IEnumerable<string> VerifySignature(byte[] dataSignature)
    {
      return Enumerable.Empty<string>();
    }
    
    #endregion
  }

  /// <summary>
  /// Исключение плагина облачного подписания.
  /// </summary>
  public class CloudPluginException : Exception
  {
    public CloudPluginException(string message) : base(message)
    {
    }
  }
}