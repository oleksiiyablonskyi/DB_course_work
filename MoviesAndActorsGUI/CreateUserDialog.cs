using System;
using Terminal.Gui;
using progbase3;
public class CreateUserDialog : Dialog
{
    public bool canceled;
    protected TextField fullnameInput;
    protected TextField nicknameInput;
    protected CheckBox isModeratorInput;
    protected User currentUser;
    public CreateUserDialog(User currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "Create user";

        Button okBtn = new Button("OK");
        Button cancelBtn = new Button("Cancel");
        okBtn.Clicked += OnCreateDialogSubmitted;
        cancelBtn.Clicked += OnCreateDialogCanceled;
        this.AddButton(cancelBtn);
        this.AddButton(okBtn);

        int rightColumnX = 20;

        Label fullnameLbl = new Label(2, 2, "Fullname:");
        fullnameInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(fullnameLbl),
            Width = 40,

        };
        this.Add(fullnameLbl, fullnameInput);


        Label nicknameLbl = new Label(2, 4, "Username:");
        nicknameInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(nicknameLbl),
            Width = 40,

        };
        this.Add(nicknameLbl, nicknameInput);


        Label isModeratorLbl = new Label(2, 6, "Is Moderator:");
        isModeratorInput = new CheckBox()
        {
            X = rightColumnX,
            Y = Pos.Top(isModeratorLbl),
            Width = 40,

        };
        this.Add(isModeratorLbl, isModeratorInput);



    }
    public User GetUser()
    {
        return new User()
        {
            fullname = fullnameInput.Text.ToString(),
            nickname = nicknameInput.Text.ToString(),
            isModerator = isModeratorInput.Checked,
        };
    }
    private void OnCreateDialogCanceled()
    {
        this.canceled = true;
        Application.RequestStop();
    }
    private void OnCreateDialogSubmitted()
    {
        //if exists TODO
        this.canceled = false;
        Application.RequestStop();
    }
}
