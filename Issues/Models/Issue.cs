using System;

namespace Issues.Models
{
    public class Issue
    {
        public Issue(string name, string description, DateTime created, int userId, string Id)
        {
            Name = name;
            Description = description;
            Created = created;
            UserId = userId;
            this.Id = Id;
        }

        public string Name { get; set; }
        public string Description { get; set;}
        public DateTime Created { get; private set; }
        public int UserId { get; set; }
        public string Id { get; private set; }
        public IssueStatuses Status { get; set; }
    }

    [Flags]
    public enum IssueStatuses
    {
        REPORTED = 2,
        IN_PROGRESS = 4,
        FIXED = 8,
        CANCELLED = 16
    }
}
