"use strict";
class Person {
    firstName;
    lastName;
    constructor(firstName, lastName) {
        this.firstName = firstName;
        this.lastName = lastName;
    }
    greetUser() {
        return "Hello and welcome to TypeScript";
    }
}
let p = new Person("Kamya", "Murali");
console.log(p.greetUser());
