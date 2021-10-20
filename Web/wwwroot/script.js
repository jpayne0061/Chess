if (window.location.href.indexOf('localhost') > -1) {
    URL_ROOT = 'https://localhost:44320';
}
else {
    URL_ROOT = 'https://chesswithhotsauce.azurewebsites.net';
}

var GAME_ID = "";

var CONNECTION = null;

var CONTAINER = document.getElementById('game-container');

var CHECK_MATE = false;

var PLAYER_COLOR = 0;

var YOUR_TURN = false;

var OTHER_PLAYER_HAS_JOINED = false;

var INVALID_PLAY_TYPES = {
    0: "Wrong player",
    1: "Out of turn"
};

var PIECE_LOOKUP = {
    "whiterook": "&#9814;",
    "whiteknight": "&#9816;",
    "whitebishop": "&#9815;",
    "whitequeen": "&#9813;",
    "whiteking": "&#9812;",
    "whitepawn": "&#9817;",
    "blackrook": "&#9820;",
    "blackknight": "&#9822;",
    "blackbishop": "&#9821;",
    "blackqueen": "&#9819;",
    "blackking": "&#9818;",
    "blackpawn": "&#9823;"
};

var GLOBAL_START = null;
var GLOBAL_END = null;

var SQUARE_SIZE = null;
var FONT_SIZE = null;

function writeMessage(message) {
    document.getElementById('messages').innerHTML = message;
}

function setUp() {
    OTHER_PLAYER_HAS_JOINED = true;

    document.getElementById('recently-started-games').style.display = 'none';

    connectToHub();

    buildBoard();

    hideGameInputs();

    PLAYER_COLOR = 0;
}


function joinGameByKey(key) {
    GAME_ID = key.trim();

    joinIfGameIdValid(setUp);
}

function listRecentlyStartedGames(gamesText) {

    var games = JSON.parse(gamesText);

    console.log(games);

    var gameDisplay = document.getElementById('recently-started-games');

    for (var i = 0; i < games.length; i++) {
        var node = document.createElement('div');

        node.innerHTML = '<div onclick="joinGameByKey(\'' + games[i].key + '\')"' +
            '<strong>' + games[i].key + '</strong>' +
            '<br> Started ' + games[i].dateDisplay +
            '</div>';

        node.classList.add('gameListing');

        gameDisplay.appendChild(node);
    }

    writeMessage('');
}


function startGame() {
    startNewGame();

    hideGameInputs();

    buildBoardForWhite();

    //rotateBoard();

    PLAYER_COLOR = 1;

    YOUR_TURN = true;

    setTimeout(writeMessage, 1700, 'it is your turn');
}

function rotateBoard() {
    CONTAINER.classList.add('rotated');

    setTimeout(rotatePieces, 1500);
}

function rotatePieces() {
    var pieces = CONTAINER.children;

    for (var i = 0; i < pieces.length; i++) {
        pieces[i].style.animation = 'spin 1s forwards';
        //pieces[i].classList.add('rotated');
    }
}


function connectToHub() {
    CONNECTION = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    console.log("GAME ID USED FOR SYNC: ", GAME_ID);

    CONNECTION.on(GAME_ID, function (message) {
        console.log("message from hub: ", message);

        if (message.pieceName && message.turn === PLAYER_COLOR) {
            writeMessage('other player has promoted their pawn to ' + message.pieceName + '. It is your turn');
            movePiece(message.message, message.command, message.pieceName);
            return;
        }

        if (message === 'player-joined') {
            OTHER_PLAYER_HAS_JOINED = true;
            alert('player has joined your game');
            return;
        }

        var command = message.command;

        if (message.isCheckMate) {
            CHECK_MATE = true;
            writeMessage('check mate');
        }
        else if (message.turn === PLAYER_COLOR && message.isCheck) {
            writeMessage('it is your turn. you are in check');
        }
        else if (message.turn === PLAYER_COLOR) {
            writeMessage('it is your turn');
        }
        else if (message.isCheck) {
            writeMessage('check');
        }

        if (message.turn === PLAYER_COLOR && message.isEligibleForPawnPromotion) {
            var choice = prompt('enter piece name for promotion');

            var pawnPromotionObject = {};

            pawnPromotionObject.location = { x: message.endLocation.x, y: message.endLocation.y };
            pawnPromotionObject.pieceName = choice;
            pawnPromotionObject.gameKey = GAME_ID;
            pawnPromotionObject.message = message;
            pawnPromotionObject.command = command;

            console.log('pawnPromotionObject to post', pawnPromotionObject);

            sendPawnPromotionRequest(pawnPromotionObject);

            return;
        }

        if (message.turn !== PLAYER_COLOR && message.isEligibleForPawnPromotion) {
            writeMessage('other player is choosing piece for pawn promotion');
            return;
        }

        if (message.turn === PLAYER_COLOR) {
            movePiece(message, command);
        }
    });

    CONNECTION.start();
}

