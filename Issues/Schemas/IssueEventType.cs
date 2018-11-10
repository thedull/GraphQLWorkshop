using GraphQL.Types;
using Issues.Models;

namespace Issues.Schemas
{
    public class IssueEventType : ObjectGraphType<IssueEvent>
    {
        public IssueEventType()
        {
            Field(e => e.Id);
            Field(e => e.Name);
            Field(e => e.IssueId);
            Field<IssueStatusesEnum>("status",
                resolve: context => context.Source.Status);
            Field(e => e.Timestamp);
         }
    }
} 
