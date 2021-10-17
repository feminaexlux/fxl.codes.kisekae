export default class Builder {
    element: HTMLElement

    constructor(tag: string) {
        this.element = document.createElement(tag)
    }

    addAttributes(attributes: { [key: string]: string | number }): Builder {
        for (let key of Object.keys(attributes)) {
            this.addAttribute(key, attributes[key])
        }

        return this
    }

    addAttribute(name: string, value: string | number): Builder {
        let attribute = document.createAttribute(name)
        attribute.value = value.toString()

        this.element.attributes.setNamedItem(attribute)
        return this
    }

    addClass(...classes: string[]): Builder {
        this.element.classList.add(...classes)
        return this
    }

    addChildren(...children: HTMLElement[] | Builder[]): Builder {
        children.forEach(child => {
            if (child instanceof HTMLElement) {
                this.element.appendChild(child)
            } else {
                this.element.appendChild(child.build())
            }
        })

        return this
    }

    setText(text: string): Builder {
        this.element.textContent = text
        return this
    }

    build(): HTMLElement {
        return this.element
    }
}