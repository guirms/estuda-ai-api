using AutoMapper;
using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Domain.Objects.Responses.Board;
using Domain.Utils.Helpers;

namespace Application.AutoMapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            UserMap();
            BoardMap();
            CardMap();
        }

        #region User

        private void UserMap()
        {
            CreateMap<UserRequest, User>()
               .ForMember(u => u.InsertedAt, opts => opts.MapFrom(u => DateTime.Now));

            CreateMap<User, UserResultsResponse>()
               .ForMember(u => u.Document, opts => opts.MapFrom(u => u.Document.ToDocument()));
        }

        #endregion

        #region Board

        private void BoardMap()
        {
            CreateMap<SaveBoardRequest, Board>()
               .ForMember(b => b.DailyStudyTime, opts => opts.MapFrom(s => TimeSpan.Parse(s.DailyStudyTime)))
               .ForMember(b => b.InsertedAt, opts => opts.MapFrom(s => DateTime.Now));

            CreateMap<UpdateBoardRequest, Board>();

            CreateMap<Board, BoardResultsResponse>();
        }

        #endregion

        #region Card

        private void CardMap()
        {
            CreateMap<Card, CardResultsResponse>();
        }

        #endregion
    }
}
