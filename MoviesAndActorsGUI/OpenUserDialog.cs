using System;
using Terminal.Gui;
using progbase3;
class OpenUserDialog : Dialog
{
    protected User user;
    public bool deleted;
    public bool updated;
    protected TextField fullnameInput;
    protected TextField nicknameInput;
    protected TextField isModeratorInput;
    protected User currentUser;
    public OpenUserDialog(User currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "Open user";

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


        Label nicknameLbl = new Label(2, 4, "Username:");
        nicknameInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(nicknameLbl),
            Width = 40,
            ReadOnly = true,

        };
        this.Add(nicknameLbl, nicknameInput);


        Label isModeratorLbl = new Label(2, 6, "Is Moderator:");
        isModeratorInput = new TextField()
        {
            X = rightColumnX,
            Y = Pos.Top(isModeratorLbl),
            Width = 40,
            ReadOnly = true,


        };
        this.Add(isModeratorLbl, isModeratorInput);

        Button editButton = new Button(2, 12, "Edit");
        editButton.Clicked += OnUserEdit;
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
        deleteButton.Clicked += OnUserDelete;
        this.Add(deleteButton);

        Label userLbl = new Label($"You logged as {currentUser.fullname}");
        this.Add(userLbl);


    }
    public void SetUser(User user)
    {
        this.user = user;
        this.fullnameInput.Text = user.fullname;
        this.nicknameInput.Text = user.nickname;
        this.isModeratorInput.Text = user.isModerator.ToString();

    }
    private void OnOpenDialogBack()
    {
        Application.RequestStop();
    }
    private void OnUserEdit()
    {
        EditUserDialog dialog = new EditUserDialog(currentUser);
        dialog.SetUser(this.user);
        Application.Run(dialog);
        if (!dialog.canceled)
        {
            User updatedUser = dialog.GetUser();
            this.updated = true;
            this.SetUser(updatedUser);
        }
    }
    private void OnUserDelete()
    {
        int index = MessageBox.Query("Delete user", "Are you sure?", "No", "Yes");
        if (index == 1)
        {
            this.deleted = true;
            this.updated = false;
            Application.RequestStop();
        }
    }
    public User GetUser()
    {
        return this.user;
    }
}
