using GraphQL.Types;
using Issues.Models;
using Issues.Services;

namespace Issues.Schemas
{
    public class IssueType : ObjectGraphType<Issue>
    {
        public IssueType(IUserService users)
        {
            Field(f => f.Id)
                .Description("Issue Id");
            Field(f => f.Name)
                .Description("Issue summary");
            Field(f => f.Description)
                .Description("Brief issue description");
            Field<UserType>(
                "user",
                resolve: context => users.GetUserByIdAsync(context.Source.UserId));
            Field(f => f.Created)
                .Description("Creation datetime");
            Field<IssueStatusesEnum>(
                "status",
                resolve: context => context.Source.Status);
        }
    }
}
