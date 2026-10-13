import { MaciLaci } from "./Game/MaciLaci.js"
import { GameStatusEnumm } from "./Game/MLState.js"
import { render } from "./Render/render.js"

const restartBtn = document.querySelector("#restartBtn")
restartBtn.addEventListener("click", onRestartPressed)
const pauseBtn = document.querySelector("#pauseGame")
pauseBtn.addEventListener("click", onPauseGame)

document.addEventListener('keypress', handleKeyPressed, false)

const restartGameEvent = new Event("restartGame")
const gameDiv = document.querySelector("#game")
const GameSpeed = 500
let game

StartGame()

// Game loop
function StartGame() {
    game = MLinit()
    MLloop(game)
}
function RestartGame() {
    game.Init()
    dispatchEvent(restartGameEvent)
    MLloop(game)
}

//Update Loop functions
function MLinit(gamestate) {
    const game = new MaciLaci()
    if (gamestate) {
        game.Init(gamestate)
    }
    else {
        game.Init()
    }
    return game
}
async function MLloop(game) {
    let terminate = false
    addEventListener("restartGame", x => { terminate = true })
    while (!terminate && !game.IsEnd()) {
        if (game.IsPaused()) {
            await WaitUntilPaused(game)
        }
        game.Update()
        renderGame(game)
        await sleep(GameSpeed)
    }
}
// Sleep method for the game loop.
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms))
}
async function WaitUntilPaused(game) {
    while (game.IsPaused()) {
        await sleep(100)
    }
}

function renderGame(game) {
    if (!gameDiv) return
    gameDiv.innerHTML = render(game.state)
}
// Processing keyboard inputs
function handleKeyPressed(param) {
    if (!game || game.state.status != GameStatusEnumm.Started) return
    switch (param.key.toLowerCase()) {
        case "w":
            game.StepPlayer(-1, 0)
            renderGame(game)
            break;
        case "s":
            game.StepPlayer(1, 0)
            renderGame(game)
            break;
        case "a":
            game.StepPlayer(0, -1)
            renderGame(game)
            break;
        case "d":
            game.StepPlayer(0, 1)
            renderGame(game)
            break;
        default:
            break;
    }
}
function onRestartPressed(param) {
    RestartGame()
}
function onPauseGame(param) {
    if (game) {
        game.TurnStart()
    }
}

