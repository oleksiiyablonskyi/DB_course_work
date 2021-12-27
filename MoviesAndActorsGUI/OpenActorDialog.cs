using System;
using Terminal.Gui;
using Microsoft.Data.Sqlite;
using progbase3;
class OpenActorDialog : Dialog
{
    protected Actor actor;
    public bool deleted;
    public bool updated;
    protected TextField fullnameInput;
    public SqliteConnection connection;
    protected TextField ageInput;
    protected TextField genderInput;
    protected User currentUser;
    protected Button moviesButton;
    public OpenActorDialog(User currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "Open Actor";

        Button backBtn = new Button("Back");
        backBtn.Clicked += OnOpenDialogBack;
        this.AddButton(backBtn);

        int rightColumnX = 20;

        Label fullnameLbl = new Label(2, 2, "Fullname:");
        fullnameInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(fullnameLbl),
            Width = 40,
            ReadOnly = true,

        };
        this.Add(fullnameLbl, fullnameInput);


        Label ageLbl = new Label(2, 4, "Age:");
        ageInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(ageLbl),
            Width = 40,
            ReadOnly = true,

        };
        this.Add(ageLbl, ageInput);


        Label genderLbl = new Label(2, 6, "Gender:");
        genderInput = new TextField()
        {
            X = rightColumnX,
            Y = Pos.Top(genderLbl),
            Width = 40,
            ReadOnly = true,
        };
        this.Add(genderLbl, genderInput);

        Button editButton = new Button(2, 12, "Edit");
        editButton.Clicked += OnActorEdit;
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
        deleteButton.Clicked += OnActorDelete;
        this.Add(deleteButton);

        Label userLbl = new Label($"You logged as {currentUser.fullname}");
        this.Add(userLbl);


    }
    public void MoviesSetButton()
    {
        moviesButton = new Button("Movies")
        {
            Y = 1,
        };
        if (actor == null) moviesButton.Visible = false;
        this.Add(moviesButton);
        moviesButton.Clicked += OnMovies;
    }
    public void OnMovies()
    {
        try
        {
            MoviesOfTheActor win = new MoviesOfTheActor(currentUser, actor) { connection = connection };
            win.SetRepository(new MovieRepository(connection));
            Application.Run(win);
        }
        catch
        {
            MessageBox.ErrorQuery("ERROR", "Can not do this, try to use another way to see the data.", "OK");
        }

    }
    public void SetActor(Actor actor)
    {
        this.actor = actor;
        this.fullnameInput.Text = actor.fullName;
        this.ageInput.Text = actor.age.ToString();
        this.genderInput.Text = actor.gender;
        MoviesSetButton();

    }
    private void OnOpenDialogBack()
    {
        Application.RequestStop();
    }
    private void OnActorEdit()
    {
        EditActorDialog dialog = new EditActorDialog(currentUser);
        dialog.SetActor(this.actor);
        Application.Run(dialog);
        if (!dialog.canceled)
        {
            Actor updatedActor = dialog.GetActor();
            this.updated = true;
            this.SetActor(updatedActor);
        }
    }
    private void OnActorDelete()
    {
        int index = MessageBox.Query("Delete actor", "Are you sure?", "No", "Yes");
        if (index == 1)
        {
            this.deleted = true;
            this.updated = false;
            Application.RequestStop();
        }
    }
    public Actor GetActor()
    {
        return this.actor;
    }
}
