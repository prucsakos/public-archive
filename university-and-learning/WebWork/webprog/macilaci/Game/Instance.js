
class StaticID {
    static id = 0
    static RequestID() {
        return ++this.id
    }
}

export class Instance {
    constructor(x, y) {
        this.id = StaticID.RequestID()
        this.pos = [x, y]
    }
    translate(dx, dy) {
        this.pos[0] += dx
        this.pos[1] += dy
    }
}

export class DirectionalInstance extends Instance {
    constructor(x, y, dx, dy) {
        super(x, y)
        this.dir = [dx, dy]
    }
}