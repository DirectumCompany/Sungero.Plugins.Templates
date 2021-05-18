using System;

namespace CryptographyPlugin
{
  /// <summary>
  /// Алгоритм хеширования.
  /// </summary>
  public sealed class HashAlgorithm : System.Security.Cryptography.HashAlgorithm
  {
    /// <summary>
    /// Идентификатор алгоритма хеширования.
    /// </summary>
    internal const string AlgorithmId = "1.3.14.3.2.26";
    
    /// <summary>
    /// Создать экземпляр класса алгоритма хеширования.
    /// </summary>
    public static new HashAlgorithm Create()
    {
      return new HashAlgorithm();
    }

    /// <summary>
    /// Сбросить алгоритм хеширования в исходное состояние.
    /// </summary>
    public override void Initialize()
    {
      /* Переопределение метода.
       *
       * Например:
       *  this.hasher = new Hasher();
       *  this.isInitialized = true;
      */

      throw new NotImplementedException();
    }

    /// <summary>
    /// Поблочное вычисление хеша.
    /// </summary>
    /// <param name="array">Входные данные, для которых вычисляется хеш-код.</param>
    /// <param name="ibStart">Смещение в массиве байтов, начиная с которого следует использовать данные.</param>
    /// <param name="cbSize">Размер блока.</param>
    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
      /* Переопределение метода.
       *
       * Например:
       *  if (!this.isInitialized)
       *    this.Initialize();
       *
       *  this.hasher.WriteNextBlock(array, ibStart, cbSize);
      */

      throw new NotImplementedException();
    }

    /// <summary>
    /// Завершить вычисление хеша.
    /// </summary>
    /// <returns>Вычисляемый хеш-код.</returns>
    /// <remarks>Этот метод завершает все частичные вычисления и возвращает правильное значение хеша для данных.</remarks>
    protected override byte[] HashFinal()
    {
      /* Переопределение метода.
       *
       * Например:
       *  this.hasher.Finish();
       *  return this.hasher.GetHash();
      */

      throw new NotImplementedException();
    }
  }
}
