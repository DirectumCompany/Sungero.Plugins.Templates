namespace Library
{
  /// <summary>
  /// Результат подписания данных.
  /// </summary>
  public class SignDataResponse
  {
    /// <summary>
    /// Отпечаток сертификата.
    /// </summary>
    public string CertificateThumbprint { get; set; }

    /// <summary>
    /// Подписанные данные.
    /// </summary>
    public string SignedData { get; set; }
  }
}