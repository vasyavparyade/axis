using System.Collections.Generic;
using System.Threading.Tasks;

namespace Axis.Share.Extensions
{
    public class ResultBuilder
    {
        private readonly List<Task<Result>> _tasks = new List<Task<Result>>();

        public ResultBuilder()
        {
        }

        public ResultBuilder(Task<Result> task)
        {
            Add(task);
        }

        public async Task<Result> ExecuteAsync()
        {
            foreach (var task in _tasks)
            {
                var result = await task.ConfigureAwait(false);

                if (!result)
                    return result;
            }

            return Result.Successful;
        }

        public ResultBuilder Add(Task<Result> task)
        {
            if (task is null)
                return this;

            _tasks.Add(task);

            return this;
        }
    }
}
