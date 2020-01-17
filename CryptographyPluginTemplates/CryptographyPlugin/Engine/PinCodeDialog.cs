namespace Library
{
  /// <summary> Класс диалога выбора пин кода.
  public static class PinCodeDialog
  {
    /// <summary>
    /// Делегат метода показа диалога.
    /// </summary>
    private static PinCodeDialogDelegate pinCodeDialogDelegate;

    /// <summary>
    /// Внешний делегат показа диалога.
    /// </summary>
    public delegate object PinCodeDialogDelegate();

    /// <summary>
    /// Установить метод показа диалога.
    /// </summary>
    /// <param name="value">Делегат.</param>
    public static void Set(PinCodeDialogDelegate value) => pinCodeDialogDelegate = value;

    /// <summary>
    /// Получить результат показа диалога.
    /// </summary>
    /// <returns>Строка пин кода.</returns>
    public static string Get() => pinCodeDialogDelegate.DynamicInvoke()?.ToString();
  }
}