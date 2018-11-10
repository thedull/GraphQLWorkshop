using System;

namespace Issues.Models
{
    public class IssueEvent
    {
        public IssueEvent(string issueId, string name, IssueStatuses status, DateTime timestamp)
        {
            IssueId = issueId;
            Name = name;
            Status = status;
            Timestamp = timestamp;
            Id = Guid.NewGuid().ToString();
        }
         public string Id { get; private set; }
        public string IssueId { get; set; }
        public string Name { get; set; }
        public IssueStatuses Status { get; set; }
        public DateTime Timestamp { get; private set; }
    }
} 
