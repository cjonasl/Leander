var Point = /** @class */ (function () {
    function Point(x, y) {
        this.x = x;
        this.y = y;
    }
    Point.prototype.getDist = function () {
        return Math.sqrt(this.x * this.x + this.y * this.y);
    };
    return Point;
}());
var point = new Point(3, 4);
console.log(point.getDist());
