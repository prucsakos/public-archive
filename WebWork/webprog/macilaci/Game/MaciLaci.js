import { MLState, GameStatusEnumm, FieldState } from "./MLState.js"
/*
Map Tile Types:
0: Empty
1: Wall
2: Player Start
3: Collectible
4: Enemy Horizontal
5: Enemy Vertical
*/


export class MaciLaci {
    Init(StateObj) {
        if (StateObj) this.state = StateObj
        else this.state = new MLState()
        this.state.status = GameStatusEnumm.Started
    }
    Update() {
        this.StepEnemies()
    }
    Terminate() {
        console.log("terminate")
        this.state.status = GameStatusEnumm.Terminated
    }
    IsEnd() {
        if (this.state.status == GameStatusEnumm.Victory
            || this.state.status == GameStatusEnumm.Defeat
            || this.state.status == GameStatusEnumm.Terminated) {
            return true
        }
        return false
    }
    TurnStart() {
        if (this.state.status == GameStatusEnumm.Paused) {
            this.state.status = GameStatusEnumm.Started
        } else if (this.state.status == GameStatusEnumm.Started) {
            this.state.status = GameStatusEnumm.Paused
        }
    }
    IsPaused() {
        return this.state.status == GameStatusEnumm.Paused
    }
    GetStatus() {
        return this.state.status
    }
    StepPlayer(dx, dy) {
        const p = this.state.player
        if (this.state.map[p.pos[0] + dx] != undefined && this.state.map[p.pos[0] + dx][p.pos[1] + dy] === FieldState.Empty) {
            p.translate(dx, dy)
        }
        this.CheckPlayer()
    }
    // Loop through enemies. Step towards dir if no walls ahead. Change dir else.
    StepEnemies() {
        for (const enemy of this.state.enemies) {
            const pos = enemy.pos
            const dir = enemy.dir
            if (dir[0] != 0 && this.state.map[pos[0] + dir[0]] != undefined && this.state.map[pos[0] + dir[0]][pos[1]] == FieldState.Empty) {
                enemy.translate(dir[0], 0)
            } else {
                enemy.dir[0] *= -1
            }

            if (dir[1] != 0 && this.state.map[pos[0]] != undefined && this.state.map[pos[0]][pos[1] + dir[1]] == FieldState.Empty) {
                enemy.translate(0, dir[1])
            } else {
                enemy.dir[1] *= -1
            }
        }
        this.CheckPlayer()
    }
    Distance2d(x1, y1, x2, y2) {
        return Math.sqrt(Math.pow((x1 - x2), 2) + Math.pow((y1 - y2), 2))
    }
    // Check if player is too close to enemies. If so then delete player obj.
    CheckPlayer() {
        const [px, py] = this.state.player.pos

        // Check if reached collectible
        const collArr = this.state.collectibles
        for (let coll of collArr) {
            if (coll.pos[0] == px && coll.pos[1] == py) {
                this.state.collectibles = this.state.collectibles.filter(x => {
                    return JSON.stringify(x) != JSON.stringify(coll)
                })
                this.CheckWinCondition()
            }
        }

        for (const enemy of this.state.enemies) {
            const [ex, ey] = enemy.pos
            if (Math.floor(this.Distance2d(px, py, ex, ey)) <= 1) {
                //Lost game
                this.state.status = GameStatusEnumm.Defeat
            }
        }

    }
    // Check if win condition is reached.
    CheckWinCondition() {
        if (this.state.collectibles.length == 0) {
            this.state.status = GameStatusEnumm.Victory
        }
    }

}