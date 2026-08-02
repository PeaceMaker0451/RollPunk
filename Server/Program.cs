using Microsoft.VisualBasic;
using RollPunk.Debug;
using RollPunk.NetcodeCommon;
using RollPunk.Server;

RPDebug.Logged += (log) => Console.WriteLine(log);
RPDebug.ErrorLogged += (log) =>
{
    var defaultColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(log);
    Console.ForegroundColor = defaultColor;
};

bool isRunning = true;
ThreadManager threadManager = new();

Thread mainThread = new Thread(new ThreadStart(() => ProcessThread(threadManager, 15, ref isRunning)));
mainThread.Start();

Server server = new(threadManager);
server.Start(15, 27010);

static void ProcessThread(ThreadManager threadManager, int ticksPerSecond, ref bool isRunning)
{
    Console.WriteLine($"Main thread started. Running at {ticksPerSecond} ticks per second.");
    DateTime _nextLoop = DateTime.Now;

    int tickMs = 1000 / ticksPerSecond;

    while (isRunning)
    {
        while (_nextLoop < DateTime.Now)
        {
            threadManager.UpdateMain();

            _nextLoop = _nextLoop.AddMilliseconds(tickMs);

            if (_nextLoop > DateTime.Now)
            {
                Thread.Sleep(_nextLoop - DateTime.Now);
            }
        }
    }
}
