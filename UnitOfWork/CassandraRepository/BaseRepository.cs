using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseDataModel;
using Cassandra;
using Cassandra.Data.Linq;
using Repository;
using UnitOfWork;

namespace CassandraRepository
{
    public class BaseRepository<TEntity> : IRepositoryReader<TEntity>, IRepositoryReaderAsync<TEntity>,
    IRepositoryWriter<TEntity>, IRepositoryWriterAsync<TEntity> where TEntity : class, IDataModel, new()
    {
        protected readonly ISession Session;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentException("IUnitOfWork cannot be null");
            if (!(unitOfWork is CassandraUnitOfWork.CassandraUnitOfWork))
                throw new ArgumentException("IUnitOfWork is not implemented by CassandraUnitOfWork class");

            this.Session = ((CassandraUnitOfWork.CassandraUnitOfWork)unitOfWork).Session;

            this.Table = this.Session.GetTable<TEntity>();

            this.Table.CreateIfNotExists(); //TODO is this a right place?
        }

        public Table<TEntity> Table { get; set; }

        public IEnumerable<TEntity> GetAll()
        {
            return this.Table.Select(i => i).Execute();
        }

        public TEntity GetById(Guid id)
        {
            return this.Table.First(entity => entity.Id == id).Execute();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await this.Table.Select(i => i).ExecuteAsync();

        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await this.Table.First(entity => entity.Id == id).ExecuteAsync();
        }

        public TEntity Delete(Guid id)
        {
            var item = this.GetById(id);
            if (item == null)
                throw new ArgumentException($"Item with {id} was not found, thus cannot be deleted.");

            this.Delete(item);
            return item;
        }

        public TEntity Delete(TEntity item)
        {
            if (item == null)
                throw new ArgumentNullException("TEntity item cannot be null.");

            this.Table.Where(i => i.Id == item.Id)
                .Delete()
                .Execute();
            return item;
        }

        public TEntity Insert(TEntity item)
        {
            if (item == null)
                throw new ArgumentNullException("TEntity item cannot be null.");

            this.Table.Insert(item).Execute();
            return item;
        }

        public IEnumerable<TEntity> InsertRange(IEnumerable<TEntity> items)
        {
            if (items == null)
                throw new ArgumentNullException("TEntity item cannot be null.");
            // Todo rewrite to appropriate bulk implementation
            var insertRange = items as TEntity[] ?? items.ToArray();
            foreach (var item in insertRange) this.Insert(item);

            return insertRange;
        }

        public void Update(TEntity item)
        {
            if (item == null)
                throw new ArgumentNullException("TEntity item cannot be null.");

            this.Delete(item);
            this.Insert(item);
        }

        public async Task<TEntity> DeleteAsync(Guid id)
        {
            var item = await this.GetByIdAsync(id);
            if (item == null)
                throw new ArgumentException($"Item with {id} was not found, thus cannot be deleted.");

            await this.DeleteAsync(item);
            return item;
        }

        public async Task<TEntity> DeleteAsync(TEntity item)
        {
            if (item == null)
                throw new ArgumentNullException("TEntity item cannot be null.");

            var result = await this.Table.Where(i => i.Id == item.Id)
                .Delete()
                .ExecuteAsync();
            return item;
        }

        public async Task<TEntity> InsertAsync(TEntity item)
        {
            if (item == null)
                throw new ArgumentNullException("TEntity item cannot be null.");

            await this.Table.Insert(item).ExecuteAsync();
            return item;
        }

        public async Task<IEnumerable<TEntity>> InsertRangeAsync(IEnumerable<TEntity> items)
        {
            if (items == null)
                throw new ArgumentNullException("TEntity item cannot be null.");

            // Todo rewrite to appropriate bulk implementation
            var insertRange = items as TEntity[] ?? items.ToArray();
            foreach (var item in insertRange) await this.InsertAsync(item);

            return insertRange;
        }

        public async Task UpdateAsync(TEntity item)
        {
            if (item == null)
                throw new ArgumentNullException("TEntity item cannot be null.");

            await this.DeleteAsync(item);
            await this.InsertAsync(item);
        }
    }

}
