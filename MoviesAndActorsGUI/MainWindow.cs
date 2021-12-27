using System.Collections.Generic;
using Terminal.Gui;
using progbase3;


public class MainWindow : Window
{
    protected MovieRepository movieRepo;
    protected ActorRepository actorRepo;
    protected ReviewRepository reviewRepo;
    protected UserRepository userRepo;
    protected MovieActorRepository movieActorRepo;
    protected User currentUser;
    private Label userLbl;
    protected ListView listV;
    public MainWindow(MovieRepository movieRepo, ActorRepository actorRepo, ReviewRepository reviewRepo, UserRepository userRepo, MovieActorRepository movieActorRepo)
    {
        this.movieRepo = movieRepo;
        this.actorRepo = actorRepo;
        this.reviewRepo = reviewRepo;
        this.userRepo = userRepo;
        this.movieActorRepo = movieActorRepo;

        Log();

        this.Title = "Films and actors";
        Label whatToDo = new Label("What do you want to see?")
        {
            X = Pos.Center(),
            Y = Pos.Center() - 6,
        };
        this.Add(whatToDo);
        Button moviesButton = new Button("Movies")
        {
            X = Pos.Center() - 17,
            Y = Pos.Center(),
        };
        Button actorsButton = new Button("Actors")
        {
            X = Pos.Right(moviesButton) + 2,
            Y = Pos.Center(),
        };
        Button reviewsButton = new Button("Reviews")
        {
            X = Pos.Right(actorsButton) + 2,
            Y = Pos.Center(),
        };
        Button usersButton = new Button("Users")
        {
            X = Pos.Right(actorsButton) + 2,
            Y = Pos.Center(),
        };
        this.Add(moviesButton, actorsButton, /*reviewsButton,*/ usersButton);

        Button logOutButton = new Button("Log Out")
        {
            X = Pos.Left(userLbl),
            Y = Pos.Bottom(userLbl),
        };
        this.Add(logOutButton);


        moviesButton.Clicked += OnMovies;
        actorsButton.Clicked += OnActors;
        /*reviewsButton.Clicked += OnReviews;*/
        usersButton.Clicked += OnUsers;
        logOutButton.Clicked += OnLogOut;

    }
    private void Log()
    {
        Authentification.RegistrationAndAuthorization dialog = new Authentification.RegistrationAndAuthorization(userRepo);
        Application.Run(dialog);
        currentUser = dialog.GetUser();
        userLbl = new Label($"You logged as {currentUser.fullname}");
        this.Add(userLbl);
    }
    private void OnLogOut()
    {
        int response = MessageBox.Query("Log out", "Are you sure you want to log out?", "Cancel", "Yes");
        if (response == 0)
        {
            return;
        }
        else
        {
            this.Remove(userLbl);
            Log();
        }
    }
    private void OnMovies()
    {
        MoviesMainWindow win = new MoviesMainWindow(currentUser);
        win.SetRepository(movieRepo);
        Application.Run(win);
    }
    private void OnActors()
    {
        ActorsMainWindow win = new ActorsMainWindow(currentUser);
        win.SetRepository(actorRepo);
        Application.Run(win);
    }
    /*private void OnReviews()
    {
        ReviewsMainWindow win = new ReviewsMainWindow(currentUser);
        win.SetRepository(reviewRepo);
        Application.Run(win);

    }*/
    private void OnUsers()
    {
        UsersMainWindow win = new UsersMainWindow(currentUser);
        win.SetRepository(userRepo);
        Application.Run(win);

    }

}

