using System;
using Microsoft.Data.Sqlite;
using progbase3;

public class DataGenerator
{
    private SqliteConnection connection;
    public void Run(SqliteConnection connection)
    {
        this.connection = connection;
        Console.WriteLine("This generator generates random entities with random relations! Enjoy:)");
        while (true)
        {
            Console.WriteLine("command format '{entity to generate} {amount of generating entities}'\r\nEntities: movie, actor, review\r\nType 'quit' to stop the program.");
            Console.Write("Type your command: ");
            string command = Console.ReadLine();
            string[] subcommands = command.Split(' ');
            if (command == "quit" || command == "exit")
            {
                Console.WriteLine("The generator is stopped, goodbye:)");
                break;
            }
            if (subcommands.Length != 2)
            {
                Console.WriteLine($"Command should consist of 2 parts, you wrote {subcommands.Length}");
                continue;
            }
            int quantity = 0;
            if (!int.TryParse(subcommands[1], out quantity))
            {
                Console.WriteLine($"The second part of command is not integer, you wrote {subcommands[1]}");
                continue;
            }
            if (subcommands[0] == "movie")
            {
                GenerateFilms(quantity);
                Console.WriteLine($"Generated {quantity} movies.");
            }
            else if (subcommands[0] == "actor")
            {
                GenerateActors(quantity);
                Console.WriteLine($"Generated {quantity} actors.");
            }
            else if (subcommands[0] == "review")
            {
                GenerateReviews(quantity);
                Console.WriteLine($"Generated {quantity} reviews.");
            }
            else
            {
                Console.WriteLine($"Unknown command '{command}'");
                continue;
            }
        }
    }

