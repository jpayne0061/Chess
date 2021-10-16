using Chess.Models;
using ChessGame;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Interfaces;

namespace Web.Services
{
    public class GameFlow
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IGameSessionDal _gameSessionDal;
        private readonly IPlayResultDal _playResultDal;

        public GameFlow(IHubContext<ChatHub> hubContext, IPlayResultDal playResultDal, IGameSessionDal gameSessionDal)
        {
            _hubContext = hubContext;
            _gameSessionDal = gameSessionDal;
            _playResultDal = playResultDal;
        }

        public async Task<string> MakeMove(string command, Game game)
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
                pr.Command = command;
                await _hubContext.Clients.All.SendAsync(gameId, pr);
            }

            _playResultDal.SavePlayResult(pr);

            return JsonConvert.SerializeObject(pr);
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
            await _hubContext.Clients.All.SendAsync(gameId.Trim(), "player-joined");
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
