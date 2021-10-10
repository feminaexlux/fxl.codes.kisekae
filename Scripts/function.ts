import Builder from "./utility"
import {ICel, IPlayset} from "./pto"

export function draw(playset: IPlayset, set: number): HTMLCanvasElement {
    let canvas = new Builder("canvas").addAttributes({
        "height": playset.height.toString(),
        "width": playset.width.toString()
    }).build() as HTMLCanvasElement
    let context = canvas.getContext("2d")
    
    playset.cels.forEach(cel => {
        drawToCanvas2dContext(cel, context, set)
    })

    return canvas
}

export function drawToCanvas2dContext(cel: ICel, context: CanvasRenderingContext2D, set: number): void {
    if (!cel.sets[set]) return

    let image = new Image()
    image.onload = () => {
        context.drawImage(image, cel.initialPositions[set].x + cel.offset.x, cel.initialPositions[set].y + cel.offset.y)
    }

    image.src = `data:image/gif;base64,${cel.defaultImage}`
}