function hideGameInputs() {
    document.getElementById('join-start').style.display = "none";
}

function restartGame() {
    CHECK_MATE = false;

    CONTAINER.innerHTML = '';
    document.getElementById('white-captured-pieces').innerHTML = '';
    document.getElementById('black-captured-pieces').innerHTML = '';

    buildBoard();

    startNewGame();
}

function showGameKey() {
    document.getElementById('game-key').innerHTML = GAME_ID;
    document.getElementById('game-key-container').style.display = 'block';
}


function startNewGame() {
    var xhttp = new XMLHttpRequest();
    xhttp.open("GET", URL_ROOT + "/api/values/CreateGame", true);
    xhttp.send();

    xhttp.onload = function () {
        console.log("post game key response text: ", this.responseText);

        GAME_ID = this.responseText.trim();

        connectToHub();

        setTimeout(showGameKey, 1700);
        document.getElementById('recently-started-games').style.display = 'none';
    };
}


function joinIfGameIdValid(setUp) {
    var xhttp = new XMLHttpRequest();
    xhttp.open("GET", URL_ROOT + "/api/values/JoinGame?gameKey=" + GAME_ID, true);
    xhttp.send();

    xhttp.onload = function () {
        console.log("response text: ", this.responseText);

        if ('true' === this.responseText) {
            setUp();
        }
        else {
            alert('you have not entered a valid game id');
            return;
        }
    };
}

function getRecentlyStartedGames() {
    var xhttp = new XMLHttpRequest();
    xhttp.open("GET", URL_ROOT + "/api/values/GetRecentGames", true);
    xhttp.send();

    xhttp.onload = function () {
        console.log("recent games: ", this.responseText);

        if (this.responseText.length <= 5) {
            writeMessage('there are currently no games to join');
            return;
        }

        listRecentlyStartedGames(this.responseText);
    };
}

function getScreenWidth() {
    return window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
}

function getScreenHeight() {
    return window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
}


function buildBoardForWhite() {
    var x = 0;
    var y = 7;

    var gameBoardWidth = getScreenWidth() > 1000 ? getScreenWidth() * 0.50 : getScreenWidth();

    SQUARE_SIZE = Math.floor(Math.floor(gameBoardWidth) / 9);

    FONT_SIZE = Math.floor(SQUARE_SIZE * 0.71);


    document.getElementById('black-captured-pieces').style.fontSize = FONT_SIZE + 'px';
    document.getElementById('white-captured-pieces').style.fontSize = FONT_SIZE + 'px';

    document.getElementById('black-captured-pieces').style.height = FONT_SIZE + 'px';
    document.getElementById('white-captured-pieces').style.height = FONT_SIZE + 'px';

    for (var i = 0; i < 8; i++) {
        for (var j = 7; j >= 0; j--) {
            var node = document.createElement("div");

            node.setAttribute('dataX', j % 8);
            node.setAttribute('dataY', y);

            var locationCoords = y.toString() + (j % 8).toString();

            node.setAttribute('id', locationCoords);

            switch (locationCoords) {
                case "00":
                    node.innerHTML = "&#9814;"; //whiterook
                    break;
                case "01":
                    node.innerHTML = "&#9816;"; //whiteknight
                    break;
                case "02":
                    node.innerHTML = "&#9815;"; //whitebishop
                    break;
                case "03":
                    node.innerHTML = "&#9812;"; //whiteking
                    break;
                case "04":
                    node.innerHTML = "&#9813;"; //whitequeen
                    break;
                case "05":
                    node.innerHTML = "&#9815;"; //whitebishop
                    break;
                case "06":
                    node.innerHTML = "&#9816;"; //whiteknight
                    break;
                case "07":
                    node.innerHTML = "&#9814;"; //whiterook
                    break;
                case "10":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "11":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "12":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "13":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "14":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "15":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "16":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "17":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "70":
                    node.innerHTML = "&#9820;";
                    break;
                case "71":
                    node.innerHTML = "&#9822;";
                    break;
                case "72":
                    node.innerHTML = "&#9821;";
                    break;
                case "73":
                    node.innerHTML = "&#9818;";
                    break;
                case "74":
                    node.innerHTML = "&#9819;";
                    break;
                case "75":
                    node.innerHTML = "&#9821;";
                    break;
                case "76":
                    node.innerHTML = "&#9822;";
                    break;
                case "77":
                    node.innerHTML = "&#9820;";
                    break;
                case "60":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "61":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "62":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "63":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "64":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "65":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "66":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "67":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                default:
                    node.innerHTML = "&#9823;&#xFE0E";
                    node.style.color = x % 2 === 0 ? '#adadad' : '#666666';
            }

            if (locationCoords[0] === "0" || locationCoords[0] === "1") {
                node.style.color = "#ebebeb";
            }

            node.style.backgroundColor = x % 2 === 0 ? '#adadad' : '#666666';
            node.style.height = SQUARE_SIZE + 'px';
            node.style.width = SQUARE_SIZE + 'px';
            node.style.fontSize = FONT_SIZE + 'px';
            node.style.paddingLeft = '6px';
            node.style.display = 'inline-block';
            node.onclick = getCoordinates;

            setTimeout(appendNodeToBoard, 200 + x * 10, node);

            x++;
        }

        x++;
        y--;

        var breakNode = document.createElement("div");
        CONTAINER.appendChild(breakNode);
    }
}

