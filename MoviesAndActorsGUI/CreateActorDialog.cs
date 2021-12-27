using System;
using Terminal.Gui;
using progbase3;
public class CreateActorDialog : Dialog
{
    public bool canceled;
    protected TextField fullnameInput;
    protected TextField ageInput;
    protected TextField genderInput;
    protected User currentUser;
    protected Movie currentMovie;
    public CreateActorDialog(User currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "Create actor";

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


        Label ageLbl = new Label(2, 4, "Age:");
        ageInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(ageLbl),
            Width = 40,

        };
        this.Add(ageLbl, ageInput);


        Label genderLbl = new Label(2, 6, "Gender:");
        genderInput = new TextField()
        {
            X = rightColumnX,
            Y = Pos.Top(genderLbl),
            Width = 40,
        };
        this.Add(genderLbl, genderInput);



    }
    public Actor GetActor()
    {
        return new Actor()
        {
            fullName = fullnameInput.Text.ToString(),
            age = int.Parse(ageInput.Text.ToString()),
            gender = genderInput.Text.ToString(),

        };
    }
    private void OnCreateDialogCanceled()
    {
        this.canceled = true;
        Application.RequestStop();
    }
    private void OnCreateDialogSubmitted()
    {
        if (!int.TryParse(ageInput.Text.ToString(), out int age))
        {
            int response = MessageBox.ErrorQuery("ERROR", "Age should be integer!", "OK");
            if (response == 0)
            {
                return;
            }

        }
        this.canceled = false;
        Application.RequestStop();
    }
}
