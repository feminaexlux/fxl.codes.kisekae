export interface Playset {
    readonly name: string
    readonly height: number
    readonly width: number
    readonly cels: Cel[]
    readonly enabledSets: boolean[]
}

export interface Cel {
    readonly id: number
    readonly fix: number
    readonly initialPositions: Coordinate[]
    readonly offset: Coordinate
    readonly sets: boolean[]

    currentPositions: Coordinate[]
    currentFix: number
}

export class Coordinate {
    readonly x: number
    readonly y: number

    constructor(x: number = 0, y: number = 0) {
        this.x = x
        this.y = y
    }
}