function buildBoard() {

    var x = 0;
    var y = 0;

    var gameBoardWidth = getScreenWidth() > 1000 ? getScreenWidth() * 0.50 : getScreenWidth();

    SQUARE_SIZE = Math.floor(Math.floor(gameBoardWidth) / 9);

    FONT_SIZE = Math.floor(SQUARE_SIZE * 0.71);


    document.getElementById('black-captured-pieces').style.fontSize = FONT_SIZE + 'px';
    document.getElementById('white-captured-pieces').style.fontSize = FONT_SIZE + 'px';

    document.getElementById('black-captured-pieces').style.height = FONT_SIZE + 'px';
    document.getElementById('white-captured-pieces').style.height = FONT_SIZE + 'px';

    for (var i = 0; i < 8; i++) {
        for (var j = 0; j < 8; j++) {
            var node = document.createElement("div");

            node.setAttribute('dataX', j % 8);
            node.setAttribute('dataY', y);

            var locationCoords = y.toString() + (j % 8).toString();

            node.setAttribute('id', locationCoords);

            switch (locationCoords) {
                case "00":
                    node.innerHTML = "&#9814;"; //whiterook
                    break;
                case "01":
                    node.innerHTML = "&#9816;"; //whiteknight
                    break;
                case "02":
                    node.innerHTML = "&#9815;"; //whitebishop
                    break;
                case "03":
                    node.innerHTML = "&#9812;"; //whiteking
                    break;
                case "04":
                    node.innerHTML = "&#9813;"; //whitequeen
                    break;
                case "05":
                    node.innerHTML = "&#9815;"; //whitebishop
                    break;
                case "06":
                    node.innerHTML = "&#9816;"; //whiteknight
                    break;
                case "07":
                    node.innerHTML = "&#9814;"; //whiterook
                    break;
                case "10":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "11":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "12":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "13":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "14":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "15":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "16":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "17":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "70":
                    node.innerHTML = "&#9820;";
                    break;
                case "71":
                    node.innerHTML = "&#9822;";
                    break;
                case "72":
                    node.innerHTML = "&#9821;";
                    break;
                case "73":
                    node.innerHTML = "&#9818;";
                    break;
                case "74":
                    node.innerHTML = "&#9819;";
                    break;
                case "75":
                    node.innerHTML = "&#9821;";
                    break;
                case "76":
                    node.innerHTML = "&#9822;";
                    break;
                case "77":
                    node.innerHTML = "&#9820;";
                    break;
                case "60":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "61":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "62":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "63":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "64":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "65":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "66":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                case "67":
                    node.innerHTML = "&#9823;&#xFE0E";
                    break;
                default:
                    node.innerHTML = "&#9823;&#xFE0E";
                    node.style.color = x % 2 === 0 ? '#adadad' : '#666666';
            }

            if (locationCoords[0] === "0" || locationCoords[0] === "1") {
                node.style.color = "#ebebeb";
            }

            node.style.backgroundColor = x % 2 === 0 ? '#adadad' : '#666666';
            node.style.height = SQUARE_SIZE + 'px';
            node.style.width = SQUARE_SIZE + 'px';
            node.style.fontSize = FONT_SIZE + 'px';
            node.style.paddingLeft = '6px';
            node.style.display = 'inline-block';
            node.classList.add('noselect');
            node.onclick = getCoordinates;

            setTimeout(appendNodeToBoard, 200 + x * 10, node);

            x++;
        }

        x++;
        y++;

        var breakNode = document.createElement("div");
        CONTAINER.appendChild(breakNode);
    }
}

