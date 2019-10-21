using System.Web;

namespace Cast.Sessions
{
    public class SessionHolder
    {
        /// <summary>
        /// Load class from session
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Object</param>
        /// <returns></returns>
        public T Load<T>(T entity) where T : new()
        {
            if (HttpContext.Current.Session == null)
            {
                return entity;
            }
            
            var name = entity.GetType().Name;
            var result = HttpContext.Current.Session[name];
            if (result == null)
            {
                HttpContext.Current.Session[name] = entity;
            }
            return (result != null) ? (T) result : entity;
            
        }

        /// <summary>
        /// Update class into session
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Object</param>
        public void UpdateFrom<T>(T entity)
        {
            if (HttpContext.Current.Session != null)
            {
                var name = entity.GetType().Name;
                HttpContext.Current.Session[name] = entity;
            }
        }

        /// <summary>
        /// Remove info from session
        /// </summary>
        /// <typeparam name="T">Ttype</typeparam>
        /// <param name="entity">Object</param>
        public void Clear<T>(T entity)
        {
            if (HttpContext.Current.Session != null)
            {
                var name = entity.GetType().Name;
                HttpContext.Current.Session.Remove(name);
            }
        }

        public void Clear(string name)
        {
            HttpContext.Current.Session.Remove(name);
        }
    }
}