using Lisa.Common.WebApi;

namespace Lisa.Breakpoint.Api
{
    public class MemberShipsValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("userName", NotEmpty, TypeOf(DataTypes.String));
            Required("project", NotEmpty, TypeOf(DataTypes.String));
            Required("role", NotEmpty, TypeOf(DataTypes.String));
        }

        protected override void ValidatePatch()
        {
            
        }
    }
}