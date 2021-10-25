using Chess.Models;
using Chess.Models.Enums;
using ChessGame;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Interfaces;

namespace Web.Services
{
    public class GameFlow
    {
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly IGameSessionDal _gameSessionDal;
        private readonly IPlayResultDal _playResultDal;

        public GameFlow(IHubContext<MessageHub> hubContext, IPlayResultDal playResultDal, IGameSessionDal gameSessionDal)
        {
            _hubContext = hubContext;
            _gameSessionDal = gameSessionDal;
            _playResultDal = playResultDal;
        }

        public async Task<PlayResult> MakeMove(string command, GameLogic game)
        {
            string[] commandParts = command.Split(' ');

            string gameId = commandParts[2];

            int fromY = int.Parse(commandParts[0][0].ToString());
            int fromX = int.Parse(commandParts[0][1].ToString());

            int toY = int.Parse(commandParts[1][0].ToString());
            int toX = int.Parse(commandParts[1][1].ToString());

            PlayResult pr = game.MakeMove(new Location(fromX, fromY), new Location(toX, toY));

            if (pr.PlayValid)
            {
                Message message = new Message(MessageType.MovePiece);
                message.MessageContent = pr;
                pr.Command = command;
                await _hubContext.Clients.All.SendAsync(gameId, message);
            }

            _playResultDal.SavePlayResult(pr);

            return pr;
        }

        public async Task PromotePawn(PawnPromotion pawnPromotion, GameLogic game)
        {
            game.PromotePawn(pawnPromotion);

            Message message = new Message(MessageType.NotifyPawnPromotionChoice);
            message.MessageContent = pawnPromotion;

            await _hubContext.Clients.All.SendAsync(pawnPromotion.GameKey, message);
        }

        public string CreateGame()
        {
            string gameKey = GenerateGameKey();

            while (_gameSessionDal.GameKeyExists(gameKey))
            {
                gameKey = GenerateGameKey();
            }

            GameSession game = new GameSession();
            game.Key = gameKey;
            game.DateStarted = DateTime.Now;
            game.DateDisplay = "";

            _gameSessionDal.InsertGame(game);

            return gameKey;
        }

        public async Task SendJoinNotification(string gameId)
        {
            Message message = new Message(MessageType.PlayerJoined);
            message.MessageContent = "player-joined";

            await _hubContext.Clients.All.SendAsync(gameId.Trim(), message);
        }

        private string GenerateGameKey()
        {
            Random random = new Random();

            List<string> adjectives = new List<string>
            {
                "silly",
                "quiet",
                "funny",
                "loud",
                "small",
                "big",
                "green",
                "jumpy",
                "slippery",
                "tiny",
                "little"
            };

            List<string> nouns = new List<string>
            {
                "dog",
                "cat",
                "rhino",
                "fish",
                "bird",
                "snake",
                "turtle",
                "dragon",
                "panda",
                "dino"
            };

            var adjective = adjectives[random.Next(0, adjectives.Count)];
            var noun = nouns[random.Next(0, nouns.Count)];

            return adjective + noun + random.Next(0, 100).ToString();
        }
    }
}
