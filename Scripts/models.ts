import {fabric} from "fabric";

export interface Playset {
    readonly borderColor: string;
    readonly name: string;
    readonly height: number;
    readonly width: number;
    readonly cels: Cel[];
    readonly sets: boolean[];
}

export interface Cel {
    readonly mark: number;
    readonly fix: number;
    readonly initialPositions: { [key: number]: fabric.Point };
    readonly offset: fabric.Point;
    readonly image: string;
    readonly zIndex: number;
    readonly height: number;
    readonly width: number;
}