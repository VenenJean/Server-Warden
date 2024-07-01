using Domain.Models;
using FluentValidation;

namespace Domain.Validators;

public class MessageValidator : AbstractValidator<Message> {
  
  public MessageValidator() {
    RuleFor(message => message.Content).NotNull().NotEmpty();
  }
}

internal class ValidationTester {
  public void StartTesting() {
    var msg = new Message();
    var msgValidator = new MessageValidator();

    var validationResult = msgValidator.Validate(msg);

    if (validationResult.IsValid) return;
    foreach (var failure in validationResult.Errors) {
      Console.WriteLine($"Property {failure.PropertyName} failed validation. Error was {failure.ErrorMessage}");
    }
  }
}