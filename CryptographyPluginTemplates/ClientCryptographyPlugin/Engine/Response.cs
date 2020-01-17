namespace Library
{
  /// <summary>Перечень результатов получения закрытого ключа.</summary>
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

  /// <summary> Класс ответа. </summary>
  internal class Response
  {
    /// <summary>
    /// Результат подписания.
    /// </summary>
    public CertificateLoadPrivateKeyResult Result { get; set; } = CertificateLoadPrivateKeyResult.Success;

    /// <summary>
    /// Подпись.
    /// </summary>
    public string Signature { get; set; }

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string Error { get; set; }
  }
}
