
namespace NetCoreMQTTExampleIdentityConfig.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Storage;
    using Storage.Database;
    using Storage.Dto;

    /// <summary>
    /// The user controller class.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// The MQTT settings.
        /// </summary>
        private readonly IOptions<MqttSettings> mqttSettings;

        /// <summary>
        /// The database context.
        /// </summary>
        private readonly DbContext databaseContext;

        /// <summary>
        /// The user manager.
        /// </summary>
        private readonly UserManager<User> userManager;

        /// <summary>
        /// The password hasher.
        /// </summary>
        private readonly IPasswordHasher<User> passwordHasher;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="mqttSettings">The MQTT settings.</param>
        public UserController(IOptions<MqttSettings> mqttSettings, DbContext databaseContext, UserManager<User> userManager, PasswordHasher<User> passwordHasher)
        {
            this.mqttSettings = mqttSettings;
            this.databaseContext = databaseContext;
            this.userManager = userManager;
            this.passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Gets the users. GET "api/user".
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return this.databaseContext.Users.ToList();
        }

        /// <summary>
        /// Gets the user by id. GET "api/user/5".
        /// </summary>
        [HttpGet("{userId}")]
        public ActionResult<User> GetUserById(long userId)
        {
            return this.databaseContext.Users.FirstOrDefault(u => u.Id == userId);
        }

        /// <summary>
        /// Creates the user. POST "api/user".
        /// </summary>
        [HttpPost]
        public async Task CreateUser([FromBody] UserModel userModel)
        {
            var user = new User { UserName = userModel.UserName };
            await this.userManager.CreateAsync(user, userModel.Password);
        }

        /// <summary>
        /// Updates the user. PUT "api/user/5".
        /// </summary>
        [HttpPut("{userId}")]
        public async Task UpdateUser(long userId, [FromBody] UserModel userModel)
        {
            var resultUser = this.databaseContext.Users.FirstOrDefault(b => b.Id == userId);
            if (resultUser != null)
            {
                resultUser.UserName = userModel.UserName;
                resultUser.PasswordHash = passwordHasher.HashPassword(resultUser, userModel.Password);
                await this.userManager.UpdateAsync(resultUser);
            }
        }

        /// <summary>
        /// Deletes the user by id. DELETE "api/user/5".
        /// </summary>
        [HttpDelete("{userId}")]
        public async Task DeleteUserById(long userId)
        {
            this.databaseContext.Users.Remove(new User { Id = userId });
            await this.databaseContext.SaveChangesAsync();
        }
    }
}
