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
            this.CreateMap<UserClaim, DtoReadUserClaim>();
            this.CreateMap<DtoReadUserClaim, UserClaim>();
            this.CreateMap<UserClaim, DtoCreateUpdateUserClaim>();
            this.CreateMap<DtoCreateUpdateUserClaim, UserClaim>();
            this.CreateMap<DtoReadUserClaim, DtoCreateUpdateUserClaim>();
            this.CreateMap<DtoCreateUpdateUserClaim, DtoReadUserClaim>();
        }
    }
}