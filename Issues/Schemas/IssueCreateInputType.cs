using GraphQL.Types;

namespace Issues.Schemas
{
    public class IssueCreateInputType : InputObjectGraphType
    {
        public IssueCreateInputType()
        {
            Name = "IssueInput";
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<NonNullGraphType<StringGraphType>>("description");
            Field<NonNullGraphType<IntGraphType>>("userId");
            Field<NonNullGraphType<DateGraphType>>("created");    
        }
    }
}
