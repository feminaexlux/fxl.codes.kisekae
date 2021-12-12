export interface Playset {
    readonly name: string
    readonly height: number
    readonly width: number
    readonly cels: Cel[]
    readonly sets: boolean[]
}

export interface Cel {
    readonly mark: number
    readonly fix: number
    readonly initialPositions: { [key: number]: Coordinate }
    readonly offset: Coordinate
    readonly image: string
    readonly zIndex: number
    readonly height: number
    readonly width: number
}

export class Coordinate {
    readonly x: number
    readonly y: number

    constructor(x: number = 0, y: number = 0) {
        this.x = x
        this.y = y
    }
}
