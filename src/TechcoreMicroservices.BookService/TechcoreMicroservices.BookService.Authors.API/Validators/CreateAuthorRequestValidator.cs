using FluentValidation;
using TechcoreMicroservices.BookService.Contracts.Requests.Author;

namespace TechcoreMicroservices.BookService.Authors.API.Validators;

public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleFor(a => a.FirstName)
                .NotEmpty()
                .WithMessage("The author's first name must not be empty.")
                .MaximumLength(100)
                .WithMessage("The author's first name must not be longer than 100 characters.");

        RuleFor(a => a.LastName)
            .NotEmpty()
            .WithMessage("The author's second name must not be empty.")
            .MaximumLength(100)
            .WithMessage("The author's second name must not be longer than 100 characters.");
    }
}
