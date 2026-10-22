import { FieldState, GameStatusEnumm } from "../Game/MLState.js";

// build up an out matrix from game state, then render it.
export function render(state) {
    // deep copy
    const out_grid = JSON.parse(JSON.stringify(state.map))
    renderFigures(out_grid, state)
    let out = renderGrid(out_grid)
    let statusM = StatusString(state)
    out = out + `<div> ${statusM} </div>`
    return out
}

function renderFigures(out_grid, state) {
    // Layer order: top->bottom: Player, Collectible, Enemies

    // Draw up Enemy
    for (const iterator of state.enemies) {
        const pos = iterator.pos
        out_grid[pos[0]][pos[1]] = FieldState.Enemy
    }

    // Draw up Collectible
    for (const collectible of state.collectibles) {
        const pos = collectible.pos
        out_grid[pos[0]][pos[1]] = FieldState.Collectible
    }

    // Draw up Player
    const pos = state.player.pos
    out_grid[pos[0]][pos[1]] = FieldState.Player

}

function renderGrid(grid) {
    return `<table class="gametable">${grid.map(renderRow).join("")}</table>`
}
function renderRow(rowArr) {
    return `<tr>${rowArr.map(renderField).join("")}</tr>`
}
function renderField(field) {
    switch (field) {
        case FieldState.Wall:
            return `<td class="wall gametd">🏰</td>`
        default:
            switch (field) {
                case FieldState.Empty:
                    return `<td class="empty gametd"></td>`
                case FieldState.Player:
                    return `<td class="empty gametd">🐷</td>`
                case FieldState.Collectible:
                    return `<td class="empty gametd">🍎</td>`
                case FieldState.Enemy:
                    return `<td class="empty gametd">👮‍♂️</td>`
                default:
                    return `<td class="empty gametd"></td>`
            }
    }
}
function StatusString(state) {
    switch (state.status) {
        case GameStatusEnumm.Defeat:
            return "Defeat"
        case GameStatusEnumm.Victory:
            return "Victory"
        case GameStatusEnumm.Started:
            return "Playing"
        case GameStatusEnumm.Paused:
            return "Paused"
        case GameStatusEnumm.Terminated:
            return "Terminated"

        default:
            return ""
    }
}