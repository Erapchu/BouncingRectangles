using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BouncingRectangles.Server.Models
{
    [DebuggerDisplay("Capacity: {_capacity}, Count: {Items.Count}, Id: {Id}")]
    public class Group
    {
        private readonly int _capacity;

        public Guid Id { get; }
        public List<Rectangle> Items { get; }

        public Group(Guid id, int capacity)
        {
            Id = id;
            _capacity = capacity;
            Items = new List<Rectangle>(capacity);
        }

        public void FillItems()
        {
            Items.Clear();
            for (int i = 0; i < _capacity; i++)
            {
                Items.Add(Rectangle.CreateNew());
            }
        }
    }
}
