using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace progbase3
{
    public class UserRepository
    {
        private SqliteConnection connection;

        public UserRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        public User GetById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            User user = new User();
            if (reader.Read())
            {
                user.id = reader.GetInt64(0);
                user.fullname = reader.GetString(1);
                user.nickname = reader.GetString(2);
                user.isModerator = bool.Parse(reader.GetString(3));
                user.password = reader.GetString(4);

            }
            else
            {
                user = null;
            }
            reader.Close();
            connection.Close();
            return user;
        }
        public bool DeleteById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            long nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged != 0;
        }
        public long Insert(User user)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO users (fullname, nickname, is_moderator, password) 
                VALUES ($fullname, $nickname, $is_moderator, $password);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$fullname", user.fullname);
            command.Parameters.AddWithValue("$nickname", user.nickname);
            command.Parameters.AddWithValue("$is_moderator", user.isModerator.ToString());
            command.Parameters.AddWithValue("$password", user.password);

            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;

        }
        private long GetCount()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users";
            long count = (long)command.ExecuteScalar();
            connection.Close();
            return count;
        }
        public List<User> GetAll()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users";
            SqliteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            while (reader.Read())
            {
                User user = new User(reader.GetInt64(0), reader.GetString(1), reader.GetString(2), bool.Parse(reader.GetString(3)), reader.GetString(4));
                users.Add(user);
            }
            reader.Close();
            connection.Close();
            return users;
        }

        public long GetTotalPages()
        {
            connection.Open();
            const long pageSize = 10;
            long total = (long)Math.Ceiling(this.GetCount() / (double)pageSize);
            connection.Close();
            return total;
        }

        public List<User> GetPage(long pageNumber)
        {
            connection.Open();
            pageNumber -= 1;
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users LIMIT $limit OFFSET $offset";
            command.Parameters.AddWithValue("$limit", 10);
            command.Parameters.AddWithValue("$offset", pageNumber * 10);
            SqliteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            while (reader.Read())
            {
                User user = new User(reader.GetInt64(0), reader.GetString(1), reader.GetString(2), bool.Parse(reader.GetString(3)), reader.GetString(4));
                users.Add(user);
            }
            reader.Close();
            connection.Close();
            return users;
        }
        public bool Update(long taskId, User user)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE users SET fullname = $fullname, nickname = $nickname, is_moderator = $is_moderator, password = $password WHERE id = $id";
            command.Parameters.AddWithValue("$id", taskId);

            command.Parameters.AddWithValue("$fullname", user.fullname);
            command.Parameters.AddWithValue("$nickname", user.nickname);
            command.Parameters.AddWithValue("$is_moderator", user.isModerator.ToString());
            command.Parameters.AddWithValue("$password", user.password);


            long nChanged = command.ExecuteNonQuery();

            connection.Close();
            return nChanged != 0;
        }
        public bool UserExists(string nickname)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE nickname = $nickname";
            command.Parameters.AddWithValue("$nickname", nickname);
            SqliteDataReader reader = command.ExecuteReader();
            bool result = reader.Read();
            connection.Close();
            return result;
        }
        public User GetByUsername(string nickname)
        {
             connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE nickname = $nickname";
            command.Parameters.AddWithValue("$nickname", nickname);
            SqliteDataReader reader = command.ExecuteReader();
            User user = new User();
            if (reader.Read())
            {
                user.id = reader.GetInt64(0);
                user.fullname = reader.GetString(1);
                user.nickname = reader.GetString(2);
                user.isModerator = bool.Parse(reader.GetString(3));
                user.password = reader.GetString(4);
            }
            else
            {
                user = null;
            }
            reader.Close();
            connection.Close();
            return user;
        }
        public List<Review> GetAllReviews(User user)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE user_id = $user_id";
            command.Parameters.AddWithValue("$user_id", user.id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();
            while (reader.Read())
            {
                Review review = new Review(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2), DateTime.Parse(reader.GetString(3)));
                reviews.Add(review);
            }
            reader.Close();
            user.reviews = reviews;
            connection.Close();
            return reviews;
        }
    }
}
