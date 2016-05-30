using Lisa.Common.WebApi;

namespace Lisa.Breakpoint.Api
{
    public class ReportValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("id");
            Required("title", NotEmpty, TypeOf(DataTypes.String));
            Required("project", NotEmpty, TypeOf(DataTypes.String));
            Optional("assignee.userName", NotEmpty, TypeOf(DataTypes.String));
            Optional("assignee.group", NotEmpty, TypeOf(DataTypes.String));
            Optional("status", NotEmpty, OneOf(ValidationOptions.CaseSensitive, "open", "fixed", "closed", "wontFix", "wontFixApproved"));
            Ignore("reported");
        }

        protected override void ValidatePatch()
        {
            Allow("status");
            Allow("assignee");
        }
    }
}