using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

namespace NetCoreMQTTExampleIdentityConfig.Controllers
{
    /// <summary>
    ///     The user controller class.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("api/user")]
    [ApiController]
    [OpenApiTag("User", Description = "User management.")]
    public class UserController : ControllerBase
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
        ///     The password hasher.
        /// </summary>
        private readonly PasswordHasher<User> _passwordHasher;

        /// <summary>
        ///     The user manager.
        /// </summary>
        private readonly UserManager<User> _userManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="databaseContext">The database context.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="autoMapper">The auto mapper service.</param>
        // ReSharper disable once StyleCop.SA1650
        public UserController(MqttContext databaseContext, UserManager<User> userManager, IMapper autoMapper)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
            _autoMapper = autoMapper;
            _passwordHasher = new PasswordHasher<User>();
        }

        /// <summary>
        ///     Gets all users.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Gets all users.
        /// </remarks>
        /// <response code="200">Users found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DtoReadUser>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DtoReadUser>>> GetUsers()
        {
            try
            {
                Log.Information("Executed GetUsers().");

                var users = await _databaseContext.Users.ToListAsync();

                if (users?.Count == 0) return Ok("[]");

                var returnUsers = _autoMapper.Map<IEnumerable<DtoReadUser>>(users);
                return Ok(returnUsers);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Gets a user by their id.
        /// </summary>
        /// <param name="userId">
        ///     The user identifier.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Gets a user by their id.
        /// </remarks>
        /// <response code="200">User found.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet("{userId:long}")]
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoReadUser>> GetUserById(long userId)
        {
            try
            {
                Log.Information($"Executed GetUserById({userId}).");

                var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    Log.Warning($"User with identifier {userId} not found.");
                    return NotFound(userId);
                }

                var returnUser = _autoMapper.Map<DtoReadUser>(user);
                return Ok(returnUser);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Creates a user.
        /// </summary>
        /// <param name="createUser">
        ///     The create user.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Creates a user.
        /// </remarks>
        /// <response code="200">User created.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateUser([FromBody] DtoCreateUpdateUser createUser)
        {
            try
            {
                Log.Information($"Executed CreateUser({createUser}).");

                var user = _autoMapper.Map<User>(createUser);
                user.CreatedAt = DateTimeOffset.Now;
                var identityResult = await _userManager.CreateAsync(user, createUser.Password);
                await _databaseContext.SaveChangesAsync();

                if (identityResult.Succeeded)
                {
                    var returnUser = _autoMapper.Map<DtoReadUser>(user);
                    return Ok(returnUser);
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
        ///     Updates a user.
        /// </summary>
        /// <param name="userId">
        ///     The user identifier.
        /// </param>
        /// <param name="updateUser">
        ///     The update user.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Updates a user.
        /// </remarks>
        /// <response code="200">User updated.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpPut("{userId:long}")]
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser(long userId, [FromBody] DtoCreateUpdateUser updateUser)
        {
            try
            {
                Log.Information($"Executed UpdateUser({updateUser}) for user identifier: {userId}.");

                var resultUser = await _databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(b => b.Id == userId);

                if (resultUser == null)
                {
                    Log.Warning($"User with identifier {userId} not found.");
                    return NotFound(userId);
                }

                var concurrencyStamp = resultUser.ConcurrencyStamp;
                var createdAt = resultUser.CreatedAt;
                resultUser = _autoMapper.Map<User>(updateUser);
                resultUser.UpdatedAt = DateTimeOffset.Now;
                resultUser.PasswordHash = _passwordHasher.HashPassword(resultUser, updateUser.Password);
                resultUser.SecurityStamp = new Guid().ToString();
                resultUser.ConcurrencyStamp = concurrencyStamp;
                resultUser.CreatedAt = createdAt;
                resultUser.Id = userId;

                var identityResult = await _userManager.UpdateAsync(resultUser);
                await _databaseContext.SaveChangesAsync();

                if (identityResult.Succeeded)
                {
                    var returnUser = _autoMapper.Map<DtoReadUser>(resultUser);
                    returnUser.Id = userId;
                    return Ok(returnUser);
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
        ///     Deletes the user by their id.
        /// </summary>
        /// <param name="userId">
        ///     The user identifier.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Deletes a user by their id.
        /// </remarks>
        /// <response code="200">User deleted.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpDelete("{userId:long}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUserById(long userId)
        {
            try
            {
                Log.Information($"Executed DeleteUserById({userId}).");

                var user = await _databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return Ok(userId);

                _databaseContext.Users.Remove(user);
                await _databaseContext.SaveChangesAsync();
                return Ok(userId);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }
    }
}