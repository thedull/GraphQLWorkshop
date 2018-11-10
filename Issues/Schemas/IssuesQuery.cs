using GraphQL.Types;
using Issues.Services;

namespace Issues.Schemas
{
    public class IssuesQuery : ObjectGraphType<object>
    {
        public IssuesQuery(IIssueService issues)
        {
            Name = "Query";
            Field<ListGraphType<IssueType>>(
                "issues",
                resolve: context => issues.GetIssuesAsync());
        }
    }
}
