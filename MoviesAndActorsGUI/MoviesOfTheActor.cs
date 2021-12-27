using Terminal.Gui;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using progbase3;

public class MoviesOfTheActor : Window
{
    private int pageLength = 10;
    private int page = 1;
    private Button prevPage;
    private Button nextPage;
    protected Label totalPagesLabel;
    protected Label pageLabel;
    protected MovieRepository repo;
    public SqliteConnection connection;
    protected ListView listV;
    protected User currentUser;
    protected Actor currentActor;
    public MoviesOfTheActor(User currentUser, Actor currentActor)
    {
        this.currentUser = currentUser;
        this.currentActor = currentActor;
        MenuBar menu = new MenuBar(new MenuBarItem[] {
           new MenuBarItem ("_File", new MenuItem [] {
               new MenuItem("_Export...", "", OnExportClicked), new MenuItem("_Import...", "", OnImportClicked), new MenuItem ("_Quit", "", OnQuit),
           }),
           new MenuBarItem ("_Help", new MenuItem [] {
               new MenuItem ("_About", "", OnAbout)
           }),
       });
        this.Add(menu);
        this.Title = "Movies db";
        listV = new ListView(new List<Movie>())
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        listV.OpenSelectedItem += OnOpenMovie;

        prevPage = new Button(2, 6, "<-");
        pageLabel = new Label("?")
        {
            X = Pos.Right(prevPage) + 3,
            Y = Pos.Top(prevPage),
            Width = 8,

        };
        totalPagesLabel = new Label("?")
        {
            X = Pos.Right(pageLabel) + 1,
            Y = Pos.Top(prevPage),
            Width = 5,
        };
        nextPage = new Button("->")
        {
            X = Pos.Right(totalPagesLabel) + 1,
            Y = Pos.Top(prevPage),
        };

        Button backBtn = new Button("Back")
        {
            X = Pos.Right(nextPage) + 1,
            Y = Pos.Top(prevPage),
        };
        Label userLbl = new Label($"You logged as {currentUser.fullname}")
        {
            Y = 1,
        };
        this.Add(userLbl);

        nextPage.Clicked += OnNextPage;
        prevPage.Clicked += OnPrevPage;

        backBtn.Clicked += OnQuit;

        this.Add(/*prevPage, pageLabel, totalPagesLabel, nextPage, */backBtn);

        FrameView frameView = new FrameView($"Movies with {currentActor.fullName}")
        {
            X = 2,
            Y = 8,
            Width = Dim.Fill() - 4,
            Height = pageLength + 2,

        };
        frameView.Add(listV);
        this.Add(frameView);
        Button createNewMovieButton = new Button(2, 4, "Add new movie");
        if (currentUser.isModerator)
        {
            createNewMovieButton.Visible = false;
        }
        createNewMovieButton.Clicked += OnCreateButtonClicked;
        this.Add(createNewMovieButton);
    }
    private void OnQuit()
    {
        Application.RequestStop();
    }
    private void OnExportClicked()
    {
        ActorRepository actorRepository = new ActorRepository(connection);
        List<Movie> moviesToExport = actorRepository.ActorMovies(currentActor.id);

        OpenDialog dialog = new OpenDialog("Open directory", "Open?");
        dialog.CanChooseDirectories = true;
        dialog.CanChooseFiles = false;

        Application.Run(dialog);

        if (!dialog.Canceled)
        {
            NStack.ustring filePath = dialog.FilePath;
            //fileLabel.Text = filePath;
            filePath += $"/{currentActor.fullName} movies.xml";
            ExportAndImport.Export(filePath.ToString(), moviesToExport);
        }
        else
        {
            //fileLabel.Text = "not selected.";
        }

    }
    private void OnImportClicked()
    {
        OpenDialog dialog = new OpenDialog("Open XML file", "Open?");
        // dialog.DirectoryPath = ...

        Application.Run(dialog);

        if (!dialog.Canceled)
        {
            NStack.ustring ufilePath = dialog.FilePath;
            string filePath = ufilePath.ToString();
            string extension = filePath[(filePath.Length - 4)..filePath.Length];
            if (File.Exists(filePath) && extension == ".xml")
            {
                try
                {
                    List<Movie> movies = ExportAndImport.Import(filePath);
                    MovieActorRepository movieActorRepository = new MovieActorRepository(connection);
                    foreach (Movie mov in movies)
                    {
                        long movId = repo.Insert(mov);
                        movieActorRepository.Insert(new MovieActor(movId, this.currentActor.id));
                    }
                }
                catch
                {
                    MessageBox.ErrorQuery("ERROR", "File format is incorrect.", "OK");
                    return;
                }

            }
            else
            {
                MessageBox.ErrorQuery("ERROR", "File format is incorrect.", "OK");
                return;
            }
            //fileLabel.Text = filePath;
        }


    }

