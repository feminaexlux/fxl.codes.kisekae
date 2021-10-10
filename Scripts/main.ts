import {MDCLinearProgress} from "@material/linear-progress"
import {MDCRipple} from "@material/ripple"
import {MDCTopAppBar} from "@material/top-app-bar"
import {draw} from "./function"
import {IPlayset} from "./pto"
import Builder from "./utility"

declare global {
    interface Document {
        kisekae: Main
    }
}

export default class Main {
    appBar: MDCTopAppBar
    linearProgress: MDCLinearProgress
    ripples: MDCRipple[] = []

    constructor() {
        this.appBar = MDCTopAppBar.attachTo(document.querySelector(".mdc-top-app-bar"))

        document.querySelectorAll(".mdc-button").forEach(button => {
            this.ripples.push(MDCRipple.attachTo(button))
        })

        this.linearProgress = MDCLinearProgress.attachTo(document.querySelector(".mdc-linear-progress"))

        this.init()
    }

    public load(containerId: string, headerId: string, playset: IPlayset, set: number = 0): void {
        let container = document.getElementById(containerId)
        let header = document.getElementById(headerId)
        let menu = header.querySelector("ul")

        let reset = header.querySelector(`button[data-rel="reset"]`)
        reset.addEventListener("click", () => {
            this.setPlayArea(container, playset, 0)
        })

        playset.enabledSets.forEach((enabled, index) => {
            if (enabled) {
                let button = new Builder("li")
                    .addChildren(new Builder("button")
                        .addClass("mdc-button")
                        .addChildren(
                            new Builder("span").addClass("mdc-button__ripple"),
                            new Builder("span").addClass("mdc-button__label").setText(index.toString())
                        )).build()

                button.addEventListener("click", () => {
                    this.setPlayArea(container, playset, index)
                })

                menu.appendChild(button)
            }
        })

        this.setPlayArea(container, playset, set)
    }

    private setPlayArea(container: HTMLElement, playset: IPlayset, set: number): void {
        // Empty
        while (container.firstChild) container.removeChild(container.firstChild)

        let canvas = draw(playset, set)
        container.appendChild(canvas)
    }

    private init() {
        let main = document.querySelector("main")
        let directories = main.querySelector("ul")

        if (!directories) return

        let links = directories.querySelectorAll("a")

        links.forEach(link => {
            link.addEventListener("click", () => {
                let directory = link.attributes.getNamedItem("data-directory")
                let form = new Builder("form")
                    .addAttributes({"method": "post", "action": "/Home/Select"})
                    .addChildren(
                        new Builder("input").addAttributes({
                            "type": "hidden",
                            "name": "directory",
                            "value": directory.value
                        }),
                        new Builder("input").addAttributes({
                            "type": "hidden",
                            "name": "file",
                            "value": link.textContent
                        })
                    ).build() as HTMLFormElement

                link.appendChild(form)
                form.submit()
            })
        })
    }
}

document.kisekae = new Main()