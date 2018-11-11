using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Issues.Models;

namespace Issues.Services
{
    public class IssueService : IIssueService
    {
        private IList<Issue> _issues;
        private readonly IIssueEventService _events;

        public IssueService(IIssueEventService events)
        {
            _issues = new List<Issue>();
            _issues.Add(new Issue("Can't login", "The login breaks", DateTime.Now, 1, "C1DFA804-5A4B-405A-B6FA-12ABD0AD09FE"));
            _issues.Add(new Issue("Main panel misaligned", "Main panel is not centered", DateTime.Now.AddHours(1), 2, "241C58BB-56FD-48BB-88D4-379DA547B6C2"));
            _issues.Add(new Issue("Tests failing", "Tests for services are failing", DateTime.Now.AddHours(2), 3, "9282CBA7-0499-4769-919C-B9E35437008A"));
            _issues.Add(new Issue("Wrong font size", "The basic font size should be 12px", DateTime.Now.AddHours(2), 4, "8902E024-E54D-430F-B600-3B3CB22F591B")); 
            this._events = events;
        }

        public Task<Issue> CreateAsync(Issue issue)
        {
            _issues.Add(issue);
            var issueEvent = new IssueEvent(
                issue.Id, 
                issue.Name, 
                IssueStatuses.REPORTED, 
                DateTime.Now);
            _events.AddEvent(issueEvent);
            return Task.FromResult(issue);
        }

        public Task<Issue> GetIssueByIdAsync(string id)
        {
            return Task.FromResult(_issues.Single(o => Equals(o.Id, id)));
        }
        public Task<IEnumerable<Issue>> GetIssuesAsync()
        {
            return Task.FromResult(_issues.AsEnumerable());
        }

        private Issue GetById(string id)
        {
            var issue = _issues.SingleOrDefault(i => Equals(i.Id, id));
            if (issue == null)
            {
                throw new ArgumentException(string.Format("Issue Id '{0}' is invalid", id));
            }
            return issue;
        }

        public Task<Issue> StartAsync(string issueId)
        {
            var issue = GetById(issueId);
            issue.Start();
            var issueEvent = new IssueEvent(
                issue.Id, 
                issue.Name, 
                IssueStatuses.IN_PROGRESS,
                DateTime.Now);
            _events.AddEvent(issueEvent);
            return Task.FromResult(issue);    
        }
    }
        
    public interface IIssueService
    {
        Task<Issue> GetIssueByIdAsync(string id);
        Task<IEnumerable<Issue>> GetIssuesAsync();
        Task<Issue> CreateAsync(Issue issue);
        Task<Issue> StartAsync(string issueId);
    }
}  