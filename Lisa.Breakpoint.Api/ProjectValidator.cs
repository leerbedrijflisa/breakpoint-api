using Lisa.Common.WebApi;

namespace Lisa.Breakpoint.Api
{
    public class ProjectValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("id");
            Required("name");
            Required("createdBy");
            Optional("status");
            Optional("platform");
            Ignore("created");
        }
    }
}