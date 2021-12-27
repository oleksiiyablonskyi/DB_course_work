using Terminal.Gui;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using progbase3;
public class ActorsOfTheMovie : Window
{
    private int pageLength = 10;
    private int page = 1;
    private Button prevPage;
    private Button nextPage;
    protected Label totalPagesLabel;
    protected Label pageLabel;
    protected ActorRepository repo;
    public SqliteConnection connection;
    protected ListView listV;
    protected User currentUser;
    protected Movie currentMovie;
    public ActorsOfTheMovie(User currentUser, Movie currentMovie)
    {
        this.currentUser = currentUser;
        this.currentMovie = currentMovie;
        MenuBar menu = new MenuBar(new MenuBarItem[] {
           new MenuBarItem ("_File", new MenuItem [] {
                new MenuItem ("_Quit", "", OnQuit),
           }),
           new MenuBarItem ("_Help", new MenuItem [] {
               new MenuItem ("_About", "", OnAbout)
           }),
       });
        this.Add(menu);
        this.Title = "Actors db";
        listV = new ListView(new List<Movie>())
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        listV.OpenSelectedItem += OnOpenActor;

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

        this.Add(/*prevPage, pageLabel, totalPagesLabel, nextPage,*/ backBtn);

        FrameView frameView = new FrameView($"Actors of '{currentMovie.name}'")
        {
            X = 2,
            Y = 8,
            Width = Dim.Fill() - 4,
            Height = pageLength + 2,

        };
        frameView.Add(listV);
        this.Add(frameView);
        Button createNewActorButton = new Button(2, 4, "Add new actor");
        if (!currentUser.isModerator)
        {
            createNewActorButton.Visible = false;
        }
        createNewActorButton.Clicked += OnCreateButtonClicked;
        this.Add(createNewActorButton);
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
        CreateActorDialog dialog = new CreateActorDialog(currentUser);


        Application.Run(dialog);

        if (!dialog.canceled)
        {
            Actor actor = dialog.GetActor();
            long actorId = repo.Insert(actor);
            actor.id = actorId;
            MovieActorRepository movActRepo = new MovieActorRepository(connection);
            movActRepo.Insert(new MovieActor(this.currentMovie.id, actorId));
            this.ShowCurrentPage();
            ListViewItemEventArgs args = new ListViewItemEventArgs(0, actor);
            OnOpenActor(args);
        }
    }
    protected void OnOpenActor(ListViewItemEventArgs args)
    {
        Actor actor = (Actor)args.Value;
        OpenActorDialog dialog = new OpenActorDialog(currentUser);
        dialog.SetActor(actor);
        Application.Run(dialog);

        MovieActorRepository movActRep = new MovieActorRepository(connection);
        long idToDelete = movActRep.GetId(this.currentMovie.id, actor.id);

        if (dialog.deleted)
        {
            bool deleteResult = repo.DeleteById(actor.id);
            deleteResult = deleteResult && movActRep.DeleteById(idToDelete);
            if (deleteResult)
            {
                this.ShowCurrentPage();
            }
            else
            {
                MessageBox.ErrorQuery("Delete actor", "Can not delete the actor", "OK");
            }
            this.ShowCurrentPage();
        }
        if (dialog.updated)
        {
            bool updateResult = repo.Update(actor.id, dialog.GetActor());
            if (updateResult)
            {
                this.ShowCurrentPage();
            }
            else
            {
                MessageBox.ErrorQuery("Update actor", "Can not update the actor", "OK");
            }
        }

    }
    public void SetRepository(ActorRepository repository)
    {
        this.repo = repository;
        ShowCurrentPage();
    }
    private void ShowCurrentPage()
    {
        MovieRepository movRep = new MovieRepository(connection);
        this.listV.SetSource(movRep.MovieActors(this.currentMovie.id));
    }


}

