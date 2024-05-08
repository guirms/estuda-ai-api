using FluentValidation;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Objects.Requests.User
{
    public record BoardRequest
    {
        public required string Name { get; set; }
        public required DateTime ExamDateTime { get; set; }
        public required TimeSpan DailyStudyTime { get; set; }
    }

    public class BoardRequestValidator : AbstractValidator<BoardRequest>
    {
        public BoardRequestValidator()
        {
            RuleFor(u => u)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(BoardRequest boardRequest)
        {
            if (boardRequest.Name.IsNullOrEmpty() || boardRequest.Name.Length > 50)
                throw new ValidationException("InvalidName");

            if (boardRequest.ExamDateTime < DateTime.Now)
                throw new ValidationException("Data de exame não pode ser menor que a data atual");

            if (boardRequest.DailyStudyTime.TotalSeconds <= 0)
                throw new ValidationException("Tempo de estudo não pode ser menor do que zero");

            return true;
        }
    }
}
