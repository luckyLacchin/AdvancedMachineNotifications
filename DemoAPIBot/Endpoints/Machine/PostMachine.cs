using DemoAPIBot.Data;
using DemoAPIBot.Dtos.MachineDto;
using DemoAPIBot.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace DemoAPIBot.Endpoints.Machine
{
    [Authorize(Roles = "admin")]
    public class PostMachine : Endpoint<CreateMacchinaDto>
    {
        public IMacchinaRepo repo;
        public AutoMapper.IMapper mapper;
        public DemoContext db;
        private readonly ILogger<PostMachine> logger;
        public PostMachine(ILogger<PostMachine> _logger,IMacchinaRepo _repo, AutoMapper.IMapper _mapper, DemoContext _db)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            this.db = _db;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("api/v1/machine/");
        }

        public override async Task HandleAsync(CreateMacchinaDto created, CancellationToken ct)
        {
            logger.LogInformation("Enter the PostMachine api");
            try
            {
                if (created == null)
                {
                    logger.LogWarning("The machine created is null");
                    await SendErrorsAsync();
                }
                else
                {
                    var getMachine = await repo.GetMachine(created.mId);
                    if (getMachine != null)
                    {
                        logger.LogWarning("The machine has been already created");
                        await SendErrorsAsync();
                    }
                    else
                    {
                        var machineCreated = mapper.Map<Macchina>(created);
                        await db.AddAsync(machineCreated);
                        await repo.SaveChanges(); //Sarebbe stato meglio chiamarlo SaveChangesAsync();
                        await SendCreatedAtAsync($"api/v1/commands/{machineCreated.mId}", null, mapper.Map<ReadMacchinaDto>(machineCreated));
                    }
                }
            }
            catch (Exception ex)
            {
                await SendErrorsAsync();
            }
        }
    }
}
