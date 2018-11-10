using System;
using GraphQL.Types;
using Issues.Models;
using Issues.Services;

namespace Issues.Schemas
{
    public class IssuesMutation : ObjectGraphType<object>
    {
        public IssuesMutation(IIssueService issues)
        {
            Name = "Mutation";
            Field<IssueType>(
                "createIssue",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IssueCreateInputType>> { Name = "issue" }),
                resolve: context => 
                {
                    var issueInput = context.GetArgument<IssueCreateInput>("issue");
                    var id = Guid.NewGuid().ToString();
                    var issue = new Issue(
                        issueInput.Name,
                        issueInput.Description,
                        issueInput.Created,
                        issueInput.UserId,
                        id);
                    return issues.CreateAsync(issue);
                }
            );
        }
    }
}
