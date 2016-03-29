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
                        Message = $"Only the values: open, fixed, closed, won't fix and won't fix (approved) are allowed.",
                        Values = new
                        {
                            Field = fieldName
                        }
                    };
                    Result.Errors.Add(error);
                }
            }
        }

        protected override void ValidateModel()
        {
            Ignore("Id");
            Required("Title", NotEmpty);
            Required("Project", NotEmpty);
            Optional("Status", ValidateArray, NotEmpty);
            Ignore("Reported");
        }

        protected override void ValidatePatch()
        {
            Allow("Status");
        }
    }
}