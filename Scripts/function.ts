import {ICel, IPlayset} from "./pto"
import Builder from "./utility";

export function draw(playset: IPlayset, canvas: HTMLCanvasElement, set: number): void {
    let context = canvas.getContext("2d")
    context.clearRect(0, 0, canvas.width, canvas.height)

    playset.cels.forEach(cel => {
        drawToCanvas2dContext(cel, context, set)
    })
}

export function drawToCanvas2dContext(cel: ICel, context: CanvasRenderingContext2D, set: number): void {
    if (!cel.sets[set]) return

    let image = new Image()
    image.onload = () => {
        context.drawImage(image, cel.initialPositions[set].x + cel.offset.x, cel.initialPositions[set].y + cel.offset.y)
    }

    image.src = `data:image/gif;base64,${cel.defaultImage}`
}

export function setPlayArea(header: HTMLElement, container: HTMLElement, playset: IPlayset, set: number): void {
    const cssClass = "active-set"
    header.querySelectorAll("button").forEach(button => {
        button.classList.remove(cssClass)
    })

    header.querySelector(`button[data-set="${set}"]`).classList.add(cssClass)
    
    let canvases = container.getElementsByTagName("canvas")
    if (canvases.length) {
        draw(playset, canvases[0], set)
        return
    }

    let canvas = new Builder("canvas")
        .addAttributes({"height": playset.height, "width": playset.width})
        .build()
    container.appendChild(canvas)
    draw(playset, canvases[0], set)
}