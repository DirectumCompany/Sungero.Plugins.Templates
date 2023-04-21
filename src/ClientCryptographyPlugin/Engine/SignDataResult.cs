namespace Library
{
  /// <summary>
  /// Результат выполнения операции подписания.
  /// </summary>
  public enum SignDataResult
  {
    Success = 0,
    NeedFileName = 1,
    InvalidPassword = 2,
    UnknownError = 3,
    IllegalCertificateID = 4,
    NotFound = 5,
    InvalidFile = 6
  }
}
