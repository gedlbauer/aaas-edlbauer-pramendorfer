using AaaS.Api.Dtos.Client;
using AaaS.Domain;
using AutoMapper;

namespace AaaS.Api.MapperProfiles
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientDto>();
        }
    }
}
