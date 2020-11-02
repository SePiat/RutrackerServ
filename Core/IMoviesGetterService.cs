using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public interface IMoviesGetterService<T> where T : class
    {
        IEnumerable<T> GetMovies();
    }

}
