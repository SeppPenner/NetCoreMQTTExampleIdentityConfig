
namespace NetCoreMQTTExampleIdentityConfig.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using NetCoreMQTTExampleIdentityConfig.Controllers.Extensions;

    using Storage;
    using Storage.Database;
    using Storage.Dto;

    /// <summary>
    /// The user controller class.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("api/user")]
    [ApiController]
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
        [ProducesResponseType(typeof(IEnumerable<DtoReadUser>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DtoReadUser>>> GetUsers()
        {
            try
            {
                var users = await this.databaseContext.Users.ToListAsync();
                var returnUsers = this.autoMapper.Map<IEnumerable<DtoReadUser>>(users);
                return this.Ok(returnUsers);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Gets the user by id. GET "api/user/5".
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoReadUser>> GetUserById(long userId)
        {
            try
            {
                var user = await this.databaseContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return this.NotFound(userId);
                }

                var returnUser = this.autoMapper.Map<DtoReadUser>(user);
                return this.Ok(returnUser);
            }
            catch (Exception ex)
            {
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
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateUser([FromBody] DtoCreateUpdateUser createUser)
        {
            try
            {
                var user = this.autoMapper.Map<User>(createUser);
                user.CreatedAt = DateTimeOffset.Now;
                await this.userManager.CreateAsync(user, createUser.Password);
                var returnUser = this.autoMapper.Map<DtoReadUser>(createUser);
                return this.Ok(returnUser);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Updates the user. PUT "api/user/5".
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="updateUser">
        /// The update User.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser(long userId, [FromBody] DtoCreateUpdateUser updateUser)
        {
            try
            {
                var resultUser = await this.databaseContext.Users.FirstOrDefaultAsync(b => b.Id == userId);

                if (resultUser == null)
                {
                    return this.NotFound(userId);
                }

                resultUser = this.autoMapper.Map<User>(updateUser);
                resultUser.UpdatedAt = DateTimeOffset.Now;
                resultUser.PasswordHash = this.passwordHasher.HashPassword(resultUser, updateUser.Password);
                await this.userManager.UpdateAsync(resultUser);
                var returnUser = this.autoMapper.Map<DtoReadUser>(updateUser);
                return this.Ok(returnUser);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// Deletes the user by id. DELETE "api/user/5".
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [HttpDelete("{userId}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUserById(long userId)
        {
            try
            {
                this.databaseContext.Users.Remove(new User { Id = userId });
                await this.databaseContext.SaveChangesAsync();
                return this.Ok(userId);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }
    }
}
