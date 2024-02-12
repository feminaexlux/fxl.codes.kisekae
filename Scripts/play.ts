import * as $ from "jquery";
import {fabric} from "fabric";
import {Playset} from "./models";

export class PlaySpace {
    private model: Playset;
    private canvas: fabric.Canvas;

    constructor(model: Playset) {
        const container = $(".container");
        this.canvas = new fabric.Canvas("play_space");
        this.canvas.selection = false;
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

        const celGroups: { [key: number]: fabric.Image[] } = {};
        for (let cel of this.model.cels.filter(x => x.initialPositions[set])) {
            const image = await new Promise<fabric.Image>(resolve => {
                fabric.Image.fromURL(cel.image, image => resolve(image));
            });

            image.set({
                left: cel.initialPositions[set].x + cel.offset.x,
                top: cel.initialPositions[set].y + cel.offset.y,
                selectable: cel.fix == 0,
                hoverCursor: "default",
                hasControls: false,
                lockRotation: true,
                lockScalingX: true,
                lockScalingY: true,
                hasBorders: false
            });
            this.canvas.add(image);

            if (!celGroups[cel.mark]) celGroups[cel.mark] = [];
            celGroups[cel.mark].push(image);
        }

        this.linkItems(celGroups);
    }

    linkItems(groups: { [key: number]: fabric.Image[] }) {
        for (let key in groups) {
            const group = groups[key];
            const length = group.length;
            const top = group.pop();

            if (length < 2) {
                top.set({hoverCursor: "move"});
                continue;
            }

            if (!top.selectable) continue;

            top.set({hoverCursor: "move"});
            const transformMatrix = top.calcTransformMatrix();
            const invertedTransform = fabric.util.invertTransform(transformMatrix);
            for (let item of group) {
                (item as any)["relationship"] = fabric.util.multiplyTransformMatrices(invertedTransform, item.calcTransformMatrix());
            }

            top.on("moving", event => {
                const currentTransform = top.calcTransformMatrix();
                for (let item of group) {
                    const relationship = (item as any).relationship;
                    if (!relationship) continue;
                    const transform = fabric.util.multiplyTransformMatrices(currentTransform, relationship);
                    const translation = fabric.util.qrDecompose(transform);
                    item.setPositionByOrigin(new fabric.Point(translation.translateX, translation.translateY), "center", "center");
                    item.set(translation);
                    item.set({flipX: false, flipY: false}).setCoords();
                }
            });
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