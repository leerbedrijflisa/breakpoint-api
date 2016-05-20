using Lisa.Common.WebApi;
using System.Linq;

namespace Lisa.Breakpoint.Api
{
    public class ReportValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("id");
            Required("title", NotEmpty, TypeOf(DataTypes.String));
            Required("project", NotEmpty, TypeOf(DataTypes.String));
            Optional("assignee", TypeOf(DataTypes.String));
            Optional("status", NotEmpty, OneOf("open", "fixed", "closed", "wontFix", "wontFixApproved"));
            Ignore("reported");
        }

        protected override void ValidatePatch()
        {
            Allow("status");
            Allow("assignee");
            Allow("comments");
        }
    }
}