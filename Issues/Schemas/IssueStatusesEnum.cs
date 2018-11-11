using GraphQL.Types;

namespace Issues.Schemas
{
    public class IssueStatusesEnum : EnumerationGraphType
    {
        public IssueStatusesEnum()
        {
            Name = "IssueStatuses";
            AddValue("REPORTED", "Issue was reported", 2);
            AddValue("IN_PROGRESS", "Issue is in progress", 4);
            AddValue("FIXED", "Issue is fixed", 8);
            AddValue("CANCELLED", "Issue was cancelled", 16);
        }
    }
}
