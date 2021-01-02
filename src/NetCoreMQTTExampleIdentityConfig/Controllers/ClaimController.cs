// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClaimController.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The claim controller class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

    using NSwag.Annotations;

    using Serilog;

    using Storage;
    using Storage.Database;
    using Storage.Dto;

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
        private readonly IMapper autoMapper;

        /// <summary>
        ///     The database context.
        /// </summary>
        private readonly MqttContext databaseContext;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger = Log.ForContext<ClaimController>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClaimController" /> class.
        /// </summary>
        /// <param name="databaseContext">The database context.</param>
        /// <param name="autoMapper">The auto mapper service.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public ClaimController(MqttContext databaseContext, IMapper autoMapper)
        {
            this.databaseContext = databaseContext;
            this.autoMapper = autoMapper;
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
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DtoReadUserClaim>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DtoReadUserClaim>>> GetClaims()
        {
            try
            {
                this.logger.Information("Executed GetClaims.");

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
                this.logger.Fatal("An error occurred: {@Exception}.", ex);
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
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "Reviewed. Suppression is OK here.")]
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
                this.logger.Information("Executed GetClaimById with user claim identifier {@ClaimId}.", claimId);

                var claim = await this.databaseContext.UserClaims.FirstOrDefaultAsync(u => u.Id == claimId);

                if (claim == null)
                {
                    this.logger.Warning("User claim with identifier {@ClaimId} not found.", claimId);
                    return this.NotFound(claimId);
                }

                var returnUserClaim = this.autoMapper.Map<DtoReadUserClaim>(claim);
                return this.Ok(returnUserClaim);
            }
            catch (Exception ex)
            {
                this.logger.Fatal("An error occurred: {@Exception}.", ex);
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
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        [ProducesResponseType(typeof(DtoReadUserClaim), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateOrUpdateClaim([FromBody] DtoCreateUpdateUserClaim createUserClaim)
        {
            try
            {
                this.logger.Information("Executed CreateOrUpdateClaim with user claim {@CreateUserClaim}.", createUserClaim);

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
                this.logger.Fatal("An error occurred: {@Exception}.", ex);
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
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "Reviewed. Suppression is OK here.")]
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
                this.logger.Information("Executed UpdateClaim with user claim {@UpdateUserClaim} for user claim identifier: {@ClaimId}.", updateUserClaim, claimId);

                var resultClaim = await this.databaseContext.UserClaims.AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == claimId);

                if (resultClaim == null)
                {
                    this.logger.Warning("User claim with identifier {@ClaimId} not found.", claimId);
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
                this.logger.Fatal("An error occurred: {@Exception}.", ex);
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
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpDelete("{claimId:long}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteClaimById(long claimId)
        {
            try
            {
                this.logger.Information("Executed DeleteClaimById with user claim identifier {@ClaimId}.", claimId);

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
                this.logger.Fatal("An error occurred: {@Exception}.", ex);
                return this.InternalServerError(ex);
            }
        }
    }
}