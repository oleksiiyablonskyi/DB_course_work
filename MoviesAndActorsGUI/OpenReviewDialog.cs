using System;
using Terminal.Gui;
using progbase3;
class OpenReviewDialog : Dialog
{
    protected Review review;
    public bool deleted;
    public bool updated;
    protected TextField textInput;
    protected TextField gradeInput;
    protected DateField postedAtInput;
    protected User currentUser;
    protected Movie currentMovie;
    private Button editButton;
    private Button deleteButton;
    public OpenReviewDialog(User currentUser, Movie currentMovie)
    {
        this.currentUser = currentUser;
        this.currentMovie = currentMovie;
        this.Title = "Open review";

        Button backBtn = new Button("Back");
        backBtn.Clicked += OnOpenDialogBack;
        this.AddButton(backBtn);

        int rightColumnX = 20;

        Label textLbl = new Label(2, 2, "Text:");
        textInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(textLbl),
            Width = 40,
            ReadOnly = true,

        };
        this.Add(textLbl, textInput);


        Label gradeLbl = new Label(2, 4, "Grade:");
        gradeInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(gradeLbl),
            Width = 40,
            ReadOnly = true,

        };
        this.Add(gradeLbl, gradeInput);


        Label postedAtLbl = new Label(2, 6, "Posted:");
        postedAtInput = new DateField()
        {
            X = rightColumnX,
            Y = Pos.Top(postedAtLbl),
            Width = 40,
            IsShortFormat = false,
            ReadOnly = true,
        };
        this.Add(postedAtLbl, postedAtInput);

        editButton = new Button(2, 12, "Edit");
        editButton.Clicked += OnReviewEdit;
        this.Add(editButton);

        deleteButton = new Button("Delete")
        {
            X = Pos.Right(editButton) + 2,
            Y = Pos.Top(editButton),
        };

        deleteButton.Clicked += OnReviewDelete;
        this.Add(deleteButton);


        Label userLbl = new Label($"You logged as {currentUser.fullname}");
        this.Add(userLbl);

    }
    private void CheckButtons()
    {
        if (!currentUser.isModerator && currentUser != review.author)
        {
            deleteButton.Visible = false;
            editButton.Visible = false;
        }
    }
    public void SetReview(Review review)
    {
        this.review = review;
        this.textInput.Text = review.text;
        this.gradeInput.Text = review.grade.ToString();
        this.postedAtInput.Text = review.postedAt.ToShortDateString();
        Label authorLbl = new Label($"Author: {review.author.fullname}")
        {
            X = 2,
            Y = 14,
        };
        Label movieLbl = new Label($"Movie: {review.movie.name}")
        {
            X = 2,
            Y = 15,
        };
        this.Add(authorLbl);
        CheckButtons();

    }
    private void OnOpenDialogBack()
    {
        Application.RequestStop();
    }
    private void OnReviewEdit()
    {
        EditReviewDialog dialog = new EditReviewDialog(currentUser, currentMovie);
        dialog.SetReview(this.review);
        Application.Run(dialog);
        if (!dialog.canceled)
        {
            Review updatedReview = dialog.GetReview();
            this.updated = true;
            this.SetReview(updatedReview);
        }
    }
    private void OnReviewDelete()
    {
        int index = MessageBox.Query("Delete review", "Are you sure?", "No", "Yes");
        if (index == 1)
        {
            this.deleted = true;
            this.updated = false;
            Application.RequestStop();
        }
    }
    public Review GetReview()
    {
        return this.review;
    }
}
