using Lisa.Common.WebApi;
using System.Linq;

namespace Lisa.Breakpoint.Api
{
    public class ReportValidator : Validator
    {
        private void ValidateArray(string fieldName, object value)
        {
            string[] status = new string[] { "open", "fixed", "closed", "won't fix", "won't fix (approved)" };

            if (value != null)
            {
                if (!status.Contains(value))
                {
                    var error = new Error
                    {
                        Code = ErrorCode.EmptyValue,
                        Message = $"In field '{fieldName}' only the values: open, fixed, closed, won't fix and won't fix (approved) are allowed.",
                         
                        Values = new
                        {
                            Field = fieldName,
                            Value = value,
                            ExpectedValues = status
                        }
                    };
                    Result.Errors.Add(error);
                }
            }
        }

        protected override void ValidateModel()
        {
            Ignore("id");
            Required("title", NotEmpty, TypeOf(DataTypes.String));
            Required("project", NotEmpty, TypeOf(DataTypes.String));
            Optional("assignee", TypeOf(DataTypes.String | DataTypes.Object));
            Optional("assignee.userName", TypeOf(DataTypes.String));
            Optional("assignee.group", TypeOf(DataTypes.String));
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