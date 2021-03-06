﻿using APPartment.Data;
using APPartment.Enums;
using APPartment.Models;
using APPartment.Models.Declaration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace APPartment.Core
{
    public class DataContext<T>
        where T : class, IObject
    {
        private DataAccessContext context;
        private HistoryContext<T> historyContext;

        public DataContext(DataAccessContext context)
        {
            this.context = context;
            this.historyContext = new HistoryContext<T>(this.context);
        }

        public async Task SaveAsync(T objectModel, long userId, long? targetObjectId, long houseId)
        {
            var objectTypeName = objectModel.GetType().Name;
            var objectTypeId = context.Set<ObjectType>().Where(x => x.Name == objectTypeName).FirstOrDefault().Id;

            var _object = new Models.Object()
            {
                CreatedById = userId,
                ModifiedById = userId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ObjectTypeId = objectTypeId
            };

            await context.AddAsync(_object);
            await context.SaveChangesAsync();

            historyContext.PopulateHistory((int)HistoryFunctionTypes.Create, objectModel, _object, userId, targetObjectId, houseId);

            objectModel.ObjectId = _object.ObjectId;

            await context.AddAsync<T>(objectModel);
            await context.SaveChangesAsync();
        }

        public void Save(T objectModel, long userId, long? targetObjectId, long houseId)
        {
            var objectTypeName = objectModel.GetType().Name;
            var objectTypeId = context.Set<ObjectType>().Where(x => x.Name == objectTypeName).FirstOrDefault().Id;

            var _object = new Models.Object()
            {
                CreatedById = userId,
                ModifiedById = userId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ObjectTypeId = objectTypeId
            };

            context.Add(_object);
            context.SaveChanges();

            historyContext.PopulateHistory((int)HistoryFunctionTypes.Create, objectModel, _object, userId, targetObjectId, houseId);

            objectModel.ObjectId = _object.ObjectId;

            context.Add<T>(objectModel);
            context.SaveChanges();
        }

        public async Task UpdateAsync(T objectModel, long userId, long houseId)
        {
            var _object = context.Set<Models.Object>().Where(x => x.ObjectId == objectModel.ObjectId).FirstOrDefault();

            historyContext.PopulateHistory((int)HistoryFunctionTypes.Update, objectModel, _object, userId, null, houseId);

            _object.ModifiedById = userId;
            _object.ModifiedDate = DateTime.Now;

            context.Update(_object);
            await context.SaveChangesAsync();

            context.Update(objectModel);
            await context.SaveChangesAsync();
        }

        public void Update(T objectModel, long userId, long houseId)
        {
            var _object = context.Set<Models.Object>().Where(x => x.ObjectId == objectModel.ObjectId).FirstOrDefault();

            historyContext.PopulateHistory((int)HistoryFunctionTypes.Update, objectModel, _object, userId, null, houseId);

            _object.ModifiedById = userId;
            _object.ModifiedDate = DateTime.Now;

            context.Update(_object);
            context.SaveChanges();

            context.Update(objectModel);
            context.SaveChanges();
        }

        public async Task DeleteAsync(T objectModel, long userId, long? targetObjectId, long houseId)
        {
            var _object = context.Set<Models.Object>().Where(x => x.ObjectId == objectModel.ObjectId).FirstOrDefault();

            historyContext.PopulateHistory((int)HistoryFunctionTypes.Delete, objectModel, _object, userId, targetObjectId, houseId);

            context.Remove(_object);
            context.Remove(objectModel);

            await context.SaveChangesAsync();
        }

        public void Delete(T objectModel, long userId, long? targetObjectId, long houseId)
        {
            var _object = context.Set<Models.Object>().Where(x => x.ObjectId == objectModel.ObjectId).FirstOrDefault();

            historyContext.PopulateHistory((int)HistoryFunctionTypes.Delete, objectModel, _object, userId, targetObjectId, houseId);

            context.Remove(_object);
            context.Remove(objectModel);

            context.SaveChanges();
        }
    }
}
