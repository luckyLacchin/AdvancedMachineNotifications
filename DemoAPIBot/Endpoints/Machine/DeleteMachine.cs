using DemoAPIBot.Data;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;

namespace DemoAPIBot.Endpoints.Machine
{
    [Authorize(Roles = "admin")]
    public class DeleteMachine : EndpointWithoutRequest
    {
        public IMacchinaRepo repo;
        public AutoMapper.IMapper mapper;
        private readonly ILogger<DeleteMachine> _logger;
        public DeleteMachine(ILogger<DeleteMachine> logger, IMacchinaRepo _repo, AutoMapper.IMapper _mapper)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            _logger = logger;
        }

        public override void Configure()
        {
            Verbs(Http.DELETE);
            Routes("api/v1/machine/{mId}");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            _logger.LogInformation("Enter DeleteMachine api");
            try
            {
                string mId = Route<string>("mId");
                var machineToDelete = await repo.GetMachine(mId);
                if (machineToDelete == null)
                {
                    _logger.LogWarning("Machine does not exist, cannot be deleted");
                    await SendNotFoundAsync();
                }
                else
                {
                    repo.DeleteMachine(machineToDelete);
                    await repo.SaveChanges();
                    await SendOkAsync();
                }
            }
            catch (Exception ex)
            {
                await SendErrorsAsync();
            }
        }
    }
}
