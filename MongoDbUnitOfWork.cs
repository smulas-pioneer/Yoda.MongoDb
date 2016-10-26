using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yoda.Common.Interfaces;
using Yoda.MongoDb.Infrastucture;

namespace Yoda.MongoDb
{
    public class MongoDbUnitOfWork : IDocumentUnitOfWork
    {
        /// <summary>
        /// Db Context corrente
        /// </summary>
        protected readonly MongoDatabase _database = null;

        private readonly IQuerySpecFactory _querySpecFactory = null;


        #region Ctor
        public MongoDbUnitOfWork(IMongoDbFactory<MongoDatabase> dbFactory)
        : this(dbFactory, null) { }
        public MongoDbUnitOfWork(IMongoDbFactory<MongoDatabase> dbFactory, IQuerySpecFactory querySpecFactory)
        : this(dbFactory.Create(null), querySpecFactory) { }

        public MongoDbUnitOfWork(MongoDatabase context)
        : this(context, null) { }

        public MongoDbUnitOfWork(MongoDatabase context, IQuerySpecFactory querySpecFactory)
        {
            this._database = context;
            this._querySpecFactory = querySpecFactory;
            this._database.Server.Ping(); // check connectivity
        }

        #endregion

        public virtual TQuerySpec GetQuerySpec<TQuerySpec>() where TQuerySpec : class, IQuerySpec
        {
            if (this._querySpecFactory == null)
                throw new NotImplementedException("Must specify query spec factory!");
            return this._querySpecFactory.Create<TQuerySpec>(new object[] { this._database });
        }


        /// <summary>
        /// Disposes all external resources.
        /// </summary>
        /// <param name="disposing">The dispose indicator.</param>
        private bool _disposed = false;
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_database != null)
                    {
                        _database.Server.Disconnect();
                        _disposed = true;
                        disposing = false;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TEntity GetByKey<TEntity>(string collectionName, object id) where TEntity : class
        {
            var ent = _database.GetCollection(collectionName).FindOneByIdAs<TEntity>(BsonValue.Create(id));
            return ent;
        }

        public long Count<TEntity>(string collectionName, Expression<Func<TEntity, bool>> filter = null) where TEntity : class
        {
            IQueryable<TEntity> query = _database.GetCollection(collectionName).AsQueryable<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.LongCount();
        }

        public IEnumerable<TEntity> FindAll<TEntity>(string collectionName,
                                              Expression<Func<TEntity, bool>> filter = null,
                                              int? pageIndex = null,
                                              int? pageSize = null,
                                              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
                                              ) where TEntity : class
        {
            return FindAll(collectionName, filter, orderBy, pageIndex, pageSize);
        }

        public IEnumerable<TEntity> FindAll<TEntity>(string collectionName, Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? pageIndex = default(int?), int? pageSize = default(int?)) where TEntity : class
        {
            IQueryable<TEntity> query = _database.GetCollection(collectionName).AsQueryable<TEntity>();
            if (filter != null) query = query.Where(filter);

            if (orderBy != null) query = orderBy(query);
            if (pageIndex != null)
            {
                query = query.Skip(((int)pageIndex - 1) * (int)pageSize);
                if (orderBy != null) query = orderBy(query); // fix wrong order when paging
            }
            if (pageSize != null) query = query.Take((int)pageSize);

            return query.ToList();
        }

        public void Add<TEntity>(string collectionName, IEnumerable<TEntity> entities) where TEntity : class
        {
            var newEntity = this._database.GetCollection(collectionName);
            newEntity.InsertBatch<TEntity>(entities);
        }

        public void Add<TEntity>(string collectionName, params TEntity[] entities) where TEntity : class
        {
            Add<TEntity>(collectionName, entities.AsEnumerable());
        }

        public void Update<TEntity>(string collectionName, IEnumerable<TEntity> items) where TEntity : class
        {
            var colSet = this._database.GetCollection(collectionName);
            foreach (var item in items)
                colSet.Save<TEntity>(item);
        }

        public void Update<TEntity>(string collectionName, params TEntity[] entities) where TEntity : class
        {
            Add<TEntity>(collectionName, entities.AsEnumerable());
        }

        public void DeleteByKey<TEntity>(string collectionName, object id) where TEntity : class
        {
            var colSet = this._database.GetCollection(collectionName);
            colSet.Remove(Query.EQ("_id", id.ToString()));
        }
    }
}