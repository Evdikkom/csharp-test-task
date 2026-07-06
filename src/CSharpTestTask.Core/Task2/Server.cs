using System.Threading;

namespace CSharpTestTask.Task2;

/// <summary>
/// Задача 2. Статический счётчик для условного сервера с безопасным доступом из разных потоков.
/// Для синхронизации используется ReaderWriterLockSlim: несколько читателей могут работать одновременно,
/// а запись выполняется только по очереди и на время изменения счётчика останавливает чтение.
/// </summary>
public static class Server
{
    private static readonly ReaderWriterLockSlim CountLock = new(LockRecursionPolicy.NoRecursion);
    private static int _count;

    public static int GetCount()
    {
        CountLock.EnterReadLock();

        try
        {
            return _count;
        }
        finally
        {
            CountLock.ExitReadLock();
        }
    }

    public static void AddToCount(int value)
    {
        CountLock.EnterWriteLock();

        try
        {
            checked
            {
                _count += value;
            }
        }
        finally
        {
            CountLock.ExitWriteLock();
        }
    }

    internal static void ResetForTests()
    {
        CountLock.EnterWriteLock();

        try
        {
            _count = 0;
        }
        finally
        {
            CountLock.ExitWriteLock();
        }
    }
}
