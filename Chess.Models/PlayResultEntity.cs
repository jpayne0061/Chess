using System.ComponentModel.DataAnnotations;

namespace Chess.Models
{
    public class PlayResultEntity
    {
        //parameterless ctor required for HotSauceDB
        public PlayResultEntity() { }

        public PlayResultEntity(PlayResult playResult)
        {
            Message = playResult.Message;
            PlayValid = playResult.PlayValid;
            IsCheck = playResult.IsCheck;
            IsCheckMate = playResult.IsCheckMate;
            Turn = playResult.Turn == (int)Enums.Color.White ? 1 : 0;
            Command = playResult.Command;
        }

        [StringLength(100)]
        public string Message { get; set; }
        public bool PlayValid { get; set; }
        public bool IsCheck { get; set; }
        public bool IsCheckMate { get; set; }
        public bool IsEligibleForPawnPromotion { get; set; }
        public int Turn { get; set; }
        [StringLength(25)]
        public string Command { get; set; }

    }
}
