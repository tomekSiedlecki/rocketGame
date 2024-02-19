using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Security.AccessControl;
using cw2;
using System.Threading.Tasks.Sources;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;

class Program
{
    static Board board = new Board();
    static Database db = new Database();
    static User currentUser;

    static CancellationTokenSource cancellationTokenSource;
    static Task TKeysAwait;
    static Task TDisplay;
    static Task TSpawnMove;

    static void Main()
    {
        //start();
        wholeProgram();
        end();
    }

    static void UsersMenu()
    {
        int choice = -1;
        do
        {
            Console.Clear();
            Console.WriteLine("Kliknij ENTER aby zalogowac");
            Console.WriteLine("Kliknij R aby zarejestrowac nowego uzytkownika");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                choice = 1;
            }
            else if (keyInfo.Key == ConsoleKey.R)
            {
                choice = 2;
            }
        } while (choice < 0);

        if(choice == 1)
        {
            currentUser = CredentialsLogic.Login();
            Console.Clear();
        }
        else if(choice == 2)
        {
            CredentialsLogic.Register();
            Console.Clear();
        }
    }

    static int Menu(int score = -1)
    {

        int choice = -1;
        do
        {
            Console.Clear();

            if(currentUser == null)
            {
                UsersMenu();
            }

            if (score == -1)
            {
                Console.WriteLine("Witaj w BARDZO imponujacej grze");
            }
            else
            {
                Console.WriteLine("Twoj wynik: " + score);
            }
            Console.WriteLine("Zalogowany: " + currentUser.Login);
            Console.WriteLine("Kliknij ENTER aby rozpoczac");
            Console.WriteLine("Kliknij R aby zobaczyc wszystkie rekordy");
            Console.WriteLine("Kliknij backspace aby wylaczyc");
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                choice = 1;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                choice = 0;
            }
            else if (keyInfo.Key == ConsoleKey.R)
            {
                choice = 2;
            }
            else
            {
                choice = -1;
            }
        } while (choice < 0);
        return choice;
    }

    static void wholeProgram()
    {
        int response = Menu();
        while (response != 0)
        {
            switch(response)
            {
                case 1:
                    game(); break;

                case 2:
                    records(); break;

            }

            response = Menu();
        }

    }

    static void game()
    {
        cancellationTokenSource = new CancellationTokenSource();

        //watek obslugujacy ruszanie prawo lewo z klawiatury
        TKeysAwait = new Task(() =>
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.A)
                {
                    board.Left();
                }
                else if (keyInfo.Key == ConsoleKey.D)
                {
                    board.Right();
                }
            }
        }, cancellationTokenSource.Token);

        TDisplay = new Task(() =>
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                board.DisplayWithColor();
                Thread.Sleep(100);
            }
        }, cancellationTokenSource.Token);

        TSpawnMove = new Task(() =>
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                Thread.Sleep(300);
                spawn();
                board.MoveObjects();
                board.score++;
            }
        }, cancellationTokenSource.Token);



        TDisplay.Start();
        TKeysAwait.Start();
        TSpawnMove.Start();

        while (board.onContact())
        {
            Thread.Sleep(50);
        }

        cancellationTokenSource.Cancel();

        TSpawnMove.Wait();
        TKeysAwait.Wait();
        RecordManagement.addRecordIfNew(currentUser,board.score);
        TDisplay.Wait();

        board = new Board();
    }

    static void records()
    {
        RecordManagement.DisplayAllRecords(currentUser);
    }
    static void spawn()
    {
        int asteroidsCount = 0;
        int bombCount = 0;
        Random random = new Random();

        int oddsForAsteroid = 0;
        int oddsForBomb = 0;

        if(board.score > 1)
        {
            oddsForAsteroid = 10;
            oddsForBomb = 5;
        }
        if(board.score > 10)
        {
            oddsForAsteroid = 15;
            oddsForBomb = 5;
        }
        if (board.score > 50)
        {
            oddsForAsteroid = 20;
            oddsForBomb = 5;
        }
        if (board.score > 100)
        {
            oddsForAsteroid = 35;
            oddsForBomb = 7;
        }
        if (board.score > 200)
        {
            oddsForAsteroid = 40;
            oddsForBomb = 9;
        }

        for(int i = 0; i < 7; i++)
        {
            int randomNumber = random.Next(1, 100);
            //asteroida
            if(randomNumber <= oddsForAsteroid)
            {
                asteroidsCount++;
            }
            //bomba
            else if (oddsForAsteroid < randomNumber && randomNumber <= oddsForBomb + oddsForAsteroid)
            {
                bombCount++;
            }
            else
            {
                //puste pole
            }

        }

        board.SpawnObjects(asteroidsCount, bombCount);
    }

    static void start()
    {
        Thread.Sleep(5000);
        animate("ROMECZEK STUDIOS");
        Thread.Sleep(1000);
        animate("prezentuje: ");
        Thread.Sleep(1000);
        Console.Clear();


        //Console.ForegroundColor = ConsoleColor.Green;
        //Console.Write(rocketString, Console.ForegroundColor);
        animate("BARDZO");
        animate("IMPONUJACA");
        animate("GRA");
        Thread.Sleep(3000);

        Console.Clear();

        animate("Specjalne podziekowania dla: ");
        animate("Igor Nowak oraz Damian Tymofijewicz");
        Thread.Sleep(3000);


        Console.Clear();
    }

    static void end()
    {
        //koniec
    }

    static void animate(string text)
    {
        Random random = new Random();
        foreach(var letter in text)
        {
            Thread.Sleep(random.Next(100,200));
            Console.Write(letter);
        }
        Console.Write("\n");
    }
}
