import {Playset} from "./pto"

const cssClass = "active-set"

export function setPlayArea(header: HTMLElement, container: HTMLElement, playset: Playset, set: number): void {
    header.querySelectorAll("button").forEach(button => {
        button.classList.remove(cssClass)
    })

    header.querySelector(`button[data-set="${set}"]`).classList.add(cssClass)
    let elements = container.querySelectorAll(".cel-image")
    
    playset.cels.forEach((cel, index) => {
        let element = elements[index] as HTMLDivElement
        element.style.visibility = "hidden"
        
        if (cel.sets[set]) {
            let position = cel.currentPositions[set]
            element.style.visibility = "visible"
            element.style.left = `${position.x}px`
            element.style.top = `${position.y}px`
        }
    })
}