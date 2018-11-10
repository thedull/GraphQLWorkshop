using GraphQL;
using GraphQL.Types;

namespace Issues.Schemas
{
    public class IssuesSchema : Schema
    {
        public IssuesSchema(IssuesQuery query, IssuesMutation mutation, IssuesSubscription subscription, IDependencyResolver resolver)
        {
            Query = query;
            Mutation = mutation;
            Subscription = subscription;
            DependencyResolver = resolver;
        }
    }
}
