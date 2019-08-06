
namespace NetCoreMQTTExampleIdentityConfig.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NetCoreMQTTExampleIdentityConfig.Controllers.Extensions;

    using Newtonsoft.Json;

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
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private readonly IMapper autoMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimController"/> class.
        /// </summary>
        /// <param name="databaseContext">The database context.</param>
        /// <param name="autoMapper">The automapper service.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public ClaimController(MqttContext databaseContext, IMapper autoMapper)
        {
            this.databaseContext = databaseContext;
            this.autoMapper = autoMapper;
        }

        /// <summary>
        /// Gets the claims. GET "api/claim".
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DtoReadUserClaim>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DtoReadUserClaim>>> GetClaims()
        {
            try
            {
                var claims = await this.databaseContext.UserClaims.ToListAsync();

                if (claims?.Count == 0)
                {
                    return this.Ok("[]");
                }

                var returnUserClaims = this.autoMapper.Map<List<DtoReadUserClaim>>(claims);
                return this.Ok(returnUserClaims);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Gets the claim by id. GET "api/claim/5".
        /// </summary>
        /// <param name="claimId">
        /// The claim Id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
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
                    return this.NotFound(claimId);
                }

                var returnUserClaim = this.autoMapper.Map<DtoReadUserClaim>(claim);
                return this.Ok(returnUserClaim);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Creates the claim. POST "api/claim".
        /// </summary>
        /// <param name="createUserClaim">
        /// The create User Claim.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        [ProducesResponseType(typeof(DtoReadUserClaim), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateOrUpdateClaim([FromBody] DtoCreateUpdateUserClaim createUserClaim)
        {
            try
            {
                var claim = this.autoMapper.Map<UserClaim>(createUserClaim);
                claim.CreatedAt = DateTimeOffset.Now;

                var foundClaim = await this.databaseContext.UserClaims.FirstOrDefaultAsync(
                    uc => uc.ClaimType == claim.ClaimType && uc.UserId == claim.UserId);

                DtoReadUserClaim returnUserClaim;

                if (foundClaim == null)
                {
                    claim.CreatedAt = DateTimeOffset.Now;
                    await this.databaseContext.UserClaims.AddAsync(claim);
                    await this.databaseContext.SaveChangesAsync();
                    returnUserClaim = this.autoMapper.Map<DtoReadUserClaim>(claim);
                }
                else
                {
                    foundClaim.UpdatedAt = DateTimeOffset.Now;
                    var currentClaimValue = JsonConvert.DeserializeObject<List<string>>(foundClaim.ClaimValue);
                    currentClaimValue.AddRange(createUserClaim.ClaimValues);
                    foundClaim.ClaimValue = JsonConvert.SerializeObject(currentClaimValue.Distinct());
                    this.databaseContext.UserClaims.Update(foundClaim);
                    await this.databaseContext.SaveChangesAsync();
                    returnUserClaim = this.autoMapper.Map<DtoReadUserClaim>(foundClaim);
                }

                return this.Ok(returnUserClaim);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Updates the claim. PUT "api/claim/5".
        /// </summary>
        /// <param name="claimId">
        /// The claim Id.
        /// </param>
        /// <param name="updateUserClaim">
        /// The update User Claim.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpPut("{claimId}")]
        [ProducesResponseType(typeof(DtoReadUserClaim), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateClaim(long claimId, [FromBody] DtoCreateUpdateUserClaim updateUserClaim)
        {
            try
            {
                var resultClaim = await this.databaseContext.UserClaims.AsNoTracking().FirstOrDefaultAsync(b => b.Id == claimId);

                if (resultClaim == null)
                {
                    return this.NotFound(claimId);
                }

                var createdAt = resultClaim.CreatedAt;
                resultClaim = this.autoMapper.Map<UserClaim>(updateUserClaim);
                resultClaim.UpdatedAt = DateTimeOffset.Now;
                resultClaim.CreatedAt = createdAt;
                resultClaim.Id = claimId;
                this.databaseContext.UserClaims.Update(resultClaim);
                await this.databaseContext.SaveChangesAsync();
                var returnUserClaim = this.autoMapper.Map<DtoReadUserClaim>(resultClaim);
                return this.Ok(returnUserClaim);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Deletes the claim by id. DELETE "api/claim/5".
        /// </summary>
        /// <param name="claimId">
        /// The claim Id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpDelete("{claimId}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteClaimById(long claimId)
        {
            try
            {
                var claim = await this.databaseContext.UserClaims.AsNoTracking().FirstOrDefaultAsync(c => c.Id == claimId);

                if (claim == null)
                {
                    return this.Ok(claimId);
                }

                this.databaseContext.UserClaims.Remove(claim);
                await this.databaseContext.SaveChangesAsync();
                return this.Ok(claimId);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }
    }
}
