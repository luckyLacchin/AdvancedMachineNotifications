using DemoAPIBot.Data;
using DemoAPIBot.Dtos.SubDto;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace DemoAPIBot.Endpoints.Sub
{
    [Authorize(Roles = "admin")]
    public class DeleteSub : EndpointWithoutRequest
    {
        public ISubRepo repo;
        private readonly ILogger<DeleteSub> logger;
        public DeleteSub(ISubRepo _repo, ILogger<DeleteSub> _logger)
        {
            this.repo = _repo;
            this.logger = logger;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.DELETE);
            Routes("api/v1/sub/{id}");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            logger.LogInformation("Enter DeleteSub api");
            try
            {
                int id = Route<int>("id");
                var subToDelete = await repo.GetSubById(id);
                if (subToDelete == null)
                {
                    logger.LogInformation("Sub does not exist, cannot be deleted");
                    await SendNotFoundAsync();
                }
                repo.DeleteSub(subToDelete);
                await repo.SaveChanges();
                await SendOkAsync();
            }
            catch (Exception ex)
            {
                await SendErrorsAsync();
            }
        }
    }
}
