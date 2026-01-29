using System.Collections.Generic;

namespace Codebase.Mechanics.StealSystem
{
    public interface IRobber<T>
    {
        List<T> StealedItems{get;}
        bool Stealing{get;}
        void Steal(T item);
    }
}