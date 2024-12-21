using CommunityToolkit.Mvvm.Messaging;
using DemoAPIBot.Data;
using DemoAPIBot.Dtos.MachineDto;
using DemoAPIBot.Dtos.MsgDispatcherDto;
using DemoAPIBot.Dtos.SubMachineDto;
using DemoAPIBot.Models;
using DemoAPIBot.ServiceDispatchers;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using TelegramClientServer.Models;

namespace DemoAPIBot.Endpoints
{
    [Authorize]
    public class PostSendMsg : Endpoint<CreateMsgDispatcherDto,IEnumerable<ReadMsgDispatcherDto>> //posso fare anche una lista dentro la DTO
    {
        public IMacchinaRepo repo;
        public ILogger<PostSendMsg> _logger;
        public AutoMapper.IMapper mapper;
        public DemoContext db;

        public PostSendMsg(IMacchinaRepo _repo, ILogger<PostSendMsg> logger, AutoMapper.IMapper _mapper, DemoContext _db)
        {
            this.repo = _repo;
            _logger = logger;
            this.mapper = _mapper;
            db = _db;
        }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("api/v1/sendMsg/");
        }

        public override async Task HandleAsync(CreateMsgDispatcherDto created, CancellationToken ct)
        {
            Logger.LogInformation("Enter the PostSendMsg api");
            try
            {
                var dispatcherList = new HashSet<ReadMsgDispatcherDto>();
                var machine = await repo.GetMachine(created.mId);
                if (machine == null)
                {
                    _logger.LogError("L'mId inviato non corrisponde a nessuna macchina registrata.");
                    await SendNotFoundAsync();
                }
                else
                {
                    await db.AddAsync(new Message { mId = created.mId, model = created.model, Date = created.dateTime, Mex = created.msg });
                    await db.SaveChangesAsync();
                    MsgDispatcher msgDispatcher = mapper.Map<MsgDispatcher>(created);
                    foreach (DemoAPIBot.Models.SubMachine subMachine in machine.subMachines)
                    {
                        msgDispatcher.typeDispatcher = subMachine.serviceDispatcher;
                        msgDispatcher.mId = subMachine.MacchinaId;
                        msgDispatcher.userId = subMachine.SubId;

                        if (DispatchersContainer.containDispatcher(new Messanger.ServiceDispatcher(subMachine.serviceDispatcher)))
                        {
                            dispatcherList.Add(new ReadMsgDispatcherDto { Dispatcher = subMachine.serviceDispatcher , Outcome = true });
                            WeakReferenceMessenger.Default.Send(msgDispatcher, msgDispatcher.typeDispatcher);
                        }
                        else
                        {
                            dispatcherList.Add(new ReadMsgDispatcherDto { Dispatcher = subMachine.serviceDispatcher, Outcome = false });
                        }
                    }
                    await SendOkAsync(dispatcherList);


                }
            }
            catch
            {
                await SendErrorsAsync();
            }
        }
    }
}
