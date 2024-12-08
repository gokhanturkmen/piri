namespace Piri.ModelExamples.Complex
{
    public class DestinationEmployee
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public CareerLevel CareerLevel { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public DestinationDivision Division { get; set; }
        public List<DestinationRoleHistory> Roles { get; set; } = new List<DestinationRoleHistory>();
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        public List<string> Competencies { get; set; } = new List<string>();
    }

}
