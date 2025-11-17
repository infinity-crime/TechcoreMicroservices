using FluentValidation;
using TechcoreMicroservices.BookService.Contracts.Requests.Book;

namespace TechcoreMicroservices.BookService.Books.API.Validators;

public class UpdateBookAuthorsRequestValidator : AbstractValidator<UpdateBookAuthorsRequest>
{
    public UpdateBookAuthorsRequestValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty()
            .WithMessage("Book Id is required");

        RuleFor(x => x.AuthorIds)
            .NotEmpty()
            .WithMessage("Authors Ids collection is required");
    }
}
