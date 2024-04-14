namespace Domain.Objects.Dto_s.Machine
{
    public record MachineSpeed
    {
        public MachineSpeed(double? progSpeed, double? currentSpeed)
        {
            ProgSpeed = progSpeed;
            CurrentSpeed = currentSpeed;
        }

        public double? ProgSpeed { get; set; }
        public double? CurrentSpeed { get; set; }
    }
}
