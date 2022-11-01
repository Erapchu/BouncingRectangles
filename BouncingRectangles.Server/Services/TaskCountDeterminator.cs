using System;

namespace BouncingRectangles.Server.Services
{
    public interface ITaskCountDeterminator
    {
        public int GetCount();
    }

    public class TaskCountDeterminator : ITaskCountDeterminator
    {
        private readonly int _count;

        public TaskCountDeterminator()
        {
            var rnd = new Random();
            _count = rnd.Next(1, Environment.ProcessorCount);
        }

        public int GetCount()
        {
            return _count;
        }
    }
}
