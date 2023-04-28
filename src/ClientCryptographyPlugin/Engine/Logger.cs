namespace Library
{
  /// <summary>
  /// Класс логгера. Означивает и вызывает метод логирования у веб-агента.
  /// </summary>
  public static class Logger
  {
    /// <summary>
    /// Уровни логирования.
    /// </summary>
    public enum LogLevel
    {
      DEBUG = 10,
      INFO = 20,
      WARNING = 30,
      ERROR = 40,
      CRITICAL = 50
    }

    /// <summary>
    /// Внешний метод логирования.
    /// </summary>
    private static LoggerDelegate loggerDelegate;

    /// <summary>
    /// Делегат внешнего метода логирования.
    /// </summary>
    public delegate void LoggerDelegate(int level, string message);

    /// <summary>
    /// Логировать сообщение.
    /// </summary>
    /// <param name="level">Уровень логирования.</param>
    /// <param name="message">Сообщение.</param>
    private static void Log(LogLevel level, string message) => loggerDelegate.DynamicInvoke((int)level, message);

    /// <summary>
    /// Установить метод логирования.
    /// </summary>
    /// <param name="value">Делегат логирования.</param>
    public static void Set(LoggerDelegate value) => loggerDelegate = value;

    /// <summary>
    /// Логировать отладочное сообщение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public static void Debug(string message) => Log(LogLevel.DEBUG, message);

    /// <summary>
    /// Логировать информационное сообщение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public static void Info(string message) => Log(LogLevel.INFO, message);

    /// <summary>
    /// Логировать предупреждение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public static void Warning(string message) => Log(LogLevel.WARNING, message);

    /// <summary>
    /// Логировать ошибку.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public static void Error(string message) => Log(LogLevel.ERROR, message);

    /// <summary>
    /// Логировать критическую ошибку.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public static void Fatal(string message) => Log(LogLevel.CRITICAL, message);
  }
}
