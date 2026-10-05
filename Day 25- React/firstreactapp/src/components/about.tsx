


function About() {
    const orgName = "Cognizant";
    const orgAddress = "123 Main St, City, State, ZIP";
    const productList = ["Product 1", "Product 2", "Product 3"];
  return (
    <>
      <h1> About Page </h1>
      <p> This is the about page of my first react app. </p>
      <p>This is the about page content. </p>
      <p>Organization: {orgName}</p>
      <p>Address: {orgAddress}</p>
      <p>Products:</p>
      <ul>
        {productList.map((product, index) => (
          <li key={index}>{product}</li>
        ))}
      </ul>
    </>
  )
}

export default About