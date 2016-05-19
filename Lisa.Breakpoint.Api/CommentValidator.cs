using Lisa.Common.WebApi;

namespace Lisa.Breakpoint.Api
{
    public class CommentValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("id");
            Ignore("datetime");
            Ignore("deletionDate");
            Required("userName", NotEmpty, TypeOf(DataTypes.String));
            Required("comment", NotEmpty, TypeOf(DataTypes.String));
            Optional("deleted", TypeOf(DataTypes.Boolean));
        }

        protected override void ValidatePatch()
        {
            Allow("comment");
            Allow("deleted");
        }
    }
}