using progbase3;
public class EditActorDialog : CreateActorDialog
{
    public EditActorDialog(User currentUser) : base(currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "Edit actor";
    }
    public void SetActor(Actor actor)
    {
        this.fullnameInput.Text = actor.fullName;
        this.ageInput.Text = actor.age.ToString();
        this.genderInput.Text = actor.gender;
    }
}
