using System;

namespace Issues.Models
{
    public class IssueCreateInput
    {
       public string Name { get; set; }
       public string Description { get; set; }
       public int UserId { get; set; }
       public DateTime Created { get; set; } 
    }
}
