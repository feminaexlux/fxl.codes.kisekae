export class Playset {
    name: string
    height: number
    width: number

    cels: Cel[]
    enabledSets: boolean[]
}

export class Cel {
    defaultImage: string
    initialPositions: Coordinate[]
    offset: Coordinate
    sets: boolean[]

    currentPositions: Coordinate[]
}

export class Coordinate {
    x: number
    y: number

    constructor(x: number = 0, y: number = 0) {
        this.x = x
        this.y = y
    }
}
