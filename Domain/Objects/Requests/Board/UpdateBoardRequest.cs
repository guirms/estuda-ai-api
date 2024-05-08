using FluentValidation;

namespace Domain.Objects.Requests.User
{
    public record UpdateBoardRequest
    {
        public int BoardId { get; set; }
        public string? Name { get; set; }
    }

    public class UpdateBoardRequestValidator : AbstractValidator<UpdateBoardRequest>
    {
        public UpdateBoardRequestValidator()
        {
            RuleFor(u => u)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(UpdateBoardRequest updateBoardRequest)
        {
            if (updateBoardRequest.Name != null && updateBoardRequest.Name.Length > 50)
                throw new ValidationException("InvalidName");

            return true;
        }
    }
}
