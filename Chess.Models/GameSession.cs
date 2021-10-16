using System;
using System.ComponentModel.DataAnnotations;

namespace Chess.Models
{
    public class GameSession
    {
        [StringLength(20)]
        public string Key { get; set; }
        public DateTime DateStarted { get; set; }

        [StringLength(30)]
        public string DateDisplay { get; set; }
    }
}