function appendNodeToBoard(node) {
    CONTAINER.appendChild(node);
}

function getCoordinates(e) {

    if (!OTHER_PLAYER_HAS_JOINED) {
        alert('you must wait for other player to join game');
        return;
    }

    if (CHECK_MATE) {
        alert("Game has ended in check mate.");
        return;
    }

    var x = e.target.getAttribute('datax');
    var y = e.target.getAttribute('datay');

    var character = document.getElementById(y.toString() + x.toString()).innerHTML;

    if ((character.charCodeAt(0) <= 9817 && PLAYER_COLOR !== 1 ||
        character.charCodeAt(0) > 9817 && PLAYER_COLOR !== 0) &&
        GLOBAL_START === null) {
        return;
    }

    giveBorder(e);

    if (GLOBAL_START === null) {
        GLOBAL_START = { x: x, y: y };
        return;
    }
    else {
        if (GLOBAL_START.x === x && GLOBAL_START.y === y) {
            GLOBAL_START = null;
            GLOBAL_END = null;
            removeBorder(e);
            return;
        }

        GLOBAL_END = { x: x, y: y };

        var move = GLOBAL_START.y + GLOBAL_START.x + ' ' + GLOBAL_END.y + GLOBAL_END.x + ' ' + GAME_ID;

        var xhttp = new XMLHttpRequest();
        xhttp.open("GET", URL_ROOT + "/api/values/" + encodeURIComponent(move), true);
        xhttp.send();

        xhttp.onload = function () {
            var playResult = JSON.parse(this.responseText);
            console.log('playResult from getCoordinates: ', playResult);

            document.getElementById("messages").innerHTML = playResult.message;

            if (playResult.isCheck) {
                if (playResult.isCheckMate) {
                    alert("Check mate");
                    CHECK_MATE = true;
                    document.getElementById("restart-game").style.display = "block";
                }
                else {
                    document.getElementById("messages").innerHTML = "Check";
                }
            }

            if (playResult.playValid) {
                movePiece(playResult, move);
            }
            else {
                removeBorders();
                GLOBAL_START = null;
                GLOBAL_END = null;
            }

        };
    }
}

function sendPawnPromotionRequest(pawnPromotionObject) {
    var xhttp = new XMLHttpRequest();
    xhttp.open("POST", URL_ROOT + "/api/values/", true);
    xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xhttp.onreadystatechange = function () {
        if (this.readyState === 4) {
            console.log("result from promotion post", this.responseText);
            var pawnPromotionObject = JSON.parse(this.responseText);
            movePiece(pawnPromotionObject.message, pawnPromotionObject.command, pawnPromotionObject.pieceName);
        }
    };

    xhttp.send(JSON.stringify(pawnPromotionObject));
}

function animateMove() {

}


function animate(x, y) {
    var head = document.getElementsByTagName('head')[0];

    if (head && head.children && head.children > 0) {
        for (var i = 4; i < head.children.length; i++) {
            head.remove(head.children[i]);
        }
    }


    var style = document.createElement('style');
    style.type = 'text/css';
    var keyFrames = '\
        @-webkit-keyframes move {\
            100% {\
                -webkit-transform: translateX(DYNAMIC_X) translateY(DYNAMIC_Y);\
            }\
        }';
    style.innerHTML = keyFrames.replace('DYNAMIC_X', x).replace('DYNAMIC_Y', y);
    document.getElementsByTagName('head')[0].appendChild(style);
}

