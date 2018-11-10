using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using GraphQL.Types;
using GraphQL.Subscription;
using GraphQL.Resolvers;
using Issues.Models;
using Issues.Services;

namespace Issues.Schemas
{
    public class IssuesSubscription : ObjectGraphType<object>
    {
        private readonly IIssueEventService _events;
        public IssuesSubscription(IIssueEventService events)
        {
            _events = events;
            Name = "Subscription";
            AddField(new EventStreamFieldType
            {
                Name = "issueEvent",
                Arguments = new QueryArguments(new QueryArgument<ListGraphType<IssueStatusesEnum>>
                {
                    Name = "statuses"
                }),
                Type = typeof(IssueEventType),
                Resolver = new FuncFieldResolver<IssueEvent>(ResolveEvent),
                Subscriber = new EventStreamResolver<IssueEvent>(Subscribe)
            });
        }
        private IObservable<IssueEvent> Subscribe(ResolveEventStreamContext context)
        {
            var statusList = context.GetArgument<IList<IssueStatuses>>("statuses", new List<IssueStatuses>());
            if (statusList.Count > 0)
            {
                IssueStatuses statuses = 0;
                 foreach (var status in statusList)
                {
                    statuses = statuses | status;    
                }
                return _events.EventStream().Where(e => (e.Status & statuses) == e.Status);
            }
            else
            {
                return _events.EventStream();
            }
        
        }
        private IssueEvent ResolveEvent(ResolveFieldContext context)
        {
            var issueEvent = context.Source as IssueEvent;
            return issueEvent;
        }
    }
} 
