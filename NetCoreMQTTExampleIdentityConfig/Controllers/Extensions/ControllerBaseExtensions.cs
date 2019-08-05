
namespace NetCoreMQTTExampleIdentityConfig.Controllers.Extensions
{
    using Microsoft.AspNetCore.Mvc;
    using System;

    /// <summary>
    /// Some extension methods for the <see cref="ControllerBase"></see> class.
    /// </summary>
    public static class ControllerBaseExtensions
    {
        public static ObjectResult InternalServerError(this ControllerBase controllerBase, Exception ex)
        {
            return controllerBase.StatusCode(500, $"{ex.Message}{ex.StackTrace}");
        }
    }
}