using System.Collections.Generic;

namespace RTS_Server.Models
{
    class PlaygroundDTO
    {
        public int Size { get; set; }
        public List<UnitDTO> UnitInfos { get; set; }
    }
}
