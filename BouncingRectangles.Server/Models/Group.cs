using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BouncingRectangles.Server.Models
{
    [DebuggerDisplay("Capacity: {_capacity}, Count: {_items.Count}, Id: {Id}")]
    public class Group
    {
        private readonly int _capacity;
        private readonly List<Rectangle> _items;
        private readonly object _locker = new();

        public Guid Id { get; }

        public Group(Guid id, int capacity)
        {
            Id = id;
            _capacity = capacity;
            _items = new List<Rectangle>(capacity);
        }

        public void RefreshItems()
        {
            lock (_locker)
            {
                _items.Clear();
                for (int i = 0; i < _capacity; i++)
                {
                    _items.Add(Rectangle.CreateNew());
                }
            }
        }

        public IReadOnlyCollection<Rectangle> GetItems()
        {
            lock (_locker)
            {
                return _items;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Group group &&
                   Id.Equals(group.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
