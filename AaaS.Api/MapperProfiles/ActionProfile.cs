using AaaS.Api.Dtos.Action;
using AaaS.Core.Actions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.MapperProfiles
{
    public class ActionProfile : Profile
    {
        public ActionProfile()
        {
            CreateMap<WebHookActionInsertDto, WebHookAction>();
            CreateMap<MailActionInsertDto, MailAction>();
        }
    }
}
