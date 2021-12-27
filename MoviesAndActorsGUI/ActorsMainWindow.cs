using Terminal.Gui;
using System.Collections.Generic;
using progbase3;
public class ActorsMainWindow : Window
{
    private int pageLength = 10;
    private int page = 1;
    private Button prevPage;
    private Button nextPage;
    protected Label totalPagesLabel;
    protected Label pageLabel;
    protected ActorRepository repo;
    protected ListView listV;
    protected User currentUser;
    public ActorsMainWindow(User currentUser)
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

        this.Add(prevPage, pageLabel, totalPagesLabel, nextPage, backBtn);

        FrameView frameView = new FrameView($"Actors")
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
            this.ShowCurrentPage();
            ListViewItemEventArgs args = new ListViewItemEventArgs(0, actor);
            OnOpenActor(args);
        }
    }
    protected void OnOpenActor(ListViewItemEventArgs args)
    {
        Actor actor = (Actor)args.Value;
        OpenActorDialog dialog = new OpenActorDialog(currentUser) { connection = repo.GetConnection() };
        dialog.SetActor(actor);
        Application.Run(dialog);

        if (dialog.deleted)
        {
            bool deleteResult = repo.DeleteById(actor.id);
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
        this.pageLabel.Text = page.ToString();
        this.totalPagesLabel.Text = repo.GetTotalPages().ToString();
        this.listV.SetSource(repo.GetPage(page));

        prevPage.Visible = page != 1;
        nextPage.Visible = page != repo.GetTotalPages();

        if (repo.GetAll().Count == 0)
        {
            this.pageLabel.Text = "No actors";
            this.totalPagesLabel.Text = " yet.";
            prevPage.Visible = false;
            nextPage.Visible = false;
        }


    }


}

