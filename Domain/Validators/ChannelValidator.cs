using Domain.Models;
using FluentValidation;

namespace Domain.Validators;

public class ChannelValidator : AbstractValidator<Channel> {
  public ChannelValidator() {
    RuleFor(channel => channel.Name).NotNull().NotEmpty();
  }
}