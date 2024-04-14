using Domain.Objects.Responses.Machine;

namespace Domain.Objects.Responses.Base
{
    public static class BaseResponse
    {
        public static ErrorObject ErrorObj(bool shouldClearData, string returnMessage, IEnumerable<GeneralProductionData>? defaultEggCategories = null)
        {
            return new ErrorObject
            {
                ShouldClearData = shouldClearData,
                ReturnMessage = returnMessage,
                DefaultEggCategories = defaultEggCategories
            };
        }
    }

    public record ErrorObject
    {
        public bool ShouldClearData { get; set; }
        public required string ReturnMessage { get; set; }
        public IEnumerable<GeneralProductionData>? DefaultEggCategories { get; set; }
    }
}
