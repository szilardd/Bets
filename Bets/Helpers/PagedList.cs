using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bets.Data.Models;

namespace System.Collections.Generic {

	public interface IPagedList {

		int TotalCount { get; set; }
		int CurrentPage { get; }
		int NextPage { get; }
		int LastPage { get; }
		int TotalPages { get; set; }
		int PageIndex { get; set; }
		int PageSize { get; set; }
		int CurrentPageCount { get; set; }
		bool FullRows { get; set; }
		bool HasPreviousPage { get; }
		bool HasNextPage { get; }
		bool IsCurrentPage(int pageNumber);
	}

	public class PagedList<M> : List<M>, IPagedList where M : IModel 
	{
		public PagedList(IQueryable<M> source, int index, int pageSize)
		{
			this.TotalCount = source.Count();
			this.PageSize = pageSize;
			this.TotalPages = 1;

			CalcPages();

			//default to first page
			if (index < 0)
				index = 0;
			//if index exceeds total number of pages, default to last valid page
			else if (index >= this.TotalPages)
				index = this.TotalPages - 1;

			this.PageIndex = index;
			this.FullRows = (this.CurrentPage < this.TotalPages) || ((this.CurrentPage == this.TotalPages) && (this.TotalCount % this.PageSize == 0));

			this.AddRange(source.Skip(index * pageSize).Take(pageSize).ToList());
		}
		void CalcPages() {
			if (PageSize > 0 && TotalCount > PageSize) {
				TotalPages = TotalCount / PageSize;
				if (TotalCount % PageSize > 0)
					TotalPages++;
			}
		}
		public int CurrentPage {
			get {
				return PageIndex + 1;
			}
		}
		public int NextPage {
			get {
				return CurrentPage + 1;
			}
		}
		public int LastPage {
			get {
				return CurrentPage - 1;
			}
		}
		public int TotalCount {
			get;
			set;
		}
		public int TotalPages {
			get;
			set;
		}
		public int PageIndex {
			get;
			set;
		}

		public int CurrentPageCount
		{
			get;
			set;
		}

		public int PageSize { get; set; }

		public bool FullRows { get; set; }

		public bool HasPreviousPage {
			get {
				return (PageIndex > 0);
			}
		}

		public bool HasNextPage {
			get {
				return (PageIndex * PageSize) <= TotalCount;
			}
		}
		public bool IsCurrentPage(int pageNumber) {
			return pageNumber == CurrentPage;
		}
	}

	public static class Pagination {
		public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index, int pageSize) where T : Model
		{
			return new PagedList<T>(source, index, pageSize);
		}

		public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index) where T : Model
		{
			return new PagedList<T>(source, index, 10);
		}
	}
}
 
