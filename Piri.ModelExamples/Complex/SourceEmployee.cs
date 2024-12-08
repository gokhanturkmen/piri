namespace Piri.ModelExamples.Complex
{
    public class SourceEmployee
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public CareerLevel CareerLevel { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public SourceDivision Division { get; set; }
        public List<SourceRoleHistory> Roles { get; set; } = [];
        public Dictionary<string, string> Metadata { get; set; } = [];
        public List<string> Competencies { get; set; } = [];

        public override bool Equals(object? obj)
        {
            if (obj is DestinationEmployee destination)
            {
                return EmployeeId == destination.EmployeeId
                    && FirstName == destination.FirstName
                    && LastName == destination.LastName
                    && BirthDate == destination.BirthDate
                    && CareerLevel == destination.CareerLevel
                    && Salary == destination.Salary
                    && IsActive == destination.IsActive
                    && DivisionIsIdentical(destination.Division)
                    && RolesAreIdentical(destination.Roles)
                    && Metadata.All(kvp => destination.Metadata.ContainsKey(kvp.Key) && destination.Metadata[kvp.Key] == kvp.Value)
                    && Competencies.All(c => destination.Competencies.Contains(c)) && Competencies.Count == destination.Competencies.Count;
            }
            return obj == this;
        }

        private bool DivisionIsIdentical(DestinationDivision destinationDivision)
        {
            return Division.Name == destinationDivision.Name
                && Division.Floor == destinationDivision.Floor
                && Division.ManagerGuid == destinationDivision.ManagerGuid
                && Division.Site.Name == destinationDivision.Site.Name
                && Division.Site.Address == destinationDivision.Site.Address
                && Division.Site.ZipCode == destinationDivision.Site.ZipCode
                && Division.Site.Country == destinationDivision.Site.Country;
        }

        private bool RolesAreIdentical(List<DestinationRoleHistory> destinationRoles)
        {
            if (Roles.Count != destinationRoles.Count)
            {
                return false;
            }
            var result = true;
            for (var i = 0; i < Roles.Count; i++)
            {
                result &= Roles[i].JobTitle == destinationRoles[i].JobTitle
                    && Roles[i].FromDate == destinationRoles[i].FromDate
                    && Roles[i].ToDate == destinationRoles[i].ToDate
                    && Roles[i].Duties == destinationRoles[i].Duties;
                if (!result)
                {
                    break;
                }
            }

            return result;
        }
    }
}
