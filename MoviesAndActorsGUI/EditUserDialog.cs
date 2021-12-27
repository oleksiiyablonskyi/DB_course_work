using progbase3;
public class EditUserDialog : CreateUserDialog
{
    public EditUserDialog(User currentUser) : base(currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "Edit user";
    }
    public void SetUser(User user)
    {
        this.fullnameInput.Text = user.fullname;
        this.nicknameInput.Text = user.nickname;
        this.isModeratorInput.Checked = user.isModerator;
    }
}
