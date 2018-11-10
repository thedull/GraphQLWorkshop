namespace Issues.Models
{
    public class User
    {
        public User(int id, string username)
        {
            Id = id;
            Username = username;
        }

        public int Id { get; private set; }
        public string Username { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
    }
}