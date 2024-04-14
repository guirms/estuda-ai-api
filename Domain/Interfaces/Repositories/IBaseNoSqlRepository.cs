using Domain.Models.Bases;
using MongoDB.Bson;

namespace Domain.Interfaces.Repositories
{
    public interface IBaseNoSqlRepository<T> where T : BaseNoSqlModel
    {
        Task Save(T modelObject);
        Task SaveMany(IEnumerable<T> modelList);
        Task Update(T modelObject);
        Task UpdateMany(IEnumerable<T> modelList);
        Task<IEnumerable<T>?> GetAll();
        Task<T?> GetById(ObjectId id);
        Task Delete(ObjectId id);
        Task StartTransaction();
        Task CommitTransaction();
        Task AbortTransaction();
    }
}
