using Lisa.Common.WebApi;

namespace Lisa.Breakpoint.Api
{
    public class ReleaseValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("version", NotEmpty, TypeOf(DataTypes.String));
            Required("branch", NotEmpty, TypeOf(DataTypes.String));
        }
    }
}
