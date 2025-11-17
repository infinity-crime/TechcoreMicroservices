using FluentValidation;
using TechcoreMicroservices.BookService.Contracts.Requests.Author;

namespace TechcoreMicroservices.BookService.Authors.API.Validators;

public class UpdateAuthorBooksRequestValidator : AbstractValidator<UpdateAuthorBooksRequest>
{
    public UpdateAuthorBooksRequestValidator()
    {
        RuleFor(a => a.AuthorId)
            .NotEmpty()
            .WithMessage("Author Id is required");

        RuleFor(a => a.BookIds)
            .NotEmpty()
            .WithMessage("Books Ids collection is required");
    }
}
