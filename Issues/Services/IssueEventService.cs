using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Issues.Models;

namespace Issues.Services
{
    public class IssueEventService : IIssueEventService
    {
        private readonly ISubject<IssueEvent> _eventStream = new ReplaySubject<IssueEvent>(1);
        public IssueEventService()
        {
            AllEvents = new ConcurrentStack<IssueEvent>();
        }
        public ConcurrentStack<IssueEvent> AllEvents { get; }
        public void AddError(Exception exception)
        {
            _eventStream.OnError(exception);
        }
        public IssueEvent AddEvent(IssueEvent orderEvent)
        {
            AllEvents.Push(orderEvent);
            _eventStream.OnNext(orderEvent);
            return orderEvent;
        }
        public IObservable<IssueEvent> EventStream()
        {
            return _eventStream.AsObservable();
        }
    }
    public interface IIssueEventService
    {
        ConcurrentStack<IssueEvent> AllEvents { get; }
        void AddError(Exception exception);
        IssueEvent AddEvent(IssueEvent orderEvent);
        IObservable<IssueEvent> EventStream();
    }
} 
