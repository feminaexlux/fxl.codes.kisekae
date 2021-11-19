import {MDCLinearProgress} from "@material/linear-progress"
import {MDCRipple} from "@material/ripple"
import {MDCTopAppBar} from "@material/top-app-bar"
import {Playset} from "./pto"
import Tracker from "./tracker"
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

    public load(containerId: string, headerId: string, playset: Playset, set: number = 0): void {
        let container = document.getElementById(containerId)
        let menu = document.getElementById(headerId).querySelector("ul")

        let tracker = new Tracker(playset, menu, container)
        tracker.setPlayArea(set)
    }

    private init() {
        let main = document.querySelector("main")
        let directories = main.querySelector("ul")

        if (!directories) return

        let links = directories.querySelectorAll("a")

        links.forEach(link => {
            link.addEventListener("click", () => {
                this.linearProgress.determinate = false
                this.linearProgress.open()

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