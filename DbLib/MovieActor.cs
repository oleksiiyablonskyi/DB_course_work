namespace progbase3
{
    public class MovieActor
    {
        public long id;
        public long movieId;
        public long actorId;

        public MovieActor(long movieId, long actorId)
        {
            this.movieId = movieId;
            this.actorId = actorId;
        }
        public MovieActor()
        {

        }

        public MovieActor(long id, long movieId, long actorId) : this(id, movieId)
        {
            this.actorId = actorId;
        }
    }
}
