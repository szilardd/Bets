using System;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.Identity;
using AutoMapper.QueryableExtensions;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace Bets.Controllers.Api
{
    public class BaseApiController : ApiController
    {
        protected int UserID => int.Parse(User.Identity.GetUserId() ?? "0");
        protected IMapper _mapper;

        public BaseApiController(IMapper mapper)
        {
            _mapper = mapper;
        }

        protected IQueryable<T> ProjectTo<T>(IQueryable entities)
        {
            return entities.ProjectTo<T>(_mapper.ConfigurationProvider);
        }

        protected IQueryable<T> ProjectAndFilter<T>(IQueryable entities, ODataQueryOptions<T> queryOptions)
        {
            var result = entities.ProjectTo<T>(_mapper.ConfigurationProvider);
            return  (IQueryable<T>)queryOptions.ApplyTo(result);
        }
    }
}
