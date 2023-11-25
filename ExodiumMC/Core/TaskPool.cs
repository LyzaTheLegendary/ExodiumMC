using System.Collections.Concurrent;

namespace Core.Async;

public class TaskPool
{
    private readonly BlockingCollection<Action> _pool = new();
    private readonly List<Task> _tasks;
    private readonly CancellationTokenSource _tokenSource = new();
    private ushort shrink = 0;
    public TaskPool(ushort size)
    {
        _tasks = new(size);
        for (int i = 0; i < size; i++)
            _tasks.Add(Task.Run(TaskLoop, _tokenSource.Token));
    }
    public void PendTask(Action task) => _pool.Add(task);
    public int GetSize() => _tasks.Count;
    public void IncreasePool(ushort amount)
    {
        for (int i = 0; i < amount; i++)
            _tasks.Add(Task.Run(TaskLoop, _tokenSource.Token));
    }
    public void ShrinkPool(ushort amount) => shrink += amount;
    public bool IsBusy() => _pool.Count !> 0;
    public void Stop() => _tokenSource.Cancel();
    private async Task<Task> TaskLoop()
    {
        foreach (Action action in _pool.GetConsumingEnumerable())
        {
            action.Invoke();
            if (shrink != 0)
            {
                shrink--;
                _tasks.RemoveAll(task => task.IsCompleted);
                return Task.CompletedTask;
            }
            
        }
        return Task.CompletedTask;
    }
}
