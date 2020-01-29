using System.Linq;

namespace Library
{
  /// <summary>
  /// Класс диалога выбора файла с ключом.
  /// </summary>
  public static class KeyPathDialog
  {
    /// <summary>
    /// Делегат метода показа диалога.
    /// </summary>
    private static KeyPathDialogDelegate keyPathDialogDelegate;

    /// <summary>
    /// Внешний делегат показа диалога.
    /// </summary>
    public delegate object KeyPathDialogDelegate(string args);

    /// <summary>
    /// Установить метод показа диалога.
    /// </summary>
    /// <param name="value">Делегат.</param>
    public static void Set(KeyPathDialogDelegate value) => keyPathDialogDelegate = value;

    /// <summary>
    /// Получить результат показа диалога.
    /// </summary>
    /// <param name="fileFilter">Фильтр выбора файла.</param>
    /// <returns>Массив строк путь и пароль.</returns>
    public static string[] Get(string fileFilter)
    {
      object[] result = keyPathDialogDelegate.DynamicInvoke(fileFilter) as object[];
      return result.Cast<string>().ToArray();
    }
  }
}