    private void OnAbout()
    {
        Window win = new Window("Hello")
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 1,
        };
        Label infoLbl = new Label("This program was deleloped by Oleksii Yablonsky, \r\nstudent of KPI.");
        Button backButton = new Button("Back")
        {
            X = Pos.Left(infoLbl),
            Y = Pos.Bottom(infoLbl) + 2,
        };
        backButton.Clicked += OnQuit;
        win.Add(infoLbl);
        win.Add(backButton);
        Application.Run(win);

    }
    private void OnNextPage()
    {
        long totalPages = repo.GetTotalPages();
        if (page >= totalPages)
        {
            return;
        }
        this.page += 1;
        ShowCurrentPage();
    }
    private void OnPrevPage()
    {
        if (page == 1)
        {
            return;
        }
        this.page -= 1;
        ShowCurrentPage();
    }
    protected void OnCreateButtonClicked()
    {
        CreateMovieDialog dialog = new CreateMovieDialog(currentUser);


        Application.Run(dialog);

        if (!dialog.canceled)
        {
            Movie movie = dialog.GetMovie();
            long movieId = repo.Insert(movie);
            movie.id = movieId;
            MovieActorRepository movActRepo = new MovieActorRepository(connection);
            movActRepo.Insert(new MovieActor(movieId, currentActor.id));
            this.ShowCurrentPage();
            ListViewItemEventArgs args = new ListViewItemEventArgs(0, movie);
            OnOpenMovie(args);
        }
    }
    protected void OnOpenMovie(ListViewItemEventArgs args)
    {
        Movie movie = (Movie)args.Value;
        OpenMovieDialog dialog = new OpenMovieDialog(currentUser) { connection = this.repo.GetConnection() };
        dialog.SetMovie(movie);
        Application.Run(dialog);

        MovieActorRepository movActRep = new MovieActorRepository(connection);
        long idToDelete = movActRep.GetId(movie.id, currentActor.id);


        if (dialog.deleted)
        {
            bool deleteResult = repo.DeleteById(movie.id);
            deleteResult = deleteResult && movActRep.DeleteById(idToDelete);

            if (deleteResult)
            {
                this.ShowCurrentPage();
            }
            else
            {
                MessageBox.ErrorQuery("Delete movie", "Can not delete the movie", "OK");
            }
            this.ShowCurrentPage();
        }
        if (dialog.updated)
        {
            bool updateResult = repo.Update(movie.id, dialog.GetMovie());
            if (updateResult)
            {
                this.ShowCurrentPage();
            }
            else
            {
                MessageBox.ErrorQuery("Update movie", "Can not update the movie", "OK");
            }
        }

    }
    public void SetRepository(MovieRepository repository)
    {
        this.repo = repository;
        ShowCurrentPage();
    }
    private void ShowCurrentPage()
    {
        ActorRepository actorRep = new ActorRepository(connection);
        this.listV.SetSource(actorRep.ActorMovies(currentActor.id));

    }


}

