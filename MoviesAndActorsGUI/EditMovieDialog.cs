using progbase3;
public class EditMovieDialog : CreateMovieDialog
{
    public EditMovieDialog(User currentUser) : base(currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "Edit movie";
    }
    public void SetMovie(Movie movie)
    {
        this.nameInput.Text = movie.name;
        this.genreInput.Text = movie.genre;
        this.dateInput.Text = movie.releaseDate.ToShortDateString();
    }
}
