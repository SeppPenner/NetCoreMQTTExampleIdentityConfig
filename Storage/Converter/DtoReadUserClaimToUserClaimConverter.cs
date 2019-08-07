namespace Storage.Converter
{
    using System.Diagnostics.CodeAnalysis;

    using AutoMapper;

    using Newtonsoft.Json;

    using Storage.Database;
    using Storage.Dto;

    /// <summary>
    ///     Converts a <see cref="DtoReadUserClaim" /> to a <see cref="UserClaim" />.
    /// </summary>
    /// <seealso cref="AutoMapper.ITypeConverter{DtoCreateUpdateUserClaim, UserClaim}" />
    public class DtoReadUserClaimToUserClaimConverter : ITypeConverter<DtoReadUserClaim, UserClaim>
    {
        /// <summary>
        ///     Performs conversion from source to destination type.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="destination">Destination object.</param>
        /// <param name="context">Resolution context.</param>
        /// <returns>
        ///     Destination object.
        /// </returns>
        [SuppressMessage(
            "StyleCop.CSharp.DocumentationRules",
            "SA1625:ElementDocumentationMustNotBeCopiedAndPasted",
            Justification = "Reviewed. Suppression is OK here.")]
        public UserClaim Convert(DtoReadUserClaim source, UserClaim destination, ResolutionContext context)
        {
            return new UserClaim
                   {
                       Id = source.Id,
                       UserId = source.UserId,
                       ClaimType = source.ClaimType.ToString(),
                       ClaimValue = JsonConvert.SerializeObject(source.ClaimValues),
                       UpdatedAt = source.UpdatedAt,
                       CreatedAt = source.CreatedAt
                   };
        }
    }
}