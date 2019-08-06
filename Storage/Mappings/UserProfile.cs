namespace Storage.Mappings
{
    using AutoMapper;

    using Storage.Database;
    using Storage.Dto;

    /// <summary>
    /// The user profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class UserProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfile"/> class.
        /// </summary>
        public UserProfile()
        {
            this.CreateMap<User, DtoReadUser>();
            this.CreateMap<DtoReadUser, User>();
            this.CreateMap<User, DtoCreateUpdateUser>();
            this.CreateMap<DtoCreateUpdateUser, User>();
            this.CreateMap<DtoReadUser, DtoCreateUpdateUser>();
            this.CreateMap<DtoCreateUpdateUser, DtoReadUser>();
        }
    }
}