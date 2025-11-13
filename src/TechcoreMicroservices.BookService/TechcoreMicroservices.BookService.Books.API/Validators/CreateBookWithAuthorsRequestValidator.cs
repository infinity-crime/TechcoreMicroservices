using FluentValidation;
using TechcoreMicroservices.BookService.Contracts.Requests.Book;

namespace TechcoreMicroservices.BookService.Books.API.Validators;

public class CreateBookWithAuthorsRequestValidator : AbstractValidator<CreateBookWithAuthorsRequest>
{
    public CreateBookWithAuthorsRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(x => x.Year)
                .GreaterThan(1900)
                .WithMessage("Year must be greater than 1900.");

        RuleFor(x => x.Authors)
            .NotEmpty()
            .WithMessage("Authors Ids collection is required");
    }
}
