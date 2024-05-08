using FluentValidation;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Objects.Requests.User
{
    public record SaveBoardRequest
    {
        public required string Name { get; set; }
        public required DateTime ExamDateTime { get; set; }
        public required string DailyStudyTime { get; set; }
    }

    public class SaveBoardRequestValidator : AbstractValidator<SaveBoardRequest>
    {
        public SaveBoardRequestValidator()
        {
            RuleFor(u => u)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(SaveBoardRequest saveBoardRequest)
        {
            if (saveBoardRequest.Name.IsNullOrEmpty() || saveBoardRequest.Name.Length > 50)
                throw new ValidationException("InvalidName");

            if (saveBoardRequest.ExamDateTime < DateTime.Now)
                throw new ValidationException("Data de exame não pode ser menor que a data atual");

            if (TimeSpan.Parse(saveBoardRequest.DailyStudyTime).TotalSeconds <= 0)
                throw new ValidationException("Tempo de estudo não pode ser menor do que zero");

            return true;
        }
    }
}
