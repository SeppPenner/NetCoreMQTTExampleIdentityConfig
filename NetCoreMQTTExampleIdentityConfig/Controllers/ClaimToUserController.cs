
namespace NetCoreMQTTExampleIdentityConfig.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Storage;
    using Storage.Database;
    using Storage.Dto;

    /// <summary>
    /// The claim to user controller class.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimToUserController : ControllerBase
    {
        /// <summary>
        /// The MQTT settings.
        /// </summary>
        private readonly IOptions<MqttSettings> mqttSettings;

        /// <summary>
        /// The database context.
        /// </summary>
        private readonly MqttContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimToUserController"/> class.
        /// </summary>
        /// <param name="mqttSettings">The MQTT settings.</param>
        public ClaimToUserController(IOptions<MqttSettings> mqttSettings, MqttContext databaseContext)
        {
            this.mqttSettings = mqttSettings;
            this.databaseContext = databaseContext;
        }

        /// <summary>
        /// Gets the claim to user connection. GET "api/claimtouser".
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<UserClaim>> GetClaims()
        {
            return this.databaseContext.UserClaims.ToList();
        }

        /// <summary>
        /// Gets the claim by id. GET "api/claim/5".
        /// </summary>
        [HttpGet("{claimId}")]
        public ActionResult<UserClaim> GetClaimById(long claimId)
        {
            return this.databaseContext.UserClaims.FirstOrDefault(u => u.Id == claimId);
        }

        /// <summary>
        /// Creates the claim. POST "api/claim".
        /// </summary>
        [HttpPost]
        public async Task CreateClaim([FromBody] ClaimModel claimModel)
        {
            var claim = new UserClaim
                            {
                                ClaimType = claimModel.ClaimType,
                                ClaimValue = claimModel.ClaimType,
                                UserId = claimModel.UserId
                            };
            await this.databaseContext.UserClaims.AddAsync(claim);
        }

        /// <summary>
        /// Updates the claim. PUT "api/claim/5".
        /// </summary>
        [HttpPut("{claimId}")]
        public async Task UpdateClaim(long claimId, [FromBody] ClaimModel claimModel)
        {
            var resultClaim = this.databaseContext.UserClaims.FirstOrDefault(b => b.Id == claimId);
            if (resultClaim != null)
            {
                resultClaim.ClaimType = claimModel.ClaimType;
                resultClaim.ClaimValue = claimModel.ClaimType;
                resultClaim.UserId = claimModel.UserId;
                await this.databaseContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes the claim by id. DELETE "api/claim/5".
        /// </summary>
        [HttpDelete("{claimId}")]
        public async Task DeleteClaimById(long claimId)
        {
            this.databaseContext.UserClaims.Remove(new UserClaim { Id = claimId });
            await this.databaseContext.SaveChangesAsync();
        }
    }
}
