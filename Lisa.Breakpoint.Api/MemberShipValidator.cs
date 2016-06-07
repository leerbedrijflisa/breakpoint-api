using Lisa.Common.WebApi;

namespace Lisa.Breakpoint.Api
{
    public class MembershipValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("id");
            Required("userName", NotEmpty, TypeOf(DataTypes.String));
            Required("project", NotEmpty, TypeOf(DataTypes.String));
            Required("role", NotEmpty, OneOf(ValidationOptions.CaseSensitive, "developer", "tester", "manager"), TypeOf(DataTypes.String));
        }

        protected override void ValidatePatch()
        {

        }
        
        public static object MembershipError(DynamicModel membership)
        {
            var error = new Error
            {
                Code = ErrorCode.EmptyValue,
                Message = $"The given membership already exist.",
                Values = membership
            };

            return error;
        }
    }
}