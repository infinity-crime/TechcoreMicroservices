using FluentValidation;
using TechcoreMicroservices.BookService.Contracts.Requests.Book;

namespace TechcoreMicroservices.BookService.Books.API.Validators;

public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Book Id is required");

        RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.");

        RuleFor(x => x.Year)
            .GreaterThan(1900)
            .WithMessage("Year must be greater than 1900.");
    }
}
