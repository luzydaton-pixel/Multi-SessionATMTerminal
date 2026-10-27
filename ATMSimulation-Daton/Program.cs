using System;

class Program
{
    static void Main(string[] args)
    {
        const int CORRECT_PIN = 4821;
        double accountBalance = 1250.00;
        double dailyLimit = 500.00;
        double dailyWithdrawn = 150.00;

        bool isInternationalCard = true;
        bool isVipCustomer = false;

        int transactionCount = 0;
        int pinAttempts = 3;
        bool isAuthenticated = false;

        double[] recentTransactions = new double[3];

        Console.WriteLine("========================================");
        Console.WriteLine("        WELCOME TO SECURE BANK ATM      ");
        Console.WriteLine("========================================");

        while (pinAttempts > 0 && !isAuthenticated)
        {
            Console.Write("Enter your 4-digit PIN: ");
            if (int.TryParse(Console.ReadLine(), out int enteredPin))
            {
                if (enteredPin == CORRECT_PIN)
                {
                    isAuthenticated = true;
                    Console.WriteLine("\n[✓] Access granted.\n");
                }
                else
                {
                    pinAttempts--;
                    Console.WriteLine($"[X] Invalid PIN. Attempts left: {pinAttempts}");
                }
            }
            else
            {
                pinAttempts--;
                Console.WriteLine($"[X] Non-numeric entry detected. Attempts left: {pinAttempts}");
            }
        }

        if (!isAuthenticated)
        {
            Console.WriteLine("\n[!] CARD CONFISCATED. Maximum failed attempts reached.");
            return;
        }

        bool sessionActive = true;

        while (sessionActive)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("1. Check Balance");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Withdraw");
            Console.WriteLine("4. Recent Transactions (Mini-Statement)");
            Console.WriteLine("5. Exit & Eject");
            Console.WriteLine("----------------------------------------");
            Console.Write("Select an option (1-5): ");

            string? choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine($"Current Balance: {accountBalance:C}");
                    Console.WriteLine($"Remaining Daily Limit: {(dailyLimit - dailyWithdrawn):C}\n");
                    break;

                case "2":
                    Console.Write("Enter deposit amount: $");
                    if (double.TryParse(Console.ReadLine(), out double deposit) && deposit > 0 && deposit <= 5000)
                    {
                        accountBalance += deposit;
                        recentTransactions[transactionCount % recentTransactions.Length] = deposit;
                        transactionCount++;

                        Console.WriteLine($"[✓] Deposited {deposit:C}. New Balance: {accountBalance:C}\n");
                    }
                    else
                    {
                        Console.WriteLine("[X] Deposit rejected. Valid amounts: $0.01 to $5,000.00.\n");
                    }
                    break;

                case "3":
                    Console.Write("Enter withdrawal amount: $");
                    if (double.TryParse(Console.ReadLine(), out double withdraw) && withdraw > 0)
                    {
                        bool validDenomination = (withdraw % 20 == 0 || withdraw % 50 == 0 || (withdraw % 20 == 10 || withdraw % 20 == 30));

                        if (!validDenomination)
                        {
                            Console.WriteLine("[X] Denomination Error: Machine only dispenses combinations of $20 and $50 bills.\n");
                            break;
                        }

                        double remainingLimit = dailyLimit - dailyWithdrawn;
                        if (withdraw > remainingLimit)
                        {
                            Console.WriteLine($"[X] Declined: Amount exceeds daily limit remaining ({remainingLimit:C}).\n");
                            break;
                        }

                        double surcharge = 0.0;
                        if (withdraw > 100)
                        {
                            surcharge += 4.50;
                        }

                        if (withdraw > 500)
                        {
                            surcharge += (withdraw * 0.01);
                        }

                        double totalRequired = withdraw + surcharge;

                        if (totalRequired > accountBalance)
                        {
                            Console.WriteLine($"[X] Declined: Total required ({totalRequired:C}) exceeds balance.\n");
                            break;
                        }

                        accountBalance -= totalRequired;
                        dailyWithdrawn += withdraw;

                        recentTransactions[transactionCount % recentTransactions.Length] = -withdraw;
                        transactionCount++;

                        Console.WriteLine($"[✓] Approved! Surcharge: {surcharge:C}");

                        int remainingToDispense = (int)withdraw;
                        int bill50Count = remainingToDispense / 50;
                        
                        
                        {
                            bill50Count--;
                        }

                        int bill20Count = (remainingToDispense - (bill50Count * 50)) / 20;

                        for (int i = 0; i < bill50Count; i++)
                        {
                            Console.WriteLine("  -> [Dispensed: $50.00 bill]");
                        }

                        for (int j = 0; j < bill20Count; j++)
                        {
                            Console.WriteLine("  -> [Dispensed: $20.00 bill]");
                        }

                        Console.WriteLine($"Remaining Balance: {accountBalance:C}\n");
                    }
                    else
                    {
                        Console.WriteLine("[X] Invalid numerical format.\n");
                    }
                    break;

                case "4":
                    Console.WriteLine("--- RECENT TRANSACTION HISTORY ---");
                    if (transactionCount == 0)
                    {
                        Console.WriteLine("No transactions logged in this session.\n");
                    }
                    else
                    {
                        for (int i = 0; i < transactionCount; i++)
                        {
                            string type = (recentTransactions[i] >= 0) ? "DEPOSIT " : "WITHDRAW";
                            Console.WriteLine($"[{i + 1}] {type}: {Math.Abs(recentTransactions[i]):C}");
                        }
                        Console.WriteLine($"Total lifetime session actions: {transactionCount}\n");
                    }
                    break;

                case "5":
                    Console.WriteLine("Ejecting card... Thank you for banking with us.");
                    sessionActive = false;
                    break;

                default:
                    Console.WriteLine("[!] Unrecognized menu option. Please select 1 through 5.\n");
                    break;
            }
        }

        Console.WriteLine("System reset. Ready for next user.");
    }
}
