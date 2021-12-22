using AaaS.Api.Dtos.Action;
using AaaS.Core.Actions;
using AutoMapper;

namespace AaaS.Api.MapperProfiles
{
    public class ActionProfile : Profile
    {
        public ActionProfile()
        {
            CreateMap<WebHookActionInsertDto, WebHookAction>();
            CreateMap<MailActionInsertDto, MailAction>();
            CreateMap<MailActionUpdateDto, MailAction>();
            CreateMap<WebHookActionUpdateDto, WebHookAction>();
            CreateMap<BaseAction, ActionDto>()
                .Include<MailAction, MailActionDto>()
                .Include<WebHookAction, WebHookActionDto>();
            CreateMap<MailAction, MailActionDto>();
            CreateMap<WebHookAction, WebHookActionDto>();
        }
    }
}
