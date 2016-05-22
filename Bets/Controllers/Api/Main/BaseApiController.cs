using System;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.Identity;
using AutoMapper.QueryableExtensions;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers.Api
{
    public class BaseApiController<T, E, M> : ApiController where T : Repository<E, M>, new() where M : Model where E : class
    {
        protected int UserID => int.Parse(User.Identity.GetUserId() ?? "0");
        protected IMapper _mapper;

        protected T Repo
        {
            get
            {
                var repo = new T();
                repo.UserID = UserID;

                return repo;
            }
        }

        public BaseApiController(IMapper mapper)
        {
            _mapper = mapper;
        }

        protected IQueryable<TProject> ProjectTo<TProject>(IQueryable entities)
        {
            return entities.ProjectTo<TProject>(_mapper.ConfigurationProvider);
        }

        protected IQueryable<TProject> ProjectAndFilter<TProject>(IQueryable entities, ODataQueryOptions<TProject> queryOptions)
        {
            var result = entities.ProjectTo<TProject>(_mapper.ConfigurationProvider);
            return  (IQueryable<TProject>)queryOptions.ApplyTo(result);
        }
    }
}
