using System;
using System.Collections.Generic;
namespace progbase3
{
    public class User
    {
        public long id;
        public string fullname;
        public string nickname;
        public bool isModerator;
        public List<Review> reviews;
        public string password;

        public User(long id, string fullname, string nickname, bool isModerator)
        {
            this.id = id;
            this.fullname = fullname;
            this.nickname = nickname;
            this.isModerator = isModerator;
        }
        public User(string fullname, string nickname, string password)
        {
            this.fullname = fullname;
            this.nickname = nickname;
            this.password = password;
        }
        public User()
        {

        }

        public User(long id, string fullname, string nickname, bool isModerator, string password) : this(id, fullname, nickname, isModerator)
        {
            this.password = password;
        }

        public override string ToString()
        {
            return $"[{id}] {fullname} '{nickname}', moderator: {isModerator}";
        }
    }
    
}
