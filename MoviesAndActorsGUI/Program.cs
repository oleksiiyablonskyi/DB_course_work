using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Terminal.Gui;
using progbase3;
using System.IO;

class Program
{

    static void Main(string[] args)
    {
        string databaseFilePath = @"../data/films_and_actors.db";

        if (!File.Exists(databaseFilePath))
        {
            Console.WriteLine("Cannot find database file or file is incorrect.");
            Environment.Exit(0);
        }
        SqliteConnection connection = new SqliteConnection($"Data Source={databaseFilePath}");



        if (args.Length == 0)
        {
            RunGui(connection);
        }
        else if (args[0] == "generator")
        {
            DataGenerator generator = new DataGenerator();
            generator.Run(connection);
        }
        else if (args[0] == "backup")
        {
            string backupPath = args[1];
            File.Copy(databaseFilePath, backupPath);
            Console.WriteLine($"Backup saved in {backupPath}. To load it, type 'dotnet run load {backupPath}'");
        }
        else if (args[0] == "load")
        {
            SqliteConnection newConnection = new SqliteConnection($"Data Source={args[1]}");
            RunGui(newConnection);
        }
        else if (args[0] == "movie")
        {
            MovieRepository movRep = new MovieRepository(connection);
            var all = movRep.GetAll();
            List<int> years = new List<int>();
            for (int i = 0; i < all.Count; i++)
            {
                years.Add(all[i].releaseDate.Year);
            }
            StatisticAnalyzer.BasicPlot(years);
            StatisticAnalyzer.StatisticDataGetter(years);
        }
        else if (args[0] == "actor")
        {
            ActorRepository repo = new ActorRepository(connection);
            var all = repo.GetAll();
            List<int> ages = new List<int>();
            for (int i = 0; i < all.Count; i++)
            {
                ages.Add(all[i].age);
            }
                StatisticAnalyzer.BasicPlot(ages);
            StatisticAnalyzer.StatisticDataGetter(ages);
        }
        else
        {
            Console.WriteLine(@"If you want to run data generator, type 'dotnet run generator',
                and if you want to run GUI program, type 'dotnet run'");
        }


    }
    static void RunGui(SqliteConnection connection)
    {
        MovieRepository movieRepository = new MovieRepository(connection);
        ActorRepository actorRepository = new ActorRepository(connection);
        ReviewRepository reviewRepository = new ReviewRepository(connection);
        UserRepository userRepository = new UserRepository(connection);
        MovieActorRepository movieActorRepository = new MovieActorRepository(connection);


        Application.Init();
        MainWindow mainWindow = new MainWindow(movieRepository, actorRepository, reviewRepository, userRepository, movieActorRepository);
        Application.Run(mainWindow);
    }

}
public static class StatisticAnalyzer
{
    public static void BasicPlot(List<int> numbers)
    {
        double[] dataY = new double[numbers.Count];
        numbers.Sort();
        double[] dataX = new double[numbers.Count];
        for (int i = 0; i < dataY.Length; i++)
        {
            dataX[i] = numbers[i];
            int amount = 0;
            for (int j = 0; j < dataY.Length; j++)
            {
                if (dataX[i] == dataX[j])
                {
                    amount++;
                }
            }
            dataY[i] = amount;
        }

        Random r = new Random();
        ScottPlot.Plot plt = new ScottPlot.Plot(400, 300);

        plt.AddScatter(dataX, dataY);
        plt.SaveFig($"{r.Next(1, 1000000)}.png");

    }
    public static void StatisticDataGetter(List<int> numbers)
    {
        Random r = new Random();
        ScottPlot.Plot plt = new ScottPlot.Plot(400, 300);
        plt.Title("Average, Median and Mode");
        plt.AddSignal(new double[] {GetAverage(numbers), Median(numbers), Mode(numbers)});  
        plt.SaveFig($"{r.Next(1, 1000000)}.png");
    }
    private static double Median(List<int> numbers)
    {
        numbers.Sort();
        return numbers[numbers.Count/2];
    }
    static double Mode(List<int> numbers)
    {

        Dictionary<double, int> dict = new Dictionary<double, int>();
        foreach(double elem in numbers)
        {
            if (dict.ContainsKey(elem))
                dict[elem]++;
            else
                dict[elem] = 1;
        }
 
        int maxCount = 0;
        double mode = Double.NaN;
        foreach(double elem in dict.Keys)
        {
            if (dict[elem] > maxCount)
            {
                maxCount = dict[elem];
                mode = elem;
            }
        }
 
        return mode;
    }
    private static int GetSum(List<int> numbers)
    {
        int sum = 0;
        foreach (int num in numbers)
        {
            sum += num;
        }
        return sum;
    }
    private static int GetAverage(List<int> numbers)
    {
        int sum = GetSum(numbers);
        int average = sum / numbers.Count;
        return average;
    }
    private static int GetMin(List<int> numbers)
    {
        int min = int.MaxValue;
        for (int i = 0; i < numbers.Count; i++)
        {
            if (min > numbers[i])
            {
                min = numbers[i];
            }
        }
        return min;
    }
    private static int GetMax(List<int> numbers)
    {
        int max = int.MinValue;
        for (int i = 0; i < numbers.Count; i++)
        {
            if (max < numbers[i])
            {
                max = numbers[i];
            }
        }
        return max;
    }
}
