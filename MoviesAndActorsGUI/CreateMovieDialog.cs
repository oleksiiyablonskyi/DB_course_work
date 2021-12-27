using System;
using Terminal.Gui;
using progbase3;
public class CreateMovieDialog : Dialog
{
    public bool canceled;
    protected TextField nameInput;
    protected TextField genreInput;
    protected DateField dateInput;
    protected User currentUser;
    public CreateMovieDialog(User currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "Create movie";

        Button okBtn = new Button("OK");
        Button cancelBtn = new Button("Cancel");
        okBtn.Clicked += OnCreateDialogSubmitted;
        cancelBtn.Clicked += OnCreateDialogCanceled;
        this.AddButton(cancelBtn);
        this.AddButton(okBtn);

        int rightColumnX = 20;

        Label nameLbl = new Label(2, 2, "Name:");
        nameInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(nameLbl),
            Width = 40,

        };
        this.Add(nameLbl, nameInput);


        Label genreLbl = new Label(2, 4, "Genre:");
        genreInput = new TextField("")
        {
            X = rightColumnX,
            Y = Pos.Top(genreLbl),
            Width = 40,

        };
        this.Add(genreLbl, genreInput);


        Label dateLbl = new Label(2, 6, "Release date:");
        dateInput = new DateField()
        {
            X = rightColumnX,
            Y = Pos.Top(dateLbl),
            Width = 40,
            IsShortFormat = false,
        };
        this.Add(dateLbl, dateInput);



    }
    public Movie GetMovie()
    {
        return new Movie()
        {
            name = nameInput.Text.ToString(),
            genre = genreInput.Text.ToString(),
            releaseDate = DateTime.Parse(dateInput.Text.ToString()),
        };
    }
    private void OnCreateDialogCanceled()
    {
        this.canceled = true;
        Application.RequestStop();
    }
    private void OnCreateDialogSubmitted()
    {
        this.canceled = false;
        Application.RequestStop();
    }
}
