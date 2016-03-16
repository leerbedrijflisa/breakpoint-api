using Lisa.Common.WebApi;

namespace Lisa.Breakpoint.Api
{
    public class ReportValidator : Validator
    {
        public override void ValidateModel()
        {
            Required("Title", NotEmpty);
            Required("Project", NotEmpty);
        }
    }
}