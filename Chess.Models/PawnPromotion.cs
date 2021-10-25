namespace Chess.Models
{
    public class PawnPromotion
    {
        public Location Location { get; set; }
        public string PieceName { get; set; }
        public string GameKey { get; set; }
        public PlayResult Message { get; set; }
        public string Command { get; set; }
        public int Turn { get; set; }
        public int PromotedPieceColor { get; set; }
    }
}
