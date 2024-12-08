using AutoMapper;
using Piri.Core;
using Piri.ModelExamples.Complex;
using IAutoMapper = AutoMapper.IMapper;
using IPiriMapper = Piri.Core.IMapper;

namespace Piri.Benchmarks.Benchmarks
{
    //[MemoryDiagnoser]
    public class ComplexObjectExplicitBenchmarks
    {
        private IPiriMapper _piriMapper;
        private IAutoMapper _autoMapper;
        private SourceEmployee _sourceEmployee;
        private DestinationEmployee _destinationEmployee;

        //[GlobalSetup]
        public void Setup()
        {
            _sourceEmployee = new SourceEmployee
            {
                EmployeeId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1990, 1, 1),
                CareerLevel = CareerLevel.Intermediate,
                Salary = 50000,
                IsActive = true,
                Division = new SourceDivision
                {
                    Name = "Division",
                    Floor = 1,
                    ManagerGuid = Guid.NewGuid(),
                    Site = new SourceSiteLocation
                    {
                        Name = "Site",
                        Address = "1234 Main St, City, State, 12345",
                        ZipCode = "12345",
                        Country = "USA"
                    }
                },
                Roles =
                [
                    new() {
                        JobTitle = "Developer",
                        FromDate = new DateTime(2010, 1, 1),
                        ToDate = new DateTime(2015, 1, 1),
                        Duties = "Develop software applications."
                    },
                    new() {
                        JobTitle = "Senior Developer",
                        FromDate = new DateTime(2015, 1, 1),
                        ToDate = new DateTime(2020, 1, 1),
                        Duties = "Develop software applications and mentor junior developers."
                    }
                ],
                Metadata =
                {
                    ["Key1"] = "Value1",
                    ["Key2"] = "Value2"
                },
                Competencies = ["C#", "ASP.NET Core", "SQL Server"]
            };

            _destinationEmployee = new DestinationEmployee
            {
                EmployeeId = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe",
                BirthDate = new DateTime(1980, 1, 1),
                CareerLevel = CareerLevel.Expert,
                Salary = 100000,
                IsActive = false,
                Division = new DestinationDivision
                {
                    Name = "Division",
                    Floor = 1,
                    ManagerGuid = Guid.NewGuid(),
                    Site = new DestinationSiteLocation
                    {
                        Name = "Site",
                        Address = "1234 Main St, City, State, 12345",
                        ZipCode = "12345",
                        Country = "USA"
                    }
                },
                Roles =
                [
                    new() {
                        JobTitle = "Developer",
                        FromDate = new DateTime(2010, 1, 1),
                        ToDate = new DateTime(2015, 1, 1),
                        Duties = "Develop software applications."
                    },
                    new() {
                        JobTitle = "Senior Developer",
                        FromDate = new DateTime(2015, 1, 1),
                        ToDate = new DateTime(2020, 1, 1),
                        Duties = "Develop software applications and mentor junior developers."
                    }
                ],
                Metadata =
                {
                    ["Key1"] = "Value1",
                    ["Key2"] = "Value2"
                },
                Competencies = ["Javascript", "C#", "ASP.NET Core", "PostgreSQL"]
            };

            _piriMapper = PiriMapper.Instance;

            _autoMapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SourceEmployee, DestinationEmployee>();
                cfg.CreateMap<DestinationEmployee, SourceEmployee>();
            }).CreateMapper();
        }

        //[Benchmark]
        public DestinationEmployee CustomMappingSimple()
        {
            return new DestinationEmployee
            {
                EmployeeId = _sourceEmployee.EmployeeId,
                FirstName = _sourceEmployee.FirstName,
                LastName = _sourceEmployee.LastName,
                BirthDate = _sourceEmployee.BirthDate,
                CareerLevel = _sourceEmployee.CareerLevel,
                Salary = _sourceEmployee.Salary,
                IsActive = _sourceEmployee.IsActive,
                Division = new DestinationDivision
                {
                    Name = _sourceEmployee.Division.Name,
                    Floor = _sourceEmployee.Division.Floor,
                    ManagerGuid = _sourceEmployee.Division.ManagerGuid,
                    Site = new DestinationSiteLocation
                    {
                        Name = _sourceEmployee.Division.Site.Name,
                        Address = _sourceEmployee.Division.Site.Address,
                        ZipCode = _sourceEmployee.Division.Site.ZipCode,
                        Country = _sourceEmployee.Division.Site.Country
                    }
                },
                Roles = _sourceEmployee.Roles.Select(r => new DestinationRoleHistory
                {
                    JobTitle = r.JobTitle,
                    FromDate = r.FromDate,
                    ToDate = r.ToDate,
                    Duties = r.Duties
                }).ToList(),
                Metadata = _sourceEmployee.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                Competencies = [.. _sourceEmployee.Competencies]
            };
        }
    }
}
