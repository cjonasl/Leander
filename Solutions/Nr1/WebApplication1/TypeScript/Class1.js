var Car = /** @class */ (function () {
    function Car(name) {
        this.name = name;
    }
    Car.prototype.getName = function () {
        return this.name;
    };
    return Car;
}());
var car = new Car("Volvo");
console.log(car.getName());
