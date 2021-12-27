using progbase3;
public class EditReviewDialog : CreateReviewDialog
{
    public EditReviewDialog(User currentUser, Movie currentMovie) : base(currentUser, currentMovie)
    {
        this.currentUser = currentUser;

        this.Title = "Edit review";
    }
    public void SetReview(Review review)
    {
        this.textInput.Text = review.text;
        this.gradeInput.Text = review.grade.ToString();
        this.postedAtInput.Text = review.postedAt.ToShortDateString();
    }
}
