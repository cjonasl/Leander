interface IPoint {
    getDist(): number;
}

class Point implements IPoint {
    x: number;
    y: number;

    constructor(x: number, y: number) {
        this.x = x;
        this.y = y;
    }

    getDist() {
        return Math.sqrt(this.x * this.x + this.y * this.y);
    }
}

var point = new Point(3, 4);
console.log(point.getDist());