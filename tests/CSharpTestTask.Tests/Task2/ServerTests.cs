using CSharpTestTask.Task2;

namespace CSharpTestTask.Tests.Task2;

public sealed class ServerTests
{
    [Fact]
    public void AddToCount_ShouldChangeCount()
    {
        Server.ResetForTests();

        Server.AddToCount(5);
        Server.AddToCount(7);

        Assert.Equal(12, Server.GetCount());
    }

    [Fact]
    public async Task AddToCount_ShouldBeThreadSafe_WhenManyClientsWriteAndRead()
    {
        Server.ResetForTests();

        const int writersCount = 1_000;
        const int readersCount = 100;
        var tasks = new Task[writersCount + readersCount];

        for (int i = 0; i < writersCount; i++)
        {
            tasks[i] = Task.Run(() => Server.AddToCount(1));
        }

        for (int i = writersCount; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 1_000; j++)
                {
                    int current = Server.GetCount();

                    if (current < 0)
                    {
                        throw new InvalidOperationException("Count must not be negative in this test.");
                    }
                }
            });
        }

        await Task.WhenAll(tasks);

        Assert.Equal(writersCount, Server.GetCount());
    }

    [Fact]
    public void AddToCount_ShouldThrowOverflowException_WhenIntOverflows()
    {
        Server.ResetForTests();
        Server.AddToCount(int.MaxValue);

        Assert.Throws<OverflowException>(() => Server.AddToCount(1));
    }
}
