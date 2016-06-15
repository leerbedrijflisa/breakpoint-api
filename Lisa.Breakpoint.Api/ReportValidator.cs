using Lisa.Common.WebApi;

namespace Lisa.Breakpoint.Api
{
    public class ReportValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("id");
            Ignore("reported");
            Required("title", NotEmpty, TypeOf(DataTypes.String));
            Required("project", NotEmpty, TypeOf(DataTypes.String));
            Required("description", NotEmpty, TypeOf(DataTypes.String));
            Optional("platform", NotEmpty, TypeOf(DataTypes.Array));
            Optional("assignee.userName", NotEmpty, TypeOf(DataTypes.String));
            Optional("assignee.group", NotEmpty, TypeOf(DataTypes.String));
            Optional("status", NotEmpty, OneOf(ValidationOptions.CaseSensitive, "open", "fixed", "closed", "wontFix", "wontFixApproved"));
            Optional("priority", OneOf(ValidationOptions.CaseSensitive, "", "fixImmediately", "fixBeforeRelease", "fixNextRelease", "fixWhenever"));
        }

        protected override void ValidatePatch()
        {
            Allow("status");
            Allow("assignee");
            Allow("description");
            Allow("priority");
            Allow("solvedCommit");
            Allow("platform");
        }
    }
}