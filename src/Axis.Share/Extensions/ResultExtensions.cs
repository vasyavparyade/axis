using System.Threading.Tasks;

namespace Axis.Share.Extensions
{
    public static class ResultExtensions
    {
        public static Task<Result> ToResult(this Task task) => Result.From(async () =>
        {
            await task.ConfigureAwait(false);
        });

        public static ResultBuilder Build(this Task<Result> task) => new ResultBuilder(task);
    }
}
