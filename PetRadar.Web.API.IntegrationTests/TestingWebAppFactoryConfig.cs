using PetRadar.Core.Data.Entities;
using PetRadar.Web.API.IntegrationTests.DataSeeds;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Web.API.IntegrationTests
{
    public class TestingWebAppFactoryConfig
    {
        public bool InitializeDatabase { get; set; } = true;
        public IDefaultDataSet DataSet { get; set; } = new DefaultDataSet();
        public bool IncludeAuthenticationHeader { get; set; } = true;
        public string ChangeEnvironmentTo { get; set; } = Constants.LocalIntegrationEnvironment;
        public UserEntity UserEntityForAuth { get; set;  }


        public TestingWebAppFactoryConfig()
        {
            UserEntityForAuth = DataSet.DefaultUserEntity;
        }

    }
}
