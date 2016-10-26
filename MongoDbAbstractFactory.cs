using MongoDB.Driver;

namespace Yoda.MongoDb
{
    public abstract class MongoDbAbstractFactory : IMongoDbFactory
    {
        private readonly MongoDbConnectionOptions _dboptions;
        private readonly MongoDbFactoryOptions _facOptions;

        public MongoDbAbstractFactory(MongoDbConnectionOptions dbOptions) : this(null, dbOptions) { }
        public MongoDbAbstractFactory(MongoDbFactoryOptions facOptions, MongoDbConnectionOptions dbOptions)
        {
            // this._dboptions = dbOptions;
            this._facOptions = facOptions;
        }

        public MongoDatabase Create(MongoDbFactoryOptions options)
        {
            return InternalCreate(options ?? this._facOptions, this._dboptions);
        }

        public MongoDatabase Create()
        {
            return InternalCreate(this._facOptions, this._dboptions);
        }

        protected abstract MongoDatabase InternalCreate(MongoDbFactoryOptions facOptions, MongoDbConnectionOptions dbOptions);

    }
}