using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using Sungero.Cryptography.Shared;

namespace ServerCryptographyPlugin
{
  /*
   * Реализация упрощённого плагина криптографии (который работает с телом документа вместо хэша).
   * В примере ииспользован алгоритм EcDsa.
   * Для простоты примера в этом классе будут написаны только методы, необходимые для подписания и проверки подписи.
   * Пример проверки сертификатов см. в классах CryptographyPlugin и Signer.
  */

  /// <summary>
  /// Плагин упрощеннго подписания.
  /// </summary>
  public sealed class SimpleCryptographyPlugin : ISimpleCryptographyPlugin
  {
    #region Поля и свойства

    /// <summary>
    /// Идентификатор алгоритма подписания.
    /// </summary>
    private const string SignAlgorithmId = "1.2.840.10045.2.1";

    /// <summary>
    /// Идентификатор алгоритма хеширования.
    /// </summary>
    private const string Sha256Oid = "2.16.840.1.101.3.4.2.1";

    #endregion

    #region ICryptographyPlugin

    // TODO: Для каждого плагина подписания должен быть свой уникальный GUID. Данный идентификатор указан для примера и его следует поменять.
    public Guid Id { get; } = Guid.Parse("5AB3F8E6-FA9A-405D-B8F9-08F149098F4F");

    public void ApplySettings(IReadOnlyDictionary<string, string> settings) { }

    public string GetHashAlgorithmIdBySignAlgorithmId(string signAlgorithmId)
    {
      if (!this.IsSignAlgorithmSupported(signAlgorithmId))
        throw new NotSupportedException();

      return Sha256Oid;
    }

    public HashAlgorithmWrapper GetHashAlgorithmWrapperByHashAlgorithmId(string hashAlgorithmId)
    {
      if (!this.IsHashAlgorithmSupported(hashAlgorithmId))
        throw new NotSupportedException();

      return new HashAlgorithmWrapper(hashAlgorithmId, SHA256.Create);
    }

    public bool IsHashAlgorithmSupported(string hashAlgorithmId)
    {
      return !string.IsNullOrEmpty(hashAlgorithmId) &&
             hashAlgorithmId.Equals(Sha256Oid, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsSignAlgorithmSupported(string signAlgorithmId)
    {
      return signAlgorithmId.Equals(SignAlgorithmId, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Методы ICryptographyPlugin опущенные для простоты.

    public IEnumerable<X509Certificate2> GetAdditionalCertificates(X509Certificate2 certificate)
    {
      return Enumerable.Empty<X509Certificate2>();
    }

    public ISigner GetSignerBySignAlgorithmId(string signAlgorithmId)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<string> VerifySignature(byte[] dataSignature)
    {
      return Enumerable.Empty<string>();
    }

    public IEnumerable<string> ValidateCertificate(X509Certificate2 certificate, DateTime verificationDateTime)
    {
      return Enumerable.Empty<string>();
    }

    #endregion

    #region ISimpleCryptographyPlugin

    public byte[] Sign(ICryptographyPluginDataProvider dataProvider, X509Certificate2 certificate)
    {
      using (var memoryStream = new MemoryStream())
      {
        var stream = dataProvider.GetData(out var needDispose);
        try
        {
          stream.CopyTo(memoryStream);
          var store = new X509Store();
          store.Open(OpenFlags.ReadOnly);
          var privateKeyCertificate = store.Certificates
            .OfType<X509Certificate2>()
            .FirstOrDefault(c => (c.Thumbprint?.Equals(certificate.Thumbprint, StringComparison.OrdinalIgnoreCase) ?? false) && c.HasPrivateKey);
          store.Close();

          var signedCms = new SignedCms(new ContentInfo(memoryStream.ToArray()), true);
          var signer = new CmsSigner(privateKeyCertificate)
          {
            IncludeOption = X509IncludeOption.WholeChain
          };
          signer.DigestAlgorithm = new Oid(Sha256Oid);
          signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.UtcNow));
          signedCms.ComputeSignature(signer, false);
          var result = signedCms.Encode();
          return result;
        }
        finally
        {
          if (needDispose)
            stream.Dispose();
        }
      }
    }

    public bool VerifySignature(ICryptographyPluginDataProvider dataProvider, byte[] signature)
    {
      using (var memoryStream = new MemoryStream())
      {
        var stream = dataProvider.GetData(out var needDispose);
        try
        {
          stream.CopyTo(memoryStream);
          var signedCms = new SignedCms(new ContentInfo(memoryStream.ToArray()), true);

          signedCms.Decode(signature);
          signedCms.CheckSignature(true);

          return true;
        }
        catch (Exception)
        {
          return false;
        }
        finally
        {
          if (needDispose)
            stream.Dispose();
        }
      }
    }

    #endregion
  }
}
