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
    /// ИД алгоритма подписания.
    /// </summary>
    private const string SignAlgorithmId = "1.3.14.3.2.29";

    #endregion

    #region ICryptographyPlugin

    public Guid Id { get; } = Guid.Parse("a9bb1128-2070-41ed-a1b4-2f206750f8c8");

    public void ApplySettings(IReadOnlyDictionary<string, string> settings) { }

    public IEnumerable<X509Certificate2> GetAdditionalCertificates(X509Certificate2 certificate)
    {
      return Enumerable.Empty<X509Certificate2>();
    }

    public string GetHashAlgorithmIdBySignAlgorithmId(string signAlgorithmId)
    {
      if (!this.IsSignAlgorithmSupported(signAlgorithmId))
        throw new NotSupportedException();

      return HashAlgorithmExample.AlgorithmId;
    }

    public HashAlgorithmWrapper GetHashAlgorithmWrapperByHashAlgorithmId(string hashAlgorithmId)
    {
      if (!this.IsHashAlgorithmSupported(hashAlgorithmId))
        throw new NotSupportedException();

      return new HashAlgorithmWrapper(hashAlgorithmId, HashAlgorithmExample.Create);
    }

    public ISigner GetSignerBySignAlgorithmId(string signAlgorithmId)
    {
      if (!this.IsSignAlgorithmSupported(signAlgorithmId))
        throw new NotSupportedException();

      return new Signer();
    }

    public bool IsHashAlgorithmSupported(string hashAlgorithmId)
    {
      return hashAlgorithmId.Equals(HashAlgorithmExample.AlgorithmId, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsSignAlgorithmSupported(string signAlgorithmId)
    {
      return signAlgorithmId.Equals(SignAlgorithmId, StringComparison.OrdinalIgnoreCase);
    }

    public IEnumerable<string> ValidateCertificate(X509Certificate2 certificate)
    {
      /* Метод реализующий дополнительную проверку сертификата. */
      return Enumerable.Empty<string>();
    }

    public IEnumerable<string> VerifySignature(byte[] dataSignature)
    {
      /* Метод реализующий дополнительную проверку подписи (в формате CMS). */
      return Enumerable.Empty<string>();
    }

    #endregion
  }
}
