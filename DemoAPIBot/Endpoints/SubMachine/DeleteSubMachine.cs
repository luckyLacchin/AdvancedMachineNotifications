using DemoAPIBot.Data;
using DemoAPIBot.Dtos.SubMachineDto;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace DemoAPIBot.Endpoints.SubMachine
{
    [Authorize(Roles = "admin")]
    public class DeleteSubMachine : EndpointWithoutRequest
    {
        public ISubMachineRepo repo;
        public AutoMapper.IMapper mapper;
        private readonly ILogger<DeleteSubMachine> logger;
        public DeleteSubMachine(ILogger<DeleteSubMachine> _logger,ISubMachineRepo _repo, AutoMapper.IMapper _mapper)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.DELETE);
            Routes("api/v1/sub/machine/{id}/{mId}");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            logger.LogInformation("Enter DeleteSubMachine api");
            try
            {
                int id = Route<int>("id");
                string mId = Route<string>("mId");
                var subMachineToDelete = await repo.GetSubMachine(id, mId);
                if (subMachineToDelete == null)
                {
                    logger.LogWarning("SubMachineToDelete is null, cannot be deleted");
                    await SendNotFoundAsync();
                }
                else
                {
                    repo.DeleteSubMachine(subMachineToDelete);
                    await repo.SaveChanges();
                    await SendOkAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error connecting to the db");
                await SendErrorsAsync();
            }
        }
    }
}
