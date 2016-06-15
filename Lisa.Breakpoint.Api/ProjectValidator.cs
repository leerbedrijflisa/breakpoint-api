using Lisa.Common.WebApi;

namespace Lisa.Breakpoint.Api
{
    public class ProjectValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("id");
            Required("name", TypeOf(DataTypes.String));
            Required("createdBy", TypeOf(DataTypes.String));
            Optional("status", OneOf(ValidationOptions.CaseSensitive, "open", "archived"), TypeOf(DataTypes.String));
            Optional("platform", TypeOf(DataTypes.Array));
            Ignore("created");
        }

        protected override void ValidatePatch()
        {
            Allow("status");
            Allow("platform");
        }
    }
}