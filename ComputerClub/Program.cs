namespace ConsoleApp1
{
    internal class Program
    {
        static void Main()
        {
            ComputerClub computerClub = new ComputerClub(8);
            computerClub.Work();
        }
    }

    class ComputerClub
    {
        private int _money = 0;
        private List<Computer> _computers = new List<Computer>();
        private Queue<Client> _clients = new Queue<Client>();

        public ComputerClub(int computersCount)
        {
            Random random = new Random();

            for (int i = 0; i < computersCount; ++i)
            {
                _computers.Add(new Computer(random.Next(5, 15)));
            }
            
            CreateNewClients(25, random);
        }

        public void CreateNewClients(int clientsCount, Random random)
        {
            for (int i = 0; i < clientsCount; ++i)
            {
                _clients.Enqueue(new Client(random.Next(100, 250), random));
            }
        }

        public void Work()
        {
            while (_clients.Count > 0)
            {
                Client newClient = _clients.Dequeue();
                Console.WriteLine($"Balance: {_money} roubles. We are waiting for new clients!");
                Console.WriteLine($"New client: {newClient.DesiredMinutes} minutes");
                ShowAllComputerStates();

                Console.WriteLine($"You give computer number: ");
                string userInput = Console.ReadLine()!;
                if (int.TryParse(userInput, out int computerNumber))
                {
                    computerNumber -= 1;

                    if (computerNumber >= 0 && computerNumber < _computers.Count)
                    {
                        if (_computers[computerNumber].IsTaken)
                        {
                            Console.WriteLine("Computer is taken :(");
                        }
                        else
                        {
                            if (newClient.CheckSolvency(_computers[computerNumber]))
                            {
                                Console.WriteLine($"Client pays and go in computer number {computerNumber + 1}");
                                _money += newClient.Pay();
                                _computers[computerNumber].BecomeTaken(newClient);
                            }
                            else
                            {
                                Console.WriteLine("Client cant pay");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Client has gone :(");
                    }
                }
                else
                {
                    CreateNewClients(1, new Random());
                    Console.WriteLine("Wrong data");
                }

                Console.WriteLine("Press any button");
                Console.ReadKey();
                Console.Clear();
                SpendOneMinute();
            }
        }

        private void ShowAllComputerStates()
        {
            Console.WriteLine("\nAll computers:");
            for (int i = 0; i < _computers.Count; ++i)
            {
                Console.Write($"{i + 1} — ");
                _computers[i].ShowState();
                Console.WriteLine();
            }
        }

        private void SpendOneMinute()
        {
            foreach (Computer computer in _computers)
            {
                computer.SpendOneMinute();
            }
        }
    }

    class Computer
    {
        private Client _client;
        private int _minutesRemainig;

        public bool IsTaken
        {
            get { return _minutesRemainig > 0; }
        }

        public int PricePerMinute { get; private set; }

        public Computer(int pricePerMinute)
        {
            PricePerMinute = pricePerMinute;
        }

        public void BecomeTaken(Client client)
        {
            _client = client;
            _minutesRemainig = _client.DesiredMinutes;
        }

        public void BecomeEmpty()
        {
            _client = null;
        }

        public void SpendOneMinute()
        {
            _minutesRemainig--;
        }

        public void ShowState()
        {
            if (IsTaken) Console.Write($"Computer is taken, minutes remained: {_minutesRemainig}");
            else Console.Write($"Computer is free. Price per minute: {PricePerMinute}");
        }
    }

    class Client
    {
        private int _money;
        private int _moneyToPay;

        public int DesiredMinutes { get; private set; }

        public Client(int money, Random random)
        {
            _money = money;
            DesiredMinutes = random.Next(10, 30);
        }

        public bool CheckSolvency(Computer computer)
        {
            _moneyToPay = DesiredMinutes * computer.PricePerMinute;
            if (_money >= _moneyToPay) return true;
            else
            {
                _moneyToPay = 0;
                return false;
            }
        }

        public int Pay()
        {
            _money -= _moneyToPay;
            return _moneyToPay;
        }
    }
}