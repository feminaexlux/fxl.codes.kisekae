import {MDCChipSet} from "@material/chips"
import Builder from "./utility"

class Uploader {
    chips: MDCChipSet

    constructor() {
        this.init()
    }

    init() {
        let upload = document.getElementById("lzh_upload") as HTMLInputElement
        let container = document.querySelector(".mdc-evolution-chip-set__chips")

        document.addEventListener("MDCChipSet:interaction", (event: Event) => {
            console.log(event)
        })

        document.addEventListener("MDCChipSet:selection", (event: Event) => {
            console.log(event)
        })

        upload.addEventListener("change", (event: InputEvent) => {
            while (container.firstChild) container.removeChild(container.firstChild)

            for (let index = 0; index < upload.files.length; index++) {
                this.addChip(container, index, upload.files.item(index))
            }

            let element = document.querySelector(".mdc-evolution-chip-set")
            this.chips = MDCChipSet.attachTo(element)
        })
    }

    addChip(container: Element, index: number, file: File) {
        const chipClass = "mdc-evolution-chip"
        
        let textButton = Builder.element("button", {"type":"button","tabindex":0}, `${chipClass}__action`, `${chipClass}__action--primary`)
            .addChildren(
                Builder.element("span", {}, `${chipClass}__ripple`, `${chipClass}__ripple--primary`),
                Builder.element("span", {}, `${chipClass}__text-label`).setText(file.name))
        let textWrapper = Builder.element("span", {"role":"gridcell"}, `${chipClass}__cell`, `${chipClass}__cell--primary`)
            .addChildren(textButton)
        
        let trailingButton = Builder.element("button", {"type":"button","tabindex":-1,"data-mdc-deletable":"true","aria-label":`Remove ${file.name}`})
            .addChildren(
                Builder.element("span", {}, `${chipClass}__ripple`,`${chipClass}__ripple--trailing`),
                Builder.element("span", {}, `${chipClass}__icon`,`${chipClass}__icon--trailing`).setText("close"))
        let trailingWrapper = Builder.element("span", {"role":"gridcell"}, `${chipClass}__cell`, `${chipClass}__cell--trailing`)
            .addChildren(trailingButton)
        
        let chip = Builder.element("span", {"role":"row","id":`chip_${index}`}, chipClass).addChildren(textWrapper, trailingWrapper).build()

        container.appendChild(chip)
    }
}

new Uploader()