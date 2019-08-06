namespace Storage.Converter
{
    using AutoMapper;
    using Newtonsoft.Json;
    using Storage.Database;
    using Storage.Dto;
    using Storage.Enumerations;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Converts a <see cref="UserClaim"/> to a <see cref="DtoReadUserClaim"/>.
    /// </summary>
    /// <seealso cref="AutoMapper.ITypeConverter{UserClaim, DtoReadUserClaim}" />
    public class UserClaimToDtoReadUserClaimConverter : ITypeConverter<UserClaim, DtoReadUserClaim>
    {
        /// <summary>
        /// Performs conversion from source to destination type.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="destination">Destination object.</param>
        /// <param name="context">Resolution context.</param>
        /// <returns>
        /// Destination object.
        /// </returns>
        public DtoReadUserClaim Convert(UserClaim source, DtoReadUserClaim destination, ResolutionContext context)
        {
            var parsed = Enum.TryParse(source.ClaimType, out ClaimType enumValue);

            return new DtoReadUserClaim
            {
                ClaimType = enumValue,
                UserId = source.UserId,
                ClaimValues = JsonConvert.DeserializeObject<List<string>>(source.ClaimValue),
                CreatedAt = source.CreatedAt,
                Id = source.Id,
                UpdatedAt = source.UpdatedAt
            };
        }
    }
}