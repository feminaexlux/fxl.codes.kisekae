import * as _ from "lodash"
import {fabric} from "fabric"
import {Cel, Playset} from "./pto"

export default class Tracker {
    set: number = 0

    private playset: Playset
    private menu: HTMLElement
    private canvas: fabric.Canvas
    private states: string[]

    constructor(playset: Playset, menu: HTMLElement, playSpace: HTMLElement) {
        this.playset = playset
        this.menu = menu

        this.canvas = new fabric.Canvas(playSpace.id)
        this.playset.cels
            .sort((first: Cel, second: Cel) => {
                return first.zIndex
            })
    }

    setPlayArea(set: number): void {
        this.set = set
        this.canvas.clear()
        
        let groups: { [key: number]: Cel[] } = {}
        let groupImages: { [key: number]: fabric.Image[] } = {}
        this.playset.cels
            .filter(cel => cel.initialPositions[set])
            .forEach(cel => {
                if (!groups[cel.mark]) groups[cel.mark] = []
                groups[cel.mark].push(cel)
            })
        
        let tempCanvas = this.canvas
        let doGrouping = function() {
            console.log("Executing groups?")
            for (let fabricGroupKey in groupImages) {
                tempCanvas.discardActiveObject()
                let selection = new fabric.ActiveSelection(groupImages[fabricGroupKey], { canvas: this.canvas })
                let group = selection.toGroup()
                tempCanvas.add(group)
            }
        }
        
        for (let key in groups) {
            groups[key].forEach(cel => {
                fabric.Image.fromURL(cel.image,image => {
                    image.left = cel.initialPositions[set].x + cel.offset.x
                    image.top = cel.initialPositions[set].y + cel.offset.y
                    
                    if (!groupImages[key]) groupImages[key] = []
                    groupImages[key].push(image)
                    this.canvas.add(image)
                    
                    console.log("Adding image")
                    _.debounce(doGrouping, 250)
                })
            })
        }
    }
}