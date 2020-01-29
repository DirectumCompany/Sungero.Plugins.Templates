namespace Library
{
  /// <summary>
  /// Класс запроса.
  /// </summary>
  internal class Request
  {
    /// <summary>
    /// Имя плагина.
    /// </summary>
    public string PluginName { get; set; }

    /// <summary>
    /// Идентификатор сертификата.
    /// </summary>
    public string CertificateID { get; set; }

    /// <summary>
    /// Подписываемые атрибуты.
    /// </summary>
    public string Attributes { get; set; }

    /// <summary>
    /// Язык интерфейса пользователя.
    /// </summary>
    public string Language { get; set; }
  }
}
