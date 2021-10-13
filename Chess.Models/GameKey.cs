using System.ComponentModel.DataAnnotations;

namespace Chess.Models
{
    public class GameKey
    {
        [StringLength(20)]
        public string Key { get; set; }
    }
}
