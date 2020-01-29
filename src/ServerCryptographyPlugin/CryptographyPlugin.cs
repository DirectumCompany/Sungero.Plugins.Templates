using Sungero.Cryptography.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CryptographyPlugin
{
  /// <summary>
  /// Плагин криптографии.
  /// </summary>
  public sealed class CryptographyPlugin : ICryptographyPlugin
  {
    #region Поля и свойства

    /// <summary>
    /// Идентификатор алгоритма подписания.
    /// </summary>
    private const string SignAlgorithmId = "1.3.14.3.2.29";

    #endregion

    #region ICryptographyPlugin

    /// <summary>
    /// Идентификатор плагина (GUID).
    /// </summary>
    // TODO: Для каждого плагина подписания должен быть свой уникальный GUID. Данный идентификатор указан для примера и его следует поменять.
    // TODO: Идентификаторы серверного и клиентского плагинов должны совпадать, идентификатор клиентского плагина задается в файле ClientPlugin.targets.
    public Guid Id { get; } = Guid.Parse("a9bb1128-2070-41ed-a1b4-2f206750f8c8");

    /// <summary>
    /// Применить настройки.
    /// </summary>
    /// <param name="settings">Настройки плагина.</param>
    public void ApplySettings(IReadOnlyDictionary<string, string> settings) { }

    /// <summary>
    /// Получить список дополнительных сертификатов для построения цепочки сертификатов.
    /// </summary>
    /// <param name="certificate">Сертификат.</param>
    /// <returns>Список дополнительных сертификатов для построения цепочки сертификатов.</returns>
    public IEnumerable<X509Certificate2> GetAdditionalCertificates(X509Certificate2 certificate)
    {
      return Enumerable.Empty<X509Certificate2>();
    }

    /// <summary>
    /// Получить идентификатор алгоритма хеширования по идентификатору алгоритма подписания.
    /// </summary>
    /// <param name="signAlgorithmId">Идентификатор алгоритма подписания.</param>
    /// <returns>Идентификатор алгоритма хеширования.</returns>
    public string GetHashAlgorithmIdBySignAlgorithmId(string signAlgorithmId)
    {
      if (!this.IsSignAlgorithmSupported(signAlgorithmId))
        throw new NotSupportedException();

      return HashAlgorithmExample.AlgorithmId;
    }

    /// <summary>
    /// Получить обертку над алгоритмом хеширования.
    /// </summary>
    /// <param name="hashAlgorithmId">Идентификатор алгоритма хеширования.</param>
    /// <returns>Обертка над алгоритмом хеширования.</returns>
    public HashAlgorithmWrapper GetHashAlgorithmWrapperByHashAlgorithmId(string hashAlgorithmId)
    {
      if (!this.IsHashAlgorithmSupported(hashAlgorithmId))
        throw new NotSupportedException();

      return new HashAlgorithmWrapper(hashAlgorithmId, HashAlgorithmExample.Create);
    }

    /// <summary>
    /// Получить класс для подписания по идентификатору алгоритма подписания.
    /// </summary>
    /// <param name="signAlgorithmId">Идентификатор алгоритма подписания.</param>
    /// <returns>Класс для подписания.</returns>
    public ISigner GetSignerBySignAlgorithmId(string signAlgorithmId)
    {
      if (!this.IsSignAlgorithmSupported(signAlgorithmId))
        throw new NotSupportedException();

      return new Signer();
    }

    /// <summary>
    /// Проверить, поддерживается ли плагином данный алгоритм хеширования.
    /// </summary>
    /// <param name="hashAlgorithmId">Идентификатор алгоритма хеширования.</param>
    /// <returns>True, если алгоритм хеширования поддерживается плагином.</returns>
    public bool IsHashAlgorithmSupported(string hashAlgorithmId)
    {
      return hashAlgorithmId.Equals(HashAlgorithmExample.AlgorithmId, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Проверить, поддерживается ли плагином данный алгоритм подписания.
    /// </summary>
    /// <param name="signAlgorithmId">Идентификатор алгоритма подписания.</param>
    /// <returns>True, если алгоритм подписания поддерживается плагином.</returns>
    public bool IsSignAlgorithmSupported(string signAlgorithmId)
    {
      return signAlgorithmId.Equals(SignAlgorithmId, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Выполнить проверку сертификата.
    /// </summary>
    /// <param name="certificate">Сертификат.</param>
    /// <returns>Список ошибок по итогам проверки сертификата.</returns>
    /// <remarks>Метод, реализующий дополнительную проверку сертификата.</remarks>
    public IEnumerable<string> ValidateCertificate(X509Certificate2 certificate)
    {
      return Enumerable.Empty<string>();
    }

    /// <summary>
    /// Выполнить проверку подписи.
    /// </summary>
    /// <param name="dataSignature">Подпись.</param>
    /// <returns>Список ошибок по итогам проверки подписи.</returns>
    /// <remarks>Метод, реализующий дополнительную проверку подписи в формате CMS.</remarks>
    public IEnumerable<string> VerifySignature(byte[] dataSignature)
    {
      return Enumerable.Empty<string>();
    }

    #endregion
  }
}
