using Lisa.Common.WebApi;

namespace Lisa.Breakpoint.Api
{
    public class CommentValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("id");
            Ignore("created");
            Ignore("statusChanged");
            Required("userName", NotEmpty, TypeOf(DataTypes.String));
            Required("comment", NotEmpty, TypeOf(DataTypes.String));
            Optional("status", NotEmpty, OneOf(ValidationOptions.CaseSensitive, "open", "archived"));
        }

        protected override void ValidatePatch()
        {
            Allow("comment");
            Allow("status");
        }
    }
}