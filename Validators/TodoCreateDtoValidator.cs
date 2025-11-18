using FluentValidation;
using TodoApi.Dtos;

public class TodoCreateDtoValidator: AbstractValidator<TodoCreateDto>
{
    public TodoCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required").Length(3,100).WithMessage("Title must be between 3 and 100 characters.");

        RuleFor(x => x.IsCompleted).NotEmpty();
    }
}