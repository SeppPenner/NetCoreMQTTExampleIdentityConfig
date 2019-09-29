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
using NSwag.Annotations;
using Serilog;
using Storage;
using Storage.Database;
using Storage.Dto;

namespace NetCoreMQTTExampleIdentityConfig.Controllers
{
    /// <summary>
    ///     The claim controller class.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("api/claim")]
    [ApiController]
    [OpenApiTag("Claim", Description = "Claim management.")]
    public class ClaimController : ControllerBase
    {
        /// <summary>
        ///     The auto mapper.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        private readonly IMapper _autoMapper;

        /// <summary>
        ///     The database context.
        /// </summary>
        private readonly MqttContext _databaseContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClaimController" /> class.
        /// </summary>
        /// <param name="databaseContext">The database context.</param>
        /// <param name="autoMapper">The auto mapper service.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public ClaimController(MqttContext databaseContext, IMapper autoMapper)
        {
            _databaseContext = databaseContext;
            _autoMapper = autoMapper;
        }

        /// <summary>
        ///     Gets all claims.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Gets all claims.
        /// </remarks>
        /// <response code="200">Claims found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DtoReadUserClaim>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DtoReadUserClaim>>> GetClaims()
        {
            try
            {
                Log.Information("Executed GetClaims().");

                var claims = await _databaseContext.UserClaims.ToListAsync();

                if (claims?.Count == 0) return Ok("[]");

                var returnUserClaims = _autoMapper.Map<List<DtoReadUserClaim>>(claims);
                return Ok(returnUserClaims);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Gets a claim by its id.
        /// </summary>
        /// <param name="claimId">
        ///     The claim identifier.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Gets a claim by its id.
        /// </remarks>
        /// <response code="200">Claims found.</response>
        /// <response code="404">Claim not found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet("{claimId:long}")]
        [ProducesResponseType(typeof(DtoReadUserClaim), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoReadUserClaim>> GetClaimById(long claimId)
        {
            try
            {
                Log.Information($"Executed GetClaimById({claimId}).");

                var claim = await _databaseContext.UserClaims.FirstOrDefaultAsync(u => u.Id == claimId);

                if (claim == null)
                {
                    Log.Warning($"Claim with identifier {claimId} not found.");
                    return NotFound(claimId);
                }

                var returnUserClaim = _autoMapper.Map<DtoReadUserClaim>(claim);
                return Ok(returnUserClaim);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Creates or updates a claim.
        /// </summary>
        /// <param name="createUserClaim">
        ///     The create user claim.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Creates or updates a claim.
        /// </remarks>
        /// <response code="200">Claim created.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        [ProducesResponseType(typeof(DtoReadUserClaim), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateOrUpdateClaim([FromBody] DtoCreateUpdateUserClaim createUserClaim)
        {
            try
            {
                Log.Information($"Executed CreateOrUpdateClaim({createUserClaim}).");

                var claim = _autoMapper.Map<UserClaim>(createUserClaim);
                claim.CreatedAt = DateTimeOffset.Now;

                var foundClaim = await _databaseContext.UserClaims.FirstOrDefaultAsync(
                    uc => uc.ClaimType == claim.ClaimType && uc.UserId == claim.UserId);

                DtoReadUserClaim returnUserClaim;

                if (foundClaim == null)
                {
                    claim.CreatedAt = DateTimeOffset.Now;
                    await _databaseContext.UserClaims.AddAsync(claim);
                    await _databaseContext.SaveChangesAsync();
                    returnUserClaim = _autoMapper.Map<DtoReadUserClaim>(claim);
                }
                else
                {
                    foundClaim.UpdatedAt = DateTimeOffset.Now;
                    var currentClaimValue = JsonConvert.DeserializeObject<List<string>>(foundClaim.ClaimValue);
                    currentClaimValue.AddRange(createUserClaim.ClaimValues);
                    foundClaim.ClaimValue = JsonConvert.SerializeObject(currentClaimValue.Distinct());
                    _databaseContext.UserClaims.Update(foundClaim);
                    await _databaseContext.SaveChangesAsync();
                    returnUserClaim = _autoMapper.Map<DtoReadUserClaim>(foundClaim);
                }

                return Ok(returnUserClaim);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Updates a claim.
        /// </summary>
        /// <param name="claimId">
        ///     The claim identifier.
        /// </param>
        /// <param name="updateUserClaim">
        ///     The update user claim.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Updates a claim.
        /// </remarks>
        /// <response code="200">Claim updated.</response>
        /// <response code="404">Claim not found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpPut("{claimId:long}")]
        [ProducesResponseType(typeof(DtoReadUserClaim), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateClaim(long claimId, [FromBody] DtoCreateUpdateUserClaim updateUserClaim)
        {
            try
            {
                Log.Information($"Executed UpdateClaim({updateUserClaim}) for claim identifier: {claimId}.");

                var resultClaim = await _databaseContext.UserClaims.AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == claimId);

                if (resultClaim == null)
                {
                    Log.Warning($"Claim with identifier {claimId} not found.");
                    return NotFound(claimId);
                }

                var createdAt = resultClaim.CreatedAt;
                resultClaim = _autoMapper.Map<UserClaim>(updateUserClaim);
                resultClaim.UpdatedAt = DateTimeOffset.Now;
                resultClaim.CreatedAt = createdAt;
                resultClaim.Id = claimId;
                _databaseContext.UserClaims.Update(resultClaim);
                await _databaseContext.SaveChangesAsync();
                var returnUserClaim = _autoMapper.Map<DtoReadUserClaim>(resultClaim);
                return Ok(returnUserClaim);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Deletes a claim by its id.
        /// </summary>
        /// <param name="claimId">
        ///     The claim identifier.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Deletes a claim by its id.
        /// </remarks>
        /// <response code="200">Claim deleted.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpDelete("{claimId:long}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteClaimById(long claimId)
        {
            try
            {
                Log.Information($"Executed DeleteClaimById({claimId}).");

                var claim = await _databaseContext.UserClaims.AsNoTracking().FirstOrDefaultAsync(c => c.Id == claimId);

                if (claim == null) return Ok(claimId);

                _databaseContext.UserClaims.Remove(claim);
                await _databaseContext.SaveChangesAsync();
                return Ok(claimId);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }
    }
}