using System.ComponentModel.DataAnnotations;

namespace SmartO_rder.Models
{
    public class Table
    {
        public int Id { get; set; }
        public int Number { get; set; }

        public int CafeId { get; set; }
        public Cafe? Cafe { get; set; }
        public bool WaiterCalled { get; set; }
    }
}
