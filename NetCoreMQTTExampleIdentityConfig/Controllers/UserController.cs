
namespace NetCoreMQTTExampleIdentityConfig.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using NetCoreMQTTExampleIdentityConfig.Controllers.Extensions;

    using NSwag.Annotations;

    using Serilog;
    using Storage;
    using Storage.Database;
    using Storage.Dto;

    /// <summary>
    /// The user controller class.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("api/user")]
    [ApiController]
    [OpenApiTag("User", Description = "User management.")]
    public class UserController : ControllerBase
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
        /// The user manager.
        /// </summary>
        private readonly UserManager<User> userManager;

        /// <summary>
        /// The password hasher.
        /// </summary>
        private readonly PasswordHasher<User> passwordHasher;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="databaseContext">The database context.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="autoMapper">The automapper service.</param>
        // ReSharper disable once StyleCop.SA1650
        public UserController(MqttContext databaseContext, UserManager<User> userManager, IMapper autoMapper)
        {
            this.databaseContext = databaseContext;
            this.userManager = userManager;
            this.autoMapper = autoMapper;
            this.passwordHasher = new PasswordHasher<User>();
        }

        /// <summary>
        /// Gets the users. GET "api/user".
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<DtoReadUser>), Description = "Users found.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(string), Description = "Internal server error.")]
        [ProducesResponseType(typeof(IEnumerable<DtoReadUser>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DtoReadUser>>> GetUsers()
        {
            try
            {
                Log.Information("Executed GetUsers().");

                var users = await this.databaseContext.Users.ToListAsync();

                if (users?.Count == 0)
                {
                    return this.Ok("[]");
                }

                var returnUsers = this.autoMapper.Map<IEnumerable<DtoReadUser>>(users);
                return this.Ok(returnUsers);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Gets the user by id. GET "api/user/5".
        /// </summary>
        /// <param name="userId">
        /// The user identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet("{userId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(DtoReadUser), Description = "User found.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(int), Description = "User not found.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(string), Description = "Internal server error.")]
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoReadUser>> GetUserById(long userId)
        {
            try
            {
                Log.Information($"Executed GetUserById({userId}).");

                var user = await this.databaseContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    Log.Warning($"User with identifier {userId} not found.");
                    return this.NotFound(userId);
                }

                var returnUser = this.autoMapper.Map<DtoReadUser>(user);
                return this.Ok(returnUser);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Creates the user. POST "api/user".
        /// </summary>
        /// <param name="createUser">
        /// The create User.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(DtoReadUser), Description = "User created.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(string), Description = "Internal server error.")]
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateUser([FromBody] DtoCreateUpdateUser createUser)
        {
            try
            {
                Log.Information($"Executed CreateUser({createUser}).");

                var user = this.autoMapper.Map<User>(createUser);
                user.CreatedAt = DateTimeOffset.Now;
                var identityResult = await this.userManager.CreateAsync(user, createUser.Password);
                await this.databaseContext.SaveChangesAsync();

                if (identityResult.Succeeded)
                {
                    var returnUser = this.autoMapper.Map<DtoReadUser>(user);
                    return this.Ok(returnUser);
                }

                var identityErrors = identityResult.Errors.Select(e => new IdentityErrorExt(e));
                var identityErrorList = identityErrors.ToList();
                Log.Fatal("Error with Asp.Net Core Identity: ", string.Join(";", identityErrorList));
                return this.InternalServerError(identityErrorList);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Updates the user. PUT "api/user/5".
        /// </summary>
        /// <param name="userId">
        /// The user identifier.
        /// </param>
        /// <param name="updateUser">
        /// The update User.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpPut("{userId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(DtoReadUser), Description = "User updated.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(int), Description = "User not found.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(string), Description = "Internal server error.")]
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser(long userId, [FromBody] DtoCreateUpdateUser updateUser)
        {
            try
            {
                Log.Information($"Executed UpdateUser({updateUser}) for user identifier: {userId}.");

                var resultUser = await this.databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(b => b.Id == userId);

                if (resultUser == null)
                {
                    Log.Warning($"User with identifier {userId} not found.");
                    return this.NotFound(userId);
                }

                var concurrencyStamp = resultUser.ConcurrencyStamp;
                var createdAt = resultUser.CreatedAt;
                resultUser = this.autoMapper.Map<User>(updateUser);
                resultUser.UpdatedAt = DateTimeOffset.Now;
                resultUser.PasswordHash = this.passwordHasher.HashPassword(resultUser, updateUser.Password);
                resultUser.SecurityStamp = new Guid().ToString();
                resultUser.ConcurrencyStamp = concurrencyStamp;
                resultUser.CreatedAt = createdAt;
                resultUser.Id = userId;

                var identityResult = await this.userManager.UpdateAsync(resultUser);
                await this.databaseContext.SaveChangesAsync();

                if (identityResult.Succeeded)
                {
                    var returnUser = this.autoMapper.Map<DtoReadUser>(resultUser);
                    returnUser.Id = userId;
                    return this.Ok(returnUser);
                }

                var identityErrors = identityResult.Errors.Select(e => new IdentityErrorExt(e));
                var identityErrorList = identityErrors.ToList();
                Log.Fatal("Error with Asp.Net Core Identity: ", string.Join(";", identityErrorList));
                return this.InternalServerError(identityErrorList);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Deletes the user by id. DELETE "api/user/5".
        /// </summary>
        /// <param name="userId">
        /// The user identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpDelete("{userId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(int), Description = "User deleted.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(string), Description = "Internal server error.")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUserById(long userId)
        {
            try
            {
                Log.Information($"Executed DeleteUserById({userId}).");

                var user = await this.databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return this.Ok(userId);
                }

                this.databaseContext.Users.Remove(user);
                await this.databaseContext.SaveChangesAsync();
                return this.Ok(userId);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }
    }
}
