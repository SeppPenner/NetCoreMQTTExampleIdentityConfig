
namespace NetCoreMQTTExampleIdentityConfig.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NetCoreMQTTExampleIdentityConfig.Controllers.Extensions;
    using Storage;
    using Storage.Database;
    using Storage.Dto;

    /// <summary>
    /// The claim controller class.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("api/claim")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly MqttContext databaseContext;

        /// <summary>
        /// The automapper.
        /// </summary>
        private readonly IMapper autoMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimController"/> class.
        /// </summary>
        /// <param name="databaseContext">The database context.</param>
        /// <param name="autoMapper">The automapper service.</param>
        public ClaimController(MqttContext databaseContext, IMapper autoMapper)
        {
            this.databaseContext = databaseContext;
            this.autoMapper = autoMapper;
        }

        /// <summary>
        /// Gets the claims. GET "api/claim".
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DtoReadUserClaim>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DtoReadUserClaim>>> GetClaims()
        {
            try
            {
                var claims = await this.databaseContext.UserClaims.ToListAsync();
                var returnUserClaims = this.autoMapper.Map<IEnumerable<DtoReadUserClaim>>(claims);
                return Ok(returnUserClaims);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Gets the claim by id. GET "api/claim/5".
        /// </summary>
        [HttpGet("{claimId}")]
        [ProducesResponseType(typeof(DtoReadUserClaim), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoReadUserClaim>> GetClaimById(long claimId)
        {
            try
            {
                var claim = await this.databaseContext.UserClaims.FirstOrDefaultAsync(u => u.Id == claimId);

                if (claim == null)
                {
                    return NotFound(claimId);
                }

                var returnUserClaim = this.autoMapper.Map<DtoReadUserClaim>(claim);
                return Ok(returnUserClaim);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Creates the claim. POST "api/claim".
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(DtoReadUserClaim), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateClaim([FromBody] DtoCreateUpdateUserClaim createUserClaim)
        {
            try
            {
                var claim = this.autoMapper.Map<UserClaim>(createUserClaim);
                claim.CreatedAt = DateTimeOffset.Now;
                await this.databaseContext.UserClaims.AddAsync(claim);
                var returnUserClaim = this.autoMapper.Map<DtoReadUserClaim>(createUserClaim);
                return Ok(returnUserClaim);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Updates the claim. PUT "api/claim/5".
        /// </summary>
        [HttpPut("{claimId}")]
        [ProducesResponseType(typeof(DtoReadUserClaim), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateClaim(long claimId, [FromBody] DtoCreateUpdateUserClaim updateUserClaim)
        {
            try
            {
                var resultClaim = await this.databaseContext.UserClaims.FirstOrDefaultAsync(b => b.Id == claimId);
                if (resultClaim != null)
                {
                    resultClaim = this.autoMapper.Map<UserClaim>(updateUserClaim);
                    resultClaim.UpdatedAt = DateTimeOffset.Now;
                    this.databaseContext.UserClaims.Update(resultClaim);
                    var returnUserClaim = this.autoMapper.Map<DtoReadUserClaim>(updateUserClaim);
                    return Ok(returnUserClaim);
                }

                return NotFound(claimId);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Deletes the claim by id. DELETE "api/claim/5".
        /// </summary>
        [HttpDelete("{claimId}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteClaimById(long claimId)
        {
            try
            {
                this.databaseContext.UserClaims.Remove(new UserClaim { Id = claimId });
                await this.databaseContext.SaveChangesAsync();
                return Ok(claimId);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }
    }
}
