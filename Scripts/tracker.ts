import {Cel, Coordinate, Playset} from "./pto"
import Builder from "./utility"

const cssClass = "active-set"

export default class Tracker {
    set: number = 0
    setPalettes: number[] = []

    private playset: Playset
    private menu: HTMLElement
    private playSpace: HTMLElement
    private maxFixById: { [key: number]: number } = {}

    constructor(playset: Playset, menu: HTMLElement, playSpace: HTMLElement) {
        this.playset = playset
        this.menu = menu
        this.playSpace = playSpace

        this.addEvents()
        this.reset()
        this.setMenu()
    }

    private static adjustPlacement(cel: Cel, set: number, movement: Coordinate): Coordinate {
        let x = cel.currentPositions[set].x + movement.x
        let y = cel.currentPositions[set].y + movement.y
        cel.currentPositions[set] = new Coordinate(x, y)

        return Tracker.getCoordinate(cel, set)
    }

    private static getCoordinate(cel: Cel, set: number): Coordinate {
        let position = cel.currentPositions[set]
        if (!position) return new Coordinate()

        if (!cel.offset) return position

        return new Coordinate(position.x + cel.offset.x, position.y + cel.offset.y)
    }

    setPlayArea(set: number): void {
        this.set = set

        this.menu.querySelectorAll("button").forEach(button => {
            button.classList.remove(cssClass)
        })
        this.menu.querySelector(`button[data-set="${set}"]`).classList.add(cssClass)

        let elements = this.playSpace.querySelectorAll(".cel-image")
        this.playset.cels.forEach((cel, index) => {
            let element = elements[index] as HTMLDivElement
            element.style.visibility = "hidden"

            if (cel.sets[set]) {
                element.style.visibility = "visible"
                let position = Tracker.getCoordinate(cel, set)
                element.style.left = `${position.x}px`
                element.style.top = `${position.y}px`
            }
        })
    }

    private addEvents() {
        let previousEvent: MouseEvent | null = null

        let elements = this.playSpace.querySelectorAll(".cel-image")
        elements.forEach((element: HTMLElement) => {
            let mousemove = (move: MouseEvent) => {
                let movement = new Coordinate(move.clientX - previousEvent.clientX, move.clientY - previousEvent.clientY)
                previousEvent = move

                let group = this.getGroup(element)
                group.forEach((item: HTMLElement) => {
                    let index = parseInt(item.attributes.getNamedItem("data-index").value)
                    let cel = this.playset.cels[index]

                    if (cel.currentFix == 0) {
                        let position = Tracker.adjustPlacement(cel, this.set, movement)
                        item.style.left = `${position.x}px`
                        item.style.top = `${position.y}px`
                    }
                })
            }

            element.addEventListener("mousedown", (down: MouseEvent) => {
                previousEvent = down
                this.getGroup(element).forEach(item => {
                    let index = parseInt(item.attributes.getNamedItem("data-index").value)
                    let cel = this.playset.cels[index]

                    if (cel.currentFix > 0) cel.currentFix--
                })

                window.addEventListener("mousemove", mousemove)
                window.addEventListener("mouseup", () => {
                    previousEvent = null
                    window.removeEventListener("mousemove", mousemove)
                })
            })
        })
    }

    private getGroup(element: Element): NodeListOf<Element> {
        let id = element.attributes.getNamedItem("data-id").value
        return this.playSpace.querySelectorAll(`.cel-image[data-id="${id}"]`)
    }

    private reset(): void {
        this.playSpace.querySelectorAll(".cel-image").forEach((element: HTMLElement) => {
            element.style.visibility = "hidden"
            element.style.left = "0"
            element.style.top = "0"
        })
        
        this.playset.cels.forEach(cel => {
            if (!this.maxFixById[cel.id]) {
                this.maxFixById[cel.id] = 0
            }
            
            if (this.maxFixById[cel.id] < cel.fix) {
                this.maxFixById[cel.id] = cel.fix
            }
        })

        this.playset.cels.forEach(cel => {
            cel.currentFix = this.maxFixById[cel.id]
            cel.currentPositions = cel.initialPositions.map(position => position ? new Coordinate(position.x, position.y) : null)
        })
    }

    private setMenu(): void {
        let reset = this.menu.querySelector(`button[data-rel="reset"]`)
        reset.addEventListener("click", () => {
            this.reset()
            this.setPlayArea(this.set)
        })

        this.playset.enabledSets.forEach((enabled, index) => {
            if (enabled) {
                let button = new Builder("li")
                    .addChildren(new Builder("button")
                        .addClass("mdc-button")
                        .addAttributes({"data-set": index})
                        .addChildren(
                            new Builder("span").addClass("mdc-button__ripple"),
                            new Builder("span").addClass("mdc-button__label").setText(index.toString())
                        )).build()

                button.addEventListener("click", () => {
                    this.setPlayArea(index)
                })

                this.menu.appendChild(button)
            }
        })
    }
}