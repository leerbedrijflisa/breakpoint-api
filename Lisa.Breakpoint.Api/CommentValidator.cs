using Lisa.Common.WebApi;

namespace Lisa.Breakpoint.Api
{
    public class CommentValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("id");
            Required("userName", NotEmpty, TypeOf(DataTypes.String));
            Required("comment", NotEmpty, TypeOf(DataTypes.String));
        }

        protected override void ValidatePatch()
        {
            Allow("comment");
        }
    }
}