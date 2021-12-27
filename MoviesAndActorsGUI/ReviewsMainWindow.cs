using Terminal.Gui;
using System.Collections.Generic;
using progbase3;
public class ReviewsMainWindow : Window
{
    /*private int pageLength = 10;
    private int page = 1;
    private Button prevPage;
    private Button nextPage;
    protected Label totalPagesLabel;
    protected Label pageLabel;*/
    protected ReviewRepository repo;
    protected ListView listV;
    protected User currentUser;
    protected Movie currentMovie;
    public ReviewsMainWindow(User currentUser, Movie currentMovie)
    {
        this.currentUser = currentUser;
        this.currentMovie = currentMovie;
        MenuBar menu = new MenuBar(new MenuBarItem[] {
           new MenuBarItem ("_File", new MenuItem [] {
                new MenuItem ("_Quit", "", OnQuit),
           }),
           new MenuBarItem ("_Help", new MenuItem [] {
               new MenuItem ("_About", "", OnAbout)
           }),
       });
        this.Add(menu);
        this.Title = $"Reviews of {this.currentMovie.name}";
        listV = new ListView(new List<Review>())
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        listV.OpenSelectedItem += OnOpenReview;

        /*prevPage = new Button(2, 6, "<-");
        pageLabel = new Label("?")
        {
            X = Pos.Right(prevPage) + 3,
            Y = Pos.Top(prevPage),
            Width = 8,

        };
        totalPagesLabel = new Label("?")
        {
            X = Pos.Right(pageLabel) + 1,
            Y = Pos.Top(prevPage),
            Width = 5,
        };
        nextPage = new Button("->")
        {
            X = Pos.Right(totalPagesLabel) + 1,
            Y = Pos.Top(prevPage),
        };*/

        Button backBtn = new Button("Back")
        {
            X = 2,
            Y = 2
        };

        Label userLbl = new Label($"You logged as {currentUser.fullname}")
        {
            Y = 1,
        };
        this.Add(userLbl);

        /*nextPage.Clicked += OnNextPage;
        prevPage.Clicked += OnPrevPage;*/

        backBtn.Clicked += OnQuit;

        this.Add(/*prevPage, pageLabel, totalPagesLabel, nextPage, */backBtn);

        FrameView frameView = new FrameView($"Reviews of {this.currentMovie.name}")
        {
            X = 2,
            Y = 8,
            Width = Dim.Fill() - 4,
            Height = 12,

        };
        frameView.Add(listV);
        this.Add(frameView);
        Button createNewReviewButton = new Button(2, 4, "Add new review");
        createNewReviewButton.Clicked += OnCreateButtonClicked;
        this.Add(createNewReviewButton);
    }
    private void OnQuit()
    {
        Application.RequestStop();
    }

    private void OnAbout()
    {
        Window win = new Window("Hello")
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 1,
        };
        Label infoLbl = new Label("This program was deleloped by Oleksii Yablonsky, \r\nstudent of KPI.");
        Button backButton = new Button("Back")
        {
            X = Pos.Left(infoLbl),
            Y = Pos.Bottom(infoLbl) + 2,
        };
        backButton.Clicked += OnQuit;
        win.Add(infoLbl);
        win.Add(backButton);
        Application.Run(win);

    }
    /*private void OnNextPage()
    {
        long totalPages = repo.GetTotalPages();
        if (page >= totalPages)
        {
            return;
        }
        this.page += 1;
        ShowCurrentPage();
    }
    private void OnPrevPage()
    {
        if (page == 1)
        {
            return;
        }
        this.page -= 1;
        ShowCurrentPage();
    }*/
    protected void OnCreateButtonClicked()
    {
        CreateReviewDialog dialog = new CreateReviewDialog(currentUser, currentMovie);


        Application.Run(dialog);

        if (!dialog.canceled)
        {
            Review review = dialog.GetReview();
            review.author = currentUser;
            long reviewId = repo.Insert(review);
            review.id = reviewId;
            this.ShowCurrentPage();
            ListViewItemEventArgs args = new ListViewItemEventArgs(0, review);
            OnOpenReview(args);
        }
    }
    protected void OnOpenReview(ListViewItemEventArgs args)
    {
        Review review = (Review)args.Value;
        OpenReviewDialog dialog = new OpenReviewDialog(currentUser, currentMovie);
        dialog.SetReview(review);
        Application.Run(dialog);

        if (dialog.deleted)
        {
            bool deleteResult = repo.DeleteById(review.id);
            if (deleteResult)
            {
                long pages = repo.GetTotalPages();
                /*if (page > pages && page > 1)
                {
                    page -= 1;*/
                this.ShowCurrentPage();
                //}
                //listV.SetSource(repo.GetPage(page));
                this.ShowCurrentPage();
            }
            else
            {
                MessageBox.ErrorQuery("Delete review", "Can not delete the review", "OK");
            }
            this.ShowCurrentPage();
        }
        if (dialog.updated)
        {
            bool updateResult = repo.Update(review.id, dialog.GetReview());
            if (updateResult)
            {
                this.ShowCurrentPage();
            }
            else
            {
                MessageBox.ErrorQuery("Update review", "Can not update the review", "OK");
            }
        }

    }
    public void SetRepository(ReviewRepository repository)
    {
        this.repo = repository;
        ShowCurrentPage();
    }
    private void ShowCurrentPage()
    {
        /*this.pageLabel.Text = page.ToString();
        this.totalPagesLabel.Text = repo.GetTotalPages().ToString();*/
        List<Review> list = repo.MovieReviews(currentMovie.id);
        this.listV.SetSource(list);

        int average = 0;
        foreach (Review rev in list)
        {
            average += rev.grade;
        }
        if (list.Count != 0) average /= list.Count;

        Label averageLabel = new Label($"Average grade: {average}")
        {
            X = 2,
            Y = 7

        };
        this.Add(averageLabel);

        /*prevPage.Visible = page != 1;
        nextPage.Visible = page != repo.GetTotalPages();*/

        /*if (repo.GetAll().Count == 0)
        {
            this.pageLabel.Text = "No reviews";
            this.totalPagesLabel.Text = " yet.";
            prevPage.Visible = false;
            nextPage.Visible = false;
        }*/


    }


}

