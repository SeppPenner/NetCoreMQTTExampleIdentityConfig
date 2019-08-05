namespace Storage.Mappings
{
    using AutoMapper;

    using Storage.Database;
    using Storage.Dto;

    /// <summary>
    /// The user claims profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class UserClaimsProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserClaimsProfile"/> class.
        /// </summary>
        public UserClaimsProfile()
        {
            CreateMap<UserClaim, DtoReadUserClaim>();
            CreateMap<DtoReadUserClaim, UserClaim>();
            CreateMap<UserClaim, DtoCreateUpdateUserClaim>();
            CreateMap<DtoCreateUpdateUserClaim, UserClaim>();
            CreateMap<DtoReadUserClaim, DtoCreateUpdateUserClaim>();
            CreateMap<DtoCreateUpdateUserClaim, DtoReadUserClaim>();
        }
    }
}