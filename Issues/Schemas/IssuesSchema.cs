using GraphQL;
using GraphQL.Types;

namespace Issues.Schemas
{
    public class IssuesSchema : Schema
    {
        public IssuesSchema(IssuesQuery query, IDependencyResolver resolver)
        {
            Query = query;
            DependencyResolver = resolver;
        }
    }
}
