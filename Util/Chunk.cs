using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    /**
     * Collectionをある数ごとに分割する
     */
    public class Chunk<T>
    {
        public int ChunkSize { get; set; }
        private IEnumerable<T> Sources;
        public Chunk(IEnumerable<T> sources)
        {
            this.Sources = sources;
            this.ChunkSize = 5;
        }
        public IEnumerable<IEnumerable<T>> Read()
        {
            return this.Sources.Select((entity, index) => new { Index = index, Entity = entity })
                     .GroupBy(x => x.Index / ChunkSize)
                     .Select(gr => gr.Select(x => x.Entity));
        }
    }
}
