using AutoMapper;
using DemoAPIBot.Data;
using DemoAPIBot.Dtos.MachineDto;
using DemoAPIBot.Dtos.MsgDispatcherDto;
using DemoAPIBot.Dtos.SubDto;
using DemoAPIBot.Dtos.SubMachineDto;
using DemoAPIBot.Models;
using DemoServiceProto;
using Microsoft.AspNetCore.Mvc;
using TelegramClientServer.Models;

namespace DemoAPIBot.Profiles
{
    public class ProfileDto : Profile
    {
        public ProfileDto()
        {
            //Source -> Target
            CreateMap<SubMachine, ReadSubMachineDto>();
            CreateMap<CreateSubMachineDto, SubMachine>();
            CreateMap<UpdateSubMachineDto, SubMachine>();
            CreateMap<Sub, ReadSubDto>();
            CreateMap<CreateSubDto, Sub>();
            CreateMap<UpdateSubDto, Sub>();
            CreateMap<Macchina, ReadMacchinaDto>();
            CreateMap<CreateMacchinaDto, Macchina>();
            CreateMap<UpdateMacchinaDto, Macchina>();
            CreateMap<CreateMsgDispatcherDto, MsgDispatcher>();
            CreateMap<Message, MessageProto>().ForMember(d => d.Model, d => d.MapFrom(x => x.model == null? "": x.model));
        }
    }
}
