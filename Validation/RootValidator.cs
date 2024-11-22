using FluentValidation;
using static writings_backend_dotnet.Utility.Utility;

namespace writings_backend_dotnet.Controllers.Validation
{

    public class RootValidatedDTO
    {
        public required short ScriptureNumber { get; set; }
        public required string RootLatin { get; set; }
    }
    public class RootValidator : AbstractValidator<RootValidatedDTO>
    {
        public RootValidator()
        {
            RuleFor(r => r.ScriptureNumber)
            .Cascade(CascadeMode.Stop)
            .Must(number => number >= 1)
            .WithMessage("Scripture number is too small; minimum is 1.")
            .Must(SCRIPTURE_DATA.ContainsKey)
            .WithMessage(r => $"Scripture number {r.ScriptureNumber} is not valid.");
            RuleFor(r => r.RootLatin).MinimumLength(MIN_LENGTH_FOR_ROOT).MaximumLength(MAX_LENGTH_FOR_ROOT);

        }

    }
}