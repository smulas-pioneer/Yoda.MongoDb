using Yoda.MongoDb.Infrastucture;

namespace Yoda.MongoDb
{
    public abstract class MongoDbAbstractFactory<TDatabase> : IMongoDbFactory<TDatabase> where TDatabase : MongoDatabase
    {
        // private readonly DbContextOptions<TContext> _dboptions;
        // private readonly DbContextFactoryOptions _facOptions;

        public MongoDbAbstractFactory(DbContextOptions<TContext> dbOptions) : this(null, dbOptions) { }
        public MongoDbAbstractFactory(DbContextFactoryOptions facOptions, DbContextOptions<TContext> dbOptions)
        {
            this._dboptions = dbOptions;
            this._facOptions = facOptions;
        }

        public TDatabase Create(DbContextFactoryOptions options)
        {
            return InternalCreate(options ?? this._facOptions, this._dboptions);
        }

        public TDatabase Create()
        {
            return InternalCreate(this._facOptions, this._dboptions);
        }

        protected abstract TDatabase InternalCreate(MongoDbFactoryOptions facOptions, DbContextOptions<TContext> dbOptions);

    }
}