function getDirectionX(x, x1) {
    if (x1 > x && PLAYER_COLOR === 1) {
        return -1;
    }

    if (x1 < x && PLAYER_COLOR === 1) {
        return 1;
    }

    if (x1 > x && PLAYER_COLOR === 0) {
        return 1;
    }

    if (x1 < x && PLAYER_COLOR === 0) {
        return -1;
    }

    return 0;
}


function getDirectionY(y, y1) {
    if (y1 > y && PLAYER_COLOR === 1) {
        return -1;
    }

    if (y1 < y && PLAYER_COLOR === 1) {
        return 1;
    }

    if (y1 > y && PLAYER_COLOR === 0) {
        return 1;
    }

    if (y1 < y && PLAYER_COLOR === 0) {
        return -1;
    }

    return 0;
}


function getDistanceX(x, x1) {
    return Math.abs(x1 - x);
}

function getDistanceY(y, y1) {
    return Math.abs(y1 - y);
}
 

function movePiece(playResult, command, overridePieceName) {

    cmdArgs = command.split(' ');
    start = cmdArgs[0];
    end = cmdArgs[1];

    var x = start[1];
    var x1 = end[1];

    var y = start[0];
    var y1 = end[0];

    var startingElement = document.getElementById(start);

    var character = startingElement.innerHTML;

    startingElement.innerHTML = "<div id='move-it' style='inline-block; position: relative'>" + character + "</div>";

    var distanceX = SQUARE_SIZE * getDistanceX(x, x1) * getDirectionX(x, x1) + 'px';

    var distanceY = SQUARE_SIZE * getDistanceY(y, y1) * getDirectionY(y, y1) + 'px';

    animate(distanceX, distanceY);

    var movingElement = document.getElementById('move-it');

    movingElement.style.animation = 'move 1s forwards';
    movingElement.style.position = 'relative';
    movingElement.style.zIndex = 1000;

    var animationCallBackParameters = {
        playResult: playResult,
        overridePieceName: overridePieceName,
        end: end,
        character: character,
        startingElement: startingElement
    };

    setTimeout(doMove, 1000, animationCallBackParameters);
    
}


function doMove(animationCallBackParameters) {
    animationCallBackParameters.startingElement.innerHTML = animationCallBackParameters.character;
    animationCallBackParameters.startingElement.style.color = animationCallBackParameters.startingElement.style.backgroundColor;

    var elTo = document.getElementById(animationCallBackParameters.end);
    elTo.innerHTML = animationCallBackParameters.character + '&#xFE0E';

    if (animationCallBackParameters.overridePieceName) {
        console.log('play result from promotion object: ', animationCallBackParameters.playResult);

        var pieceColor = animationCallBackParameters.playResult.turn === 1 ? "white" : "black";

        elTo.innerHTML = PIECE_LOOKUP[pieceColor + animationCallBackParameters.overridePieceName.toLowerCase()] + '&#xFE0E';
    }

    if (animationCallBackParameters.character.charCodeAt(0) <= 9817) {
        elTo.style.color = "#ebebeb";
    }
    else {
        elTo.style.color = "#000";
    }

    if (GLOBAL_START !== null) {
        removeBorders();
    }


    GLOBAL_START = null;
    GLOBAL_END = null;

    if (animationCallBackParameters.playResult.capturedPiece !== null) {
        var color = animationCallBackParameters.playResult.capturedPiece.color === 0 ? "black" : "white";

        console.log("color: ", color);

        var piece = PIECE_LOOKUP[color + animationCallBackParameters.playResult.capturedPiece.name.toLowerCase()];

        document.getElementById(color.toLowerCase() + "-captured-pieces").innerHTML += piece + '&#xFE0E';
    }
}

function giveBorder(e) {
    var element = e.target;

    element.style.boxShadow = "0px 0px 0px 2px black inset";
}

function removeBorder(e) {
    var element = e.target;

    element.style.boxShadow = "0px 0px 0px 0px black inset";
}

function removeBorders() {
    document.getElementById(GLOBAL_END.y + GLOBAL_END.x).style.boxShadow = "0px 0px 0px 0px black inset";
    document.getElementById(GLOBAL_START.y + GLOBAL_START.x).style.boxShadow = "0px 0px 0px 0px black inset";
}


