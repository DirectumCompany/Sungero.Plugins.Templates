namespace Library
{
  /// <summary> Класс локализатора. Означивает и вызывает метод локализации у агента. </summary>
  public static class Localizer
  {
    /// <summary>
    /// Внешний метод локализатора.
    /// </summary>
    private static LocalizeDelegate localizeDelegate;

    /// <summary>
    /// Внешний метод локализатора.
    /// </summary>
    public delegate object LocalizeDelegate(string args);

    /// <summary>
    /// Установить метод локализатора.
    /// </summary>
    /// <param name="value">Делегат локализатора.</param>
    public static void Set(LocalizeDelegate value) => localizeDelegate = value;

    /// <summary>
    /// Локализовать строку с помощью метода агента.
    /// </summary>
    /// <param name="localize_id">Код строки локализации.</param>
    public static string L(string localize_id) => localizeDelegate.DynamicInvoke(localize_id).ToString();
  }
}