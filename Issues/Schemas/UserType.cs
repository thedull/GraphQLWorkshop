using GraphQL.Types;
using Issues.Models;

namespace Issues.Schemas
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Field(f => f.Id)
                .Description("User Id");
            Field(f => f.Username)
                .Description("User handle");
        }
    }
}