    public void GenerateFilms(int quantity)
    {
        string[] namesBeginnings = new string[]
        {
                "Hello ", "Meet ", "Let me introduce you to ", "Midnight ", "Just ", "Dear ", "Dangerous ", "Scary ",
                "Angry ", "Nice ", "Classy ", "Kind ", "Style of ", "Forbidden ", "Some interesting facts about ",
                "Your ", "Forget about ", "Forgive ", "Classical ", "Sweet ", "It is ", "Cry with ", "It's just ",
                "It's all about ", "All about ", "Stop thinking about ", "Lovely ", "Sincerely your`s, ", "Literally ",
                "Friendly ",
        };
        string[] namesEndings = new string[]
        {
                "love", "Paris", "Kyiv", "New York", "Hollywood", "Berlin", "Amsterdam", "London", "friends",
                "Emily", "Sara", "Betty", "Hanna", "Carla", "Tanya", "Sabrina", "Rose", "Veronica", "Diana", "Kate",
                "Andrew", "Matt", "Alex", "Harry", "Thomas", "Lenny", "Michael", "Bryce", "Simon", "Victor", "James",
                "John", "Jeremy",
        };
        string[] genres = new string[]
        {
                "Action", "Comedy", "Drama", "Fantasy", "Horror", "Mystery", "Romance", "Thriller", "Western",
        };
        Random random = new Random();
        MovieRepository movieRepository = new MovieRepository(connection);
        ActorRepository actorRepository = new ActorRepository(connection);
        MovieActorRepository movieActorRepository = new MovieActorRepository(connection);
        
        int actorsAmount = actorRepository.GetAll().Count;
        for (int i = 0; i < quantity; i++)
        {
            string name = namesBeginnings[random.Next(0, namesBeginnings.Length)] + namesEndings[random.Next(0, namesEndings.Length)];
            string genre = genres[random.Next(0, genres.Length)];
            DateTime releaseDate = DateTime.Parse($"{random.Next(1, 29)}.{random.Next(1, 13)}.{random.Next(1950, 2022)}");
            Movie movie = new Movie(name, genre, releaseDate);
            long movieId = movieRepository.Insert(movie);
            movieActorRepository.Insert(new MovieActor(movieId, random.Next(1, actorsAmount)));

        }
    }
    public void GenerateActors(int quantity)
    {
        string[] firstNames = new string[]
        {
                "Emily ", "Sara ", "Betty ", "Hanna ", "Carla ", "Tanya ", "Sabrina ", "Rose ", "Veronica ", "Diana ", "Kate ",
                "Andrew ", "Matt ", "Alex ", "Harry ", "Thomas ", "Lenny ", "Michael ", "Bryce ", "Simon ", "Victor ", "James ",
                "John ", "Jeremy ",
        };
        string[] lastNames = new string[]
        {
                "Smith","Johnson","Williams","Brown","Jones","Garcia","Miller","Davis","Rodriguez","Martinez","Hernandez",
                "Lopez","Gonzalez","Wilson","Anderson","Thomas","Taylor","Moore","Jackson","Martin","Lee","Perez",
                "Thompson","White","Harris","Sanchez","Clark","Ramirez","Lewis","Robinson","Walker","Young","Allen",
                "King","Wright","Scott","Torres","Nguyen","Hill","Flores","Green","Adams","Nelson","Baker","Hall",
                "Rivera","Campbell","Mitchell","Carter","Roberts",
        };
        string[] genders = new string[] { "Male", "Female", "Other" };


        Random random = new Random();
        MovieRepository movieRepository = new MovieRepository(connection);
        ActorRepository actorRepository = new ActorRepository(connection);
        int movieAmount = movieRepository.GetAll().Count;
        MovieActorRepository movieActorRepository = new MovieActorRepository(connection);
        for (int i = 0; i < quantity; i++)
        {
            string name = firstNames[random.Next(0, firstNames.Length)] + lastNames[random.Next(0, lastNames.Length)];
            int age = random.Next(10, 70);
            string gender = genders[random.Next(0, genders.Length)];
            Actor newActor = new Actor(name, age, gender);
            long actorId = actorRepository.Insert(newActor);
            movieActorRepository.Insert(new MovieActor(random.Next(1, movieAmount), actorId));

        }
    }
    public void GenerateReviews(int quantity)
    {
        string[] part1 = new string[]
        {
                "This ", "The ",
        };
        string[] part2 = new string[]
        {
                "film ", "movie ", "pic ", "tape ",
        };
        string[] part3 = new string[]
        {
                "is ", "is really ", "is literally ", "is actually ", "turned out to be ",
        };
        string[] part4 = new string[]
        {
                "very ", "so much ", "",
        };
        string[] part5 = new string[]
        {
                "interesting.", "exciting.", "funny.", "ridiculous.", "unusual", "usual.", "boring.",
                "good.", "bad.", "genial.", "ingenious.", "scary.", "hillarious.", "sad.", "facsinating."
        };
        UserRepository userRepository = new UserRepository(connection);
        MovieRepository movieRepository = new MovieRepository(connection);
        ReviewRepository reviewRepository = new ReviewRepository(connection);
        int userAmount = userRepository.GetAll().Count;
        int moviesAmount = movieRepository.GetAll().Count;
        Random random = new Random();
        for (int i = 0; i < quantity; i++)
        {
            string text = $"{part1[random.Next(0, part1.Length)]}{part2[random.Next(0, part2.Length)]}{part3[random.Next(0, part3.Length)]}{part4[random.Next(0, part4.Length)]}{part5[random.Next(0, part5.Length)]}";
            int grade = random.Next(1, 11);
            DateTime postedAt = DateTime.Parse($"{random.Next(1, 29)}.{random.Next(1, 13)}.{random.Next(2015, 2022)}");
            Review newReview = new Review()
            {
                text = text,
                grade = grade,
                postedAt = postedAt,
                author = userRepository.GetById(random.Next(1, userAmount)),
                movie = movieRepository.GetById(random.Next(1, moviesAmount)),
            };
            reviewRepository.Insert(newReview);
        }

    }
}
