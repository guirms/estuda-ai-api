namespace Domain.Models.Enums.Machine
{
    public enum EMachineStatus
    {
        Off = 0,
        StandBy = 1,
        Emergency = 2,
        Operation = 3,
        ScheduledStop = 4,
        LoadCellError = 5,
        SolenoidError = 6,
        InsufficientOutputs = 7,
        RemainingTime = 8
    }
}
