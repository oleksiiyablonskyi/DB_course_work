using System;
using Terminal.Gui;
using Microsoft.Data.Sqlite;
using progbase3;
class OpenMovieDialog : Dialog
{
    protected Movie movie;
    public bool deleted;
    public bool updated;
    public SqliteConnection connection;
    protected TextField nameInput;
    protected TextField genreInput;
    protected DateField dateInput;
    protected User currentUser;
    public OpenMovieDialog(User currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "Open movie";

        Button backBtn = new Button("Back");
        backBtn.Clicked += OnOpenDialogBack;
        this.AddButton(backBtn);

        int rightColumnX = 20;

        Label nameLbl = new Label(2, 2, "Name:");
        nameInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(nameLbl),
            Width = 40,
            ReadOnly = true,

        };
        this.Add(nameLbl, nameInput);


        Label genreLbl = new Label(2, 4, "Genre:");
        genreInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(genreLbl),
            Width = 40,
            ReadOnly = true,

        };
        this.Add(genreLbl, genreInput);


        Label dateLbl = new Label(2, 6, "Release date:");
        dateInput = new DateField()
        {
            X = rightColumnX,
            Y = Pos.Top(dateLbl),
            Width = 40,
            IsShortFormat = false,
            ReadOnly = true,
        };
        this.Add(dateLbl, dateInput);

        Button editButton = new Button(2, 12, "Edit");
        editButton.Clicked += OnMovieEdit;
        this.Add(editButton);

        Button deleteButton = new Button("Delete")
        {
            X = Pos.Right(editButton) + 2,
            Y = Pos.Top(editButton),
        };
        if (!currentUser.isModerator)
        {
            deleteButton.Visible = false;
            editButton.Visible = false;
        }
        deleteButton.Clicked += OnMovieDelete;
        this.Add(deleteButton);

        Label userLbl = new Label($"You logged as {currentUser.fullname}");
        this.Add(userLbl);


    }
    private void ReviewsAndActorsSetButtons()
    {
        Button reviewButton = new Button("Reviews")
        {
            Y = 1,
        };
        this.Add(reviewButton);
        reviewButton.Clicked += OnReviews;

        Button actorsButton = new Button("Actors")
        {
            X = Pos.Right(reviewButton) + 1,
            Y = 1
        };
        if (movie == null) actorsButton.Visible = false;
        this.Add(actorsButton);
        actorsButton.Clicked += OnActors;
    }
    private void OnActors()
    {
        try
        {
            ActorsOfTheMovie win = new ActorsOfTheMovie(currentUser, movie) { connection = connection };
            win.SetRepository(new ActorRepository(connection));
            Application.Run(win);
        }
        catch
        {
            MessageBox.ErrorQuery("ERROR", "Can not do this, try to use another way to see the data.", "OK");
        }
    }
    private void OnReviews()
    {
        ReviewsMainWindow win = new ReviewsMainWindow(currentUser, this.movie);
        win.SetRepository(new ReviewRepository(connection));
        Application.Run(win);

    }
    public void SetMovie(Movie movie)
    {
        this.movie = movie;
        this.nameInput.Text = movie.name;
        this.genreInput.Text = movie.genre;
        this.dateInput.Text = movie.releaseDate.ToShortDateString();
        ReviewsAndActorsSetButtons();
    }
    private void OnOpenDialogBack()
    {
        Application.RequestStop();
    }
    private void OnMovieEdit()
    {
        EditMovieDialog dialog = new EditMovieDialog(currentUser);
        dialog.SetMovie(this.movie);
        Application.Run(dialog);
        if (!dialog.canceled)
        {
            Movie updatedMovie = dialog.GetMovie();
            this.updated = true;
            this.SetMovie(updatedMovie);
        }
    }
    private void OnMovieDelete()
    {
        int index = MessageBox.Query("Delete movie", "Are you sure?", "No", "Yes");
        if (index == 1)
        {
            this.deleted = true;
            this.updated = false;
            Application.RequestStop();
        }
    }
    public Movie GetMovie()
    {
        return this.movie;
    }
}
