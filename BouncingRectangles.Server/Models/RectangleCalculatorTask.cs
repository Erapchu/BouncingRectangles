using System;
using System.Threading.Tasks;

namespace BouncingRectangles.Server.Models
{
    public class RectangleCalculatorTask
    {
        public Task Task { get; }
        public Guid Id { get; }

        private RectangleCalculatorTask()
        {

        }

        public static RectangleCalculatorTask CreateNew()
        {
            return new RectangleCalculatorTask();
        }
    }
}
