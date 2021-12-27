using Terminal.Gui;
using System.Collections.Generic;
using progbase3;
public class MoviesMainWindow : Window
{
    private int pageLength = 10;
    private int page = 1;
    private Button prevPage;
    private Button nextPage;
    protected Label totalPagesLabel;
    protected Label pageLabel;
    protected MovieRepository repo;
    protected ListView listV;
    protected User currentUser;
    public MoviesMainWindow(User currentUser)
    {
        this.currentUser = currentUser;
        MenuBar menu = new MenuBar(new MenuBarItem[] {
           new MenuBarItem ("_File", new MenuItem [] {
                new MenuItem ("_Quit", "", OnQuit),
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

        this.Add(prevPage, pageLabel, totalPagesLabel, nextPage, backBtn);

        FrameView frameView = new FrameView("Movies")
        {
            X = 2,
            Y = 8,
            Width = Dim.Fill() - 4,
            Height = pageLength + 2,

        };
        frameView.Add(listV);
        this.Add(frameView);
        Button createNewMovieButton = new Button(2, 4, "Add new movie");
        if (!currentUser.isModerator)
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
        this.ShowCurrentPage();
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
        this.ShowCurrentPage();
        if (!dialog.canceled)
        {
            Movie movie = dialog.GetMovie();
            long movieId = repo.Insert(movie);
            movie.id = movieId;
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
        this.ShowCurrentPage();

        if (dialog.deleted)
        {
            bool deleteResult = repo.DeleteById(movie.id);
            if (deleteResult)
            {
                long pages = repo.GetTotalPages();
                if (page > pages && page > 1)
                {
                    page -= 1;
                    this.ShowCurrentPage();
                }
                //listV.SetSource(repo.GetPage(page));
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
            this.ShowCurrentPage();
        }

    }
    public void SetRepository(MovieRepository repository)
    {
        this.repo = repository;
        ShowCurrentPage();
    }
    private void ShowCurrentPage()
    {
        this.pageLabel.Text = page.ToString();
        this.totalPagesLabel.Text = repo.GetTotalPages().ToString();
        this.listV.SetSource(repo.GetPage(page));

        prevPage.Visible = page != 1;
        nextPage.Visible = page != repo.GetTotalPages();

        if (repo.GetAll().Count == 0)
        {
            this.pageLabel.Text = "No movies";
            this.totalPagesLabel.Text = " yet.";
            prevPage.Visible = false;
            nextPage.Visible = false;
        }


    }


}

