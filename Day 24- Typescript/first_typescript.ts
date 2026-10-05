class Person {
    firstName: string;
    lastName: string;

    constructor(firstName: string, lastName: string) {
        this.firstName = firstName;
        this.lastName = lastName;
    }

    greetUser(): string {
        return "Hello and welcome to TypeScript";
    }
}

let p = new Person("Kamya", "Murali");
console.log(p.greetUser());
