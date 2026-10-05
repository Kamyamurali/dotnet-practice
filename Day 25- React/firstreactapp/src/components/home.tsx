import React from 'react'

class Home extends React.Component {
  // every component must have a render method which returns JSX
  render() {
    // render must return a single block, e.g. <div> or <></>
    const firstName = "Kamya"; // this will come from REST API in future
    const lastName = "Murali"; // this will come from REST API in future
    const skills = ["HTML", "CSS", "JavaScript", "React", "NodeJS"];

    return (
      <div>
        <h1> This is my home component </h1>
        <p> Welcome to my home page, this is the very first component I have learned and designed in React. </p>
        <p> I have learned about the class based components and functional components in React. </p>
        <h2> Keep checking this space for more updates </h2>

        <p> some jsx expression </p>
        <p> Addition of my Fav numbers : {5 + 10}</p>
        <p> Is 5 greater than 10? {5 > 10 ? "Yes" : "No"}</p>
        <p> My full name is {firstName} {lastName} </p>
        <p> My skills are : </p>
        <ul>
          {skills.map((skill, index) => (
            <li key={index}>{skill}</li>
          ))}
        </ul>
      </div>
    )
  }
}

export default Home