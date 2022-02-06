// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClaimToDtoReadUserClaimConverter.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   Converts a <see cref="UserClaim" /> to a <see cref="DtoReadUserClaim" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Converter
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;

    using Newtonsoft.Json;

    using Storage.Database;
    using Storage.Dto;
    using Storage.Enumerations;

    /// <summary>
    ///     Converts a <see cref="UserClaim" /> to a <see cref="DtoReadUserClaim" />.
    /// </summary>
    /// <seealso cref="ITypeConverter{TSource,TDestination}" />
    public class UserClaimToDtoReadUserClaimConverter : ITypeConverter<UserClaim, DtoReadUserClaim>
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
        public DtoReadUserClaim Convert(UserClaim source, DtoReadUserClaim destination, ResolutionContext context)
        {
            _ = Enum.TryParse(source.ClaimType, out ClaimType enumValue);

            return new DtoReadUserClaim
            {
                ClaimType = enumValue,
                UserId = source.UserId,
                ClaimValues = JsonConvert.DeserializeObject<List<string>>(source.ClaimValue) ?? new List<string>(),
                CreatedAt = source.CreatedAt,
                Id = source.Id,
                UpdatedAt = source.UpdatedAt
            };
        }
    }
}