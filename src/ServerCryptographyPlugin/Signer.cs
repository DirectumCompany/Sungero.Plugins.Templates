using Sungero.Cryptography.Shared;
using System;
using System.Security.Cryptography.X509Certificates;

namespace CryptographyPlugin
{
  /// <summary>
  /// Класс подписания по выбранному алгоритму.
  /// </summary>
  public sealed class Signer : ISigner
  {
    #region ISigner

    /// <summary>
    /// Сертификат.
    /// </summary>
    public X509Certificate2 Certificate { get; private set; }

    /// <summary>
    /// Усилить подпись при необходимости (после подписания).
    /// </summary>
    /// <param name="signature">Подпись.</param>
    /// <returns>Усиленная подпись.</returns>
    /// <remarks>Если усиливать подпись не надо, вернуть исходную подпись.</remarks>
    public byte[] EnhanceSignature(byte[] signature)
    {
      /* Пример. Добавление штампа времени.
       *   var hashAlgorithmWrapper = new HashAlgorithmWrapper(HashAlgorithmExample.AlgorithmId, HashAlgorithmExample.Create);
       *   var hash = hashAlgorithmWrapper.ComputeHash(signature);
       *   var requestGenerator = new TimeStampRequestGenerator();
       *   requestGenerator.SetCertReq(true);
       *   var nonce = BigInteger.ValueOf(DateTime.Now.Ticks);
       *   var tspRequest = requestGenerator.Generate(hashAlgorithmWrapper.AlgorithmId, hash, nonce);
       *   var tspRequestData = tspRequest.GetEncoded();
       *   var request = WebRequest.Create(tspServerUrl);
       *   request.Method = WebRequestMethods.Http.Post;
       *   request.ContentType = "application/timestamp-query";
       *   request.ContentLength = tspRequestData.Length;
       *   using (var requestStream = request.GetRequestStream())
       *   {
       *     requestStream.Write(tspRequestData, 0, tspRequestData.Length);
       *   }
       *   var response = (HttpWebResponse)request.GetResponse();
       *   using (var responseStream = response.GetResponseStream())
       *   {
       *     var tspResponse = new TimeStampResponse(responseStream);
       *     tspResponse.Validate(tspRequest);
       *     return tspResponse.TimeStampToken.GetEncoded();
       *   }
      */

      return signature;
    }

    /// <summary>
    /// Модифицировать подписываемые атрибуты (до подписания).
    /// </summary>
    /// <param name="encodedSigningAttributes">Подписываемые атрибуты.</param>
    /// <returns>Модифицированные подписываемые атрибуты.</returns>
    /// <remarks>Позволяет добавить новые или изменить существующие подписываемые атрибуты. Если нет необходимости в модификации, возвращает входной параметр.
    /// Подробнее о атрибутах см. в статье https://ru.wikipedia.org/wiki/CAdES#Обязательные_подписываемые_атрибуты_CAdES-BES. </remarks>
    public byte[] EnhanceSigningAttributes(byte[] encodedSigningAttributes)
    {
      /* Пример. Изменение атрибута IdAASigningCertificateV2.
       *  var attributesSet = (Asn1Set)Asn1Object.FromByteArray(encodedSigningAttributes);
       *  var attributeTable = new AttributeTable(attributesSet);
       *  var attributeDictionary = attributeTable.ToDictionary();
       *  if (attributeDictionary.Contains(PkcsObjectIdentifiers.IdAASigningCertificateV2))
       *  {
       *    // Код изменения атрибута.
       *  }
      */

      return encodedSigningAttributes;
    }

    /// <summary>
    /// Выполнить инициализацию.
    /// </summary>
    /// <param name="certificate">Сертификат.</param>
    /// <remarks>Реализация метода зависит от особенностей подписания и может изменяться в зависимости от решаемых задач.</remarks>
    public void Initialize(X509Certificate2 certificate)
    {
      this.Certificate = certificate;
    }

    /// <summary>
    /// Подписать данные.
    /// </summary>
    /// <param name="data">Данные (массив байт).</param>
    /// <param name="hashAlgorithmWrapper">Обертка над алгоритмом хеширования.</param>
    /// <returns>Подпись.</returns>
    /// <remarks>Необходимо вернуть "сырую подпись" (не CMS).</remarks>
    public byte[] SignData(byte[] data, HashAlgorithmWrapper hashAlgorithmWrapper)
    {
      /* Например:
       *  var cryptoServiceProvider = (RSACryptoServiceProvider)this.Certificate.PrivateKey;
       *  using (var hashAlgorithm = hashAlgorithmWrapper.GetHashAlgorithm())
       *    return cryptoServiceProvider.SignHash(hashAlgorithm.ComputeHash(data), hashAlgorithmWrapper.AlgorithmId);
      */

      throw new NotImplementedException();
    }

    /// <summary>
    /// Попытаться загрузить закрытый ключ сертификата.
    /// </summary>
    /// <returns>Признак успешной загрузки закрытого ключа.</returns>
    /// <remarks>Если метод вернул false, то при подписании сгенерируется исключение.</remarks>
    public bool TryLoadPrivateKey()
    {
      /* Например:
       *  var store = new X509Store();
       *  store.Open(OpenFlags.ReadOnly);
       *  var privateKeyCertificate = store.Certificates
       *    .OfType<X509Certificate2>()
       *    .FirstOrDefault(c => (c.Thumbprint?.Equals(this.Certificate.Thumbprint, StringComparison.OrdinalIgnoreCase) ?? false) && c.HasPrivateKey);
       *  store.Close();
       *
       *  if (privateKeyCertificate != null)
       *  {
       *    this.Certificate = privateKeyCertificate;
       *    return true;
       *  }
      */

      throw new NotImplementedException();
    }

    /// <summary>
    /// Проверить подпись данных.
    /// </summary>
    /// <param name="data">Данные (массив байт).</param>
    /// <param name="signature">Подписанные данные.</param>
    /// <param name="hashAlgorithmWrapper">Обертка над алгоритмом хеширования.</param>
    /// <returns></returns>
    /// <remarks>Проверяется "сырая подпись" (не CMS).</remarks>
    public bool VerifySignature(byte[] data, byte[] signature, HashAlgorithmWrapper hashAlgorithmWrapper)
    {
      /* Например:
       *  var cryptoServiceProvider = (RSACryptoServiceProvider)this.Certificate.PublicKey.Key;
       *  var hash = hashAlgorithmWrapper.ComputeHash(data);
       *  return cryptoServiceProvider.VerifyHash(hash, hashAlgorithmWrapper.AlgorithmId, signature);
      */

      throw new NotImplementedException();
    }

    #endregion
  }
}
