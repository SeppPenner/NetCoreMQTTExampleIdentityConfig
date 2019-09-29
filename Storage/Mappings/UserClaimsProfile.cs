using AutoMapper;
using Storage.Converter;
using Storage.Database;
using Storage.Dto;

namespace Storage.Mappings
{
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
            CreateMap<UserClaim, DtoReadUserClaim>().ConvertUsing(new UserClaimToDtoReadUserClaimConverter());
            CreateMap<DtoReadUserClaim, UserClaim>().ConvertUsing(new DtoReadUserClaimToUserClaimConverter());
            CreateMap<UserClaim, DtoCreateUpdateUserClaim>()
                .ConvertUsing(new UserClaimToDtoCreateUpdateUserClaimConverter());
            CreateMap<DtoCreateUpdateUserClaim, UserClaim>()
                .ConvertUsing(new DtoCreateUpdateUserClaimToUserClaimConverter());
            CreateMap<DtoReadUserClaim, DtoCreateUpdateUserClaim>();
            CreateMap<DtoCreateUpdateUserClaim, DtoReadUserClaim>();
        }
    }
}