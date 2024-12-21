using DemoAPIBot.Data;
using DemoAPIBot.Dtos.SubDto;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace DemoAPIBot.Endpoints.Sub
{
    [Authorize(Roles = "admin")]
    public class PostSub : EndpointWithoutRequest
    {
        public ISubRepo repo;
        public AutoMapper.IMapper mapper;
        private readonly ILogger<PostSub> logger;
        public PostSub(ILogger<PostSub> _logger,ISubRepo _repo, AutoMapper.IMapper _mapper)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("api/v1/sub");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            logger.LogInformation("Enter PostSub api.");
            try
            {
                var subCreated = await repo.CreateSub();
                await repo.SaveChanges();
                var newId = subCreated.Id;
                Console.WriteLine(newId);
                await SendCreatedAtAsync($"api/v1/commands/{subCreated.Id}", null, mapper.Map<ReadSubDto>(subCreated));
            }
            catch (Exception ex)
            {
                logger.LogError("Error in creating the sub");
                await SendErrorsAsync();
            }
        }
    }
}
