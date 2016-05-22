using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Bets.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Bets.Data
{
	public enum StoredProcResult
	{
		Error = 0,
		Success = 1,
		ErrChanged = 2,
		ErrNoRecord = 3,
		ErrRecExist = 4,
		ErrRecHasLink = 5,
		ErrUnknown = 9,
		InvalidPassword = 6,
		InvalidUsername = 7,
		UserNotActive = 8
	}

	public enum DBActionType
	{
		Update = 0,
		Insert = 1,
		Delete = 2
	}

    public class ActionStatus<T>
    {
        [Required]
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }

        public ActionStatus()
        {
        }

        public ActionStatus(T result)
        {
            Result = result;
        }
    }

    public class ActionStatus : ActionStatus<object>
	{
    }

    public class ActionStatusCollection<T> : ActionStatus
    {
        [Required]
        public new IQueryable<T> Result { get; set; }

        public ActionStatusCollection(IQueryable<T> result)
        {
            Result = result;
        }
    }

    public class SPResult
	{
		public int? Result { get; set; }
		public string Error { get; set; }
	}

	public interface IRepository<TEntity, M> where TEntity : class where M : Model
	{
		IQueryable<TEntity> GetQuery();
		IQueryable<TEntity> GetAll();
		bool Add(TEntity entity, bool save = false);
		bool DeleteWithAttach(TEntity entity, bool save = true);
		void Attach(TEntity entity);
		StoredProcResult Save();

		IQueryable<IModel> GetLookupItems(ListingParams<M> listingData);
		IQueryable<IModel> GetListingItems(ListingParams<M> listingData);
		M GetItem(int id);
		M GetItem(string id);
		M GetItem(string id, M model);
		M GetItem(int id, M model);
		ActionStatus SaveItem(M model, DBActionType dbActionType);
		ActionStatus DeleteItem(M model);
	}
}
