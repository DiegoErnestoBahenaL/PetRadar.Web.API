using PetRadar.Core.Data.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers
{
    public interface IEmailHelperService
    {
        Task<RestResponse> SendVerificationEmail(UserEntity user, string token);
        Task<RestResponse> SendMatchFoundEmail(UserEntity user);
        Task<RestResponse> SendRecoverPasswordEmail(UserEntity user, string password);
    }
}
