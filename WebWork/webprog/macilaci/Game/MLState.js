import { Instance, DirectionalInstance } from "./Instance.js"

export const GameStatusEnumm = {
    Paused: 0,
    Started: 1,
    Victory: 2,
    Defeat: 3,
    Terminated: 4
}

export const FieldState = {
    Empty: 0,
    Wall: 1,
    Collectible: 2,
    Player: 3,
    Enemy: 4
}
// Vector [hor, ver]
export class MLState {
    status = GameStatusEnumm.Paused
    map = [
        [0, 0, 1, 1, 1, 1, 1, 0],
        [0, 0, 1, 1, 1, 0, 0, 0],
        [1, 0, 1, 0, 1, 1, 0, 0],
        [1, 0, 1, 0, 1, 0, 0, 0],
        [0, 0, 1, 0, 1, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0]
    ]
    player = new Instance(0, 0)
    enemies = [
        new DirectionalInstance(5, 3, 1, 0),
        new DirectionalInstance(5, 7, 0, 0),
        new DirectionalInstance(1, 7, 0, 1)
    ]
    collectibles = [
        new Instance(5, 0),
        new Instance(0, 7)
    ]
    LoadState(StateObj) {
        this.status = StateObj.Status
        this.map = StateObj.Map
        this.player = StateObj.Player
        this.enemies = Enemies
        this.collectibles = StateObj.Collectibles
    }
    ExportState() {
        return {
            Status: this.status,
            Map: this.map,
            Player: this.player,
            Enemies: this.enemies,
            Collectibles: this.collectibles
        }
    }
}