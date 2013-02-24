using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class BooksController : EntitySetController<Book, string>
    {
        [Queryable]
        public override IQueryable<Book> Get()
        {
            return new MarcQueryable<Book>();
        }
    }
}
