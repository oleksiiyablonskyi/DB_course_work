using Terminal.Gui;
using System.Collections.Generic;
using progbase3;
public class UsersMainWindow : Window
{
    private int pageLength = 10;
    private int page = 1;
    private Button prevPage;
    private Button nextPage;
    protected Label totalPagesLabel;
    protected Label pageLabel;
    protected UserRepository repo;
    protected User currentUser;
    protected ListView listV;
    public UsersMainWindow(User currentUser)
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
        this.Title = "Reviews db";
        listV = new ListView(new List<Movie>())
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        listV.OpenSelectedItem += OnOpenUser;

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

        FrameView frameView = new FrameView("Users")
        {
            X = 2,
            Y = 8,
            Width = Dim.Fill() - 4,
            Height = pageLength + 2,

        };
        frameView.Add(listV);
        this.Add(frameView);
        Button createNewUserButton = new Button(2, 4, "Add new user");
        createNewUserButton.Clicked += OnCreateButtonClicked;
        //this.Add(createNewUserButton);
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
        CreateUserDialog dialog = new CreateUserDialog(currentUser);


        Application.Run(dialog);

        if (!dialog.canceled)
        {
            User user = dialog.GetUser();
            long userId = repo.Insert(user);
            user.id = userId;
            this.ShowCurrentPage();
            ListViewItemEventArgs args = new ListViewItemEventArgs(0, user);
            OnOpenUser(args);
        }
    }
    protected void OnOpenUser(ListViewItemEventArgs args)
    {
        User user = (User)args.Value;
        OpenUserDialog dialog = new OpenUserDialog(currentUser);
        dialog.SetUser(user);
        Application.Run(dialog);

        if (dialog.deleted)
        {
            bool deleteResult = repo.DeleteById(user.id);
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
                MessageBox.ErrorQuery("Delete user", "Can not delete the user", "OK");
            }
            this.ShowCurrentPage();
        }
        if (dialog.updated)
        {
            bool updateResult = repo.Update(user.id, dialog.GetUser());
            if (updateResult)
            {
                this.ShowCurrentPage();
            }
            else
            {
                MessageBox.ErrorQuery("Update user", "Can not update the user", "OK");
            }
        }

    }
    public void SetRepository(UserRepository repository)
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
            this.pageLabel.Text = "No users";
            this.totalPagesLabel.Text = " yet.";
            prevPage.Visible = false;
            nextPage.Visible = false;
        }


    }


}

