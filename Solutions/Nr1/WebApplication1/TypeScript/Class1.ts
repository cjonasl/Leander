class Car {
    name: string;

    constructor(name: string) {
        this.name = name;
    }

    getName(): string {
        return this.name;
    }
}

var car = new Car("Volvo");
console.log(car.getName());