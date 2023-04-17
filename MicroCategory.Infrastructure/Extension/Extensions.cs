using Microsoft.Extensions.Configuration;

namespace MicroCategory.Infrastructure.Extension
{
    public static class Extensions
    {

        /// <summary>
        /// Underscore
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Underscore(this string value)
        {
            return string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));
        }

        /// <summary>
        /// Get Options
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static TModel GetTModelOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(section).Bind(model);

            return model;
        }

        /// <summary>
        /// Hàm lấy danh sách đối tượng theo pagesize và pagenumber sử dụng IQueryable
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IQueryable<T> GetPagedList<T>(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var data = source.Skip(((pageNumber - 1) * pageSize)).Take(pageSize);

            return data;
        }

        /// <summary>
        /// Hàm lấy danh sách đối tượng theo pagesize và pagenumber sử dụng IEnumerable
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetPagedList<T>(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var data = source.Skip(((pageNumber - 1) * pageSize)).Take(pageSize);

            return data;
        }
    }
}
