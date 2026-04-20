using PetRadar.Core.Data;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Web.API.IntegrationTests.DataSeeds
{
    public interface IDefaultDataSet
    {
        public UserEntity DefaultUserEntity  { get; }
        public UserPetEntity DefaultPetEntity { get; }
        public ReportEntity DefaultLostReportEntity { get; }
        public ReportEntity DefaultStrayReportEntity { get; }
        public MatchEntity DefaultMatchEntity { get; }
        public AdoptionAnimalEntity DefaultAdoptionAnimalEntity { get; }
        void SeedData(PetRadarDbContext dbContext);
    }
}
