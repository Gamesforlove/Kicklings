using System;
using System.Threading.Tasks;

public static class TaskUtils
{
    public static async Task WaitUntil(Func<bool> predicate, int sleep = 50)
    {
        while (!predicate())
        {
            await Task.Delay(sleep);
        }
    }
    public static Task WaitForEvent(Action<Action> subscribe, Action<Action> unsubscribe)
    {
        var tcs = new TaskCompletionSource<bool>();

        void Handler()
        {
            tcs.TrySetResult(true);
        }

        subscribe(Handler);

        // Отписываемся когда задача завершена
        tcs.Task.ContinueWith(_ => unsubscribe(Handler));

        return tcs.Task;
    }
}
