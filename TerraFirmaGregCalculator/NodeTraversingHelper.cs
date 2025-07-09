using System.Threading;

namespace TerraFirmaGregCalculator;

public static class NodeTraversingHelper
{
    private readonly static object _lockObj = new();


    public static bool CanStartNewThread()
    {
        lock (_lockObj)
        {
            ThreadPool.GetAvailableThreads(out var workerThreadsCount, out _);

            if (workerThreadsCount > 0)
                return true;

            return false;
        }
    }

}
