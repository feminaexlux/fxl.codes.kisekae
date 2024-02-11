import * as $ from "jquery";
import {fabric} from "fabric";
import {Playset} from "./models";

export class PlaySpace {
    private model: Playset;
    private canvas: fabric.Canvas;

    constructor(model: Playset) {
        const container = $(".container");
        this.canvas = new fabric.Canvas("play_space");
        this.canvas.setHeight(container.height()).setWidth(container.width());
        this.model = model;
        this.selectSet();

        $(".set-link").on("click", event => {
            const set = parseInt($(event.target).attr("data-set"));
            this.selectSet(set);
        });

        this.canvas.setZoom(this.canvas.width / model.width);
    }

    async selectSet(set?: number) {
        if (!set) {
            set = this.model.sets.indexOf(true);
        }

        $(".set-link").removeClass("active");
        $(`.set-link[data-set=${set}]`).addClass("active");

        this.canvas.clear();
        if (this.model.borderColor) this.canvas.backgroundColor = this.model.borderColor;

        const cels = this.model.cels.filter(x => x.initialPositions[set]);
        for (let cel of cels) {
            const image = await new Promise<fabric.Image>(resolve => {
                fabric.Image.fromURL(cel.image, image => resolve(image));
            });

            image.set({
                left: cel.initialPositions[set].x + cel.offset.x,
                top: cel.initialPositions[set].y + cel.offset.y,
                selectable: cel.fix == 0
            });
            this.canvas.add(image);
        }
    }
}

declare global {
    interface Window {
        playSet: Playset;
    }
}

$(() => {
    new PlaySpace(window.playSet);
});