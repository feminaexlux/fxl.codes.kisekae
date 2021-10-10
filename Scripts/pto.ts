export interface IPlayset {
    name: string
    height: number
    width: number

    cels: ICel[]
    enabledSets: boolean[]
}

export interface ICel {
    defaultImage: string
    initialPositions: Coordinate[]
    offset: Coordinate
    sets: boolean[]
}

export class Coordinate {
    x: number
    y: number
}
