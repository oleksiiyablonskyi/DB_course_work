using System;
using Terminal.Gui;
using progbase3;
public class CreateReviewDialog : Dialog
{
    public bool canceled;
    protected TextField textInput;
    protected TextField gradeInput;
    protected DateField postedAtInput;
    protected User currentUser;
    protected Movie currentMovie;
    public CreateReviewDialog(User currentUser, Movie currentMovie)
    {
        this.currentUser = currentUser;
        this.currentMovie = currentMovie;
        this.Title = "Create review";

        Button okBtn = new Button("OK");
        Button cancelBtn = new Button("Cancel");
        okBtn.Clicked += OnCreateDialogSubmitted;
        cancelBtn.Clicked += OnCreateDialogCanceled;
        this.AddButton(cancelBtn);
        this.AddButton(okBtn);

        int rightColumnX = 20;

        Label textLbl = new Label(2, 2, "Text:");
        textInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(textLbl),
            Width = 40,

        };
        this.Add(textLbl, textInput);


        Label gradeLbl = new Label(2, 4, "Grade:");
        gradeInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(gradeLbl),
            Width = 40,

        };
        this.Add(gradeLbl, gradeInput);


        Label postedAtLbl = new Label(2, 6, "Posted:");
        postedAtInput = new DateField(DateTime.Now)
        {
            X = rightColumnX,
            Y = Pos.Top(postedAtLbl),
            Width = 40,
            IsShortFormat = false,
            ReadOnly = true,
        };
        postedAtInput.Date = DateTime.Now;
        this.Add(postedAtLbl, postedAtInput);



    }
    public Review GetReview()
    {
        return new Review()
        {
            text = textInput.Text.ToString(),
            grade = int.Parse(gradeInput.Text.ToString()),
            postedAt = DateTime.Parse(postedAtInput.Text.ToString()),
            author = currentUser,
            movie = currentMovie,
        };
    }
    private void OnCreateDialogCanceled()
    {
        this.canceled = true;
        Application.RequestStop();
    }
    private void OnCreateDialogSubmitted()
    {
        if (!int.TryParse(gradeInput.Text.ToString(), out int grade) || grade < 1 || grade > 10)
        {
            int response = MessageBox.ErrorQuery("ERROR", "Max grade should be integer from 1 to 10!", "OK");
            if (response == 0)
            {
                return;
            }

        }
        this.canceled = false;
        Application.RequestStop();
    }
}
