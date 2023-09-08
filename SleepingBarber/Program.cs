int maxNumOfClients = 5;
int chairsInWaitingRoom = 3;

object locker = new object();

Semaphore waitingRoom = new Semaphore(chairsInWaitingRoom, chairsInWaitingRoom);
Semaphore barberSleepChair = new Semaphore(0, 1);
Semaphore haircutProcess = new Semaphore(0, 1);

bool isAllDone = false;

Thread barberThread = new Thread(Barber);
barberThread.Start();

Thread[] clientsThreads = new Thread[maxNumOfClients];

for (int i = 0; i < maxNumOfClients; i++)
{
    clientsThreads[i] = new Thread(Client);
    clientsThreads[i].Name = $"Client {i + 1}";
    clientsThreads[i].Start();
}

for (int i = 0; i < maxNumOfClients; i++)
    clientsThreads[i].Join();

isAllDone = true;
barberSleepChair.Release();
barberThread.Join();

Console.WriteLine("End of work!");

void Barber()
{
    while (!isAllDone)
    {
        Console.WriteLine("The barber is sleeping...");
        barberSleepChair.WaitOne();

        if (!isAllDone)
        {
            Console.WriteLine("Barber cuts hair...");
            Thread.Sleep(4000);
            Console.WriteLine("The barber finished the haircut!");
            haircutProcess.Release();
        }
        else
            Console.WriteLine("The barber is sleeping...");
    }
}

void Client()
{
    Console.WriteLine($"{Thread.CurrentThread.Name} goes to the barbershop...");
    Thread.Sleep(1500);
    Console.WriteLine($"{Thread.CurrentThread.Name} came.");
    waitingRoom.WaitOne();
    Console.WriteLine($"{Thread.CurrentThread.Name} takes up a seat in the waiting room...");

    lock (locker)
    {
        waitingRoom.Release();
        Console.WriteLine($"{Thread.CurrentThread.Name} wakes up the barber...");
        barberSleepChair.Release();
        Console.WriteLine($"The haircut process begins...");
        haircutProcess.WaitOne();
    }

    Console.WriteLine($"{Thread.CurrentThread.Name} leaves the barbershop...");
}