// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClaimsProfile.cs" company="Haemmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The user claims profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Mappings
{
    using AutoMapper;

    using Storage.Converter;
    using Storage.Database;
    using Storage.Dto;

    /// <summary>
    ///     The user claims profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class UserClaimsProfile : Profile
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserClaimsProfile" /> class.
        /// </summary>
        public UserClaimsProfile()
        {
            this.CreateMap<UserClaim, DtoReadUserClaim>().ConvertUsing(new UserClaimToDtoReadUserClaimConverter());
            this.CreateMap<DtoReadUserClaim, UserClaim>().ConvertUsing(new DtoReadUserClaimToUserClaimConverter());
            this.CreateMap<UserClaim, DtoCreateUpdateUserClaim>()
                .ConvertUsing(new UserClaimToDtoCreateUpdateUserClaimConverter());
            this.CreateMap<DtoCreateUpdateUserClaim, UserClaim>()
                .ConvertUsing(new DtoCreateUpdateUserClaimToUserClaimConverter());
            this.CreateMap<DtoReadUserClaim, DtoCreateUpdateUserClaim>();
            this.CreateMap<DtoCreateUpdateUserClaim, DtoReadUserClaim>();
        }
    }
}