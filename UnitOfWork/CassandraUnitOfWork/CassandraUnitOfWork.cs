using System;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using UnitOfWork;

namespace CassandraUnitOfWork
{
    public class CassandraUnitOfWork : IUnitOfWork
    {
        private static Boolean _setUpDone;

        public CassandraUnitOfWork(ICluster cluster, Mappings mappings)
        {
            DoOneTimeSetUp(mappings);
            this.Session = cluster.Connect();
        }

        public ISession Session { get; }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {

        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.Session?.Dispose();
        }

        private static void DoOneTimeSetUp(Mappings mappings)
        {
            if (_setUpDone) return;

            MappingConfiguration.Global.Define(mappings);
            _setUpDone = true;
        }
    }
}
