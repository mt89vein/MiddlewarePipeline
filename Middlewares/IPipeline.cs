using System.Threading.Tasks;

namespace Middlewares
{
    /// <summary>
    /// Pipeline for <typeparamref name="TParameter"/>.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline paramer type.</typeparam>
    public interface IPipeline<TParameter>
    {
        /// <summary>
        /// Execute configured pipeline.
        /// </summary>
        /// <param name="parameter">Pipeline parameter.</param>
        Task ExecuteAsync(TParameter parameter);
    }
}