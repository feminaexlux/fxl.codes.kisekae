import { MDCTopAppBar } from '@material/top-app-bar'

declare global {
    interface Document {
        kisekae: Main
    }
}

export default class Main {
    appBar: MDCTopAppBar
    constructor() {
        this.appBar = MDCTopAppBar.attachTo(document.querySelector(".mdc-top-app-bar"))
        
        this.init()
    }
    
    init() {
        let main = document.querySelector("main")
        let directories = main.querySelector("ul")
        
        if (!directories) return
        
        let links = directories.querySelectorAll("a")
        
        links.forEach(link => {
            link.addEventListener("click", () => {
                let directory = link.attributes.getNamedItem("data-directory")
                let form = document.createElement("form")
                
                let method = document.createAttribute("method")
                method.value = "post"
                let action = document.createAttribute("action")
                action.value = "/Home/Select"
                
                form.attributes.setNamedItem(method)
                form.attributes.setNamedItem(action)
                
                form.innerHTML = `<input type="hidden" name="directory" value="${directory.value}">`
                    + `<input type="hidden" name="file" value="${link.textContent}">`
                
                link.appendChild(form)
                
                form.submit()
            })
        })
    }
}

document.kisekae = new Main()