namespace RTS_Server.Models
{
    public class UnitDTO
    {
        public int Id { get; set; }
        public Pos2D ParentPos { get; set; }
        public Pos2D NextPos { get; set; }
        public bool Moving { get; set; }
        public float MoveProgress { get; set; }

        public static UnitDTO ToDTO(Unit unit)
        {
            return new UnitDTO()
            {
                Id = unit.id,
                ParentPos = unit.parent.pos,
                NextPos = unit.next == null ? null : unit.next.pos,
                Moving = unit.moving,
                MoveProgress = unit.move_progress
            };
        }
    }
}
