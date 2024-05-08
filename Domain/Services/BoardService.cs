using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Domain.Utils.Languages;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Domain.Services
{
    public class BoardService() : IBoardService
    {
        public Task<string> Save(BoardRequest boardRequest)
        {
            throw new NotImplementedException();
        }
    }
}
