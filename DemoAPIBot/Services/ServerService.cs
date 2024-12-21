using Grpc.Core;
using DemoAPIBot.Messanger;
using DemoServiceProto;
using CommunityToolkit.Mvvm.Messaging;
using DemoAPIBot.ServiceDispatchers;
using DemoAPIBot.Data;
using DemoAPIBot.Models;
using DemoAPIBot.Endpoints.SubMachine;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Identity.Client.Region;
using FastEndpoints;

namespace DemoAPIBot.Services
{
    public class ServerService : ServerApi.ServerApiBase
    {
        private readonly ILogger<ServerService> _logger;
        private readonly DemoContext db;
        AutoMapper.IMapper mapper;
        public ServerService(ILogger<ServerService> logger, DemoContext _db, AutoMapper.IMapper _mapper)
        {
            _logger = logger;
            db = _db;
            mapper = _mapper;
            
        }

        public override Task<DeleteSubResponse> DeleteSub(DeleteSubRequest request, ServerCallContext context)
        {
            SubMachine subMachineToDelete = null;
            DeleteSubResponse deleteSubResponse = new DeleteSubResponse();
            foreach (int userId in request.UsersId) {
                subMachineToDelete = db.SubMachines.Where(x => x.MacchinaId == request.MId && x.SubId == userId).FirstOrDefault();
                if(subMachineToDelete != null)
                {
                    deleteSubResponse.Outcome = true;
                    deleteSubResponse.UserId = subMachineToDelete.SubId;
                    db.SubMachines.Remove(subMachineToDelete);
                    db.Subs.Remove(db.Subs.Where(x => x.Id == subMachineToDelete.SubId).FirstOrDefault());
                    db.SaveChanges();
                    return Task.FromResult(deleteSubResponse);
                }
            }
            //si è inserito un mId a cui non si è realmente iscritti.
            deleteSubResponse.Outcome = false;
            deleteSubResponse.UserId = -1; //potrei anche non mettere nulla.
            return Task.FromResult(deleteSubResponse);
        }

        public override Task<GetMessagesMIdResponse> GetMessagesMId(GetMessagesMIdRequest request, ServerCallContext context)
        {
            var response = new GetMessagesMIdResponse();
            if (db.Messages.Where(x => x.mId == request.MId).Any())
            {
                response.Outcome = true;
                List<MessageProto> messageProtos = mapper.Map<List<MessageProto>>(db.Messages.Where(x => x.mId == request.MId).ToList());
                response.Messages.AddRange(messageProtos);
            }
            else
            {
                response.Outcome = false;
                //messages rimane vuoto, null
            }
            return Task.FromResult(response);
        }

        //Con questo metodo ottengo tutte le macchine a cui è iscritto un utente!
        public override Task<getSubsResponse> GetSubs(GetSubsRequest request, ServerCallContext context)
        {
            List<string> subMachines = new List<string>(); 
            foreach (int id in request.UserId) {
                subMachines.Add(db.SubMachines.Where(x => x.SubId == id).Select(x => x.MacchinaId).FirstOrDefault());
                //è impossibile che sia null, in quanto se è iscritto per forza c'è.
            }
            var response = new getSubsResponse();
            response.Outcome = true;
            response.MIds.AddRange(subMachines);
            return Task.FromResult(response);
        }

        public override Task<ServerAliveResponse> IsServerAlive(ServerAlive request, ServerCallContext context)
        {
            _logger.LogWarning("Just enter IsServerAlive");
            /*
             * Sto mettendo direttamente nell'implementazione del serviceDispatcher
             * il controllo se l'ip e port hanno dei valori utilizzabili.
             * Quindi in questo servizio viene creato un dispatcher al di là di tutto.
             * Nel caso in cui avesse dei valori non validi, l'istanza dovrebbe essere cancellata.
             */
            if (!DispatchersContainer.containDispatcher(new ServiceDispatcher(request.Name)))
            {
                _logger.LogWarning("Before new service");
                ServiceDispatcher serviceDispatcher = new ServiceDispatcher();
                _logger.LogWarning("After new service");
                serviceDispatcher.SetService(request);
                _logger.LogWarning("Set service");
                DispatchersContainer.addDispatcher(serviceDispatcher);
                _logger.LogWarning($"{request.Name} added to DispatchersContainer");
                return Task.FromResult(new ServerAliveResponse
                {
                    Message = "Connection is coming from demoapi",
                });
            }else
            {
                return Task.FromResult(new ServerAliveResponse
                {
                    Message = "Connection already created"
                });
            }

        }

        public override Task<SendIdResponse> SendId(SendIdRequest request, ServerCallContext context)
        {
            if (db.InProgressSubs.Where<InProgressSub>(x => x.token == request.Token).Any())
            {
                InProgressSub inProgress = db.InProgressSubs.Where(x => x.token == request.Token).FirstOrDefault();
                SubMachine subMachine = new SubMachine(request.UserId, inProgress.mId, request.Dispatcher);
                if (!db.Machines.Where(x => x.mId == inProgress.mId).Any())
                        db.Add(new Macchina(inProgress.mId));
                if (!db.Subs.Where(x => x.Id == request.UserId).Any()) //non dovrebbe essere possibile, metto il controllo lo stesso per sicurezza
                    db.Add(new Sub(request.UserId));
                if (!db.SubMachines.Where(x => x.SubId == subMachine.SubId && x.MacchinaId == subMachine.MacchinaId).Any())
                    db.Add(subMachine); //non dovrebbe essere possibile, metto il controllo lo stesso per sicurezza
                db.SaveChanges();
                return Task.FromResult(new SendIdResponse
                {
                    Outcome = true
                });
            }
            else
            {
                //il token dovrebbe essere sempre esistente, ma per sicurezza mettiamo anche questo controllo
                return Task.FromResult(new SendIdResponse
                {
                    Outcome = false
                });
            }
        }

        public override Task<TokenResponse> SendToken(TokenRequest request, ServerCallContext context)
        {
            if (db.InProgressSubs.Where<InProgressSub>(x => x.token == request.Token).Any())
            {
                InProgressSub inProgress = db.InProgressSubs.Where(x => x.token == request.Token).FirstOrDefault();
                return Task.FromResult(new TokenResponse
                {
                    Outcome = true,
                    MId = inProgress.mId
                });
            }
            else
            {
                //è il caso in cui il token inviato è sbagliato!
                return Task.FromResult(new TokenResponse
                {
                    Outcome = false,
                    MId = "Token is wrong"
                });
            }
        }
    }
}
