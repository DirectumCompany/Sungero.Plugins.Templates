using System;
using System.Text;

namespace Library
{
  /// <summary>
  /// Результаты получения закрытого ключа.
  /// </summary>
  public enum CertificateLoadPrivateKeyResult
  {
    Success = 0,
    NeedFileName = 1,
    InvalidPassword = 2,
    UnknownError = 3,
    IllegalCertificateID = 4,
    NotFound = 5,
    InvalidFile = 6
  }

  /// <summary>
  /// Класс ответа.
  /// </summary>
  internal class Response
  {
    /// <summary>
    /// Результат подписания.
    /// </summary>
    public CertificateLoadPrivateKeyResult Result { get; set; } = CertificateLoadPrivateKeyResult.Success;

    /// <summary>
    /// Подпись.
    /// </summary>
    public string Signature { get; private set; }

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Установить значение подписи.
    /// </summary>
    /// <param name="signature">Подпись в формате массива байт.</param>
    public void SetSignature(byte[] signature)
    {
      this.SetSignature(Convert.ToBase64String(signature));
    }

    /// <summary>
    /// Установить значение подписи.
    /// </summary>
    /// <param name="signature">Подпись в формате Base64.</param>
    public void SetSignature(string signature)
    {
      this.Signature = Convert.ToBase64String(Encoding.ASCII.GetBytes(signature));
    }
  }
}
