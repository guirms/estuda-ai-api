using AutoMapper;
using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Domain.Utils.Helpers;

namespace Application.AutoMapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            UserMap();
        }

        #region User

        private void UserMap()
        {
            CreateMap<UserRequest, User>()
               .ForMember(u => u.Key, opts => opts.MapFrom(u => u.UserKey))
               .ForMember(u => u.InsertedAt, opts => opts.MapFrom(u => DateTime.Now));

            CreateMap<User, UserResultsResponse>()
               .ForMember(u => u.Document, opts => opts.MapFrom(u => u.Document.ToDocument()));
        }

        #endregion
    }
}
