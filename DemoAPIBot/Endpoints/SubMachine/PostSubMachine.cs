using DemoAPIBot.Data;
using DemoAPIBot.Dtos.MachineDto;
using DemoAPIBot.Dtos.SubMachineDto;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;

namespace DemoAPIBot.Endpoints.SubMachine
{
    [Authorize(Roles = "admin")]
    public class PostSubMachine : Endpoint<CreateSubMachineDto>
    {
        public ISubMachineRepo repo;
        public IMacchinaRepo macchinaRepo;
        public ISubRepo subRepo;
        public AutoMapper.IMapper mapper;
        public DemoContext db;
        private readonly ILogger<PostSubMachine> logger;
        public PostSubMachine(ILogger<PostSubMachine> _logger,ISubMachineRepo _repo, AutoMapper.IMapper _mapper, DemoContext _db,IMacchinaRepo _macchinaRepo, ISubRepo _subRepo)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            this.db = _db;
            macchinaRepo = _macchinaRepo;
            subRepo = _subRepo;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("api/v1/sub/machine");
        }

        public override async Task HandleAsync(CreateSubMachineDto created, CancellationToken ct)
        {
            logger.LogInformation("Enter PostSubMachine api");
            try
            {
                if (created == null)
                {
                    logger.LogWarning("SubMachine is null, cannot be created");
                    await SendErrorsAsync();
                }
                else
                {
                    var getSubMachine = await repo.GetSubMachine(created.SubId, created.MacchinaId);
                    var machine = await macchinaRepo.GetMachine(created.MacchinaId);
                    var sub = await subRepo.GetSubById(created.SubId);
                    if (getSubMachine != null || machine == null || sub == null)
                    {
                        logger.LogWarning("One between machine or sub is null or submachine already exists");
                        await SendErrorsAsync();
                    }
                    else
                    {
                        var subMachineCreated = mapper.Map<DemoAPIBot.Models.SubMachine>(created);
                        await db.AddAsync(subMachineCreated);
                        await repo.SaveChanges(); //Sarebbe stato meglio chiamarlo SaveChangesAsync();
                        await SendCreatedAtAsync($"api/v1/commands/{subMachineCreated.SubId}", null, mapper.Map<ReadSubMachineDto>(subMachineCreated));
                    }
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
